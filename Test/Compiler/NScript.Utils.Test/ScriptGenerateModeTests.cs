namespace NScript.Utils.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Runs the real Sdk.targets ScriptGenerate against a stand-in nscript that writes its mode
    /// into the JS. The JS is shared by every configuration and is also written by the build
    /// service and its watch, so a timestamp-only up-to-date check kept JS of the wrong mode (PC-I3d).
    /// </summary>
    [TestClass]
    public class ScriptGenerateModeTests
    {
        // Logs its mode and writes it into the JS; with fail.flag present it writes a partial JS and fails.
        private const string FakeNScript =
            "@echo off\r\n" +
            "set mode=batch\r\n" +
            "echo %* | findstr /C:\" -service\" >nul && set mode=dev\r\n" +
            "echo %* | findstr /C:\" -minify\" >nul && set mode=release\r\n" +
            "if exist \"%~dp0fail.flag\" (\r\n" +
            "  echo // partial> \"%~dp0out\\Fake.js\"\r\n" +
            "  echo failed>> \"%~dp0out\\runs.log\"\r\n" +
            "  exit /b 1\r\n" +
            ")\r\n" +
            "echo // mode=%mode%> \"%~dp0out\\Fake.js\"\r\n" +
            "echo %mode%>> \"%~dp0out\\runs.log\"\r\n";

        private const string FakeProject =
            "<Project>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.props\" />\n" +
            "  <PropertyGroup>\n" +
            "    <AssemblyName>Fake</AssemblyName>\n" +
            "    <GenerateJs>True</GenerateJs>\n" +
            "    <JsOutputPath>out</JsOutputPath>\n" +
            "    <NScriptExe>$(MSBuildThisFileDirectory)fake-nscript.cmd</NScriptExe>\n" +
            "  </PropertyGroup>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.targets\" />\n" +
            "</Project>\n";

        // Distinct global properties per step give each step a fresh evaluation in one MSBuild run.
        private const string Driver =
            "<Project>\n" +
            "  <Target Name=\"Run\">\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=1;NScriptService=true\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=2\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=3\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=4;Configuration=Release\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=5\" />\n" +
            "    <WriteLinesToFile File=\"out\\Fake.js\" Lines=\"// rewritten by watch\" Overwrite=\"true\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=6\" />\n" +
            "    <Delete Files=\"out\\Fake.js\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=7\" />\n" +
            "    <Touch Files=\"fail.flag\" AlwaysCreate=\"true\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=8;Configuration=Release\" ContinueOnError=\"true\" />\n" +
            "    <Delete Files=\"fail.flag\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=9;Configuration=Release\" />\n" +
            "    <MSBuild Projects=\"Fake.proj\" Targets=\"ScriptGenerate\" Properties=\"SdkDir=$(SdkDir);Step=10;Configuration=Release\" />\n" +
            "  </Target>\n" +
            "</Project>\n";

        private static string GetRepoRoot()
        {
            // The test DLL lives at Test/Compiler/bin/<tfm>/; climb four levels to the worktree root.
            string dir = Path.GetDirectoryName(typeof(ScriptGenerateModeTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s); the only check of ScriptGenerate's up-to-date decision.
        public void ScriptGenerate_RerunsWhenModeOrWriterChanges_SkipsWhenUnchanged()
        {
            if (!OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("The stand-in nscript is a .cmd script.");
            }

            string sdkDir = Path.Combine(GetRepoRoot(), "Sources", "Compiler", "NScript.Sdk", "Sdk") + Path.DirectorySeparatorChar;
            Assert.IsTrue(File.Exists(sdkDir + "Sdk.targets"), $"Expected SDK targets in {sdkDir}");

            string dir = Path.Combine(Path.GetTempPath(), "nscript-jsmode-" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(Path.Combine(dir, "out"));
                foreach (string config in new[] { "Debug", "Release" })
                {
                    // Older than every JS the steps write, as after a build with no source change.
                    string objDir = Path.Combine(dir, "obj", config, "netstandard2.1");
                    Directory.CreateDirectory(objDir);
                    File.WriteAllText(Path.Combine(objDir, "Fake.dll"), string.Empty);
                }

                File.WriteAllText(Path.Combine(dir, "fake-nscript.cmd"), FakeNScript);
                File.WriteAllText(Path.Combine(dir, "Fake.proj"), FakeProject);
                File.WriteAllText(Path.Combine(dir, "Driver.proj"), Driver);

                var psi = new ProcessStartInfo("dotnet")
                {
                    WorkingDirectory = dir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                foreach (string arg in new[] { "msbuild", "Driver.proj", "-t:Run", "-nologo", "-nr:false", "-v:m", "-p:SdkDir=" + sdkDir })
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
                string output = stdout + stderr.Result;

                // Step 8's failure is the only error, and ContinueOnError lets the run go on.
                Assert.AreEqual(0, process.ExitCode, output);

                // 1 service writes dev JS; 2 a plain build replaces it; 3 nothing changed, skipped;
                // 4 Release replaces it; 5 Debug replaces it; 6 another writer rewrote it, replaced;
                // 7 JS deleted, rebuilt; 8 Release fails after a partial write; 9 Release reruns;
                // 10 nothing changed, skipped.
                string[] runs = File.ReadAllLines(Path.Combine(dir, "out", "runs.log"));
                CollectionAssert.AreEqual(
                    new[] { "dev", "batch", "release", "batch", "batch", "batch", "failed", "release" },
                    runs,
                    "nscript runs: " + string.Join(",", runs) + Environment.NewLine + output);
                Assert.AreEqual("// mode=release", File.ReadAllText(Path.Combine(dir, "out", "Fake.js")).Trim());
                StringAssert.Contains(output, "exited with code 1", "step 8 should have failed");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
