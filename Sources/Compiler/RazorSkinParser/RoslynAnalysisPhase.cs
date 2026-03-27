using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin
{
    public static class RoslynAnalysisPhase
    {
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

            // Walk all IR nodes and refine classifications
            RefineNodes(ir.Children, modelType, controlType, compilation, semanticModel);

            // Refine loop nodes
            RefineLoopNodes(ir.Children, modelType, compilation);

            // Refine conditional nodes
            RefineConditionalNodes(ir.Children, modelType, controlType);
        }

        private static void RefineNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType,
            CSharpCompilation compilation,
            SemanticModel semanticModel)
        {
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                {
                    RefineExpressionBinding(binding, modelType, controlType);
                }

                RefineNodes(node.Children, modelType, controlType, compilation, semanticModel);
            }
        }

        private static void RefineExpressionBinding(
            ExpressionBindingNode binding,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType)
        {
            var expr = binding.Classification.CSharpExpression;
            var dependencies = new List<ObservableDependency>();

            // Extract property references from Model.* expressions
            if (modelType != null)
            {
                var propertyNames = ExtractPropertyReferences(expr, "Model.");
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

        private static void RefineConditionalNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            INamedTypeSymbol controlType)
        {
            foreach (var node in nodes)
            {
                if (node is ConditionalNode cond)
                {
                    if (modelType != null)
                    {
                        var propNames = ExtractPropertyReferences(
                            cond.Condition.CSharpExpression, "Model.");
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

                    // Recurse into nested branches (M3)
                    RefineConditionalNodes(cond.TrueBranch, modelType, controlType);
                    RefineConditionalNodes(cond.FalseBranch, modelType, controlType);
                }

                RefineConditionalNodes(node.Children, modelType, controlType);
            }
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
