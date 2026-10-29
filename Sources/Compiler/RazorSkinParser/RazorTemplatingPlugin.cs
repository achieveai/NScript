using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.Utils;
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

        /// <summary>
        /// Compiled skin IR by content, kept for the process (see <see cref="ContentCache{T}"/>).
        /// </summary>
        internal static readonly ContentCache<SkinTemplateNode> SkinIRs = new ContentCache<SkinTemplateNode>(512);

        /// <summary>
        /// Returns this build's own copy of the compiled IR. The IR depends only on these
        /// strings; codegen later writes Cecil-derived values into it, so the cached IR is
        /// never handed out directly.
        /// </summary>
        internal static SkinTemplateNode GetSkinIR(
            string templateName, string templateSource, string[] additionalSources, string fileName)
            => GetSkinIR(templateName, templateSource, additionalSources, fileName, out _);

        internal static SkinTemplateNode GetSkinIR(
            string templateName, string templateSource, string[] additionalSources, string fileName, out bool cacheHit)
        {
            var irKey = ContentCache<SkinTemplateNode>.Key(
                new[] { templateName, templateSource, fileName }.Concat(additionalSources).ToArray());
            return TemplateIR.IRCloner.Clone(SkinIRs.GetOrCreate(
                irKey,
                () => RazorSkinCompiler.CompileToIR(templateName, templateSource, additionalSources, fileName),
                out cacheHit));
        }

        private RuntimeScopeManager _runtimeScopeManager;
        private ClrContext _clrContext;
        private CecilTypeHelper _typeHelper;
        private CecilModelStubGenerator _stubGenerator;

        /// <summary>
        /// Per-compilation data index counter for Razor templates, from 0. The indexes are into
        /// Razor's own per-document store, so they never meet XWML's.
        /// </summary>
        private int _nextDataIndex;

        /// <summary>
        /// Map of assembly module and full resource name to compiled template IR.
        /// Used by GetPostJavascript to generate proper JST nodes.
        /// </summary>
        private readonly Dictionary<string, TemplateIR.SkinTemplateNode> _compiledIRs
            = new Dictionary<string, TemplateIR.SkinTemplateNode>();

        /// <summary>
        /// Maps template identity to a readable short name for diagnostics.
        /// </summary>
        private readonly Dictionary<string, string> _templateShortNames = new Dictionary<string, string>();

        /// <summary>
        /// A skin's identity: its assembly and resource. Not the Mvid, which a compile changes,
        /// so a dev patch after a .cs save still finds the skins it read.
        /// </summary>
        internal static string GetTemplateKey(ModuleDefinition module, string resourceName)
            => (module.Assembly?.Name.Name ?? module.Name) + "|" + resourceName;

        private string FindTemplateKey(ModuleDefinition module, string resourceName,
            TypeDefinition ownerType, out string ambiguity)
        {
            return SelectTemplateKey(module, resourceName, ownerType, _compiledIRs,
                _templateShortNames,
                (name, imports) => RazorSkinJSTGenerator.ResolveSubControlType(
                    _clrContext.GetTypes(), name, imports), out ambiguity);
        }

        internal static string SelectTemplateKey(ModuleDefinition module, string resourceName,
            TypeDefinition ownerType, IReadOnlyDictionary<string, SkinTemplateNode> templates,
            IReadOnlyDictionary<string, string> shortNames,
            Func<string, IEnumerable<string>, TypeDefinition> resolveType,
            out string ambiguity)
        {
            ambiguity = null;
            var ownKey = GetTemplateKey(module, resourceName);
            if (templates.ContainsKey(ownKey)) return ownKey;

            var exactSuffix = "|" + resourceName;
            var exactMatches = templates.Keys.Where(key => key.EndsWith(exactSuffix,
                StringComparison.Ordinal)).ToList();
            if (exactMatches.Count == 1) return exactMatches[0];

            var candidates = exactMatches.Count > 1 ? exactMatches : shortNames
                .Where(pair => pair.Value == resourceName && templates.ContainsKey(pair.Key))
                .Select(pair => pair.Key).ToList();
            if (candidates.Count == 1) return candidates[0];
            if (candidates.Count == 0 || ownerType == null) return null;

            var compatible = new List<(string Key, int Distance)>();
            foreach (var key in candidates)
            {
                var template = templates[key];
                TypeDefinition controlType;
                try
                {
                    controlType = resolveType(template.ControlTypeName, template.UsingNamespaces);
                }
                catch (InvalidOperationException ex)
                {
                    ambiguity = ex.Message;
                    return null;
                }
                var distance = InheritanceDistance(ownerType, controlType);
                if (distance >= 0) compatible.Add((key, distance));
            }

            if (compatible.Count == 0) return null;
            var closestDistance = compatible.Min(candidate => candidate.Distance);
            var best = compatible.Where(candidate => candidate.Distance == closestDistance).ToList();
            if (best.Count == 1) return best[0].Key;

            ambiguity = "Ambiguous Razor skin '" + resourceName + "' for " + ownerType.FullName
                + ": " + string.Join(", ", best.Select(candidate => candidate.Key));
            return null;
        }

        private static int InheritanceDistance(TypeDefinition type, TypeDefinition ancestor)
        {
            if (ancestor == null) return -1;
            for (var distance = 0; type != null; distance++)
            {
                if (type.FullName == ancestor.FullName
                    && type.Module?.Mvid == ancestor.Module?.Mvid) return distance;
                try { type = type.BaseType?.Resolve(); }
                catch (AssemblyResolutionException) { return -1; }
            }
            return -1;
        }

        internal static Statement CreateTemplateAmbiguityThrow(IdentifierScope scope, string message)
        {
            return new ThrowStatement(null, scope, new StringLiteralExpression(scope, message), false);
        }

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

        // This build's parsed-CSS cache hits and misses (Probe.RazorInit).
        private int _cssHits, _cssMisses;

        /// <summary>
        /// All embedded CSS resources found during module scanning, keyed by resource name.
        /// Used to resolve @styles references to actual CSS content.
        /// </summary>
        private readonly Dictionary<string, EmbeddedResource> _cssResources
            = new Dictionary<string, EmbeddedResource>();

        // CSS resource name -> its source path from $$ResInfo$$, so CSS diagnostics name the file.
        private readonly Dictionary<string, string> _cssFileNames = new Dictionary<string, string>();

        // Template key -> template text, so a missing CSS class is reported at its line.
        private readonly Dictionary<string, string> _templateSources = new Dictionary<string, string>();

        // A stylesheet shared by several templates fails for each of them; report it once.
        private readonly HashSet<string> _reportedCssErrors = new HashSet<string>();

        // Dev patch emit: each skin ("skin:" + template key) and stylesheet ("css:" + name)
        // this build's output read, with its content hash.
        private readonly Dictionary<string, byte[]> _resourcesRead = new Dictionary<string, byte[]>();

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
        /// Every getter the skins of the last full emit read as its backing field, and every
        /// setter they wrote as one (see
        /// <see cref="TryPatchBodies"/>).
        /// </summary>
        private readonly List<(TypeDefinition Type, PropertyDefinition Property, FieldDefinition Field, bool Setter)> _inlinedGetters
            = new List<(TypeDefinition Type, PropertyDefinition Property, FieldDefinition Field, bool Setter)>();

        /// <summary>Each template's names in the bundle, by template key, so a patch writes the same ones.</summary>
        private readonly Dictionary<string, RazorSkinJSTGenerator.TemplateNames> _templateNames
            = new Dictionary<string, RazorSkinJSTGenerator.TemplateNames>();

        /// <summary>
        /// Patch-emit bundles: a skin save goes live in the open page (<see cref="LiveSkinHook"/>).
        /// Plain bundles are as before.
        /// </summary>
        private bool LiveHook => _runtimeScopeManager?.Context?.LivePatchHook == true;

        /// <summary>Dev bundles: the marker on the line before the skins' CSS.</summary>
        internal static readonly string CssMarker = MarkerStatement.ForLiteral("RazorSkinParser.Css");

        /// <summary>The CSS the last full emit or patch wrote.</summary>
        private string _writtenCss;

        /// <summary>Whether the CSS line has the live hook (<see cref="LiveLiteral"/>).</summary>
        private bool _liveCssHook;

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

            // Reset per-compilation state. A retried build initializes this plugin again with a
            // new RuntimeScopeManager: start as a new plugin would, so no identifier of the
            // discarded scope and no "already reported" CSS error carries into the retry.
            _nextDataIndex = 0;
            _templateMethodReferences = null;
            _compiledIRs.Clear();
            _templateShortNames.Clear();
            _hasRazorTemplates = false;
            _razorKnownTypes = null;
            _templateCssManagers.Clear();
            _cssClassMap.Clear();
            _cssLiteralReplacer = null;
            _cssHits = 0;
            _cssMisses = 0;
            _cssResources.Clear();
            _cssFileNames.Clear();
            _templateSources.Clear();
            _reportedCssErrors.Clear();
            _resourcesRead.Clear();
            _templateGetterIdentifiers.Clear();
            _resolvedIdentifiers.Clear();
            _resolvedTypeIdentifiers.Clear();
            _inlinedGetters.Clear();
            _templateNames.Clear();
            _writtenCss = null;
            _liveCssHook = false;
            var probeTotal = System.Diagnostics.Stopwatch.StartNew();
            long probeStubTicks = 0, probeCompileTicks = 0;
            int probeTemplates = 0;
            int irHits = 0, irMisses = 0;

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
                        _cssFileNames[embeddedResource.Name] = runtimeScopeManager.Context.GetResourceFileName(
                            module, embeddedResource.Name);
                        Log.Debug("Discovered CSS resource {ResourceName}", embeddedResource.Name);
                    }

                    var fileName = runtimeScopeManager.Context.GetResourceFileName(
                        module, embeddedResource.Name);

                    if (fileName != null && CanHandle(fileName))
                    {
                        _resourcesRead["skin:" + GetTemplateKey(module, embeddedResource.Name)] =
                            System.Security.Cryptography.SHA256.HashData(embeddedResource.GetResourceData());
                        try
                        {
                            using var stream = embeddedResource.GetResourceStream();
                            Log.Debug("Discovered .skin.cshtml resource {ResourceName} (size {ResourceSize} bytes)",
                                embeddedResource.Name, stream.Length);
                            using var reader = new StreamReader(stream);
                            var templateSource = reader.ReadToEnd();

                            var templateName = Path.GetFileNameWithoutExtension(
                                Path.GetFileNameWithoutExtension(fileName));

                            var ir = CompileSkin(templateName, templateSource, fileName, out var irHit, ref probeStubTicks, ref probeCompileTicks);
                            if (irHit) { irHits++; } else { irMisses++; }
                            probeTemplates++;
                            var resourceKey = GetTemplateKey(module, embeddedResource.Name);
                            _compiledIRs[resourceKey] = ir;
                            _templateSources[resourceKey] = templateSource;
                            _templateShortNames[resourceKey] = templateName;
                            _hasRazorTemplates = true;

                            // Pre-create the getter function identifier in the scope system
                            // so it's available when GetOverwrite is called (before GetPostJavascript)
                            var getterId = SimpleIdentifier.CreateScopeIdentifier(
                                runtimeScopeManager.Scope,
                                templateName,
                                false);
                            _templateGetterIdentifiers[resourceKey] = getterId;

                            Log.Debug("Compilation succeeded for template {TemplateName} from resource {ResourceName}",
                                templateName, embeddedResource.Name);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Compilation failed for resource {ResourceName}", embeddedResource.Name);

                            runtimeScopeManager.Context.AddTemplateError(
                                (ex as RazorSkinPreprocessorException)?.Location
                                    ?? (ex as NScript.Converter.ConverterLocationException)?.Location,
                                $"Error compiling Razor skin template '{fileName}': {ex.Message}",
                                false);
                        }
                    }
                }
            }

            long probeKnownMs = 0, probeResolveMs = 0, probeCssMs = 0, probeScanMs = 0;
            var probePhase = System.Diagnostics.Stopwatch.StartNew();
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

                probeKnownMs = probePhase.ElapsedMilliseconds; probePhase.Restart();

                // Razor's own document store getter, written by GetPostJavascript. XWML has its
                // own: the two never share a skin's cached DOM.
                _resolvedIdentifiers["DocStorageGetter"] = SimpleIdentifier.CreateScopeIdentifier(
                    runtimeScopeManager.Scope, "DocStorageGetter", false);
                ResolveRuntimeIdentifiers(clrContext, runtimeScopeManager);
                probeResolveMs = probePhase.ElapsedMilliseconds; probePhase.Restart();

                // Load CSS for templates with @styles directives
                LoadCssForTemplates(runtimeScopeManager);
                probeCssMs = probePhase.ElapsedMilliseconds; probePhase.Restart();

                // Scan [CssClass] const fields and enable minification
                ScanCssClassAttributes(runtimeScopeManager);
                probeScanMs = probePhase.ElapsedMilliseconds;
            }

            var probeTicksPerMs = System.Diagnostics.Stopwatch.Frequency / 1000.0;
            Log.Information(
                "Probe.RazorInit TotalMs={TotalMs} StubMs={StubMs} CompileMs={CompileMs} Templates={Templates} KnownMs={KnownMs} ResolveMs={ResolveMs} CssMs={CssMs} ScanMs={ScanMs} IrHits={IrHits} IrMisses={IrMisses} CssHits={CssHits} CssMisses={CssMisses} ProcessIrHits={ProcessIrHits} ProcessIrMisses={ProcessIrMisses} ProcessCssHits={ProcessCssHits} ProcessCssMisses={ProcessCssMisses}",
                probeTotal.ElapsedMilliseconds,
                System.Math.Round(probeStubTicks / probeTicksPerMs),
                System.Math.Round(probeCompileTicks / probeTicksPerMs),
                probeTemplates, probeKnownMs, probeResolveMs, probeCssMs, probeScanMs,
                irHits, irMisses, _cssHits, _cssMisses,
                SkinIRs.Hits, SkinIRs.Misses, RazorCssManager.ParsedSheets.Hits, RazorCssManager.ParsedSheets.Misses);
        }

        /// <summary>
        /// Compiles a skin's source to its IR. C# stubs for the model type, from Cecil type info,
        /// let the Roslyn analysis phase detect observable properties and promote bindings from
        /// OneTime to OneWay.
        /// </summary>
        private SkinTemplateNode CompileSkin(
            string templateName,
            string templateSource,
            string fileName,
            out bool irHit,
            ref long stubTicks,
            ref long compileTicks)
        {
            var probeStart = System.Diagnostics.Stopwatch.GetTimestamp();
            var modelTypeStub = _stubGenerator.GenerateModelTypeStub(templateSource);
            stubTicks += System.Diagnostics.Stopwatch.GetTimestamp() - probeStart;
            probeStart = System.Diagnostics.Stopwatch.GetTimestamp();
            var additionalSources = modelTypeStub != null
                ? new[] { FrameworkTypeStubs, modelTypeStub }
                : new[] { FrameworkTypeStubs };

            var ir = GetSkinIR(templateName, templateSource, additionalSources, fileName, out irHit);
            compileTicks += System.Diagnostics.Stopwatch.GetTimestamp() - probeStart;
            return ir;
        }

        /// <summary>
        /// Loads CSS for templates with @styles directives.
        /// Creates a RazorCssManager per template, loads referenced CSS from embedded resources,
        /// validates class usage, and optimizes names for minification.
        /// </summary>
        private void LoadCssForTemplates(RuntimeScopeManager runtimeScopeManager)
        {
            foreach (var kvp in _compiledIRs)
            {
                var ir = kvp.Value;

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

                        _resourcesRead["css:" + cssResourceName] =
                            System.Security.Cryptography.SHA256.HashData(cssResource.GetResourceData());
                        using var stream = cssResource.GetResourceStream();
                        using var reader = new StreamReader(stream);
                        var cssText = reader.ReadToEnd();

                        if (cssManager.AddStylesheet(cssResourceName, cssText, _cssFileNames.GetValueOrDefault(cssResourceName))) { _cssHits++; } else { _cssMisses++; }
                        Log.Debug("Loaded CSS {ResourceName} for template {TemplateName}",
                            cssResourceName, ir.TemplateName);
                    }

                    if (cssManager.HasStylesheets)
                    {
                        // Validate CSS variables are declared
                        cssManager.ValidateCssVariables();

                        // Validate class names used in template HTML
                        TemplateIR.TemplateIRBuilder.ValidateCssClasses(
                            ir, cssManager, _templateSources.GetValueOrDefault(kvp.Key));

                        // Note: CompressNames() is called later in ScanCssClassAttributes()
                        // once [CssClass] const fields are validated, ensuring all dynamic
                        // class references are tracked before minification.

                        _templateCssManagers[kvp.Key] = cssManager;

                        Log.Debug("CSS loaded for template {TemplateName}: {SheetCount} sheets",
                            ir.TemplateName, cssManager.Sheets.Count);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "CSS loading failed for template {TemplateName}", ir.TemplateName);
                    var location = (ex as NScript.Converter.ConverterLocationException)?.Location;
                    if (_reportedCssErrors.Add(
                            $"{location?.FileName}|{location?.StartLine}|{location?.StartColumn}|{ex.Message}"))
                    {
                        runtimeScopeManager.Context.AddTemplateError(
                            location,
                            $"Error loading CSS for Razor template '{ir.TemplateName}': {ex.Message}",
                            false);
                    }
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
                // releaseNaming is hardcoded to false until Builder exposes a minify flag
                // that the plugin can read during Initialize().
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
        /// </summary>
        private List<Statement> EmitCssStatements()
        {
            var result = new List<Statement>();
            if (_templateCssManagers.Count == 0) return result;

            var cssText = CollectCssForEmission(_templateCssManagers.Values);
            if (cssText.Length == 0) return result;

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

            // s.textContent = "...css...", or with the live hook DocStorageGetter.__nsRazorCss || "...css...":
            // a load after a live patch takes the patch's CSS. Dev bundles mark the line, so a
            // patch writes the new CSS in place.
            _writtenCss = cssText;
            _liveCssHook = LiveHook && _resolvedIdentifiers.ContainsKey("DocStorageGetter");
            if (_runtimeScopeManager.Context.DevMode)
            {
                body.Add(new MarkerStatement(iifeScope, CssMarker));
            }

            Expression cssValue = new StringLiteralExpression(iifeScope, cssText);
            if (_liveCssHook)
            {
                cssValue = new BinaryExpression(null, iifeScope, BinaryOperator.LogicalOr, LiveCssKey(iifeScope, "__nsRazorCss"), cssValue);
            }

            body.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null, iifeScope,
                        new IdentifierExpression(styleVar, iifeScope),
                        new StringLiteralExpression(iifeScope, "textContent")),
                    cssValue));

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

            // DocStorageGetter.__nsRazorStyle = s
            if (_liveCssHook)
            {
                body.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        LiveCssKey(iifeScope, "__nsRazorStyle"),
                        new IdentifierExpression(styleVar, iifeScope)));
            }

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

        /// <inheritdoc/>
        public List<Statement> LiveLiteral(string marker, string value, IdentifierScope root)
        {
            if (marker != CssMarker || !_liveCssHook)
            {
                return null;
            }

            return new List<Statement>
            {
                // DocStorageGetter.__nsRazorCss = "CSS";
                ExpressionStatement.CreateAssignmentExpression(
                    LiveCssKey(root, "__nsRazorCss"),
                    new StringLiteralExpression(root, value)),

                // if (DocStorageGetter.__nsRazorStyle) DocStorageGetter.__nsRazorStyle.textContent = DocStorageGetter.__nsRazorCss;
                new IfBlockStatement(
                    null,
                    root,
                    LiveCssKey(root, "__nsRazorStyle"),
                    new ScopeBlock(
                        null,
                        root,
                        new List<Statement>
                        {
                            ExpressionStatement.CreateAssignmentExpression(
                                new IndexExpression(null, root, LiveCssKey(root, "__nsRazorStyle"), new StringLiteralExpression(root, "textContent")),
                                LiveCssKey(root, "__nsRazorCss")),
                        }),
                    null),
            };
        }

        private Expression LiveCssKey(IdentifierScope scope, string key)
            => new IndexExpression(
                null,
                scope,
                new IdentifierExpression(_resolvedIdentifiers["DocStorageGetter"], scope),
                new StringLiteralExpression(scope, key));

        internal static string CollectCssForEmission(IEnumerable<RazorCssManager> cssManagers)
        {
            var allCss = new System.Text.StringBuilder();
            var emittedSheets = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (var cssManager in cssManagers)
            {
                foreach (var sheet in cssManager.Sheets)
                {
                    var css = cssManager.GetSerializedCssForSheet(sheet);
                    if (string.IsNullOrEmpty(css))
                        continue;

                    if (!emittedSheets.TryGetValue(sheet.ResourceName, out var variants))
                    {
                        variants = new HashSet<string>(StringComparer.Ordinal);
                        emittedSheets.Add(sheet.ResourceName, variants);
                    }

                    // Different class-name scopes can serialize one resource differently.
                    // Keep those variants so generated HTML still matches its stylesheet.
                    if (variants.Add(css))
                        allCss.Append(css);
                }
            }
            return allCss.ToString();
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
            // Called for every method of every build: build FullName only when it is logged.
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose("Checking interest level for method {MethodName}", methodDefinition.FullName);
            }

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
                if (templateName != null
                    && (FindTemplateKey(methodDefinition.Module, templateName,
                        methodDefinition.DeclaringType, out var ambiguity) != null
                        || ambiguity != null))
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
            string ambiguity = null;
            var templateKey = templateName != null
                ? FindTemplateKey(methodConverter.MethodDefinition.Module, templateName,
                    methodConverter.MethodDefinition.DeclaringType, out ambiguity)
                : null;
            if (ambiguity != null)
                return new List<Statement>
                {
                    CreateTemplateAmbiguityThrow(methodConverter.Scope, ambiguity)
                };
            if (templateKey == null)
            {
                return null;
            }

            // Look up the short template name (used as the JS getter function name)
            var shortName = _templateShortNames.ContainsKey(templateKey)
                ? _templateShortNames[templateKey] : templateName;

            Log.Debug("Resolved template {TemplateName} (short: {ShortName}) for overwrite", templateName, shortName);

            // Use the JST getter identifier if available (from JST generation in GetPostJavascript),
            // otherwise fall back to raw JS with the short name.
            if (_templateGetterIdentifiers.TryGetValue(templateKey, out var getterId))
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

        /// <summary>
        /// Methods the templates reference. They depend only on the compiled IRs and type metadata, both fixed
        /// after Initialize, so they are collected on the first pass and reused on later passes of this build.
        /// </summary>
        private List<MethodReference> _templateMethodReferences;

        public List<MethodReference> GetMethodsToEmitPassN()
        {
            // Collect methods referenced by template event handlers so the demand-driven
            // converter emits their bodies. Without this, methods called only from
            // templates (e.g., onclick="@Model.OnSelectTodo(todo)") would be dead-code-eliminated.
            if (_templateMethodReferences != null)
            {
                return new List<MethodReference>(_templateMethodReferences);
            }

            var methods = new List<MethodReference>();
            if (_hasRazorTemplates && _clrContext != null)
            {
                var seen = new HashSet<string>();
                foreach (var kvp in _compiledIRs)
                {
                    CollectEventMethodReferences(kvp.Value, kvp.Value.ModelTypeName,
                        kvp.Value.ControlTypeName, methods, seen);
                    CollectSubControlMethodReferences(kvp.Value, kvp.Value.ModelTypeName,
                        kvp.Value.UsingNamespaces, methods, seen);
                    CollectBindingExpressionReferences(kvp.Value, kvp.Value.ModelTypeName,
                        kvp.Value.ControlTypeName, methods, seen);
                }
            }

            _templateMethodReferences = methods;
            return new List<MethodReference>(methods);
        }

        /// <summary>
        /// Walks an IR tree and collects MethodDefinition references for all event handlers
        /// so the demand-driven converter emits their bodies.
        /// </summary>
        private void CollectEventMethodReferences(
            IRNode node, string modelTypeName, string controlTypeName,
            List<MethodReference> methods, HashSet<string> seen,
            string itemTypeName = null, string itemVariableName = null)
        {
            if (node is TemplateIR.EventNode evt && !string.IsNullOrEmpty(evt.HandlerExpression))
                AddEventMethodReference(evt.HandlerExpression, modelTypeName, controlTypeName,
                    itemTypeName, itemVariableName, methods, seen);

            if (node is TemplateIR.SubControlNode subControl)
            {
                foreach (var subEvent in subControl.EventBindings)
                    AddEventMethodReference(subEvent.HandlerExpression, modelTypeName, controlTypeName,
                        itemTypeName, itemVariableName, methods, seen);
                foreach (var binding in subControl.PropertyBindings)
                    AddEventMethodReference(binding.Classification.CSharpExpression,
                        modelTypeName, controlTypeName, itemTypeName, itemVariableName, methods, seen);
            }

            // Gate branches are not in Children; walk them with the same item context so
            // an item handler inside @if within @foreach (e.g. onchange="@cell.OnInputChange")
            // is retained like a top-level one.
            if (node is TemplateIR.ConditionalNode conditional)
            {
                foreach (var child in conditional.TrueBranch)
                    CollectEventMethodReferences(child, modelTypeName, controlTypeName, methods, seen,
                        itemTypeName, itemVariableName);
                foreach (var child in conditional.FalseBranch)
                    CollectEventMethodReferences(child, modelTypeName, controlTypeName, methods, seen,
                        itemTypeName, itemVariableName);
            }

            // For loops, also scan item template with the item type for item-level methods
            if (node is TemplateIR.LoopNode loop && loop.ItemTemplate != null)
            {
                // Resolve item type from the collection path (Model.Items, Model.Child.Items, item.Tags)
                var loopItemTypeName = TryResolveItemTypeName(
                    modelTypeName, controlTypeName, itemTypeName, itemVariableName, loop);

                foreach (var child in loop.ItemTemplate)
                    CollectEventMethodReferences(child, modelTypeName, controlTypeName, methods, seen,
                        loopItemTypeName, loop.ItemVariableName);
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    CollectEventMethodReferences(child, modelTypeName, controlTypeName, methods, seen,
                        itemTypeName, itemVariableName);
            }
        }

        private void AddEventMethodReference(string handler, string modelTypeName,
            string controlTypeName, string itemTypeName, string itemVariableName,
            List<MethodReference> methods, HashSet<string> seen)
        {
            if (string.IsNullOrEmpty(handler)) return;
            var itemPrefix = itemVariableName + ".";
            var isItemMethod = !string.IsNullOrEmpty(itemVariableName)
                && handler.StartsWith(itemPrefix, StringComparison.Ordinal);
            var typeName = isItemMethod ? itemTypeName
                : handler.StartsWith("Control.", StringComparison.Ordinal)
                    ? controlTypeName : modelTypeName;
            var expression = isItemMethod ? handler.Substring(itemPrefix.Length) : handler;
            var method = TryFindEventMethodDefinition(expression, typeName);
            if (method != null && seen.Add(method.FullName)) methods.Add(method);
        }

        /// <summary>
        /// Walks an IR tree and collects methods referenced by binding expressions (not event
        /// handlers, which <see cref="CollectEventMethodReferences"/> covers) so the demand-driven
        /// converter emits their bodies. Two kinds are retained: methods invoked from a binding
        /// (e.g. <c>class="@(Model.Decorate(Model.Name))"</c>) and computed-property getters read
        /// by a binding path (e.g. <c>@Model.ReproComputed</c>, issue #82) — both are reachable
        /// only through hand-built getter JST, so dead-code elimination (ADR-0022) would otherwise
        /// drop them and the emitted getter would call a missing function.
        /// </summary>
        private void CollectBindingExpressionReferences(
            IRNode node, string modelTypeName, string controlTypeName,
            List<MethodReference> methods, HashSet<string> seen,
            string itemTypeName = null, string itemVariableName = null)
        {
            if (node is TemplateIR.ExpressionBindingNode binding)
                AddBindingExpressionReferences(binding.Classification?.CSharpExpression,
                    modelTypeName, controlTypeName, itemTypeName, itemVariableName, methods, seen);

            if (node is TemplateIR.ConditionalNode conditional)
            {
                AddBindingExpressionReferences(conditional.Condition?.CSharpExpression,
                    modelTypeName, controlTypeName, itemTypeName, itemVariableName, methods, seen);
                foreach (var child in conditional.TrueBranch)
                    CollectBindingExpressionReferences(child, modelTypeName, controlTypeName,
                        methods, seen, itemTypeName, itemVariableName);
                foreach (var child in conditional.FalseBranch)
                    CollectBindingExpressionReferences(child, modelTypeName, controlTypeName,
                        methods, seen, itemTypeName, itemVariableName);
            }

            if (node is TemplateIR.LoopNode loop)
            {
                // The collection path itself (Model.Workspaces, Model.Child.Items) may read computed
                // getters, exactly like a text or attribute binding does.
                AddBindingExpressionReferences(loop.CollectionExpression,
                    modelTypeName, controlTypeName, itemTypeName, itemVariableName, methods, seen);

                if (loop.ItemTemplate != null)
                {
                    var loopItemTypeName = TryResolveItemTypeName(
                        modelTypeName, controlTypeName, itemTypeName, itemVariableName, loop);
                    foreach (var child in loop.ItemTemplate)
                        CollectBindingExpressionReferences(child, modelTypeName, controlTypeName,
                            methods, seen, loopItemTypeName, loop.ItemVariableName);
                }
            }

            if (node.Children != null)
                foreach (var child in node.Children)
                    CollectBindingExpressionReferences(child, modelTypeName, controlTypeName,
                        methods, seen, itemTypeName, itemVariableName);
        }

        private void AddBindingExpressionReferences(
            string expression, string modelTypeName, string controlTypeName,
            string itemTypeName, string itemVariableName,
            List<MethodReference> methods, HashSet<string> seen)
        {
            if (string.IsNullOrEmpty(expression) || _clrContext == null) return;

            foreach (var invocation in BindingExpressionConverter.CollectInvocations(expression))
            {
                var method = ResolveBindingInvocationMethod(
                    invocation, modelTypeName, controlTypeName, itemTypeName, itemVariableName);
                if (method != null && seen.Add(method.FullName)) methods.Add(method);
            }

            foreach (var path in BindingExpressionConverter.CollectMemberPaths(expression))
                AddBindingPropertyGetterReferences(
                    path, modelTypeName, controlTypeName, itemTypeName, itemVariableName, methods, seen);
        }

        /// <summary>
        /// Retains the getter of every computed property read along a binding path
        /// (e.g. <c>Model.ComposerUpload.StatusText</c>): each hop resolves against the previous
        /// hop's type, and a getter that is not a trivial auto-property field read — which the
        /// emitter inlines to a field access rather than a call — is added for emission. A trivial
        /// auto-property getter is skipped because the getter JST reads its backing field directly.
        /// Unknown roots (static types) are left to the emitter, which reports them.
        /// </summary>
        private void AddBindingPropertyGetterReferences(
            IReadOnlyList<string> path, string modelTypeName, string controlTypeName,
            string itemTypeName, string itemVariableName,
            List<MethodReference> methods, HashSet<string> seen)
        {
            if (path.Count < 2) return;

            var rootTypeName = ResolveBindingRootTypeName(
                path[0], modelTypeName, controlTypeName, itemTypeName, itemVariableName);
            if (rootTypeName == null) return;

            var currentType = FindSubControlTypeInAssemblies(rootTypeName);
            for (int i = 1; i < path.Count && currentType != null; i++)
            {
                var property = FindPropertyOnHierarchy(currentType, path[i]);
                if (property == null) return;
                var getter = property.GetMethod;
                if (getter != null && !IsTrivialFieldGetter(getter) && seen.Add(getter.FullName))
                    methods.Add(getter);
                currentType = SafeResolve(property.PropertyType);
            }
        }

        /// <summary>
        /// Declared type name of a binding root: <c>Model</c>, <c>Control</c>, or the enclosing
        /// loop variable (the item type). Null for anything else (static types, unknown names).
        /// </summary>
        private static string ResolveBindingRootTypeName(string root, string modelTypeName,
            string controlTypeName, string itemTypeName, string itemVariableName)
        {
            if (root == "Model") return modelTypeName;
            if (root == "Control") return controlTypeName;
            if (!string.IsNullOrEmpty(itemVariableName) && root == itemVariableName) return itemTypeName;
            return null;
        }

        /// <summary>
        /// True when <paramref name="getter"/> is a simple auto-property backing-field read
        /// (<c>ldarg.0; ldfld; ret</c>, allowing the debug-build temporary/branch noise). These are
        /// inlined to a field access by the getter emitter, so they need no method emission; any
        /// other body is a computed getter that must be retained. Mirrors the backing-field
        /// detection in GraphDescriptorJSTEmitter.TryFindBackingFieldOnType.
        /// </summary>
        private static bool IsTrivialFieldGetter(MethodDefinition getter)
        {
            if (getter.Body == null) return false;

            bool sawLdarg0 = false;
            bool sawLdfld = false;
            foreach (var instr in getter.Body.Instructions)
            {
                var op = instr.OpCode;
                if (op == OpCodes.Nop || op == OpCodes.Ret || op == OpCodes.Stloc_0
                    || op == OpCodes.Ldloc_0 || op == OpCodes.Br_S)
                    continue;
                if (op == OpCodes.Ldarg_0) { sawLdarg0 = true; continue; }
                if (op == OpCodes.Ldfld && sawLdarg0 && !sawLdfld) { sawLdfld = true; continue; }
                return false;
            }
            return sawLdfld;
        }

        /// <summary>
        /// Resolves a binding invocation to the method the emitter will call: the receiver root
        /// (Model/Control/loop variable) gives the starting type, each further receiver segment is
        /// a property hop, and the method is matched by name and argument count on the final type.
        /// Returns null for an unknown root (e.g. a static type) — the emitter reports that case.
        /// </summary>
        private MethodDefinition ResolveBindingInvocationMethod(
            BindingExpressionConverter.InvocationRef invocation,
            string modelTypeName, string controlTypeName, string itemTypeName, string itemVariableName)
        {
            var segments = invocation.ReceiverSegments;
            if (segments.Count == 0) return null;

            string rootTypeName;
            var root = segments[0];
            if (root == "Model") rootTypeName = modelTypeName;
            else if (root == "Control") rootTypeName = controlTypeName;
            else if (!string.IsNullOrEmpty(itemVariableName) && root == itemVariableName)
                rootTypeName = itemTypeName;
            else return null;

            var currentType = FindSubControlTypeInAssemblies(rootTypeName);
            for (int i = 1; i < segments.Count && currentType != null; i++)
            {
                var property = FindPropertyOnHierarchy(currentType, segments[i]);
                currentType = SafeResolve(property?.PropertyType);
            }
            if (currentType == null) return null;

            return FindInvocableMethodOnHierarchy(
                currentType, invocation.MethodName, invocation.ArgumentCount);
        }

        private static PropertyDefinition FindPropertyOnHierarchy(TypeDefinition type, string propertyName)
        {
            for (var current = type; current != null; current = SafeResolve(current.BaseType))
            {
                var property = current.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property != null) return property;
            }
            return null;
        }

        private static MethodDefinition FindInvocableMethodOnHierarchy(
            TypeDefinition type, string methodName, int argumentCount)
        {
            for (var current = type; current != null; current = SafeResolve(current.BaseType))
            {
                var method = current.Methods.FirstOrDefault(candidate =>
                    candidate.Name == methodName && candidate.IsPublic && !candidate.IsConstructor
                    && candidate.HasThis && candidate.Parameters.Count == argumentCount);
                if (method != null) return method;
            }
            return null;
        }

        private static TypeDefinition SafeResolve(TypeReference reference)
        {
            if (reference == null) return null;
            try { return reference.Resolve(); }
            catch (AssemblyResolutionException) { return null; }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Walks an IR tree and collects constructor + DefaultSkin getter references
        /// for SubControlNodes so the demand-driven converter emits their factory functions.
        /// </summary>
        private void CollectSubControlMethodReferences(
            IRNode node, string modelTypeName, IEnumerable<string> usingNamespaces,
            List<MethodReference> methods, HashSet<string> seen)
        {
            if (node is TemplateIR.SubControlNode sub)
            {
                TypeDefinition controlType;
                try
                {
                    controlType = RazorSkinJSTGenerator.ResolveSubControlType(
                        _clrContext.GetTypes(), sub.ResolvedTypeName ?? sub.TypeName,
                        usingNamespaces);
                }
                catch (InvalidOperationException ex)
                {
                    _runtimeScopeManager.Context.AddTemplateError(sub.Location, ex.Message, false);
                    controlType = null;
                }
                if (controlType != null)
                {
                    var ctor = RazorSkinJSTGenerator.FindElementConstructor(controlType);
                    if (ctor != null && seen.Add(ctor.FullName))
                        methods.Add(ctor);

                    // DefaultSkin getter
                    var skinProp = controlType.Properties.FirstOrDefault(p =>
                        p.Name == "DefaultSkin" && p.GetMethod != null && p.GetMethod.IsStatic);
                    if (skinProp?.GetMethod != null && seen.Add(skinProp.GetMethod.FullName))
                        methods.Add(skinProp.GetMethod);

                    foreach (var binding in sub.PropertyBindings)
                    {
                        var expression = binding.Classification.CSharpExpression;
                        if (!string.IsNullOrEmpty(expression) && expression.StartsWith("Model."))
                        {
                            var modelPropertyName = expression.Substring(6);
                            var modelType = FindSubControlTypeInAssemblies(modelTypeName);
                            var modelProperty = modelType?.Properties.FirstOrDefault(p =>
                                p.Name == modelPropertyName);
                            if (modelProperty?.SetMethod != null
                                && seen.Add(modelProperty.SetMethod.FullName))
                                methods.Add(modelProperty.SetMethod);
                        }
                        var dot = expression?.LastIndexOf('.') ?? -1;
                        if (dot > 0 && dot < expression.Length - 1)
                        {
                            var typeName = expression.Substring(0, dot);
                            var propertyName = expression.Substring(dot + 1);
                            var staticType = FindSubControlTypeInAssemblies(typeName);
                            var staticProperty = staticType?.Properties.FirstOrDefault(p =>
                                p.Name == propertyName && p.GetMethod != null
                                && p.GetMethod.IsStatic);
                            if (staticProperty?.GetMethod != null
                                && seen.Add(staticProperty.GetMethod.FullName))
                                methods.Add(staticProperty.GetMethod);
                        }

                        var currentType = controlType;
                        while (currentType != null)
                        {
                            var property = currentType.Properties.FirstOrDefault(p =>
                                p.Name == binding.PropertyName);
                            if (property?.SetMethod != null)
                            {
                                if (seen.Add(property.SetMethod.FullName))
                                    methods.Add(property.SetMethod);
                                if (property.GetMethod != null
                                    && seen.Add(property.GetMethod.FullName))
                                    methods.Add(property.GetMethod);
                                break;
                            }
                            currentType = currentType.BaseType?.Resolve();
                        }
                    }
                }
            }

            if (node is TemplateIR.ConditionalNode conditional)
            {
                foreach (var child in conditional.TrueBranch)
                    CollectSubControlMethodReferences(child, modelTypeName,
                        usingNamespaces, methods, seen);
                foreach (var child in conditional.FalseBranch)
                    CollectSubControlMethodReferences(child, modelTypeName,
                        usingNamespaces, methods, seen);
            }

            if (node is TemplateIR.LoopNode loop && loop.ItemTemplate != null)
            {
                foreach (var child in loop.ItemTemplate)
                    CollectSubControlMethodReferences(child, modelTypeName,
                        usingNamespaces, methods, seen);
            }

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    CollectSubControlMethodReferences(child, modelTypeName,
                        usingNamespaces, methods, seen);
            }
        }

        /// <summary>
        /// Searches loaded assemblies for a type by short or full name.
        /// </summary>
        private Mono.Cecil.TypeDefinition FindSubControlTypeInAssemblies(string typeName)
        {
            if (_clrContext == null || string.IsNullOrEmpty(typeName)) return null;

            foreach (var type in _clrContext.GetTypes())
            {
                if (type.FullName == typeName || type.Name == typeName)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// Resolves the item type name for a foreach loop from its collection path. The root
        /// (Model, Control or the enclosing loop variable) is walked hop by hop, so a chained path
        /// (<c>Model.Child.Items</c>) and an item-rooted nested loop (<c>item.Tags</c>) type their
        /// loop variable like a one-hop source; the final property's generic argument is the item
        /// type (ObservableCollection&lt;T&gt; → T).
        /// </summary>
        private string TryResolveItemTypeName(string modelTypeName, string controlTypeName,
            string itemTypeName, string itemVariableName, TemplateIR.LoopNode loop)
        {
            if (_clrContext == null || string.IsNullOrEmpty(loop.CollectionExpression)) return null;

            var segments = loop.CollectionExpression.Split('.');
            var rootTypeName = ResolveBindingRootTypeName(
                segments[0], modelTypeName, controlTypeName, itemTypeName, itemVariableName);
            var rootType = FindSubControlTypeInAssemblies(rootTypeName);
            return CecilTypeHelper.CollectionItemTypeName(
                _typeHelper.FindPropertyPath(rootType, segments.Skip(1)));
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

            var arrowIdx = expr.IndexOf("=>", StringComparison.Ordinal);
            if (arrowIdx >= 0)
                expr = expr.Substring(arrowIdx + 2).Trim();

            // Strip Model. prefix — method is on the model type
            if (expr.StartsWith("Model."))
                expr = expr.Substring(6);
            else if (expr.StartsWith("Control."))
                expr = expr.Substring(8);

            // Remove parenthesized arguments: "Method(arg)" → "Method"
            var parenIdx = expr.IndexOf('(');
            if (parenIdx > 0)
                expr = expr.Substring(0, parenIdx);

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

                for (var currentType = typeDef; currentType != null;
                    currentType = currentType.BaseType?.Resolve())
                {
                    foreach (var m in currentType.Methods)
                    {
                        if (m.Name == methodName && m.IsPublic && !m.IsConstructor)
                            return m;
                    }
                }
            }
            catch { }

            return null;
        }

        public List<Statement> GetPreJavascript()
        {
            return new List<Statement>();
        }

        /// <inheritdoc/>
        public string PatchSectionName => "RazorSkinParser";

        /// <inheritdoc/>
        public bool PatchSectionLive => LiveHook;

        /// <summary>
        /// Dev patch emit: writes each changed skin again, with the names the bundle has and a
        /// storage index past the old ones (each document caches a skin's DOM by its index).
        /// Misses when a skin was added or removed, a stylesheet changed, or a changed skin
        /// would need what the bundle lacks: other types, other stylesheets, a class the CSS
        /// does not declare, or a method no skin called.
        /// </summary>
        public string TryPatchResources(List<Statement> statements, Dictionary<string, string> literals)
        {
            var current = new Dictionary<string, EmbeddedResource>();
            var fileNames = new Dictionary<string, string>();
            foreach (var module in _clrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    if (!(resource is EmbeddedResource embeddedResource))
                    {
                        continue;
                    }

                    if (embeddedResource.Name.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
                    {
                        current["css:" + embeddedResource.Name] = embeddedResource;
                    }

                    var fileName = _runtimeScopeManager.Context.GetResourceFileName(module, embeddedResource.Name);
                    if (fileName != null && CanHandle(fileName))
                    {
                        var key = "skin:" + GetTemplateKey(module, embeddedResource.Name);
                        if (!_resourcesRead.ContainsKey(key))
                        {
                            return "razor skin added " + fileName;
                        }

                        current[key] = embeddedResource;
                        fileNames[key] = fileName;
                    }
                }
            }

            var changed = new List<string>();
            foreach (var read in _resourcesRead)
            {
                if (!current.TryGetValue(read.Key, out var resource)
                    || !System.Security.Cryptography.SHA256.HashData(resource.GetResourceData()).AsSpan().SequenceEqual(read.Value))
                {
                    if (resource == null)
                    {
                        return "razor resource changed " + read.Key;
                    }

                    changed.Add(read.Key);
                }
            }

            if (changed.Count == 0)
            {
                return null;
            }

            foreach (var read in changed.Where(key => key.StartsWith("css:", StringComparison.Ordinal)).ToList())
            {
                changed.Remove(read);
                string cssMiss = PatchStylesheet(read.Substring("css:".Length), current[read]);
                if (cssMiss != null)
                {
                    return cssMiss;
                }

                _resourcesRead[read] = System.Security.Cryptography.SHA256.HashData(current[read].GetResourceData());
            }

            var known = new HashSet<string>((_templateMethodReferences ?? new List<MethodReference>()).Select(method => method.FullName));
            var patched = new List<(string Key, SkinTemplateNode IR, string Source)>();
            foreach (var read in changed)
            {
                var key = read.Substring("skin:".Length);
                var fileName = fileNames[read];
                if (!_compiledIRs.TryGetValue(key, out var old) || !_templateNames.ContainsKey(key))
                {
                    return "razor skin not emitted " + fileName;
                }

                string source;
                using (var reader = new StreamReader(current[read].GetResourceStream()))
                {
                    source = reader.ReadToEnd();
                }

                SkinTemplateNode ir;
                try
                {
                    long ticks = 0;
                    ir = CompileSkin(_templateShortNames[key], source, fileName, out _, ref ticks, ref ticks);
                }
                catch (Exception ex)
                {
                    return "razor skin " + fileName + ": " + ex.Message;
                }

                if (ir.ModelTypeName != old.ModelTypeName
                    || ir.ControlTypeName != old.ControlTypeName
                    || !ir.StylesheetResourceNames.SequenceEqual(old.StylesheetResourceNames))
                {
                    return "razor skin types or styles " + fileName;
                }

                try
                {
                    TemplateIR.TemplateIRBuilder.ValidateCssClasses(ir, _templateCssManagers.GetValueOrDefault(key), source);
                }
                catch (Exception ex)
                {
                    return "razor skin css " + fileName + ": " + ex.Message;
                }

                var methods = new List<MethodReference>();
                var seen = new HashSet<string>();
                CollectEventMethodReferences(ir, ir.ModelTypeName, ir.ControlTypeName, methods, seen);
                CollectSubControlMethodReferences(ir, ir.ModelTypeName, ir.UsingNamespaces, methods, seen);
                CollectBindingExpressionReferences(ir, ir.ModelTypeName, ir.ControlTypeName, methods, seen);
                var added = methods.FirstOrDefault(method => !known.Contains(method.FullName));
                if (added != null)
                {
                    return "razor skin calls " + added.FullName;
                }

                patched.Add((key, ir, source));
                _resourcesRead[read] = System.Security.Cryptography.SHA256.HashData(current[read].GetResourceData());
            }

            int errors = _runtimeScopeManager.Context.Errors.Count;
            var section = new List<Statement>();
            foreach (var (key, ir, source) in patched)
            {
                _compiledIRs[key] = ir;
                _templateSources[key] = source;
                section.AddRange(GenerateTemplate(key, ir));
            }

            if (_runtimeScopeManager.Context.Errors.Count > errors)
            {
                return "razor errors";
            }

            statements.AddRange(_cssLiteralReplacer != null ? _cssLiteralReplacer.TransformStatements(section) : section);
            // A bundle with no Razor CSS wrote no CSS line: empty is no change.
            string css = CollectCssForEmission(_templateCssManagers.Values);
            if (css != (_writtenCss ?? ""))
            {
                if (_writtenCss == null)
                {
                    return "razor css added";
                }

                literals[CssMarker] = css;
                _writtenCss = css;
            }

            return null;
        }

        /// <summary>
        /// Dev patch emit: each skin's CSS manager that reads the stylesheet takes its new text,
        /// keeping its class names. Returns a miss reason, or null.
        /// </summary>
        private string PatchStylesheet(string resourceName, EmbeddedResource resource)
        {
            string cssText;
            using (var reader = new StreamReader(resource.GetResourceStream()))
            {
                cssText = reader.ReadToEnd();
            }

            foreach (var manager in _templateCssManagers.Values)
            {
                try
                {
                    var miss = manager.ReplaceStylesheet(resourceName, cssText);
                    if (miss != null)
                    {
                        return miss;
                    }
                }
                catch (Exception ex)
                {
                    return "razor css " + (_cssFileNames.GetValueOrDefault(resourceName) ?? resourceName) + ": " + ex.Message;
                }
            }

            return null;
        }

        /// <summary>
        /// Dev patch emit: a skin reads a simple getter as its backing field and writes a
        /// simple setter as one, so when such an accessor's body changed to anything but that
        /// read or store, the skins must be written again.
        /// </summary>
        public string TryPatchBodies(IReadOnlyList<TypeDefinition> changedTypes)
        {
            foreach (var (type, property, field, setter) in _inlinedGetters)
            {
                if (changedTypes.Contains(property.DeclaringType)
                    && (setter ? GraphDescriptorJSTEmitter.SimpleSetterField(type, property) : GraphDescriptorJSTEmitter.SimpleGetterField(type, property)) != field)
                {
                    return "razor inlined " + (setter ? "setter " : "getter ") + property.DeclaringType.FullName + "." + property.Name;
                }
            }

            return null;
        }

        public List<Statement> GetPostJavascript()
        {
            if (!_hasRazorTemplates)
                return new List<Statement>();

            var statements = new List<Statement>();

            // Razor's own document store getter, which its skins and live helpers use.
            var docStorageGetterStatements = EmitDocStorageGetterFunction();
            if (docStorageGetterStatements != null)
            {
                statements.AddRange(docStorageGetterStatements);
                if (LiveHook)
                {
                    statements.AddRange(CreateLiveSkinHelpers());
                }
            }

            foreach (var kvp in _compiledIRs)
            {
                statements.AddRange(GenerateTemplate(kvp.Key, kvp.Value));
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
                statements.Count, _compiledIRs.Count);

            return statements;
        }

        /// <summary>
        /// A template's statements, with graph descriptor emission. A template written before
        /// (a patch) keeps its names and takes the next storage index.
        /// </summary>
        private List<Statement> GenerateTemplate(string key, SkinTemplateNode ir)
        {
            try
            {
                IIdentifier preCreatedGetter = null;
                _templateGetterIdentifiers.TryGetValue(key, out preCreatedGetter);

                RazorCssManager cssManager = null;
                if (!_templateCssManagers.TryGetValue(key, out cssManager)
                    && _templateCssManagers.Count > 0)
                {
                    // Sub-templates without @styles inherit the parent's CSS manager
                    // so their static HTML class names get resolved through the CSS scope.
                    cssManager = _templateCssManagers.Values.First();
                }

                var jstGenerator = new RazorSkinJSTGenerator(
                    ir,
                    _runtimeScopeManager,
                    _clrContext,
                    _resolvedIdentifiers,
                    _resolvedTypeIdentifiers,
                    _razorKnownTypes,
                    _nextDataIndex++,
                    preCreatedGetter,
                    cssManager,
                    _inlinedGetters,
                    LiveHook,
                    _templateNames.GetValueOrDefault(key));

                var jstStatements = jstGenerator.Generate();
                _templateNames[key] = jstGenerator.Names;

                // Store the getter identifier for use in GetOverwrite
                var getterIdentifier = jstGenerator.GetGetterIdentifier();
                if (getterIdentifier != null)
                {
                    _templateGetterIdentifiers[key] = getterIdentifier;
                }

                Log.Debug("Generated {StatementCount} JST statements for template {TemplateName}",
                    jstStatements.Count, ir.TemplateName);
                return jstStatements;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "JST generation failed for template {TemplateName}", ir.TemplateName);

                _runtimeScopeManager.Context.AddTemplateError(
                    (ex as RazorSubControlDiagnosticException)?.Location,
                    $"Error generating JST for Razor template '{ir.TemplateName}': {ex.Message}",
                    false);
                return new List<Statement>();
            }
        }

        /// <summary>
        /// The live skin helpers (<see cref="LiveSkinHook.CreateHelpers"/>), for a bundle whose
        /// document storage getter this plugin writes.
        /// </summary>
        private List<Statement> CreateLiveSkinHelpers()
        {
            var scope = _runtimeScopeManager.Scope;
            var skinable = _razorKnownTypes.UISkinableElement;
            IIdentifier Field(TypeDefinition type, string name)
                => _runtimeScopeManager.Resolve(type.Fields.First(field => field.Name == name));

            // function(c, v) { c.Skin = v; }
            var setter = skinable.Properties.First(property => property.Name == "Skin").SetMethod;
            var setScope = new IdentifierScope(scope, new[] { "c", "v" }, false);
            var control = new IdentifierExpression(setScope.ParameterIdentifiers[0], setScope);
            var value = new IdentifierExpression(setScope.ParameterIdentifiers[1], setScope);
            var call = (setter.IsVirtual && !setter.IsFinal) || !_runtimeScopeManager.ImplementInstanceAsStatic
                ? new MethodCallExpression(
                    null,
                    setScope,
                    new IndexExpression(null, setScope, control, new IdentifierExpression(_runtimeScopeManager.Resolve(setter), setScope)),
                    value)
                : new MethodCallExpression(
                    null,
                    setScope,
                    new IdentifierExpression(_runtimeScopeManager.ResolveStatic(setter), setScope),
                    control,
                    value);
            var setSkin = new FunctionExpression(null, scope, setScope, setScope.ParameterIdentifiers, null);
            setSkin.AddStatement(new ExpressionStatement(null, setScope, call));

            return LiveSkinHook.CreateHelpers(
                scope,
                _resolvedIdentifiers["DocStorageGetter"],
                Field(_razorKnownTypes.SkinType, "factoryMethod"),
                Field(_razorKnownTypes.SkinType, "id"),
                Field(_razorKnownTypes.SkinInstanceType, "skinableParent"),
                Field(skinable, "skinInstance"),
                setSkin);
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

                // Look up the Document type so we can create the razorStore field identifier on it.
                var documentTypeDef = _clrContext.GetTypeDefinition(
                    Tuple.Create("System.Web.Html", "System.Web.Html.Document"));

                if (documentTypeDef == null)
                {
                    Log.Warning("Could not find Document type for DocStorageGetter emission");
                    return null;
                }

                // Razor's own extension field on Document's type scope; XWML's is stateStore.
                IIdentifier razorStoreId = _runtimeScopeManager.GetTypeScope(documentTypeDef)
                    .GetIdentifier("razorStore", true, false);

                // Build: function DocStorageGetter(doc) { if (!doc.razorStore) { doc.razorStore = []; } return doc.razorStore; }
                var methodScope = new IdentifierScope(
                    scope,
                    new string[] { "doc" },
                    false);

                IIdentifier docParam = methodScope.ParameterIdentifiers[0];

                // doc.razorStore = []
                var initStmts = new List<Statement>();
                initStmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, razorStoreId }),
                        new NewArrayExpression(null, methodScope, null)));

                // if (!doc.razorStore) { doc.razorStore = []; }
                var ifStmt = new IfBlockStatement(
                    null, methodScope,
                    new UnaryExpression(
                        null, methodScope,
                        UnaryOperator.LogicalNot,
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, razorStoreId })),
                    new ScopeBlock(null, methodScope, initStmts),
                    null);

                // function DocStorageGetter(doc) { ... }
                var funcExpr = new FunctionExpression(
                    null, scope, methodScope,
                    methodScope.ParameterIdentifiers,
                    docStorageGetterId);

                funcExpr.AddStatement(ifStmt);

                // return doc.razorStore;
                funcExpr.AddStatement(
                    new ReturnStatement(
                        null, methodScope,
                        IdentifierExpression.Create(
                            null, methodScope,
                            new IIdentifier[] { docParam, razorStoreId })));

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
