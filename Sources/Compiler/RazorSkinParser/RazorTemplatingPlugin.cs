using System;
using System.IO;

namespace NScript.RazorSkin
{
    /// <summary>
    /// NScript compiler plugin that processes .skin.cshtml files.
    /// Implements the same interfaces as XwmlTemplatingPlugin for coexistence.
    /// </summary>
    public class RazorTemplatingPlugin
    {
        // NOTE: This class will implement IMethodConverterPlugin and IRuntimeConverterPlugin
        // once wired into the NScript compiler. For now it provides a static entry point
        // for processing .skin.cshtml files found in a project directory.

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
    }
}
