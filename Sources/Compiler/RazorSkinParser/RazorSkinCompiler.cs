using System.Diagnostics;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;
using Serilog;

namespace NScript.RazorSkin
{
    public static class RazorSkinCompiler
    {
        /// <summary>
        /// Static logger accessible to other classes in the pipeline. Routes through
        /// the shared <see cref="CompilerLog"/> facility — returns a silent no-op
        /// logger when <c>--log</c> has not been supplied to the host (csc/cs2jsc),
        /// so no file I/O occurs by default.
        /// </summary>
        public static ILogger Logger => CompilerLog.ForComponent("RazorSkinParser");

        /// <summary>
        /// Compiles a .skin.cshtml template through all phases and returns the template IR.
        /// JavaScript emission is handled by RazorSkinJSTGenerator via the plugin pipeline.
        /// </summary>
        public static SkinTemplateNode CompileToIR(
            string templateName,
            string templateSource,
            string[] additionalCSharpSources = null,
            string sourceFile = null)
        {
            var log = Logger;
            var totalSw = Stopwatch.StartNew();
            log.Debug("Compile started for template {TemplateName}", templateName);

            // Phase 1: Preprocess
            var phaseSw = Stopwatch.StartNew();
            log.Debug("Phase {Phase} started for template {TemplateName}", "Preprocess", templateName);
            var preprocessed = RazorSkinPreprocessor.Process(templateSource);
            log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "Preprocess", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 2: Razor parse
            phaseSw.Restart();
            log.Debug("Phase {Phase} started for template {TemplateName}", "RazorParse", templateName);
            var parsed = RazorParserPhase.Parse(templateName, preprocessed.CleanedTemplate);
            log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "RazorParse", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 3: Build IR
            phaseSw.Restart();
            log.Debug("Phase {Phase} started for template {TemplateName}", "BuildIR", templateName);
            var ir = TemplateIRBuilder.Build(templateName, preprocessed, parsed, sourceFile);
            log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "BuildIR", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 4: Roslyn analysis (refine classifications)
            if (additionalCSharpSources != null && additionalCSharpSources.Length > 0)
            {
                phaseSw.Restart();
                log.Debug("Phase {Phase} started for template {TemplateName}", "RoslynAnalysis", templateName);
                RoslynAnalysisPhase.RefineClassifications(
                    ir, parsed.GeneratedCSharp, additionalCSharpSources);
                log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "RoslynAnalysis", phaseSw.ElapsedMilliseconds, templateName);
            }

            totalSw.Stop();
            log.Debug("Compile completed successfully in {TotalElapsedMs}ms for template {TemplateName}", totalSw.ElapsedMilliseconds, templateName);

            return ir;
        }
    }
}
