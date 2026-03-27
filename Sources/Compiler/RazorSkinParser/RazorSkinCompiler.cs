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
            if (additionalCSharpSources != null)
            {
                RoslynAnalysisPhase.RefineClassifications(
                    ir, parsed.GeneratedCSharp, additionalCSharpSources);
            }

            // Phase 5: Generate JS
            return RazorSkinCodeGenerator.Generate(ir);
        }
    }
}
