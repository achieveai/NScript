using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Validates that all ExpectedOutput/*.js snapshot files exist, are non-empty,
    /// and have corresponding Templates/*.skin.cshtml source files.
    ///
    /// TODO: These snapshot files should be connected to actual generation tests that:
    /// 1. Compile a template from Templates/*.skin.cshtml using RazorSkinCompiler.CompileToIR()
    /// 2. Generate JS output via RazorSkinJSTGenerator.Generate() (requires ClrContext/RuntimeScopeManager)
    /// 3. Compare against the corresponding ExpectedOutput/*.js file
    /// Full pipeline snapshot tests require the NScript compilation infrastructure (ClrContext,
    /// RuntimeScopeManager, Mono.Cecil module loading) which is too heavy for unit tests.
    /// A future integration test project could wire these up.
    /// </summary>
    [TestClass]
    public class SnapshotFileValidationTests
    {
        private static readonly string[] ExpectedSnapshotNames = new[]
        {
            "AttributeBinding",
            "ComputedExpression",
            "ConditionalBlock",
            "ControlBinding",
            "EventLambda",
            "EventMethodRef",
            "ForeachBlock",
            "ModelFunction",
            "NestedControlFlow",
            "OneTimeBinding",
            "PureFunction",
            "ReactiveForeach",
            "ReactiveIf",
            "SimpleBinding",
            "StaticIf",
            "TextBinding"
        };

        [TestMethod]
        public void AllSnapshotFilesExist()
        {
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            foreach (var name in ExpectedSnapshotNames)
            {
                var path = Path.Combine(outputDir, name + ".js");
                File.Exists(path).Should().BeTrue(
                    $"Expected snapshot file {name}.js should exist in ExpectedOutput/");
            }
        }

        [TestMethod]
        public void AllSnapshotFilesAreNonEmpty()
        {
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            foreach (var name in ExpectedSnapshotNames)
            {
                var path = Path.Combine(outputDir, name + ".js");
                if (File.Exists(path))
                {
                    var content = File.ReadAllText(path);
                    content.Should().NotBeNullOrWhiteSpace(
                        $"Snapshot file {name}.js should not be empty");
                }
            }
        }

        [TestMethod]
        public void AllSnapshotFilesHaveCorrespondingTemplates()
        {
            var templateDir = Path.Combine(GetTestOutputDir(), "Templates");

            foreach (var name in ExpectedSnapshotNames)
            {
                var templatePath = Path.Combine(templateDir, name + ".skin.cshtml");
                File.Exists(templatePath).Should().BeTrue(
                    $"Template file {name}.skin.cshtml should exist as source for snapshot {name}.js");
            }
        }

        [TestMethod]
        public void AllTemplateFilesHaveCorrespondingSnapshots()
        {
            var templateDir = Path.Combine(GetTestOutputDir(), "Templates");
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            if (!Directory.Exists(templateDir)) return;

            var templateNames = Directory.GetFiles(templateDir, "*.skin.cshtml")
                .Select(f => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)));

            foreach (var name in templateNames)
            {
                var snapshotPath = Path.Combine(outputDir, name + ".js");
                File.Exists(snapshotPath).Should().BeTrue(
                    $"Template {name}.skin.cshtml should have a corresponding snapshot {name}.js");
            }
        }

        [TestMethod]
        public void SnapshotCount_Matches_ExpectedCount()
        {
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            if (!Directory.Exists(outputDir)) return;

            var actualFiles = Directory.GetFiles(outputDir, "*.js");
            actualFiles.Length.Should().Be(ExpectedSnapshotNames.Length,
                "the number of .js snapshot files should match the expected count");
        }

        [TestMethod]
        public void SnapshotFiles_ContainJavaScriptCode()
        {
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            foreach (var name in ExpectedSnapshotNames)
            {
                var path = Path.Combine(outputDir, name + ".js");
                if (!File.Exists(path)) continue;

                var content = File.ReadAllText(path);

                // All snapshots should contain function declarations (template factory pattern)
                content.Should().Contain("function",
                    $"Snapshot {name}.js should contain JavaScript function declarations");
            }
        }

        [TestMethod]
        public void SnapshotFiles_FollowNamingConvention()
        {
            var outputDir = Path.Combine(GetTestOutputDir(), "ExpectedOutput");

            foreach (var name in ExpectedSnapshotNames)
            {
                var path = Path.Combine(outputDir, name + ".js");
                if (!File.Exists(path)) continue;

                var content = File.ReadAllText(path);

                // Each snapshot should reference its template name in factory/getter functions
                content.Should().Contain(name + "_factory",
                    $"Snapshot {name}.js should contain {name}_factory function");
                content.Should().Contain(name + "_var",
                    $"Snapshot {name}.js should contain {name}_var storage variable");
            }
        }

        private static string GetTestOutputDir()
        {
            // MSTest copies files to the output directory
            return Path.GetDirectoryName(typeof(SnapshotFileValidationTests).Assembly.Location);
        }
    }
}
