namespace NScript.Utils.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Lib.Service;

    /// <summary>
    /// Runs the real Sdk.targets watch sync against a stand-in nscript that answers yes and vouches
    /// for one project reference. A synced build skips evaluating that reference, so it must hand
    /// ResolveAssemblyReferences the item GetTargetPath would have returned: the implementation DLL
    /// with the reference assembly in metadata. Passing the reference assembly itself compiled the
    /// same C# but gave ScriptGenerate a DLL it cannot convert (S3 D-S3-4).
    /// </summary>
    [TestClass]
    public class WatchSyncReferencesTests
    {
        private const string Fields = "%(Identity)|%(ReferenceAssembly)|%(MSBuildSourceProjectFile)|%(OriginalItemSpec)|%(ReferenceSourceTarget)|%(CopyUpToDateMarker)|%(TargetFrameworkIdentifier)|%(TargetFrameworkVersion)|%(TargetPlatformIdentifier)|%(TargetPlatformMoniker)";

        private const string LibProject =
            "<Project>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.props\" />\n" +
            "  <PropertyGroup>\n" +
            "    <AssemblyName>Lib</AssemblyName>\n" +
            "    <ProduceReferenceAssembly>true</ProduceReferenceAssembly>\n" +
            "  </PropertyGroup>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.targets\" />\n" +
            "</Project>\n";

        private const string AppProject =
            "<Project>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.props\" />\n" +
            "  <PropertyGroup>\n" +
            "    <AssemblyName>App</AssemblyName>\n" +
            "    <GenerateJs>True</GenerateJs>\n" +
            "    <NScriptExe>$(MSBuildThisFileDirectory)..\\fake-nscript.cmd</NScriptExe>\n" +
            "  </PropertyGroup>\n" +
            "  <ItemGroup>\n" +
            "    <ProjectReference Include=\"..\\Lib\\Lib.proj\" />\n" +
            "  </ItemGroup>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.targets\" />\n" +
            "  <Target Name=\"Dump\">\n" +
            "    <WriteLinesToFile File=\"$(DumpFile)\" Lines=\"@(_ResolvedProjectReferencePaths->'" + Fields + "')\" Overwrite=\"true\" />\n" +
            "  </Target>\n" +
            "</Project>\n";

        // Distinct global properties per step give each step a fresh evaluation in one MSBuild run.
        private const string Driver =
            "<Project>\n" +
            "  <Target Name=\"Run\">\n" +
            "    <MSBuild Projects=\"Lib\\Lib.proj;App\\App.proj\" Targets=\"Restore\" Properties=\"SdkDir=$(SdkDir);Step=0\" />\n" +
            // A watch build of the referenced project, as at registration (the target is absent before D-S3-4),
            // then a plain build of it: its IncrementalClean must keep the record.
            "    <MSBuild Projects=\"Lib\\Lib.proj\" Targets=\"_NScriptWatchTargetPath;IncrementalClean\" Properties=\"SdkDir=$(SdkDir);Step=1;NScriptWatch=true\" SkipNonexistentTargets=\"true\" />\n" +
            "    <MSBuild Projects=\"Lib\\Lib.proj\" Targets=\"IncrementalClean\" Properties=\"SdkDir=$(SdkDir);Step=5\" />\n" +
            "    <MSBuild Projects=\"App\\App.proj\" Targets=\"ResolveProjectReferences;Dump\" Properties=\"SdkDir=$(SdkDir);Step=2;NScriptWatchSync=false;BuildProjectReferences=false;DumpFile=$(MSBuildThisFileDirectory)off.txt\" />\n" +
            "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync;ResolveProjectReferences;Dump\" Properties=\"SdkDir=$(SdkDir);Step=3;DumpFile=$(MSBuildThisFileDirectory)synced.txt\" />\n" +
            "    <Delete Files=\"Lib\\obj\\Debug\\netstandard2.1\\nscript.targetpath\" />\n" +
            "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync;ResolveProjectReferences;Dump\" Properties=\"SdkDir=$(SdkDir);Step=4;BuildProjectReferences=false;DumpFile=$(MSBuildThisFileDirectory)unrecorded.txt\" />\n" +
            "  </Target>\n" +
            "</Project>\n";

        private static string GetRepoRoot()
        {
            // The test DLL lives at Test/Compiler/bin/<tfm>/; climb four levels to the worktree root.
            string dir = Path.GetDirectoryName(typeof(WatchSyncReferencesTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s); the only check of the synced reference swap's items.
        public void SyncedBuild_VouchedReference_IsGetTargetPathItem_UnrecordedKeepsReferences()
        {
            if (!OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("The stand-in nscript is a .cmd script.");
            }

            string sdkDir = Path.Combine(GetRepoRoot(), "Sources", "Compiler", "NScript.Sdk", "Sdk") + Path.DirectorySeparatorChar;
            Assert.IsTrue(File.Exists(sdkDir + "Sdk.targets"), $"Expected SDK targets in {sdkDir}");

            string dir = Path.Combine(Path.GetTempPath(), "nscript-syncrefs-" + Guid.NewGuid().ToString("N"));
            try
            {
                string libDir = Path.Combine(dir, "Lib") + Path.DirectorySeparatorChar;
                string libObj = Path.Combine(libDir, "obj", "Debug", "netstandard2.1") + Path.DirectorySeparatorChar;
                string appObj = Path.Combine(dir, "App", "obj", "Debug", "netstandard2.1");
                Directory.CreateDirectory(libObj);
                Directory.CreateDirectory(appObj);
                File.WriteAllText(Path.Combine(appObj, "nscript.watch"), "pipe=fake\n");
                File.WriteAllText(Path.Combine(libDir, "Lib.proj"), LibProject);
                File.WriteAllText(Path.Combine(dir, "App", "App.proj"), AppProject);
                File.WriteAllText(Path.Combine(dir, "Driver.proj"), Driver);

                // The daemon's yes: Lib's directory, the reference csc reads (the reference
                // assembly) and Lib's intermediate directory, written by the daemon's own
                // formatter (BuildServiceTests.Sync_Yes_ReplyListsForeignJsAndVouchedReference
                // checks the daemon uses it), so the two sides cannot drift apart.
                File.WriteAllText(
                    Path.Combine(dir, "fake-nscript.cmd"),
                    "@echo off\r\n" +
                    "echo nscript service: sync yes App.dll (1 ms)\r\n" +
                    "echo " + ServiceHost.SyncRefLine(libDir, libObj + @"ref\Lib.dll", libObj).Replace("|", "^|") + "\r\n" +
                    "exit /b 0\r\n");

                var psi = new ProcessStartInfo("dotnet")
                {
                    WorkingDirectory = dir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                foreach (string arg in new[] { "msbuild", "Driver.proj", "-t:Run", "-nologo", "-nr:false", "-v:n", "-p:SdkDir=" + sdkDir })
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
                Assert.AreEqual(0, process.ExitCode, output);

                string off = File.ReadAllText(Path.Combine(dir, "off.txt")).Trim();
                StringAssert.StartsWith(off, libDir + "bin", "control: GetTargetPath returns the implementation DLL");
                StringAssert.Contains(off, "|" + libObj + "ref" + Path.DirectorySeparatorChar + "Lib.dll|", "control: with the reference assembly in metadata");

                // Synced: Lib is not evaluated, and its item equals GetTargetPath's.
                StringAssert.Contains(output, "App reads 1 vouched references, evaluated no project references", output);
                Assert.AreEqual(off, File.ReadAllText(Path.Combine(dir, "synced.txt")).Trim(), "synced vs GetTargetPath item");

                // No recorded target path (Lib not built in watch mode since): today's references.
                StringAssert.Contains(output, "project references kept", output);
                Assert.AreEqual(off, File.ReadAllText(Path.Combine(dir, "unrecorded.txt")).Trim(), "unrecorded vs GetTargetPath item");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
