using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin
{
    public static class RazorSkinCompiler
    {
        /// <summary>
        /// Full pipeline: .skin.cshtml source → JavaScript factory code.
        /// </summary>
        public static string Compile(
            string templateName,
            string templateSource,
            string[] additionalCSharpSources = null)
        {
            // Phase 1: Preprocess
            var preprocessed = RazorSkinPreprocessor.Process(templateSource);

            // Phase 2: Razor parse
            var parsed = RazorParserPhase.Parse(templateName, preprocessed.CleanedTemplate);

            // Phase 3: Build IR
            var ir = TemplateIRBuilder.Build(templateName, preprocessed, parsed);

            // Phase 4: Roslyn analysis (refine classifications)
            // When additionalCSharpSources is null, Roslyn analysis is skipped and all
            // bindings remain classified as OneTime. This is acceptable for preview/tooling
            // scenarios. For production compilation, pass framework type stubs (at minimum
            // ObservableObject, INotifyPropertyChanged, IObservableCollection) plus the
            // model/control type source to enable accurate OneWay/observable classification.
            if (additionalCSharpSources != null && additionalCSharpSources.Length > 0)
            {
                RoslynAnalysisPhase.RefineClassifications(
                    ir, parsed.GeneratedCSharp, additionalCSharpSources);
            }

            // Phase 5: Generate JS
            return RazorSkinCodeGenerator.Generate(ir);
        }
    }
}
