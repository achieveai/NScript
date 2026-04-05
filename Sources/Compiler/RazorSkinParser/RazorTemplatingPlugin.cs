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
        private CecilTypeHelper _typeHelper;
        private CecilModelStubGenerator _stubGenerator;

        /// <summary>
        /// Per-compilation data index counter for Razor templates.
        /// Starts at 100 to avoid collision with XWML's sequential indices (starting from 0).
        /// </summary>
        private int _nextDataIndex = 100;

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
        /// Flag set when the Razor plugin had to create its own DocStorageGetter
        /// identifier because XWML was not active. When true,
        /// <see cref="GetPostJavascript"/> must also emit the function body.
        /// </summary>
        private bool _needsDocStorageGetterEmission;

        /// <summary>
        /// Resolved runtime types needed for graph descriptor JST emission.
        /// Created during Initialize when Razor templates are found.
        /// </summary>
        private RazorKnownTypes _razorKnownTypes;

        /// <summary>
        /// Per-template CSS managers, keyed by template name.
        /// Populated during Initialize when templates have @styles directives.
        /// </summary>
        private readonly Dictionary<string, RazorCssManager> _templateCssManagers
            = new Dictionary<string, RazorCssManager>();

        /// <summary>
        /// Global CSS class map: class name → IIdentifier (from CSS scope).
        /// Built from [CssClass] attribute scanning across all assemblies.
        /// Used by CssLiteralReplacer to swap string literals → IdentifierStringExpression.
        /// </summary>
        private readonly Dictionary<string, IIdentifier> _cssClassMap
            = new Dictionary<string, IIdentifier>();

        /// <summary>
        /// Lazily initialized CSS literal replacer.
        /// Created after [CssClass] scanning if any CSS classes were registered.
        /// </summary>
        private CssLiteralReplacer _cssLiteralReplacer;

        /// <summary>
        /// All embedded CSS resources found during module scanning, keyed by resource name.
        /// Used to resolve @styles references to actual CSS content.
        /// </summary>
        private readonly Dictionary<string, EmbeddedResource> _cssResources
            = new Dictionary<string, EmbeddedResource>();

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
            _typeHelper = new CecilTypeHelper(clrContext);
            _stubGenerator = new CecilModelStubGenerator(clrContext);

            // Reset per-compilation data index counter
            _nextDataIndex = 100;

            // Scan embedded resources for .skin.cshtml and .css files
            foreach (var module in clrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    var embeddedResource = resource as EmbeddedResource;
                    if (embeddedResource == null) continue;

                    // Collect CSS resources for @styles resolution
                    if (embeddedResource.Name.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                    {
                        _cssResources[embeddedResource.Name] = embeddedResource;
                        Log.Debug("Discovered CSS resource {ResourceName}", embeddedResource.Name);
                    }

                    var fileName = runtimeScopeManager.Context.GetResourceFileName(
                        module, embeddedResource.Name);

                    if (fileName != null && CanHandle(fileName))
                    {
                        try
                        {
                            using var stream = embeddedResource.GetResourceStream();
                            Log.Debug("Discovered .skin.cshtml resource {ResourceName} (size {ResourceSize} bytes)",
                                embeddedResource.Name, stream.Length);
                            using var reader = new StreamReader(stream);
                            var templateSource = reader.ReadToEnd();

                            var templateName = Path.GetFileNameWithoutExtension(
                                Path.GetFileNameWithoutExtension(fileName));

                            // Generate C# stubs for the model type from Cecil type info.
                            // This allows the Roslyn analysis phase to detect observable
                            // properties and promote bindings from OneTime to OneWay.
                            var modelTypeStub = _stubGenerator.GenerateModelTypeStub(templateSource);
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
                            Log.Error(ex, "Compilation failed for resource {ResourceName}", embeddedResource.Name);

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
                try
                {
                    _razorKnownTypes = new RazorKnownTypes(clrContext, runtimeScopeManager.Context.ClrKnownReferences);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Could not create RazorKnownTypes — graph descriptor emission will fail");

                    runtimeScopeManager.Context.AddError(
                        null,
                        $"Could not create RazorKnownTypes: {ex.Message}",
                        false);
                }

                ResolveRuntimeIdentifiers(clrContext, runtimeScopeManager);

                // Load CSS for templates with @styles directives
                LoadCssForTemplates(runtimeScopeManager);

                // Scan [CssClass] const fields and enable minification
                ScanCssClassAttributes(runtimeScopeManager);
            }
        }

        /// <summary>
        /// Loads CSS for templates with @styles directives.
        /// Creates a RazorCssManager per template, loads referenced CSS from embedded resources,
        /// validates class usage, and optimizes names for minification.
        /// </summary>
        private void LoadCssForTemplates(RuntimeScopeManager runtimeScopeManager)
        {
            // Deduplicate — _compiledIRs stores each IR under both short name and resource name
            var processedTemplates = new HashSet<string>(StringComparer.Ordinal);

            foreach (var kvp in _compiledIRs)
            {
                var ir = kvp.Value;
                if (!processedTemplates.Add(ir.TemplateName))
                    continue;

                if (ir.StylesheetResourceNames == null || ir.StylesheetResourceNames.Count == 0)
                    continue;

                try
                {
                    var cssManager = new RazorCssManager();

                    foreach (var cssResourceName in ir.StylesheetResourceNames)
                    {
                        EmbeddedResource cssResource;
                        if (!_cssResources.TryGetValue(cssResourceName, out cssResource))
                        {
                            runtimeScopeManager.Context.AddError(
                                null,
                                $"CSS resource '{cssResourceName}' referenced by @styles in template " +
                                $"'{ir.TemplateName}' was not found as an embedded resource.",
                                false);
                            continue;
                        }

                        using var stream = cssResource.GetResourceStream();
                        using var reader = new StreamReader(stream);
                        var cssText = reader.ReadToEnd();

                        cssManager.AddStylesheet(cssResourceName, cssText);
                        Log.Debug("Loaded CSS {ResourceName} for template {TemplateName}",
                            cssResourceName, ir.TemplateName);
                    }

                    if (cssManager.HasStylesheets)
                    {
                        // Validate CSS variables are declared
                        cssManager.ValidateCssVariables();

                        // Validate class names used in template HTML
                        TemplateIR.TemplateIRBuilder.ValidateCssClasses(ir, cssManager);

                        // Note: CompressNames() is called later in ScanCssClassAttributes()
                        // once [CssClass] const fields are validated, ensuring all dynamic
                        // class references are tracked before minification.

                        _templateCssManagers[ir.TemplateName] = cssManager;

                        Log.Debug("CSS loaded for template {TemplateName}: {SheetCount} sheets",
                            ir.TemplateName, cssManager.Sheets.Count);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "CSS loading failed for template {TemplateName}", ir.TemplateName);
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"Error loading CSS for Razor template '{ir.TemplateName}': {ex.Message}",
                        false);
                }
            }
        }

        /// <summary>
        /// Scans assembly types for [CssClass] const string fields.
        /// Validates each field and registers its value in the global CSS class map.
        /// Must be called after LoadCssForTemplates so CSS managers are available.
        /// </summary>
        private void ScanCssClassAttributes(RuntimeScopeManager runtimeScopeManager)
        {
            if (_templateCssManagers.Count == 0) return;

            foreach (var module in _clrContext.Modules)
            {
                foreach (var type in module.Types)
                {
                    ScanTypeForCssClassFields(type, runtimeScopeManager);
                    if (type.HasNestedTypes)
                    {
                        foreach (var nested in type.NestedTypes)
                            ScanTypeForCssClassFields(nested, runtimeScopeManager);
                    }
                }
            }

            if (_cssClassMap.Count > 0)
            {
                // Always run CompressNames so identifiers get assigned names.
                // In debug mode (releaseNaming: false), names become "original_XY"
                // (e.g., "pane-left_a") proving the pipeline is active.
                // In release/minify mode, names become pure short ("a").
                // TODO: Accept minify flag from Builder to switch releaseNaming.
                foreach (var mgr in _templateCssManagers.Values)
                    mgr.CompressNames(releaseNaming: false);

                _cssLiteralReplacer = new CssLiteralReplacer(_cssClassMap);

                Log.Information("Registered {Count} [CssClass] const fields, CSS literal replacement enabled",
                    _cssClassMap.Count);
            }
        }

        private void ScanTypeForCssClassFields(TypeDefinition type, RuntimeScopeManager runtimeScopeManager)
        {
            if (!type.HasFields) return;

            foreach (var field in type.Fields)
            {
                if (field.CustomAttributes == null || field.CustomAttributes.Count == 0) continue;

                var cssClassAttr = field.CustomAttributes.FirstOrDefault(
                    a => a.AttributeType.Name == "CssClassAttribute" ||
                         a.AttributeType.FullName.EndsWith(".CssClassAttribute"));

                if (cssClassAttr == null) continue;

                if (!field.HasConstant || field.FieldType.FullName != "System.String")
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass] can only be applied to const string fields. " +
                        $"'{type.FullName}.{field.Name}' is not a const string.",
                        false);
                    continue;
                }

                // Parse attribute argument: "ResourceName:ClassName"
                if (!cssClassAttr.HasConstructorArguments)
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass] on '{type.FullName}.{field.Name}' is missing the " +
                        $"CSS class reference argument (format: \"ResourceName:ClassName\").",
                        false);
                    continue;
                }

                var reference = cssClassAttr.ConstructorArguments[0].Value as string;
                if (string.IsNullOrEmpty(reference) || !reference.Contains(":"))
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass(\"{reference}\")] on '{type.FullName}.{field.Name}' has invalid format. " +
                        $"Expected \"EmbeddedResourceName:CssClassName\".",
                        false);
                    continue;
                }

                var colonIdx = reference.LastIndexOf(':');
                var resourceName = reference.Substring(0, colonIdx);
                var className = reference.Substring(colonIdx + 1);

                // Find the CSS manager for this resource
                RazorCssManager targetManager = null;
                foreach (var kvp in _templateCssManagers)
                {
                    var manager = kvp.Value;
                    foreach (var sheet in manager.Sheets)
                    {
                        if (sheet.ResourceName == resourceName)
                        {
                            targetManager = manager;
                            break;
                        }
                    }
                    if (targetManager != null) break;
                }

                if (targetManager == null)
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass(\"{reference}\")] on '{type.FullName}.{field.Name}': " +
                        $"CSS resource '{resourceName}' not found. Make sure the resource is " +
                        $"loaded via @styles directive in a .skin.cshtml template.",
                        false);
                    continue;
                }

                // Validate className exists in CSS
                IIdentifier cssId;
                if (!targetManager.TryGetCssClassIdentifier(className, out cssId))
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass(\"{reference}\")] on '{type.FullName}.{field.Name}': " +
                        $"CSS class '{className}' not found in resource '{resourceName}'.",
                        false);
                    continue;
                }

                // Validate const value matches className
                var constValue = field.Constant as string;
                if (constValue != className)
                {
                    runtimeScopeManager.Context.AddError(
                        null,
                        $"[CssClass(\"{reference}\")] on '{type.FullName}.{field.Name}': " +
                        $"const value \"{constValue}\" doesn't match CSS class name \"{className}\".",
                        false);
                    continue;
                }

                // Register in global map
                if (!_cssClassMap.ContainsKey(constValue))
                {
                    _cssClassMap[constValue] = cssId;
                    Log.Debug("Registered CSS class '{ClassName}' from {TypeName}.{FieldName}",
                        className, type.FullName, field.Name);
                }
            }
        }

        /// <summary>
        /// Emits CSS from Razor templates as JST statements.
        /// Creates a &lt;style&gt; element, sets textContent to the serialized CSS, and appends to document.head.
        /// Also contributes to ConverterContext for XWML merge (when XWML plugin is active).
        /// </summary>
        private List<Statement> EmitCssStatements()
        {
            var result = new List<Statement>();
            if (_templateCssManagers.Count == 0) return result;

            // Collect all serialized CSS
            var allCss = new System.Text.StringBuilder();
            var emittedManagers = new HashSet<RazorCssManager>();
            foreach (var cssManager in _templateCssManagers.Values)
            {
                if (!emittedManagers.Add(cssManager))
                    continue;

                var css = cssManager.GetSerializedCss();
                if (!string.IsNullOrEmpty(css))
                    allCss.Append(css);
            }

            if (allCss.Length == 0) return result;

            var cssText = allCss.ToString();

            // Also contribute to ConverterContext for XWML merge (if XWML plugin is active)
            _runtimeScopeManager?.Context?.AddCssContribution(cssText);

            // Emit standalone <style> element creation via IIFE:
            // (function(d){var s=d.createElement("style");s.textContent="...";d.head.appendChild(s)})(document)
            var scope = _runtimeScopeManager.Scope;
            var iifeScope = new IdentifierScope(scope, new[] { "d" }, false);
            var docParam = iifeScope.ParameterIdentifiers[0];

            var styleVar = SimpleIdentifier.CreateScopeIdentifier(iifeScope, "s", true);
            var iifeName = SimpleIdentifier.CreateScopeIdentifier(scope, "_razorCssInit", false);
            var body = new List<Statement>();

            // s = d.createElement("style")
            body.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(styleVar, iifeScope),
                    new MethodCallExpression(
                        null,
                        iifeScope,
                        new IndexExpression(
                            null, iifeScope,
                            new IdentifierExpression(docParam, iifeScope),
                            new StringLiteralExpression(iifeScope, "createElement")),
                        new StringLiteralExpression(iifeScope, "style"))));

            // s.textContent = "...css..."
            body.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null, iifeScope,
                        new IdentifierExpression(styleVar, iifeScope),
                        new StringLiteralExpression(iifeScope, "textContent")),
                    new StringLiteralExpression(iifeScope, cssText)));

            // d.head.appendChild(s)
            body.Add(
                new ExpressionStatement(
                    null,
                    iifeScope,
                    new MethodCallExpression(
                        null,
                        iifeScope,
                        new IndexExpression(
                            null, iifeScope,
                            new IndexExpression(
                                null, iifeScope,
                                new IdentifierExpression(docParam, iifeScope),
                                new StringLiteralExpression(iifeScope, "head")),
                            new StringLiteralExpression(iifeScope, "appendChild")),
                        new IdentifierExpression(styleVar, iifeScope))));

            // Wrap in IIFE: (function(d){ ... })(document)
            var iifeFunc = new FunctionExpression(
                null, scope, iifeScope,
                iifeScope.ParameterIdentifiers,
                iifeName);
            iifeFunc.AddStatements(body);

            var iife = new MethodCallExpression(
                null, scope, iifeFunc,
                new IdentifierExpression(
                    RawNameIdentifier.Create(scope, "document"), scope));

            result.Add(new ExpressionStatement(null, scope, iife));

            Log.Debug("Emitted CSS style element with {CssLength} chars", cssText.Length);
            return result;
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

            // Use types from _razorKnownTypes when available to avoid redundant lookups.
            // Fall back to direct lookup if RazorKnownTypes creation failed.
            var skinType = _razorKnownTypes?.SkinType
                ?? clrContext.GetTypeDefinition(Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Skin"));
            var skinInstanceType = _razorKnownTypes?.SkinInstanceType
                ?? clrContext.GetTypeDefinition(Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinInstance"));
            var skinBinderInfoType = _razorKnownTypes?.SkinBinderInfoType
                ?? clrContext.GetTypeDefinition(Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderInfo"));
            var binderHelperType = _razorKnownTypes?.BinderHelperType
                ?? clrContext.GetTypeDefinition(Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderHelper"));
            var elementRefType = _razorKnownTypes?.ElementRefType
                ?? clrContext.GetTypeDefinition(Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Element"));
            var documentRefType = _razorKnownTypes?.DocumentRefType
                ?? clrContext.GetTypeDefinition(Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Document"));
            var nodeRefType = _razorKnownTypes?.NodeRefType
                ?? clrContext.GetTypeDefinition(Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Node"));
            var uiSkinableElementType = _razorKnownTypes?.UISkinableElement
                ?? clrContext.GetTypeDefinition(Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UISkinableElement"));

            // --- Resolve constructor factories ---
            try
            {
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

                var funcObjObj = new GenericInstanceType(func2);
                funcObjObj.GenericArguments.Add(clrKnownRefs.Object);
                funcObjObj.GenericArguments.Add(clrKnownRefs.Object);

                var act2ObjObj = new GenericInstanceType(act2);
                act2ObjObj.GenericArguments.Add(clrKnownRefs.Object);
                act2ObjObj.GenericArguments.Add(clrKnownRefs.Object);

                var nativeArray1Func2 = new GenericInstanceType(nativeArray1);
                nativeArray1Func2.GenericArguments.Add(funcObjObj);

                var nativeArray1Str = new GenericInstanceType(nativeArray1);
                nativeArray1Str.GenericArguments.Add(clrKnownRefs.String);

                var nativeArrayInt = new GenericInstanceType(nativeArray1);
                nativeArrayInt.GenericArguments.Add(clrKnownRefs.Int32);

                // --- Resolve Skin constructor factory ---
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
                var skinBinderCtorOneWay1 = clrContext.GetMethodReference(
                    ".ctor", clrKnownRefs.Void, skinBinderInfoType,
                    nativeArray1Func2, nativeArray1Str, act2ObjObj,
                    runtimeScopeManager.Context.ClrKnownReferences.ClrContext.GetTypeDefinition(
                        Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.BinderType")),
                    clrKnownRefs.Int32, clrKnownRefs.Int32,
                    funcObjObj, clrKnownRefs.Object).Resolve();

                var skinBinderInfoFactoryId = runtimeScopeManager.ResolveFactory(skinBinderCtorOneWay1);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory"] = skinBinderInfoFactoryId;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error resolving constructor factories for Razor templates");
                runtimeScopeManager.Context.AddError(
                    null,
                    $"Error resolving constructor factories for Razor templates: {ex.Message}",
                    false);
            }

            // --- Resolve SkinBinderHelper static methods ---
            try
            {
                var nativeArray1 = clrContext.GetTypeDefinition(
                    Tuple.Create(ClrKnownReferences.MSCorlibStr, "System.NativeArray`1"));
                var nativeArrayInt = new GenericInstanceType(nativeArray1);
                nativeArrayInt.GenericArguments.Add(clrKnownRefs.Int32);

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
                var setAttributeMethod = clrContext.GetMethodReference(
                    "SetAttribute", clrKnownRefs.Void, binderHelperType,
                    nodeRefType, clrKnownRefs.String, clrKnownRefs.String).Resolve();
                var setAttributeId = ResolveStaticMethodIdentifier(
                    runtimeScopeManager, setAttributeMethod);
                _resolvedIdentifiers["Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute"] = setAttributeId;

                // SetCssClass(Element elem, bool add, string className) — optional
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
                    Log.Debug(ex, "Could not resolve SetCssClass — optional method");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error resolving SkinBinderHelper static methods for Razor templates");
                runtimeScopeManager.Context.AddError(
                    null,
                    $"Error resolving SkinBinderHelper methods: {ex.Message}",
                    false);
            }

            // --- Resolve type identifiers ---
            try
            {
                if (uiSkinableElementType != null)
                    ResolveTypeIdentifier(runtimeScopeManager, uiSkinableElementType,
                        "Sunlight.Framework.UI.UISkinableElement");

                ResolveModelTypeIdentifiers(clrContext, runtimeScopeManager);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error resolving type identifiers for Razor templates");
            }

            // --- Force resolution of event handler methods ---
            try
            {
                ResolveEventHandlerMethods(clrContext, runtimeScopeManager);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error resolving event handler methods for Razor templates");
            }

            Log.Debug("Resolved {Count} runtime identifiers for Razor template JS replacement",
                _resolvedIdentifiers.Count + _resolvedTypeIdentifiers.Count);
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

            // Use CecilTypeHelper's cached lookup instead of iterating all modules
            var typeDef = _typeHelper.FindTypeDefinition(csharpName);
            if (typeDef != null)
            {
                ResolveTypeIdentifier(runtimeScopeManager, typeDef, csharpName);
                return;
            }

            // Check nested types (CecilTypeHelper indexes by FullName which uses '/' for nested)
            var nestedName = csharpName.Replace(".", "/");
            typeDef = _typeHelper.FindTypeDefinition(nestedName);
            if (typeDef != null)
            {
                ResolveTypeIdentifier(runtimeScopeManager, typeDef, csharpName);
                return;
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

                // Find the model type in Cecil using cached lookup
                var modelType = _typeHelper.FindTypeDefinition(ir.ModelTypeName);
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
        /// Delegates to CecilModelStubGenerator for model type stub generation.
        /// </summary>
        private string GenerateModelTypeStub(string templateSource)
            => _stubGenerator.GenerateModelTypeStub(templateSource);

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
            if (propertyDefinition == null)
            {
                // Not a property — check if CSS literal replacement is active
                return _cssLiteralReplacer != null ? IntrestLevel.Encapsulate : IntrestLevel.None;
            }

            if (propertyDefinition.SetMethod != null)
            {
                // Property with setter — check if CSS literal replacement is active
                return _cssLiteralReplacer != null ? IntrestLevel.Encapsulate : IntrestLevel.None;
            }

            var skinAttr = propertyDefinition.CustomAttributes?.FirstOrDefault(
                a => a.AttributeType.Name == "SkinAttribute" ||
                     a.AttributeType.FullName.EndsWith(".SkinAttribute"));

            if (skinAttr == null)
            {
                // Not a [Skin] property — check for CSS replacement
                return _cssLiteralReplacer != null ? IntrestLevel.Encapsulate : IntrestLevel.None;
            }

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

            return _cssLiteralReplacer != null ? IntrestLevel.Encapsulate : IntrestLevel.None;
        }

        // Not used: RazorTemplatingPlugin only returns IntrestLevel.Overwrite or None.
        public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter) => null;

        public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter) => null;

        public List<Statement> GetEncapsulationStatements(
            MethodConverter methodConverter,
            List<Statement> methodStatments)
        {
            if (_cssLiteralReplacer == null) return methodStatments;
            return _cssLiteralReplacer.TransformStatements(methodStatments);
        }

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

            // Collect methods referenced by template event handlers so the demand-driven
            // converter emits their bodies. Without this, methods called only from
            // templates (e.g., onclick="@Model.OnSelectTodo(todo)") would be dead-code-eliminated.
            var methods = new List<MethodReference>();
            if (_hasRazorTemplates && _clrContext != null)
            {
                var seen = new HashSet<string>();
                foreach (var kvp in _compiledIRs)
                {
                    CollectEventMethodReferences(kvp.Value, kvp.Value.ModelTypeName, methods, seen);
                }
            }

            return methods;
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

            // Not found — create it ourselves so the emitted call is minification-safe.
            var newId = SimpleIdentifier.CreateScopeIdentifier(scope, "DocStorageGetter", false);
            _resolvedIdentifiers["DocStorageGetter"] = newId;
            _needsDocStorageGetterEmission = true;
            Log.Debug("Created DocStorageGetter identifier (XWML not active); will emit function body");
        }

        /// <summary>
        /// Walks an IR tree and collects MethodDefinition references for all event handlers
        /// so the demand-driven converter emits their bodies.
        /// </summary>
        private void CollectEventMethodReferences(
            IRNode node, string modelTypeName, List<MethodReference> methods, HashSet<string> seen)
        {
            if (node is TemplateIR.EventNode evt && !string.IsNullOrEmpty(evt.HandlerExpression))
            {
                var methodDef = TryFindEventMethodDefinition(evt.HandlerExpression, modelTypeName);
                if (methodDef != null && seen.Add(methodDef.FullName))
                    methods.Add(methodDef);
            }

            // For loops, also scan item template with the item type for item-level methods
            if (node is TemplateIR.LoopNode loop && loop.ItemTemplate != null)
            {
                // Resolve item type from collection property on the model
                string itemTypeName = TryResolveItemTypeName(modelTypeName, loop);

                foreach (var child in loop.ItemTemplate)
                {
                    // Model.XXX references inside item templates
                    CollectEventMethodReferences(child, modelTypeName, methods, seen);

                    // Item-level methods (e.g., "todo.ToggleImportant")
                    if (!string.IsNullOrEmpty(itemTypeName)
                        && child is TemplateIR.EventNode itemEvt
                        && !string.IsNullOrEmpty(itemEvt.HandlerExpression)
                        && !itemEvt.HandlerExpression.StartsWith("Model."))
                    {
                        // Strip item variable prefix: "todo.ToggleImportant" → "ToggleImportant"
                        var itemHandler = itemEvt.HandlerExpression;
                        var dotIdx = itemHandler.IndexOf('.');
                        if (dotIdx > 0)
                            itemHandler = itemHandler.Substring(dotIdx + 1);

                        var itemMethod = TryFindEventMethodDefinition(itemHandler, itemTypeName);
                        if (itemMethod != null && seen.Add(itemMethod.FullName))
                            methods.Add(itemMethod);
                    }
                }
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    CollectEventMethodReferences(child, modelTypeName, methods, seen);
            }
        }

        /// <summary>
        /// Resolves the item type name for a foreach loop by inspecting
        /// the collection property's generic type argument on the model type.
        /// </summary>
        private string TryResolveItemTypeName(string modelTypeName, TemplateIR.LoopNode loop)
        {
            try
            {
                // CollectionExpression is like "Model.CurrentTodos"
                var collExpr = loop.CollectionExpression;
                if (string.IsNullOrEmpty(collExpr)) return null;

                if (collExpr.StartsWith("Model."))
                    collExpr = collExpr.Substring(6);

                // Find the property on the model type
                TypeDefinition modelType = null;
                foreach (var t in _clrContext.GetTypes())
                {
                    if (t.FullName == modelTypeName || t.Name == modelTypeName)
                    {
                        modelType = t;
                        break;
                    }
                }
                if (modelType == null) return null;

                var prop = modelType.Properties.FirstOrDefault(p => p.Name == collExpr);
                if (prop == null) return null;

                // Extract generic type argument from ObservableCollection<T>
                var propType = prop.PropertyType;
                if (propType is Mono.Cecil.GenericInstanceType git && git.GenericArguments.Count > 0)
                    return git.GenericArguments[0].FullName;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Extracts a method name from a handler expression and looks up the MethodDefinition.
        /// Handles patterns: "Model.Method", "Model.Method(arg)", "item.Method", "Method".
        /// </summary>
        private MethodDefinition TryFindEventMethodDefinition(string handler, string modelTypeName)
        {
            if (string.IsNullOrEmpty(handler) || string.IsNullOrEmpty(modelTypeName))
                return null;

            var expr = handler;

            // Strip Model. prefix — method is on the model type
            if (expr.StartsWith("Model."))
                expr = expr.Substring(6);

            // Remove parenthesized arguments: "Method(arg)" → "Method"
            var parenIdx = expr.IndexOf('(');
            if (parenIdx > 0)
                expr = expr.Substring(0, parenIdx);

            // Skip lambdas
            if (expr.Contains("=>"))
                return null;

            // Skip if it contains dots (nested access not supported here)
            if (expr.Contains("."))
                return null;

            var methodName = expr.Trim();
            if (string.IsNullOrEmpty(methodName))
                return null;

            try
            {
                // Search all loaded types for the model type
                TypeDefinition typeDef = null;
                foreach (var t in _clrContext.GetTypes())
                {
                    if (t.FullName == modelTypeName || t.Name == modelTypeName)
                    {
                        typeDef = t;
                        break;
                    }
                }
                if (typeDef == null) return null;

                foreach (var m in typeDef.Methods)
                {
                    if (m.Name == methodName && m.IsPublic && !m.IsConstructor)
                        return m;
                }
            }
            catch { }

            return null;
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

            // If Razor created its own DocStorageGetter identifier (no XWML), emit the function body.
            if (_needsDocStorageGetterEmission)
            {
                var docStorageGetterStatements = EmitDocStorageGetterFunction();
                if (docStorageGetterStatements != null)
                    statements.AddRange(docStorageGetterStatements);
            }

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

                    RazorCssManager cssManager = null;
                    if (!_templateCssManagers.TryGetValue(kvp.Value.TemplateName, out cssManager)
                        && _templateCssManagers.Count > 0)
                    {
                        // Sub-templates without @styles inherit the parent's CSS manager
                        // so their static HTML class names get resolved through the CSS scope.
                        cssManager = _templateCssManagers.Values.First();
                    }

                    var jstGenerator = new RazorSkinJSTGenerator(
                        kvp.Value,
                        _runtimeScopeManager,
                        _clrContext,
                        _resolvedIdentifiers,
                        _resolvedTypeIdentifiers,
                        _razorKnownTypes,
                        _nextDataIndex++,
                        preCreatedGetter,
                        cssManager);

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

            // Emit CSS <style> element for templates with @styles directives
            statements.AddRange(EmitCssStatements());

            // Apply CssLiteralReplacer to all template-generated code (binding graph
            // getters contain StringLiteralExpression nodes from const-folded [CssClass]
            // references that need CSS scope resolution).
            if (_cssLiteralReplacer != null)
            {
                statements = _cssLiteralReplacer.TransformStatements(statements);
            }

            Log.Debug("GetPostJavascript emitting {StatementCount} statements for {TemplateCount} templates",
                statements.Count, emittedTemplates.Count);

            return statements;
        }

        /// <summary>
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
