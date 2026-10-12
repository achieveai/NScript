namespace NScript.Utils.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The repo's own build files: each test guards a bug the build service work found in them.
    /// </summary>
    [TestClass]
    public class RepoBuildFilesTests
    {
        private static string GetRepoRoot()
        {
            // The test DLL lives at Test/Compiler/bin/<tfm>/; climb four levels to the worktree root.
            string dir = Path.GetDirectoryName(typeof(RepoBuildFilesTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        /// <summary>Runs <c>dotnet msbuild</c> and returns its output; fails the test on a non-zero exit.</summary>
        private static string MsBuild(string workingDirectory, params string[] args)
        {
            var psi = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            foreach (string arg in new[] { "msbuild", "-nologo", "-nr:false" }.Concat(args))
            {
                psi.ArgumentList.Add(arg);
            }

            // dotnet test sets these for its own MSBuild; the child build resolves its own SDK.
            foreach (string name in psi.Environment.Keys.Where(k => k.StartsWith("MSBuild", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                psi.Environment.Remove(name);
            }

            using var process = Process.Start(psi);
            var stderr = process.StandardError.ReadToEndAsync();
            string stdout = process.StandardOutput.ReadToEnd();
            Assert.IsTrue(process.WaitForExit(120_000), "dotnet msbuild did not finish in 120 s");
            Assert.AreEqual(0, process.ExitCode, stdout + stderr.Result);
            return stdout;
        }

        private static string Property(string json, string name)
            => JsonDocument.Parse(json).RootElement.GetProperty("Properties").GetProperty(name).GetString();

        private static string Normalize(string path) => path.Replace('\\', '/').Replace("//", "/");

        /// <summary>
        /// B9: <c>dotnet build</c> without <c>-c</c> failed with MSB6004. The SDK sets Configuration
        /// after Directory.Build.props, so the compiler path came out as <c>bin\net8.0\</c>.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild evaluation (~1 s).
        public void FrameworkProject_NoConfiguration_UsesTheDebugCompiler()
        {
            string root = GetRepoRoot();
            string json = MsBuild(
                root,
                Path.Combine(root, "Sources", "Framework", "Sunlight.Framework", "Sunlight.Framework.csproj"),
                "-getProperty:Configuration",
                "-getProperty:CscToolPath");

            Assert.AreEqual("Debug", Property(json, "Configuration"));
            StringAssert.EndsWith(Normalize(Property(json, "CscToolPath")), "/NScriptToolSet/bin/Debug/net8.0/");
        }

        /// <summary>
        /// A Test/Compiler build rebuilt its compiler references into NScriptToolSet/bin, the
        /// toolset the SDK and the build service run from: the toolset hash changed and the
        /// daemon was orphaned. Its references now build into Test/Compiler/compiler-out.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // Two dotnet msbuild evaluations (~2 s).
        public void CompilerTestProject_BuildsItsCompilerReferencesOutsideTheToolset()
        {
            string root = GetRepoRoot();
            string json = MsBuild(
                root,
                Path.Combine(root, "Test", "Compiler", "NScript.Utils.Test", "NScript.Utils.Test.csproj"),
                "-getItem:ProjectReference");
            var references = JsonDocument.Parse(json).RootElement.GetProperty("Items").GetProperty("ProjectReference").EnumerateArray().ToList();
            Assert.IsTrue(references.Count > 0, json);
            string privateOutput = Normalize(Path.Combine(root, "Test", "Compiler", "compiler-out") + "/");
            foreach (var reference in references)
            {
                Assert.AreEqual(
                    "NScriptCompilerPrivateOutput=" + privateOutput,
                    Normalize(reference.GetProperty("AdditionalProperties").GetString()),
                    reference.GetProperty("Identity").GetString());
            }

            json = MsBuild(
                root,
                Path.Combine(root, "Sources", "Compiler", "NScript.Converter", "NScript.Converter.csproj"),
                "-p:NScriptCompilerPrivateOutput=" + privateOutput,
                "-getProperty:OutputPath",
                "-getProperty:BaseIntermediateOutputPath");
            StringAssert.StartsWith(Normalize(Property(json, "OutputPath")), privateOutput + "bin/");
            StringAssert.StartsWith(Normalize(Property(json, "BaseIntermediateOutputPath")), privateOutput + "obj/");
        }

        /// <summary>
        /// B16: on a fresh clone the toolset bootstrap built JsCsc and NScript, which the
        /// building project's restore never saw, and failed with NETSDK1004. Each bootstrap
        /// target restores them before it builds them.
        /// </summary>
        [TestMethod]
        public void ToolsetBootstrap_RestoresTheCompilerProjectsBeforeBuildingThem()
        {
            string root = GetRepoRoot();
            foreach (var (file, target) in new[]
            {
                (Path.Combine(root, "Sources", "Framework", "Directory.Build.props"), "EnsureNScriptToolsetBuilt"),
                (Path.Combine(root, "Test", "Framework", "Directory.build.props"), "EnsureNScriptDebugToolsetBuilt"),
            })
            {
                var calls = XDocument.Load(file).Descendants("Target")
                    .Single(t => (string)t.Attribute("Name") == target)
                    .Elements("MSBuild")
                    .Select(m => ((string)m.Attribute("Targets"), (string)m.Attribute("Projects")))
                    .ToList();
                int build = calls.FindIndex(c => c.Item1 == "Build");
                int restore = calls.FindIndex(c => c.Item1 == "Restore");
                Assert.IsTrue(build >= 0, target + " builds the toolset");
                Assert.IsTrue(restore >= 0 && restore < build, target + " must restore before it builds");
                Assert.AreEqual(calls[build].Item2, calls[restore].Item2, target + " restores the projects it builds");
            }
        }

        /// <summary>
        /// The framework projects' <c>.repo.bin</c> is a CoreCompile input. WriteLinesToFile
        /// skips its content check when Encoding is set, so it was written on every build and
        /// every framework project compiled again. An unchanged capture keeps its timestamp.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // Two dotnet msbuild runs (~3 s) and git.
        public void PackageMetadata_CapturedAgainUnchanged_KeepsTheRepoBinTimestamp()
        {
            string root = GetRepoRoot();
            string dir = Path.Combine(Path.GetTempPath(), "nscript-repobin-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                File.WriteAllText(
                    Path.Combine(dir, "Capture.proj"),
                    "<Project>\n" +
                    "  <PropertyGroup>\n" +
                    "    <NScriptRoot>" + root + Path.DirectorySeparatorChar + "</NScriptRoot>\n" +
                    "    <PackageId>Fake.Package</PackageId>\n" +
                    "    <IntermediateOutputPath>obj" + Path.DirectorySeparatorChar + "</IntermediateOutputPath>\n" +
                    "  </PropertyGroup>\n" +
                    "  <Import Project=\"" + Path.Combine(root, "Sources", "Framework", "NScript.PackageMetadata.targets") + "\" />\n" +
                    "</Project>\n");

                string repoBin = Path.Combine(dir, "obj", "Fake.Package.repo.bin");
                MsBuild(dir, "Capture.proj", "-t:CaptureNScriptPackageRepoMetadata");
                Assert.IsTrue(File.Exists(repoBin), "The capture must write " + repoBin + " (is git on PATH?)");
                string content = File.ReadAllText(repoBin);
                var old = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                File.SetLastWriteTimeUtc(repoBin, old);

                MsBuild(dir, "Capture.proj", "-t:CaptureNScriptPackageRepoMetadata");
                Assert.AreEqual(content, File.ReadAllText(repoBin));
                Assert.AreEqual(old, File.GetLastWriteTimeUtc(repoBin), "An unchanged .repo.bin must not be written again.");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
