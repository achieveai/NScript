using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NScript.RazorSkin.TemplateIR;
using Serilog;

namespace NScript.RazorSkin
{
    public static class RoslynAnalysisPhase
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        // Cache metadata references since they are expensive to create and
        // don't change between invocations. Uses Lazy<T> for thread safety.
        private static readonly Lazy<MetadataReference[]> _cachedReferences =
            new Lazy<MetadataReference[]>(BuildReferences);

        private static MetadataReference[] BuildReferences()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            };

            var runtimeDir = System.IO.Path.GetDirectoryName(typeof(object).Assembly.Location);
            foreach (var asmName in new[]
            {
                "System.Runtime.dll",
                "System.Collections.dll",
                "System.Linq.dll",
                "System.Collections.Generic.dll",
                "netstandard.dll"
            })
            {
                var path = System.IO.Path.Combine(runtimeDir, asmName);
                if (System.IO.File.Exists(path))
                    references.Add(MetadataReference.CreateFromFile(path));
            }

            return references.ToArray();
        }

        public static void RefineClassifications(
            SkinTemplateNode ir,
            string generatedCSharp,
            string[] additionalSources)
        {
            // Build a Roslyn compilation from the generated C# + framework stubs.
            // The compilation is local-scoped and becomes garbage-collectable after
            // this method returns, ensuring no memory leaks for builds with many templates.
            var trees = new List<SyntaxTree>();
            trees.Add(CSharpSyntaxTree.ParseText(generatedCSharp));
            foreach (var src in additionalSources)
            {
                trees.Add(CSharpSyntaxTree.ParseText(src));
            }

            var compilation = CSharpCompilation.Create(
                "RazorSkinAnalysis",
                trees,
                _cachedReferences.Value,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generatedTree = trees[0];
            var semanticModel = compilation.GetSemanticModel(generatedTree);

            // Resolve model type and control type
            var modelType = ResolveModelType(ir.ModelTypeName, compilation);
            var controlType = !string.IsNullOrEmpty(ir.ControlTypeName)
                ? ResolveModelType(ir.ControlTypeName, compilation) : null;

            Log.Debug("Model type {ModelTypeName} resolved: {ModelFound}, Control type {ControlTypeName} resolved: {ControlFound}",
                ir.ModelTypeName, modelType != null,
                ir.ControlTypeName, controlType != null);

            // Walk all IR nodes and refine classifications.
            // This now handles expression bindings, conditionals, and loop item types.
            RefineNodes(ir.Children, modelType, controlType, compilation, semanticModel);

            // Refine loop nodes (sets IsObservableCollection flag)
            RefineLoopNodes(ir.Children, modelType, compilation);

            // Count promotions (OneTime -> OneWay)
            var promotionCount = CountPromotions(ir.Children);
            Log.Debug("Binding refinement complete: {PromotionCount} bindings promoted from OneTime to OneWay", promotionCount);
        }

        private static void RefineNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType,
            CSharpCompilation compilation,
            SemanticModel semanticModel,
            string modelPrefix = "Model.")
        {
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                {
                    RefineExpressionBinding(binding, modelType, controlType, modelPrefix);
                }
                else if (node is ConditionalNode cond)
                {
                    // Refine condition with the current model prefix (handles item.Prop inside loops)
                    RefineConditionalBinding(cond, modelType, controlType, modelPrefix);
                    RefineNodes(cond.TrueBranch, modelType, controlType, compilation, semanticModel, modelPrefix);
                    RefineNodes(cond.FalseBranch, modelType, controlType, compilation, semanticModel, modelPrefix);
                }
                else if (node is LoopNode loop)
                {
                    // Resolve item type from the collection property's generic argument.
                    var itemType = ResolveLoopItemType(loop, modelType);
                    var itemPrefix = loop.ItemVariableName + ".";
                    if (itemType != null)
                    {
                        RefineNodes(loop.ItemTemplate, itemType, controlType, compilation, semanticModel, itemPrefix);
                    }
                    else
                    {
                        RefineNodes(loop.ItemTemplate, modelType, controlType, compilation, semanticModel, modelPrefix);
                    }
                }

                // Recurse into generic children
                RefineNodes(node.Children, modelType, controlType, compilation, semanticModel, modelPrefix);
            }
        }

        /// <summary>
        /// Resolves the item type for a loop node by finding the collection property
        /// on the model type and extracting its generic type argument.
        /// E.g., ObservableCollection&lt;RazorItemVM&gt; → RazorItemVM.
        /// </summary>
        private static INamedTypeSymbol ResolveLoopItemType(LoopNode loop, INamedTypeSymbol modelType)
        {
            if (modelType == null) return null;

            var collExpr = loop.CollectionExpression;
            var propName = collExpr.Replace("Model.", "").Split('.')[0];
            var prop = FindProperty(modelType, propName);
            if (prop == null) return null;

            var collectionType = prop.Type as INamedTypeSymbol;
            if (collectionType == null) return null;

            // Check generic type arguments (e.g., ObservableCollection<T> has one arg)
            if (collectionType.TypeArguments.Length > 0)
            {
                return collectionType.TypeArguments[0] as INamedTypeSymbol;
            }

            // Check interfaces for IEnumerable<T>
            foreach (var iface in collectionType.AllInterfaces)
            {
                if (iface.Name == "IEnumerable" && iface.TypeArguments.Length > 0)
                {
                    return iface.TypeArguments[0] as INamedTypeSymbol;
                }
            }

            return null;
        }

        private static void RefineExpressionBinding(
            ExpressionBindingNode binding,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType,
            string modelPrefix = "Model.")
        {
            var expr = binding.Classification.CSharpExpression;
            var dependencies = new List<ObservableDependency>();

            // Extract property references from model expressions (Model.* or item.*)
            if (modelType != null)
            {
                var propertyNames = ExtractPropertyReferences(expr, modelPrefix);
                foreach (var propName in propertyNames)
                {
                    var prop = FindProperty(modelType, propName);
                    if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                    {
                        dependencies.Add(new ObservableDependency(
                            BindingSourceKind.DataContext, propName, propName));
                    }
                }
            }

            // Extract property references from Control.* expressions (H5)
            if (controlType != null)
            {
                var controlPropNames = ExtractPropertyReferences(expr, "Control.");
                foreach (var propName in controlPropNames)
                {
                    var prop = FindProperty(controlType, propName);
                    if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                    {
                        dependencies.Add(new ObservableDependency(
                            BindingSourceKind.TemplateParent, propName, propName));
                    }
                }
            }

            // Update classification
            binding.Classification.Dependencies = dependencies;
            binding.Classification.Mode = dependencies.Count > 0
                ? BindingMode.OneWay
                : BindingMode.OneTime;
        }

        private static void RefineLoopNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            CSharpCompilation compilation)
        {
            foreach (var node in nodes)
            {
                if (node is LoopNode loop && modelType != null)
                {
                    // Check if the collection is observable
                    var collExpr = loop.CollectionExpression;
                    var propName = collExpr.Replace("Model.", "").Split('.')[0];
                    var prop = FindProperty(modelType, propName);
                    if (prop != null)
                    {
                        loop.IsObservableCollection =
                            ObservableAnalyzer.IsObservableCollection(prop.Type);
                    }

                    // Recurse into item template (M3)
                    RefineLoopNodes(loop.ItemTemplate, modelType, compilation);
                }

                RefineLoopNodes(node.Children, modelType, compilation);
            }
        }

        /// <summary>
        /// Refines a conditional node's binding with the current model prefix.
        /// Handles both top-level (Model.*) and item-level (item.*) conditions.
        /// </summary>
        private static void RefineConditionalBinding(
            ConditionalNode cond,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType,
            string modelPrefix)
        {
            if (modelType != null)
            {
                var propNames = ExtractPropertyReferences(
                    cond.Condition.CSharpExpression, modelPrefix);
                foreach (var propName in propNames)
                {
                    var prop = FindProperty(modelType, propName);
                    if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                    {
                        cond.IsReactive = true;
                        cond.Condition.Mode = BindingMode.OneWay;
                        cond.Condition.Dependencies.Add(new ObservableDependency(
                            BindingSourceKind.DataContext, propName, propName));
                    }
                }
            }

            if (controlType != null)
            {
                var controlPropNames = ExtractPropertyReferences(
                    cond.Condition.CSharpExpression, "Control.");
                foreach (var propName in controlPropNames)
                {
                    var prop = FindProperty(controlType, propName);
                    if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                    {
                        cond.IsReactive = true;
                        cond.Condition.Mode = BindingMode.OneWay;
                        cond.Condition.Dependencies.Add(new ObservableDependency(
                            BindingSourceKind.TemplateParent, propName, propName));
                    }
                }
            }
        }

        private static int CountPromotions(List<IRNode> nodes)
        {
            int count = 0;
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding && binding.Classification.Mode == BindingMode.OneWay)
                    count++;
                count += CountPromotions(node.Children);
                if (node is ConditionalNode cond)
                {
                    count += CountPromotions(cond.TrueBranch);
                    count += CountPromotions(cond.FalseBranch);
                }
                else if (node is LoopNode loop)
                {
                    count += CountPromotions(loop.ItemTemplate);
                }
            }
            return count;
        }

        private static INamedTypeSymbol ResolveModelType(
            string modelTypeName, CSharpCompilation compilation)
        {
            if (string.IsNullOrEmpty(modelTypeName)) return null;

            // Try direct lookup
            var type = compilation.GetTypeByMetadataName(modelTypeName);
            if (type != null) return type;

            // Try without namespace (short name)
            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();
                foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    if (classDecl.Identifier.Text == modelTypeName)
                    {
                        var symbol = model.GetDeclaredSymbol(classDecl);
                        if (symbol != null) return symbol;
                    }
                }
            }

            return null;
        }

        private static List<string> ExtractPropertyReferences(string expression, string prefix)
        {
            var props = new List<string>();
            var idx = 0;
            while ((idx = expression.IndexOf(prefix, idx, StringComparison.Ordinal)) >= 0)
            {
                idx += prefix.Length;
                var end = idx;
                while (end < expression.Length && (char.IsLetterOrDigit(expression[end]) || expression[end] == '_'))
                    end++;

                if (end > idx)
                    props.Add(expression.Substring(idx, end - idx));
                idx = end;
            }
            return props.Distinct().ToList();
        }

        private static IPropertySymbol FindProperty(INamedTypeSymbol type, string name)
        {
            var current = type;
            while (current != null)
            {
                var prop = current.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop;
                current = current.BaseType;
            }
            return null;
        }
    }
}
