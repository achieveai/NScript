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
        /// Whether any .skin.cshtml resources were found during initialization.
        /// </summary>
        private bool _hasRazorTemplates;

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

                            var js = RazorSkinCompiler.Compile(
                                templateName, templateSource,
                                new[] { FrameworkTypeStubs });
                            _compiledTemplates[templateName] = js;
                            _hasRazorTemplates = true;

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

            Log.Debug("Resolved template {TemplateName} for overwrite", templateName);

            // Return a call to the template getter function
            // This mirrors what XwmlTemplatingPlugin does: return SkinTemplateName();
            return new List<Statement>
            {
                new ReturnStatement(
                    null,
                    methodConverter.Scope,
                    new MethodCallExpression(
                        null,
                        methodConverter.Scope,
                        new IdentifierExpression(
                            SimpleIdentifier.CreateScopeIdentifier(
                                methodConverter.Scope,
                                templateName,
                                false),
                            methodConverter.Scope)))
            };
        }

        // --- IRuntimeConverterPlugin ---

        public List<MethodReference> GetMethodsToEmitPass1()
        {
            return new List<MethodReference>();
        }

        public List<MethodReference> GetMethodsToEmitPassN()
        {
            return new List<MethodReference>();
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
            var totalSize = 0;
            foreach (var kvp in _compiledTemplates)
            {
                statements.Add(new RawJavaScriptStatement(kvp.Value));
                totalSize += kvp.Value.Length;
            }

            Log.Debug("GetPostJavascript emitting {TemplateCount} templates, total JS size {TotalJsSize} chars",
                _compiledTemplates.Count, totalSize);

            return statements;
        }
    }
}
