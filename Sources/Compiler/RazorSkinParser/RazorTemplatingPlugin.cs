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
        /// Map of template name to compiled JS output.
        /// Populated during Initialize when embedded .skin.cshtml resources are found.
        /// </summary>
        private readonly Dictionary<string, string> _compiledTemplates = new Dictionary<string, string>();

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
        /// These are always passed to RazorSkinCompiler.Compile so that the Roslyn analysis
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

        public static string CompileTemplate(string filePath, string[] frameworkSources)
        {
            var templateName = Path.GetFileNameWithoutExtension(
                Path.GetFileNameWithoutExtension(filePath)); // Remove .skin.cshtml
            var templateSource = File.ReadAllText(filePath);

            return RazorSkinCompiler.Compile(templateName, templateSource, frameworkSources);
        }

        // --- IConverterPlugin ---

        public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            _clrContext = clrContext;
            _runtimeScopeManager = runtimeScopeManager;

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

                            var (ir, js) = RazorSkinCompiler.CompileWithIR(
                                templateName, templateSource,
                                additionalSources);
                            // Store under both short name and full resource name
                            // so [Skin("full.resource.name.skin.cshtml")] matches
                            _compiledTemplates[templateName] = js;
                            _compiledTemplates[embeddedResource.Name] = js;
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

                // --- Resolve SkinInstance constructor factory ---
                var skinInstanceCtor = clrContext.GetMethodReference(
                    ".ctor", clrKnownRefs.Void, skinInstanceType,
                    skinType, elementRefType, nativeArrayInt,
                    nativeArray, nativeArraySkinBinderInfo,
                    clrKnownRefs.Object, clrKnownRefs.Int32, clrKnownRefs.Int32).Resolve();

                var skinInstanceFactoryId = runtimeScopeManager.ResolveFactory(skinInstanceCtor);
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
                // since RazorSkinCodeGenerator.MangleTypeName uses "__"
                var mangledName = csharpFullName.Replace(".", "__");
                _resolvedTypeIdentifiers[mangledName] = identifiers;
            }
        }

        /// <summary>
        /// Scans compiled template JS to find mangled type names and resolves them.
        /// Looks for patterns like MangleTypeName(ir.ControlTypeName) and MangleTypeName(ir.ModelTypeName)
        /// which appear as arguments to Skin_factory calls.
        /// </summary>
        private void ResolveModelTypeIdentifiers(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            // Scan all compiled JS for type references in Skin_factory calls
            // Pattern: Skin_factory(ControlType, ModelType, factoryFunc, "index")
            var typeNameRegex = new System.Text.RegularExpressions.Regex(
                @"Skin_factory\((\w+),\s*(\w+),");

            foreach (var kvp in _compiledTemplates)
            {
                var match = typeNameRegex.Match(kvp.Value);
                if (!match.Success) continue;

                var controlTypeMangle = match.Groups[1].Value;
                var modelTypeMangle = match.Groups[2].Value;

                // Try to resolve each mangled type name
                TryResolveTypeFromMangled(clrContext, runtimeScopeManager, controlTypeMangle);
                TryResolveTypeFromMangled(clrContext, runtimeScopeManager, modelTypeMangle);
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
                catch { break; }
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

            sb.AppendLine("  }");

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("}");
            }

            var stub = sb.ToString();
            Log.Debug("Generated model type stub for {TypeName}: {StubLength} chars, base={BaseType}",
                modelTypeName, stub.Length, baseTypeName);
            return stub;
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
                default: return "object";
            }
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
                if (templateName != null && _compiledTemplates.ContainsKey(templateName))
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
            if (templateName == null || !_compiledTemplates.ContainsKey(templateName))
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

            // Fallback: emit raw JS
            return new List<Statement>
            {
                new RawJavaScriptStatement($"return {shortName}();")
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

            return new List<MethodReference>();
        }

        /// <summary>
        /// Searches the runtime scope for the DocStorageGetter identifier created by
        /// the XWML CodeGenerator. This must be called after XWML's GetMethodsToEmitPassN
        /// has run so the identifier exists in the scope.
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

            Log.Debug("DocStorageGetter identifier not found in scope — XWML plugin may not be active");
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        public List<Statement> GetPostJavascript()
        {
            if (!_hasRazorTemplates)
                return new List<Statement>();

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
                    // Generate proper JST nodes using the template IR
                    // Pass the pre-created getter identifier so it matches what GetOverwrite references
                    IIdentifier preCreatedGetter = null;
                    _templateGetterIdentifiers.TryGetValue(kvp.Value.TemplateName, out preCreatedGetter);

                    var jstGenerator = new RazorSkinJSTGenerator(
                        kvp.Value,
                        _runtimeScopeManager,
                        _clrContext,
                        _resolvedIdentifiers,
                        _resolvedTypeIdentifiers,
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
                    // Fallback to ResolvedJavaScriptStatement if JST generation fails
                    Log.Debug("JST generation failed for template {TemplateName}: {Error}. " +
                        "Falling back to ResolvedJavaScriptStatement.",
                        kvp.Value.TemplateName, ex.Message);

                    if (_compiledTemplates.TryGetValue(kvp.Key, out var js))
                    {
                        if (_resolvedIdentifiers.Count > 0 || _resolvedTypeIdentifiers.Count > 0)
                        {
                            statements.Add(new ResolvedJavaScriptStatement(
                                js, _resolvedIdentifiers, _resolvedTypeIdentifiers));
                        }
                        else
                        {
                            statements.Add(new RawJavaScriptStatement(js));
                        }
                    }
                }
            }

            Log.Debug("GetPostJavascript emitting {StatementCount} statements for {TemplateCount} templates",
                statements.Count, emittedTemplates.Count);

            return statements;
        }
    }
}
