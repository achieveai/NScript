using System;
using System.IO;
using Microsoft.AspNetCore.Razor.Language;

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
        public static RazorParseResult Parse(string templateName, string cleanedTemplate)
        {
            var projectEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create("."),
                builder =>
                {
                    builder.SetRootNamespace("NScript.RazorSkin.Generated");
                });

            var sourceDocument = RazorSourceDocument.Create(
                cleanedTemplate,
                $"{templateName}.skin.cshtml");

            var codeDocument = RazorCodeDocument.Create(sourceDocument);
            projectEngine.Engine.Process(codeDocument);

            var csharpDocument = codeDocument.GetCSharpDocument();
            var syntaxTree = codeDocument.GetSyntaxTree();

            return new RazorParseResult
            {
                GeneratedCSharp = csharpDocument.GeneratedCode,
                SyntaxTree = syntaxTree,
                CodeDocument = codeDocument
            };
        }
    }
}
