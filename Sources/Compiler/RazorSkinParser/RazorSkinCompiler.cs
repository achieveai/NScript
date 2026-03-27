using System.Diagnostics;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using Serilog;
using Serilog.Formatting.Compact;

namespace NScript.RazorSkin
{
    public static class RazorSkinCompiler
    {
        private static readonly ILogger Log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithProperty("application", "RazorSkinCompiler")
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: "logs/razor-skin-compiler.log.jsonl",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        /// <summary>
        /// Static logger accessible to other classes in the pipeline.
        /// </summary>
        public static ILogger Logger => Log;

        /// <summary>
        /// Full pipeline: .skin.cshtml source → JavaScript factory code.
        /// </summary>
        public static string Compile(
            string templateName,
            string templateSource,
            string[] additionalCSharpSources = null)
        {
            var totalSw = Stopwatch.StartNew();
            Log.Debug("Compile started for template {TemplateName}", templateName);

            // Phase 1: Preprocess
            var phaseSw = Stopwatch.StartNew();
            Log.Debug("Phase {Phase} started for template {TemplateName}", "Preprocess", templateName);
            var preprocessed = RazorSkinPreprocessor.Process(templateSource);
            Log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "Preprocess", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 2: Razor parse
            phaseSw.Restart();
            Log.Debug("Phase {Phase} started for template {TemplateName}", "RazorParse", templateName);
            var parsed = RazorParserPhase.Parse(templateName, preprocessed.CleanedTemplate);
            Log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "RazorParse", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 3: Build IR
            phaseSw.Restart();
            Log.Debug("Phase {Phase} started for template {TemplateName}", "BuildIR", templateName);
            var ir = TemplateIRBuilder.Build(templateName, preprocessed, parsed);
            Log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "BuildIR", phaseSw.ElapsedMilliseconds, templateName);

            // Phase 4: Roslyn analysis (refine classifications)
            // When additionalCSharpSources is null, Roslyn analysis is skipped and all
            // bindings remain classified as OneTime. This is acceptable for preview/tooling
            // scenarios. For production compilation, pass framework type stubs (at minimum
            // ObservableObject, INotifyPropertyChanged, IObservableCollection) plus the
            // model/control type source to enable accurate OneWay/observable classification.
            if (additionalCSharpSources != null && additionalCSharpSources.Length > 0)
            {
                phaseSw.Restart();
                Log.Debug("Phase {Phase} started for template {TemplateName}", "RoslynAnalysis", templateName);
                RoslynAnalysisPhase.RefineClassifications(
                    ir, parsed.GeneratedCSharp, additionalCSharpSources);
                Log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "RoslynAnalysis", phaseSw.ElapsedMilliseconds, templateName);
            }

            // Phase 5: Generate JS
            phaseSw.Restart();
            Log.Debug("Phase {Phase} started for template {TemplateName}", "GenerateJS", templateName);
            var result = RazorSkinCodeGenerator.Generate(ir);
            Log.Debug("Phase {Phase} completed in {ElapsedMs}ms for template {TemplateName}", "GenerateJS", phaseSw.ElapsedMilliseconds, templateName);

            totalSw.Stop();
            Log.Debug("Compile completed successfully in {TotalElapsedMs}ms for template {TemplateName}", totalSw.ElapsedMilliseconds, templateName);

            return result;
        }
    }
}
