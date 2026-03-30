using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using Serilog;

namespace NScript.RazorSkin
{
    /// <summary>
    /// NScript compiler plugin that processes .skin.cshtml files.
    /// Implements IMethodConverterPlugin (for [Skin] attribute overwrite) and
    /// IRuntimeConverterPlugin (for emitting compiled template JS).
    /// </summary>
    public class RazorTemplatingPlugin : IMethodConverterPlugin, IRuntimeConverterPlugin
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private RuntimeScopeManager _runtimeScopeManager;
        private ClrContext _clrContext;

        /// <summary>
        /// Map of template name to compiled template IR.
        /// Used by GetPostJavascript to generate proper JST nodes.
        /// </summary>
        private readonly Dictionary<string, TemplateIR.SkinTemplateNode> _compiledIRs
            = new Dictionary<string, TemplateIR.SkinTemplateNode>();

        /// <summary>
        /// Maps full resource name to short template name (for GetOverwrite JS call).
        /// </summary>
        private readonly Dictionary<string, string> _templateShortNames = new Dictionary<string, string>();

        /// <summary>
        /// Whether any .skin.cshtml resources were found during initialization.
        /// </summary>
        private bool _hasRazorTemplates;

        /// <summary>
        /// Resolved runtime types needed for graph descriptor JST emission.
        /// Created during Initialize when Razor templates are found.
        /// </summary>
        private RazorKnownTypes _razorKnownTypes;

        /// <summary>
        /// Maps template name to its JST getter function identifier.
        /// Populated during GetPostJavascript when JST generation succeeds.
        /// Used by GetOverwrite to emit proper JST return statements.
        /// </summary>
        private readonly Dictionary<string, IIdentifier> _templateGetterIdentifiers
            = new Dictionary<string, IIdentifier>();

        /// <summary>
        /// Resolved runtime identifiers for replacing mangled names in compiled JS.
        /// Maps the Razor-generated mangled name (e.g. "Sunlight__Framework__UI__Skin_factory")
        /// to the IIdentifier resolved through the NScript scope system.
        /// </summary>
        private readonly Dictionary<string, IIdentifier> _resolvedIdentifiers = new Dictionary<string, IIdentifier>();

        /// <summary>
        /// Resolved type identifiers for replacing mangled type names in compiled JS.
        /// Maps the Razor-generated mangled type name to the list of IIdentifiers from ResolveType.
        /// </summary>
        private readonly Dictionary<string, IList<IIdentifier>> _resolvedTypeIdentifiers = new Dictionary<string, IList<IIdentifier>>();

