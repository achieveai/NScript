using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Utility class to generate expected output baselines.
    /// Run these tests ONCE to populate ExpectedOutput/, then verify the output manually.
    /// After verification, these tests can be disabled (they are marked [Ignore] by default).
    /// </summary>
    [TestClass]
    public class SnapshotGenerator
    {
        private static readonly string[] AllTemplates = new[]
        {
            "TextBinding",
            "ComputedExpression",
            "AttributeBinding",
            "OneTimeBinding",
            "ReactiveIf",
            "StaticIf",
            "ReactiveForeach",
            "NestedControlFlow",
            "EventMethodRef",
            "EventLambda",
            "PureFunction",
            "ModelFunction",
            "ControlBinding",
            "SimpleBinding",
            "ConditionalBlock",
            "ForeachBlock"
        };

        [TestMethod]
        [TestCategory("SnapshotGeneration")]
        public void GenerateAllSnapshots()
        {
            var dir = Path.GetDirectoryName(typeof(RazorSkinTestHelper).Assembly.Location);
            var outputDir = Path.Combine(dir, "ExpectedOutput");
            Directory.CreateDirectory(outputDir);

            foreach (var name in AllTemplates)
            {
                try
                {
                    var js = RazorSkinTestHelper.CompileTemplate(name);
                    var outputPath = Path.Combine(outputDir, name + ".js");
                    File.WriteAllText(outputPath, js);

                    Console.WriteLine($"Generated: {name}.js ({js.Length} chars)");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"FAILED: {name} - {ex.Message}");
                }
            }
        }
    }
}
