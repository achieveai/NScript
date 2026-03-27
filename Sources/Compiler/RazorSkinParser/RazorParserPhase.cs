using System;
using System.IO;
using Microsoft.AspNetCore.Razor.Language;
using Serilog;

namespace NScript.RazorSkin
{
    public class RazorParseResult
    {
        public string GeneratedCSharp { get; set; }
        public RazorSyntaxTree SyntaxTree { get; set; }
        public RazorCodeDocument CodeDocument { get; set; }
    }

    public static class RazorParserPhase
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        // Cache the engine since configuration doesn't change between calls
        private static readonly RazorProjectEngine _engine = RazorProjectEngine.Create(
            RazorConfiguration.Default,
            RazorProjectFileSystem.Create("."),
            builder =>
            {
                builder.SetRootNamespace("NScript.RazorSkin.Generated");
            });

        public static RazorParseResult Parse(string templateName, string cleanedTemplate)
        {
            var projectEngine = _engine;

            var sourceDocument = RazorSourceDocument.Create(
                cleanedTemplate,
                $"{templateName}.skin.cshtml");

            var codeDocument = RazorCodeDocument.Create(sourceDocument);
            projectEngine.Engine.Process(codeDocument);

            var csharpDocument = codeDocument.GetCSharpDocument();
            var syntaxTree = codeDocument.GetSyntaxTree();

            var result = new RazorParseResult
            {
                GeneratedCSharp = csharpDocument.GeneratedCode,
                SyntaxTree = syntaxTree,
                CodeDocument = codeDocument
            };

            Log.Debug("Razor parse produced C# of length {GeneratedCSharpLength}, syntax tree has {DiagnosticCount} diagnostics",
                csharpDocument.GeneratedCode?.Length ?? 0,
                syntaxTree.Diagnostics?.Count ?? 0);

            return result;
        }
    }
}