        /// <summary>
        /// Framework type stubs needed for Roslyn analysis to classify observable properties.
        /// These are always passed to RazorSkinCompiler.CompileToIR so that the Roslyn analysis
        /// phase can detect ObservableObject-derived types and promote bindings to OneWay.
        /// </summary>
        private const string FrameworkTypeStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection
    {
        public void Add(T item) { }
        public void Remove(T item) { }
    }
}";

        public static bool CanHandle(string templateFileName)
        {
            return templateFileName.EndsWith(".skin.cshtml", StringComparison.OrdinalIgnoreCase);
        }

        // --- IConverterPlugin ---

        public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            _clrContext = clrContext;
            _runtimeScopeManager = runtimeScopeManager;

            // Reset per-compilation state to ensure deterministic output
            // when the compiler is hosted in a long-running process.
            RazorSkinJSTGenerator.ResetDataIndex();

            // Scan embedded resources for .skin.cshtml files
            foreach (var module in clrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    var embeddedResource = resource as EmbeddedResource;
                    if (embeddedResource == null) continue;

                    var fileName = runtimeScopeManager.Context.GetResourceFileName(
                        module, embeddedResource.Name);

                    if (fileName != null && CanHandle(fileName))
                    {
                        Log.Debug("Discovered .skin.cshtml resource {ResourceName} (size {ResourceSize} bytes)",
                            embeddedResource.Name, embeddedResource.GetResourceStream().Length);

                        try
                        {
                            using var stream = embeddedResource.GetResourceStream();
                            using var reader = new StreamReader(stream);
                            var templateSource = reader.ReadToEnd();

                            var templateName = Path.GetFileNameWithoutExtension(
                                Path.GetFileNameWithoutExtension(fileName));

                            // Generate C# stubs for the model type from Cecil type info.
                            // This allows the Roslyn analysis phase to detect observable
                            // properties and promote bindings from OneTime to OneWay.
                            var modelTypeStub = GenerateModelTypeStub(templateSource, clrContext);
                            var additionalSources = modelTypeStub != null
                                ? new[] { FrameworkTypeStubs, modelTypeStub }
                                : new[] { FrameworkTypeStubs };

                            var ir = RazorSkinCompiler.CompileToIR(
                                templateName, templateSource,
                                additionalSources);
                            // Store under both short name and full resource name
                            // so [Skin("full.resource.name.skin.cshtml")] matches
                            _compiledIRs[templateName] = ir;
                            _compiledIRs[embeddedResource.Name] = ir;
                            _templateShortNames[embeddedResource.Name] = templateName;
                            _hasRazorTemplates = true;

                            // Pre-create the getter function identifier in the scope system
                            // so it's available when GetOverwrite is called (before GetPostJavascript)
                            var getterId = SimpleIdentifier.CreateScopeIdentifier(
                                runtimeScopeManager.Scope,
                                templateName,
                                false);
                            _templateGetterIdentifiers[templateName] = getterId;

                            Log.Debug("Compilation succeeded for template {TemplateName} from resource {ResourceName}",
                                templateName, embeddedResource.Name);
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("Compilation failed for resource {ResourceName}: {ErrorMessage}",
                                embeddedResource.Name, ex.Message);

                            runtimeScopeManager.Context.AddError(
                                null,
                                $"Error compiling Razor skin template '{fileName}': {ex.Message}",
                                false);
                        }
                    }
                }
            }

            if (_hasRazorTemplates)
            {
                ResolveRuntimeIdentifiers(clrContext, runtimeScopeManager);

                try
                {
                    _razorKnownTypes = new RazorKnownTypes(clrContext, runtimeScopeManager.Context.ClrKnownReferences);
                }
                catch (Exception ex)
                {
                    Log.Debug("Could not create RazorKnownTypes: {Error}. " +
                        "Graph descriptor emission will fail.", ex.Message);
                }
            }
        }

        /// <summary>
        /// Resolves key runtime identifiers via the NScript scope system, mirroring the
        /// KnownTemplateTypes + TypeResolver approach used by XwmlTemplatingPlugin.
        /// This allows GetPostJavascript to replace Razor-generated mangled names
        /// (e.g. "Sunlight__Framework__UI__Skin_factory") with scope-resolved names
        /// that participate in the NScript minification system.
        /// </summary>
        private void ResolveRuntimeIdentifiers(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            var clrKnownRefs = runtimeScopeManager.Context.ClrKnownReferences;

            const string uiFrameworkDll = "Sunlight.Framework.UI";
            const string systemWebHtmlDll = "System.Web.Html";

            try
            {
                // Look up key framework types (same as KnownTemplateTypes)
                var skinType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Skin"));
                var skinInstanceType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinInstance"));
                var skinBinderInfoType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderInfo"));
                var binderHelperType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderHelper"));
                var elementRefType = clrContext.GetTypeDefinition(
                    Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Element"));
                var documentRefType = clrContext.GetTypeDefinition(
                    Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Document"));
                var uiSkinableElementType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UISkinableElement"));

                // Generic type building for constructor signatures
                var nativeArray1 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray`1"));
                var nativeArray = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray"));
                var func2 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Func`2"));
                var func3 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Func`3"));
                var act2 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action`2"));
                var act3 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.Action`3"));

                var funcObjObj = new GenericInstanceType(func2);
                funcObjObj.GenericArguments.Add(clrKnownRefs.Object);
                funcObjObj.GenericArguments.Add(clrKnownRefs.Object);

                var act2ObjObj = new GenericInstanceType(act2);
                act2ObjObj.GenericArguments.Add(clrKnownRefs.Object);
                act2ObjObj.GenericArguments.Add(clrKnownRefs.Object);

                var act3ObjObjObj = new GenericInstanceType(act3);
                act3ObjObjObj.GenericArguments.Add(clrKnownRefs.Object);
                act3ObjObjObj.GenericArguments.Add(clrKnownRefs.Object);
                act3ObjObjObj.GenericArguments.Add(clrKnownRefs.Object);

                var nativeArray1Func2 = new GenericInstanceType(nativeArray1);
                nativeArray1Func2.GenericArguments.Add(funcObjObj);

                var nativeArray1Str = new GenericInstanceType(nativeArray1);
                nativeArray1Str.GenericArguments.Add(clrKnownRefs.String);

                var nativeArrayInt = new GenericInstanceType(nativeArray1);
                nativeArrayInt.GenericArguments.Add(clrKnownRefs.Int32);

                var nativeArrayObject = new GenericInstanceType(nativeArray1);
                nativeArrayObject.GenericArguments.Add(clrKnownRefs.Object);

                var nativeArraySkinBinderInfo = new GenericInstanceType(nativeArray1);
                nativeArraySkinBinderInfo.GenericArguments.Add(skinBinderInfoType);

                // --- Resolve Skin constructor factory ---
                // Skin(Type controlType, Type modelType, Func<Skin,Document,SkinInstance> factory, string dataIndex)
                var func3SkinDocSI = new GenericInstanceType(func3);
                func3SkinDocSI.GenericArguments.Add(skinType);
                func3SkinDocSI.GenericArguments.Add(documentRefType);
                func3SkinDocSI.GenericArguments.Add(skinInstanceType);

                var skinCtor = clrContext.GetMethodReference(
                    ".ctor", clrKnownRefs.Void, skinType,
                    clrKnownRefs.TypeType, clrKnownRefs.TypeType,
                    func3SkinDocSI, clrKnownRefs.String).Resolve();

                var skinFactoryId = runtimeScopeManager.ResolveFactory(skinCtor);
                _resolvedIdentifiers["Sunlight__Framework__UI__Skin_factory"] = skinFactoryId;

                // --- Resolve SkinInstance graph-mode constructor factory ---
                // SkinInstance(Skin, Element, NativeArray<int>, NativeArray, GraphDescriptor, object, int, int)
                var graphDescriptorType = clrContext.GetTypeDefinition(
                    Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.BindingGraph.GraphDescriptor"));

                var skinInstanceGraphCtor = clrContext.GetMethodReference(
                    ".ctor", clrKnownRefs.Void, skinInstanceType,
                    skinType, elementRefType, nativeArrayInt,
                    nativeArray, graphDescriptorType,
                    clrKnownRefs.Object, clrKnownRefs.Int32, clrKnownRefs.Int32).Resolve();

                var skinInstanceFactoryId = runtimeScopeManager.ResolveFactory(skinInstanceGraphCtor);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinInstance_factory"] = skinInstanceFactoryId;

                // --- Resolve SkinBinderInfo constructor factory ---
                // Use the OneWay1 overload (8 params): the one most commonly used by Razor templates
                var skinBinderCtorOneWay1 = clrContext.GetMethodReference(
                    ".ctor", clrKnownRefs.Void, skinBinderInfoType,
                    nativeArray1Func2, nativeArray1Str, act2ObjObj,
                    runtimeScopeManager.Context.ClrKnownReferences.ClrContext.GetTypeDefinition(
                        Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.BinderType")),
                    clrKnownRefs.Int32, clrKnownRefs.Int32,
                    funcObjObj, clrKnownRefs.Object).Resolve();

                var skinBinderInfoFactoryId = runtimeScopeManager.ResolveFactory(skinBinderCtorOneWay1);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory"] = skinBinderInfoFactoryId;

                // --- Resolve SkinBinderHelper static methods ---
                // GetElementFromPath(Element root, NativeArray<int> path)
                var getElementFromPath = clrContext.GetMethodReference(
                    "GetElementFromPath", elementRefType, binderHelperType,
                    elementRefType, nativeArrayInt);
                var getElementFromPathId = ResolveStaticMethodIdentifier(
                    runtimeScopeManager, getElementFromPath);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath"] = getElementFromPathId;

                // SetTextContent(Element elem, string text)
                var setTextContent = clrContext.GetMethodReference(
                    "SetTextContent", clrKnownRefs.Void, binderHelperType,
                    elementRefType, clrKnownRefs.String).Resolve();
                var setTextContentId = ResolveStaticMethodIdentifier(
                    runtimeScopeManager, setTextContent);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent"] = setTextContentId;

                // SetAttribute(Node node, string attrName, string attrValue)
                var nodeRefType = clrContext.GetTypeDefinition(
                    Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Node"));
                var setAttributeMethod = clrContext.GetMethodReference(
                    "SetAttribute", clrKnownRefs.Void, binderHelperType,
                    nodeRefType, clrKnownRefs.String, clrKnownRefs.String).Resolve();
                var setAttributeId = ResolveStaticMethodIdentifier(
                    runtimeScopeManager, setAttributeMethod);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute"] = setAttributeId;

                // SetCssClass(Element elem, bool add, string className)
                try
                {
                    var setCssClass = clrContext.GetMethodReference(
                        "SetCssClass", clrKnownRefs.Void, binderHelperType,
                        elementRefType, clrKnownRefs.Boolean, clrKnownRefs.String);
                    var setCssClassId = ResolveStaticMethodIdentifier(
                        runtimeScopeManager, setCssClass);
                    _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetClassName"] = setCssClassId;
                }
                catch (Exception ex)
                {
                    Log.Debug("Could not resolve SetCssClass: {Error}", ex.Message);
                }

                // --- Resolve type identifiers for UISkinableElement ---
                ResolveTypeIdentifier(runtimeScopeManager, uiSkinableElementType,
                    "Sunlight.Framework.UI.UISkinableElement");

                // --- Resolve type identifiers for model types referenced in compiled templates ---
                ResolveModelTypeIdentifiers(clrContext, runtimeScopeManager);

                // --- Force resolution of event handler methods referenced in templates ---
                // This ensures the compiler emits them in the JS output, even if they're
                // not directly called from compiled C# code.
                ResolveEventHandlerMethods(clrContext, runtimeScopeManager);

                Log.Debug("Resolved {Count} runtime identifiers for Razor template JS replacement",
                    _resolvedIdentifiers.Count + _resolvedTypeIdentifiers.Count);
            }
            catch (Exception ex)
            {
                Log.Debug("Error resolving runtime identifiers: {Error}. " +
                    "Razor templates will emit unresolved identifiers.", ex.Message);
            }
        }

        /// <summary>
        /// Resolves a static method to its NScript identifier using ResolverHelper.
        /// </summary>
        private static IIdentifier ResolveStaticMethodIdentifier(
            RuntimeScopeManager runtimeScopeManager, MethodReference method)
        {
            var methodDef = method.Resolve();
            return runtimeScopeManager.ResolveStatic(methodDef);
        }

        /// <summary>
        /// Resolves a type to its NScript identifier list and stores the mapping
        /// from the Razor-generated mangled name to the resolved identifiers.
        /// </summary>
        private void ResolveTypeIdentifier(
            RuntimeScopeManager runtimeScopeManager,
            TypeReference typeRef,
            string csharpFullName)
        {
            var identifiers = runtimeScopeManager.ResolveType(typeRef);
            if (identifiers != null && identifiers.Count > 0)
            {
                // Store both double-underscore and single-underscore mangled forms
                // using "__" as the namespace separator (matching NScript's JS identifier convention)
                var mangledName = csharpFullName.Replace(".", "__");
                _resolvedTypeIdentifiers[mangledName] = identifiers;
            }
        }

        /// <summary>
        /// Resolves type identifiers for model and control types referenced in compiled templates.
        /// Uses the template IR (which stores ControlTypeName and ModelTypeName directly)
        /// rather than scanning raw JS output.
        /// </summary>
        private void ResolveModelTypeIdentifiers(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            var seen = new HashSet<string>();
            foreach (var kvp in _compiledIRs)
            {
                var ir = kvp.Value;

                if (!string.IsNullOrEmpty(ir.ControlTypeName))
                {
                    var mangledControl = ir.ControlTypeName.Replace(".", "__");
                    if (seen.Add(mangledControl))
                        TryResolveTypeFromMangled(clrContext, runtimeScopeManager, mangledControl);
                }

                if (!string.IsNullOrEmpty(ir.ModelTypeName))
                {
                    var mangledModel = ir.ModelTypeName.Replace(".", "__");
                    if (seen.Add(mangledModel))
                        TryResolveTypeFromMangled(clrContext, runtimeScopeManager, mangledModel);
                }
            }
        }

        /// <summary>
        /// Attempts to resolve a mangled type name (e.g. "Sunlight__Framework__UI__UISkinableElement")
        /// to its NScript identifier list by reconstructing the C# fully-qualified name.
        /// </summary>
        private void TryResolveTypeFromMangled(
            ClrContext clrContext, RuntimeScopeManager runtimeScopeManager, string mangledName)
        {
            if (_resolvedTypeIdentifiers.ContainsKey(mangledName))
                return;

            // Convert double-underscore mangling back to dotted C# name
            var csharpName = mangledName.Replace("__", ".");

            // Try to find the type in loaded assemblies
            foreach (var module in clrContext.Modules)
            {
                foreach (var typeDef in module.Types)
                {
                    if (typeDef.FullName == csharpName)
                    {
                        ResolveTypeIdentifier(runtimeScopeManager, typeDef, csharpName);
                        return;
                    }

                    // Check nested types
                    foreach (var nestedType in typeDef.NestedTypes)
                    {
                        if (nestedType.FullName == csharpName || nestedType.FullName.Replace("/", ".") == csharpName)
                        {
                            ResolveTypeIdentifier(runtimeScopeManager, nestedType, csharpName);
                            return;
                        }
                    }
                }
            }

            Log.Debug("Could not find type definition for mangled name {MangledName}", mangledName);
        }

        /// <summary>
        /// Resolves event handler methods referenced in template IR nodes.
        /// Calling Resolve() on the scope manager marks the method as "used",
        /// ensuring the compiler emits it in the JS output even if no compiled
        /// C# code directly calls it.
        /// </summary>
        private void ResolveEventHandlerMethods(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            var resolvedMethods = new HashSet<string>();
            foreach (var kvp in _compiledIRs)
            {
                var ir = kvp.Value;
                if (string.IsNullOrEmpty(ir.ModelTypeName)) continue;

                // Find the model type in Cecil
                TypeDefinition modelType = null;
                foreach (var module in clrContext.Modules)
                {
                    foreach (var t in module.Types)
                    {
                        if (t.FullName == ir.ModelTypeName)
                        {
                            modelType = t;
                            break;
                        }
                    }
                    if (modelType != null) break;
                }
                if (modelType == null) continue;

                // Walk IR nodes to find EventNode references
                CollectAndResolveEventMethods(ir.Children, modelType, runtimeScopeManager, resolvedMethods);
            }

            if (resolvedMethods.Count > 0)
                Log.Debug("Resolved {Count} event handler methods for template emission: {Methods}",
                    resolvedMethods.Count, string.Join(", ", resolvedMethods));
        }

        private static void CollectAndResolveEventMethods(
            List<TemplateIR.IRNode> nodes,
            TypeDefinition modelType,
            RuntimeScopeManager runtimeScopeManager,
            HashSet<string> resolved)
        {
            foreach (var node in nodes)
            {
                if (node is TemplateIR.EventNode evt)
                {
                    // Extract method name from handler expression (e.g., "Model.IncrementClick")
                    var expr = evt.HandlerExpression ?? "";
                    if (expr.StartsWith("Model."))
                        expr = expr.Substring(6);

                    // For simple method references (no parens/lambda)
                    if (expr.IndexOfAny(new[] { '(', ')', '=', '>' }) < 0)
                    {
                        var key = modelType.FullName + "." + expr;
                        if (resolved.Add(key))
                        {
                            foreach (var method in modelType.Methods)
                            {
                                if (method.Name == expr && method.IsPublic && !method.IsConstructor)
                                {
                                    runtimeScopeManager.Resolve(method);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Recurse into children
                CollectAndResolveEventMethods(node.Children, modelType, runtimeScopeManager, resolved);

                if (node is TemplateIR.ConditionalNode cond)
                {
                    CollectAndResolveEventMethods(cond.TrueBranch, modelType, runtimeScopeManager, resolved);
                    CollectAndResolveEventMethods(cond.FalseBranch, modelType, runtimeScopeManager, resolved);
                }
                else if (node is TemplateIR.LoopNode loop)
                {
                    CollectAndResolveEventMethods(loop.ItemTemplate, modelType, runtimeScopeManager, resolved);
                }
            }
        }

        /// <summary>
        /// Generates a C# source stub for the model type referenced by @model in the template.
        /// Uses Cecil type information to produce a minimal class declaration with properties,
        /// so the Roslyn analysis phase can detect observable properties and promote bindings
        /// from OneTime to OneWay.
        /// </summary>
        private string GenerateModelTypeStub(string templateSource, ClrContext clrContext)
        {
            // Extract @model type name from the template source
            string modelTypeName = null;
            foreach (var line in templateSource.Split('\n'))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("@model "))
                {
                    modelTypeName = trimmed.Substring("@model ".Length).Trim();
                    break;
                }
            }

            if (string.IsNullOrEmpty(modelTypeName))
                return null;

            // Find the type in Cecil
            TypeDefinition typeDef = null;
            foreach (var module in clrContext.Modules)
            {
                foreach (var t in module.Types)
                {
                    if (t.FullName == modelTypeName)
                    {
                        typeDef = t;
                        break;
                    }
                }
                if (typeDef != null) break;
            }

            if (typeDef == null)
            {
                Log.Debug("Could not find Cecil type {TypeName} for model stub generation", modelTypeName);
                return null;
            }

            // Determine base class
            var baseTypeName = "object";
            var currentBase = typeDef.BaseType;
            while (currentBase != null)
            {
                if (currentBase.FullName == "Sunlight.Framework.Observables.ObservableObject")
                {
                    baseTypeName = "Sunlight.Framework.Observables.ObservableObject";
                    break;
                }
                try { currentBase = currentBase.Resolve()?.BaseType; }
                catch (Exception) { break; }
            }

            // Build namespace and class declaration
            var ns = typeDef.Namespace;
            var className = typeDef.Name;
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine($"namespace {ns} {{");
            }

            sb.AppendLine($"  public class {className} : {baseTypeName} {{");

            // Generate property stubs
            foreach (var prop in typeDef.Properties)
            {
                var propTypeName = MapCecilTypeToSimpleName(prop.PropertyType);
                if (prop.GetMethod != null && prop.SetMethod != null)
                {
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; set; }}");
                }
                else if (prop.GetMethod != null)
                {
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; }}");
                }
            }

            // Generate method stubs (for event handlers)
            foreach (var method in typeDef.Methods)
            {
                if (!method.IsPublic || method.IsConstructor || method.IsGetter || method.IsSetter)
                    continue;
                var retType = MapCecilTypeToSimpleName(method.ReturnType);
                var paramStrs = method.Parameters
                    .Select(p => $"{MapCecilTypeToSimpleName(p.ParameterType)} {p.Name}");
                sb.AppendLine($"    public {retType} {method.Name}({string.Join(", ", paramStrs)}) {{ }}");
            }

            sb.AppendLine("  }");

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("}");
            }

            // Generate stubs for types referenced in collection properties AFTER
            // closing the main namespace, so they get their own proper namespace blocks.
            var referencedTypes = new HashSet<string>();
            foreach (var prop in typeDef.Properties)
            {
                if (prop.PropertyType is GenericInstanceType genPropType)
                {
                    foreach (var arg in genPropType.GenericArguments)
                    {
                        if (!arg.IsPrimitive && arg.FullName != "System.String" && arg.FullName != "System.Object")
                        {
                            referencedTypes.Add(arg.FullName);
                        }
                    }
                }
            }

            foreach (var refTypeName in referencedTypes)
            {
                GenerateReferencedTypeStub(sb, refTypeName, clrContext);
            }

            var stub = sb.ToString();
            Log.Debug("Generated model type stub for {TypeName}: {StubLength} chars, base={BaseType}",
                modelTypeName, stub.Length, baseTypeName);
            return stub;
        }

        private void GenerateReferencedTypeStub(
            System.Text.StringBuilder sb,
            string fullTypeName,
            ClrContext clrContext)
        {
            TypeDefinition refTypeDef = null;
            foreach (var module in clrContext.Modules)
            {
                foreach (var t in module.Types)
                {
                    if (t.FullName == fullTypeName)
                    {
                        refTypeDef = t;
                        break;
                    }
                }
                if (refTypeDef != null) break;
            }

            if (refTypeDef == null) return;

            // Determine base class for the referenced type
            var refBaseType = "object";
            var refBase = refTypeDef.BaseType;
            while (refBase != null)
            {
                if (refBase.FullName == "Sunlight.Framework.Observables.ObservableObject")
                {
                    refBaseType = "Sunlight.Framework.Observables.ObservableObject";
                    break;
                }
                try { refBase = refBase.Resolve()?.BaseType; }
                catch (Exception) { break; }
            }

            // If the type is in a different namespace, wrap in its own namespace block
            var refNs = refTypeDef.Namespace;
            var refClassName = refTypeDef.Name;
            bool needsNamespaceClose = false;

            // Only open a namespace if it differs from what's already open
            if (!string.IsNullOrEmpty(refNs))
            {
                sb.AppendLine($"  namespace {refNs} {{");
                needsNamespaceClose = true;
            }

            sb.AppendLine($"    public class {refClassName} : {refBaseType} {{");

            foreach (var prop in refTypeDef.Properties)
            {
                var propTypeName = MapCecilTypeToSimpleName(prop.PropertyType);
                if (prop.GetMethod != null && prop.SetMethod != null)
                    sb.AppendLine($"      public {propTypeName} {prop.Name} {{ get; set; }}");
                else if (prop.GetMethod != null)
                    sb.AppendLine($"      public {propTypeName} {prop.Name} {{ get; }}");
            }

            sb.AppendLine("    }");

            if (needsNamespaceClose)
                sb.AppendLine("  }");
        }

        /// <summary>
        /// Maps a Cecil TypeReference to a simple C# type name for stub generation.
        /// </summary>
        private static string MapCecilTypeToSimpleName(TypeReference typeRef)
        {
            if (typeRef == null) return "object";

            switch (typeRef.FullName)
            {
                case "System.String": return "string";
                case "System.Int32": return "int";
                case "System.Boolean": return "bool";
                case "System.Double": return "double";
                case "System.Single": return "float";
                case "System.Int64": return "long";
                case "System.Decimal": return "decimal";
                case "System.Object": return "object";
                case "System.Void": return "void";
            }

            // Handle generic types like ObservableCollection<RazorItemVM>
            if (typeRef is GenericInstanceType genType)
            {
                var baseName = genType.ElementType.FullName;
                // Strip arity suffix (e.g., ObservableCollection`1 → ObservableCollection)
                var arityIdx = baseName.IndexOf('`');
                if (arityIdx >= 0) baseName = baseName.Substring(0, arityIdx);
                var args = string.Join(", ", genType.GenericArguments
                    .Select(a => MapCecilTypeToSimpleName(a)));
                return $"{baseName}<{args}>";
            }

            // For other non-primitive types, return the full type name
            return typeRef.FullName;
        }

        public void ParseArgs(IList<Tuple<string, string>> args)
        {
            // No custom args needed for Razor templates
        }

        // --- IMethodConverterPlugin ---

        public IntrestLevel GetInterestLevel(
            MethodDefinition methodDefinition,
            ConverterContext converterContext)
        {
            Log.Verbose("Checking interest level for method {MethodName}", methodDefinition.FullName);

            // Check if this is a [Skin("...")] property getter where the template
            // name corresponds to a compiled .skin.cshtml template
            PropertyDefinition propertyDefinition = methodDefinition.GetPropertyDefinition();
            if (propertyDefinition == null) return IntrestLevel.None;
            if (propertyDefinition.SetMethod != null) return IntrestLevel.None;

            var skinAttr = propertyDefinition.CustomAttributes?.FirstOrDefault(
                a => a.AttributeType.Name == "SkinAttribute" ||
                     a.AttributeType.FullName.EndsWith(".SkinAttribute"));

            if (skinAttr == null) return IntrestLevel.None;

            // Check if the template name is a Razor template
            if (skinAttr.HasConstructorArguments)
            {
                var templateName = skinAttr.ConstructorArguments[0].Value as string;
                if (templateName != null && _compiledIRs.ContainsKey(templateName))
                {
                    Log.Debug("[Skin] match found for method {MethodName} with template {TemplateName}",
                        methodDefinition.FullName, templateName);
                    return IntrestLevel.Overwrite;
                }
            }

            return IntrestLevel.None;
        }

        // Not used: RazorTemplatingPlugin only returns IntrestLevel.Overwrite or None.
        public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter) => null;

        public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter) => null;

        public List<Statement> GetEncapsulationStatements(
            MethodConverter methodConverter,
            List<Statement> methodStatments) => null;

        public List<Statement> GetOverwrite(MethodConverter methodConverter)
        {
            var propertyDefinition = methodConverter.MethodDefinition.GetPropertyDefinition();
            var skinAttr = propertyDefinition.CustomAttributes?.FirstOrDefault(
                a => a.AttributeType.Name == "SkinAttribute" ||
                     a.AttributeType.FullName.EndsWith(".SkinAttribute"));

            var templateName = (skinAttr?.HasConstructorArguments == true && skinAttr.ConstructorArguments.Count > 0)
                ? skinAttr.ConstructorArguments[0].Value as string : null;
            if (templateName == null || !_compiledIRs.ContainsKey(templateName))
            {
                return null;
            }

            // Look up the short template name (used as the JS getter function name)
            var shortName = _templateShortNames.ContainsKey(templateName)
                ? _templateShortNames[templateName] : templateName;

            Log.Debug("Resolved template {TemplateName} (short: {ShortName}) for overwrite", templateName, shortName);

            // Use the JST getter identifier if available (from JST generation in GetPostJavascript),
            // otherwise fall back to raw JS with the short name.
            if (_templateGetterIdentifiers.TryGetValue(shortName, out var getterId))
            {
                var scope = methodConverter.Scope;
                return new List<Statement>
                {
                    new ReturnStatement(
                        null,
                        scope,
                        new MethodCallExpression(
                            null,
                            scope,
                            new IdentifierExpression(getterId, scope)))
                };
            }

            // Fallback: use scope-registered identifier for the getter function.
            // RawNameIdentifier participates in scope naming but won't be minified
            // since the original name is used as the suggested name.
            var fallbackScope = _runtimeScopeManager.Scope;
            var fallbackId = RawNameIdentifier.Create(fallbackScope, shortName);
            return new List<Statement>
            {
                new ReturnStatement(
                    null,
                    fallbackScope,
                    new MethodCallExpression(
                        null,
                        fallbackScope,
                        new IdentifierExpression(fallbackId, fallbackScope)))
            };
        }

        // --- IRuntimeConverterPlugin ---

        public List<MethodReference> GetMethodsToEmitPass1()
        {
            return new List<MethodReference>();
        }

        public List<MethodReference> GetMethodsToEmitPassN()
        {
            // After XWML's pass has run and created DocStorageGetter, look it up
            // in the shared scope so Razor templates can reference it by the correct name.
            if (_hasRazorTemplates && !_resolvedIdentifiers.ContainsKey("DocStorageGetter"))
            {
                TryResolveDocStorageGetter();
            }

            // Collect all event handler methods from Razor templates so the tree-shaker
            // includes them. Template-bound methods are only referenced from JST graph
            // descriptors (emitted in GetPostJavascript), which runs AFTER tree-shaking.
            // By registering them here, the methods are marked as used during the
            // main conversion loop where WalkUsedDependencies() processes the queue.
            var methods = new List<MethodReference>();
            if (_hasRazorTemplates && _clrContext != null)
            {
                var visited = new HashSet<SkinTemplateNode>();
                foreach (var kvp in _compiledIRs)
                {
                    var ir = kvp.Value;
                    if (visited.Contains(ir)) continue; // Skip duplicates (short name + resource name)
                    visited.Add(ir);
                    Log.Debug("GetMethodsToEmitPassN: scanning template {Name}, modelType={ModelType}, children={Count}",
                        kvp.Key, ir.ModelTypeName ?? "(null)", ir.Children?.Count ?? 0);
                    CollectEventMethodReferences(ir.Children, ir.ModelTypeName, null, methods);
                }
                Log.Debug("GetMethodsToEmitPassN: found {Count} event method references", methods.Count);
            }
            return methods;
        }

        /// <summary>
        /// Recursively walks IR nodes to find EventNode objects and resolve their handler
        /// methods to MethodReference objects for the tree-shaker.
        /// </summary>
        private void CollectEventMethodReferences(
            List<IRNode> nodes, string modelTypeName, string itemVarPrefix,
            List<MethodReference> methods)
        {
            if (nodes == null) return;
            foreach (var node in nodes)
            {
                if (node is EventNode evt && !string.IsNullOrEmpty(evt.HandlerExpression))
                {
                    var methodRef = ResolveEventMethodReference(
                        evt.HandlerExpression, modelTypeName, itemVarPrefix);
                    if (methodRef != null)
                        methods.Add(methodRef);
                }
                else if (node is LoopNode loop)
                {
                    var itemTypeName = ResolveCollectionItemType(loop.CollectionExpression, modelTypeName);
                    var ivp = loop.ItemVariableName + ".";
                    CollectEventMethodReferences(loop.ItemTemplate, itemTypeName ?? modelTypeName, ivp, methods);
                }
                else if (node is ConditionalNode cond)
                {
                    CollectEventMethodReferences(cond.TrueBranch, modelTypeName, itemVarPrefix, methods);
                    CollectEventMethodReferences(cond.FalseBranch, modelTypeName, itemVarPrefix, methods);
                }

                // Recurse into generic children
                if (node.Children != null && node.Children.Count > 0)
                    CollectEventMethodReferences(node.Children, modelTypeName, itemVarPrefix, methods);
            }
        }

        /// <summary>
        /// Resolves an event handler expression (e.g., "folder.OnSelect" or "Model.SelectedTodo.OnTitleChange")
        /// to a MethodReference by walking the property chain to find the declaring type.
        /// </summary>
        private MethodReference ResolveEventMethodReference(
            string handlerExpression, string modelTypeName, string itemVarPrefix)
        {
            if (string.IsNullOrEmpty(modelTypeName)) return null;

            var expr = handlerExpression;
            if (expr.StartsWith("Model."))
                expr = expr.Substring(6);
            if (!string.IsNullOrEmpty(itemVarPrefix) && expr.StartsWith(itemVarPrefix))
                expr = expr.Substring(itemVarPrefix.Length);

            // Skip lambdas and complex expressions
            if (expr.IndexOfAny(new[] { '(', ')', '=', '>' }) >= 0)
                return null;

            // Handle chained paths like "SelectedTodo.OnTitleChange" by walking properties
            var currentTypeName = modelTypeName;
            var parts = expr.Split('.');
            for (int i = 0; i < parts.Length - 1; i++)
            {
                var propName = parts[i];
                var typeDef = FindTypeDefinitionByName(currentTypeName);
                if (typeDef == null) return null;

                string nextTypeName = null;
                foreach (var prop in typeDef.Properties)
                {
                    if (prop.Name == propName)
                    {
                        nextTypeName = prop.PropertyType.FullName;
                        break;
                    }
                }
                if (nextTypeName == null) return null;
                currentTypeName = nextTypeName;
            }

            var methodName = parts[parts.Length - 1];
            var targetType = FindTypeDefinitionByName(currentTypeName);
            if (targetType == null) return null;

            foreach (var method in targetType.Methods)
            {
                if (method.Name == methodName && method.IsPublic && !method.IsConstructor)
                    return method;
            }
            return null;
        }

        /// <summary>
        /// Resolves the item type of a collection expression (e.g., "Model.Folders" → "FolderViewModel").
        /// </summary>
        private string ResolveCollectionItemType(string collectionExpression, string modelTypeName)
        {
            if (string.IsNullOrEmpty(collectionExpression) || string.IsNullOrEmpty(modelTypeName))
                return null;

            var propName = collectionExpression;
            if (propName.StartsWith("Model."))
                propName = propName.Substring(6);
            if (propName.Contains("."))
                return null;

            var modelType = FindTypeDefinitionByName(modelTypeName);
            if (modelType == null) return null;

            foreach (var prop in modelType.Properties)
            {
                if (prop.Name == propName)
                {
                    var returnType = prop.PropertyType as Mono.Cecil.GenericInstanceType;
                    if (returnType != null && returnType.GenericArguments.Count > 0)
                        return returnType.GenericArguments[0].FullName;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a TypeDefinition by full name across all loaded modules.
        /// </summary>
        private Mono.Cecil.TypeDefinition FindTypeDefinitionByName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return null;
            foreach (var module in _clrContext.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == fullName) return type;
                    foreach (var nested in type.NestedTypes)
                    {
                        if (nested.FullName == fullName) return nested;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Flag set when the Razor plugin had to create its own DocStorageGetter
        /// identifier because XWML did not provide one.  When true,
        /// <see cref="GetPostJavascript"/> must also emit the function body.
        /// </summary>
        private bool _needsDocStorageGetterEmission;

        /// <summary>
        /// Searches the runtime scope for the DocStorageGetter identifier created by
        /// the XWML CodeGenerator. If not found (e.g. XWML plugin has no templates
        /// or is absent), creates the identifier in scope so the Razor factory
        /// emits a resolved (minification-safe) call, and sets a flag so that
        /// <see cref="GetPostJavascript"/> emits the function body.
        /// </summary>
        private void TryResolveDocStorageGetter()
        {
            var scope = _runtimeScopeManager.Scope;
            foreach (var identifier in scope.ScopedIdentifiers)
            {
                if (identifier.OriginalSuggestedName == "DocStorageGetter")
                {
                    _resolvedIdentifiers["DocStorageGetter"] = identifier;
                    Log.Debug("Found DocStorageGetter identifier in scope");
                    return;
                }
            }

            // Not found — create it ourselves so the emitted call is minification-safe.
            var newId = SimpleIdentifier.CreateScopeIdentifier(scope, "DocStorageGetter", false);
            _resolvedIdentifiers["DocStorageGetter"] = newId;
            _needsDocStorageGetterEmission = true;
            Log.Debug("Created DocStorageGetter identifier (XWML not active); will emit function body");
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        public List<Statement> GetPostJavascript()
        {
            if (!_hasRazorTemplates)
                return new List<Statement>();

            // Re-attempt DocStorageGetter resolution here because XWML's GetPostJavascript()
            // creates the identifier lazily during template emission.  GetMethodsToEmitPassN()
            // runs before GetPostJavascript(), so the first attempt may have been too early.
            if (!_resolvedIdentifiers.ContainsKey("DocStorageGetter"))
            {
                TryResolveDocStorageGetter();
            }

            var statements = new List<Statement>();

            // Track which template IRs we've already emitted to avoid duplicates
            // (_compiledIRs stores the same IR under both short name and resource name)
            var emittedTemplates = new HashSet<string>(StringComparer.Ordinal);

            foreach (var kvp in _compiledIRs)
            {
                if (!emittedTemplates.Add(kvp.Value.TemplateName))
                    continue; // Skip duplicate entries

                try
                {
                    // Generate proper JST nodes with graph descriptor emission
                    IIdentifier preCreatedGetter = null;
                    _templateGetterIdentifiers.TryGetValue(kvp.Value.TemplateName, out preCreatedGetter);

                    var jstGenerator = new RazorSkinJSTGenerator(
                        kvp.Value,
                        _runtimeScopeManager,
                        _clrContext,
                        _resolvedIdentifiers,
                        _resolvedTypeIdentifiers,
                        _razorKnownTypes,
                        preCreatedGetter);

                    var jstStatements = jstGenerator.Generate();
                    statements.AddRange(jstStatements);

                    // Store the getter identifier for use in GetOverwrite
                    var getterIdentifier = jstGenerator.GetGetterIdentifier();
                    if (getterIdentifier != null)
                    {
                        _templateGetterIdentifiers[kvp.Value.TemplateName] = getterIdentifier;
                    }

                    Log.Debug("Generated {StatementCount} JST statements for template {TemplateName}",
                        jstStatements.Count, kvp.Value.TemplateName);
                }
                catch (System.Exception ex)
                {
                    Log.Error(ex, "JST generation failed for template {TemplateName}", kvp.Value.TemplateName);

                    _runtimeScopeManager.Context.AddError(
                        null,
                        $"Error generating JST for Razor template '{kvp.Value.TemplateName}': {ex.Message}",
                        false);
                }
            }

            // If Razor created its own DocStorageGetter identifier (no XWML), emit the function body.
            if (_needsDocStorageGetterEmission)
            {
                var docStorageGetterStatements = EmitDocStorageGetterFunction();
                if (docStorageGetterStatements != null)
                    statements.AddRange(docStorageGetterStatements);
            }

            Log.Debug("GetPostJavascript emitting {StatementCount} statements for {TemplateCount} templates",
                statements.Count, emittedTemplates.Count);

            return statements;
        }

        /// <summary>
        /// Emits the DocStorageGetter function when the XWML plugin is not present.
        /// The function initialises a stateStore array on the document object and returns it.
        /// This mirrors the function generated by XWML's CodeGenerator.GenerateDocumentInitializerMethod().
        /// </summary>
        private List<Statement> EmitDocStorageGetterFunction()
        {
            try
            {
                var scope = _runtimeScopeManager.Scope;
                IIdentifier docStorageGetterId = _resolvedIdentifiers["DocStorageGetter"];

                // Look up the Document type so we can create the stateStore field identifier on it.
                var documentTypeDef = _clrContext.GetTypeDefinition(
                    Tuple.Create("System.Web.Html", "System.Web.Html.Document"));

                if (documentTypeDef == null)
                {
                    Log.Warning("Could not find Document type for DocStorageGetter emission");
                    return null;
                }

                // Get or create the 'stateStore' extension field on Document's type scope.
                IIdentifier stateStoreId = _runtimeScopeManager.GetTypeScope(documentTypeDef)
                    .GetIdentifier("stateStore", true, false);

                // Build: function DocStorageGetter(doc) { if (!doc.stateStore) { doc.stateStore = []; } return doc.stateStore; }
                var methodScope = new IdentifierScope(
                    scope,
                    new string[] { "doc" },
                    false);

                IIdentifier docParam = methodScope.ParameterIdentifiers[0];

                // doc.stateStore = []
                var initStmts = new List<Statement>();
                initStmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, stateStoreId }),
                        new NewArrayExpression(null, methodScope, null)));

                // if (!doc.stateStore) { doc.stateStore = []; }
                var ifStmt = new IfBlockStatement(
                    null, methodScope,
                    new UnaryExpression(
                        null, methodScope,
                        UnaryOperator.LogicalNot,
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, stateStoreId })),
                    new ScopeBlock(null, methodScope, initStmts),
                    null);

                // function DocStorageGetter(doc) { ... }
                var funcExpr = new FunctionExpression(
                    null, scope, methodScope,
                    methodScope.ParameterIdentifiers,
                    docStorageGetterId);

                funcExpr.AddStatement(ifStmt);

                // return doc.stateStore;
                funcExpr.AddStatement(
                    new ReturnStatement(
                        null, methodScope,
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, stateStoreId })));

                return new List<Statement>
                {
                    new ExpressionStatement(null, scope, funcExpr)
                };
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to emit DocStorageGetter function");
                return null;
            }
        }
    }
}
