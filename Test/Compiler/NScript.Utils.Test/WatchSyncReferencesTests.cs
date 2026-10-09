namespace NScript.Utils.Test
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Service;
    using NScript.Lib.Service;

    /// <summary>
    /// Runs the real Sdk.targets watch sync against a stand-in nscript (a .cmd script) in one
    /// dotnet msbuild run per test: a driver project calls the targets step by step.
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
            "    <EmbeddedResource Include=\"*.skin\" />\n" +
            "  </ItemGroup>\n" +
            "  <Import Project=\"$(SdkDir)Sdk.targets\" />\n" +
            "  <Target Name=\"Dump\">\n" +
            "    <WriteLinesToFile File=\"$(DumpFile)\" Lines=\"@(_ResolvedProjectReferencePaths->'" + Fields + "')\" Overwrite=\"true\" />\n" +
            "  </Target>\n" +
            "  <Target Name=\"DumpEnvironment\">\n" +
            "    <WriteLinesToFile File=\"$(DumpFile)\" Lines=\"$(CscEnvironment)\" Overwrite=\"true\" />\n" +
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

        /// <summary>
        /// S3 D-S3-4: a synced build skips evaluating the vouched reference, so it must hand
        /// ResolveAssemblyReferences the item GetTargetPath would have returned: the implementation
        /// DLL with the reference assembly in metadata. Passing the reference assembly itself
        /// compiled the same C# but gave ScriptGenerate a DLL it cannot convert.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s); the only check of the synced reference swap's items.
        public void SyncedBuild_VouchedReference_IsGetTargetPathItem_UnrecordedKeepsReferences()
        {
            using var tree = new SyncTree(Driver);
            string output = tree.Run();

            string off = File.ReadAllText(Path.Combine(tree.Dir, "off.txt")).Trim();
            StringAssert.StartsWith(off, tree.LibDir + "bin", "control: GetTargetPath returns the implementation DLL");
            StringAssert.Contains(off, "|" + tree.LibObj + "ref" + Path.DirectorySeparatorChar + "Lib.dll|", "control: with the reference assembly in metadata");

            // Synced: Lib is not evaluated, and its item equals GetTargetPath's.
            StringAssert.Contains(output, "App reads 1 vouched references, evaluated no project references", output);
            Assert.AreEqual(off, File.ReadAllText(Path.Combine(tree.Dir, "synced.txt")).Trim(), "synced vs GetTargetPath item");

            // No recorded target path (Lib not built in watch mode since): today's references.
            StringAssert.Contains(output, "project references kept", output);
            Assert.AreEqual(off, File.ReadAllText(Path.Combine(tree.Dir, "unrecorded.txt")).Trim(), "unrecorded vs GetTargetPath item");
        }

        /// <summary>
        /// F-002, F-017, F-018: the daemon replays the watch build's csc command line and answers
        /// no to a sync whose compile properties hash differs from the one the watch compile
        /// sent. So the hash a watch compile sends (csc's environment) must equal the hash a build
        /// with the same properties syncs with, and differ for -p:TreatWarningsAsErrors=true and
        /// -p:DefineConstants=X. The watch compile also gets its evaluation time in UTC ticks.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s).
        public void WatchCompileAndSync_SendTheSameBuildPropsHash_OtherPropertiesDiffer()
        {
            // The watch steps run PrepareForBuild first, as a real build does before CoreCompile. It
            // adds the implicit DefineConstants (NETSTANDARD2_1, ...) after the sync has asked.
            const string driver =
                "<Project>\n" +
                "  <Target Name=\"Run\">\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"Restore\" Properties=\"SdkDir=$(SdkDir);Step=0\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"PrepareForBuild;_NScriptWatchPropsEnvironment;DumpEnvironment\" Properties=\"SdkDir=$(SdkDir);Step=1;NScriptWatch=true;DumpFile=$(MSBuildThisFileDirectory)watch.txt\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync\" Properties=\"SdkDir=$(SdkDir);Step=2;NScriptExe=$(MSBuildThisFileDirectory)props-nscript.cmd\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync\" Properties=\"SdkDir=$(SdkDir);Step=3;NScriptExe=$(MSBuildThisFileDirectory)props-nscript.cmd;TreatWarningsAsErrors=true\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync\" Properties=\"SdkDir=$(SdkDir);Step=4;NScriptExe=$(MSBuildThisFileDirectory)props-nscript.cmd;DefineConstants=X\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"PrepareForBuild;_NScriptWatchPropsEnvironment;DumpEnvironment\" Properties=\"SdkDir=$(SdkDir);Step=5;NScriptWatch=true;DefineConstants=X;DumpFile=$(MSBuildThisFileDirectory)watchX.txt\" />\n" +
                "  </Target>\n" +
                "</Project>\n";
            using var tree = new SyncTree(driver);
            File.WriteAllText(
                Path.Combine(tree.Dir, "props-nscript.cmd"),
                "@echo off\r\necho nscript service: sync no App.dll: props=%~5\r\nexit /b 1\r\n");
            string output = tree.Run();

            string[] synced = Regex.Matches(output, @"NScript watch: full build \(nscript service: sync no App\.dll: props=(-?\d+)\)")
                .Select(m => m.Groups[1].Value)
                .ToArray();
            Assert.AreEqual(3, synced.Length, output);
            string[] watch = File.ReadAllLines(Path.Combine(tree.Dir, "watch.txt"));
            Assert.AreEqual(1, watch.Count(l => l == ServiceArgs.WatchPropsHashEnvVar + "=" + synced[0]), "the same properties: one hash\n" + string.Join("\n", watch));
            Assert.AreNotEqual(synced[0], synced[1], "TreatWarningsAsErrors=true");
            Assert.AreNotEqual(synced[0], synced[2], "DefineConstants=X");
            Assert.AreNotEqual(synced[1], synced[2]);
            Assert.AreEqual(1, File.ReadAllLines(Path.Combine(tree.Dir, "watchX.txt")).Count(l => l == ServiceArgs.WatchPropsHashEnvVar + "=" + synced[2]), "an X watch compile sends the X hash");

            string ticks = watch.Single(l => l.StartsWith(ServiceArgs.WatchEvaluatedUtcTicksEnvVar + "=", StringComparison.Ordinal)).Split('=')[1];
            var evaluated = new DateTime(long.Parse(ticks, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture), DateTimeKind.Utc);
            Assert.IsTrue((DateTime.UtcNow - evaluated).Duration() < TimeSpan.FromMinutes(5), "evaluation time in UTC ticks: " + evaluated.ToString("o"));
        }

        /// <summary>
        /// F-004: only exit code 0 is a yes; a busy daemon (exit 2) builds in full. And a synced
        /// build keeps the watch's JS only while the daemon wrote it last: a JS file the reply
        /// lists as foreign (another configuration rewrote it) is regenerated, while a synced
        /// build without that line keeps the JS although its jsmode stamp does not match.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s).
        public void SyncedBuild_OnlyExitZeroSyncs_ForeignJsIsRegenerated()
        {
            const string driver =
                "<Project>\n" +
                "  <Target Name=\"Run\">\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"Restore\" Properties=\"SdkDir=$(SdkDir);Step=0\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync\" Properties=\"SdkDir=$(SdkDir);Step=2;NScriptExe=$(MSBuildThisFileDirectory)busy-nscript.cmd\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync;_NScriptCheckJsMode\" Properties=\"SdkDir=$(SdkDir);Step=3\" />\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"_NScriptWatchSync;_NScriptCheckJsMode\" Properties=\"SdkDir=$(SdkDir);Step=4;NScriptExe=$(MSBuildThisFileDirectory)foreign-nscript.cmd\" />\n" +
                "  </Target>\n" +
                "</Project>\n";
            using var tree = new SyncTree(driver);
            string appJs = Path.Combine(tree.Dir, "App", "App.js");
            File.WriteAllText(appJs, "another configuration's JS");
            File.WriteAllText(
                Path.Combine(tree.Dir, "busy-nscript.cmd"),
                "@echo off\r\necho nscript service: sync busy App.dll: a batch still running after 1 s\r\nexit /b 2\r\n");
            File.WriteAllText(
                Path.Combine(tree.Dir, "foreign-nscript.cmd"),
                "@echo off\r\necho nscript service: sync yes App.dll (1 ms)\r\necho " + ServiceHost.SyncJsForeignPrefix + appJs + "\r\nexit /b 0\r\n");
            string output = tree.Run();

            Assert.AreEqual(1, Count(output, "NScript watch: full build (nscript service: sync busy"), "busy is not a yes\n" + output);
            Assert.AreEqual(2, Count(output, "NScript watch: App current, skipped"), "control: steps 3 and 4 sync\n" + output);
            Assert.AreEqual(1, Count(output, "NScript: " + appJs + " was not written by this configuration"), "only the foreign JS is regenerated\n" + output);
        }

        /// <summary>
        /// D1: a skin or CSS save is patched into the obj DLL by the daemon, which leaves the PDB
        /// older than the resource. A synced build then skips csc, so it keeps the daemon's DLL and
        /// JS, and the next sync does not find a DLL rewritten outside the watch. Any other input
        /// newer than an output, a resource newer than the DLL, or no sync still runs csc.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run (~5-10 s) with four builds against a stand-in csc.
        public void SyncedBuild_ResourcePatchedDll_SkipsCsc_OtherInputsStillCompile()
        {
            // Outputs in the future, so every SDK, repo and generated input is older than them.
            DateTime pdb = DateTime.Now.AddHours(1);
            DateTime skin = pdb.AddMinutes(10);
            DateTime patched = pdb.AddMinutes(20);
            DateTime late = pdb.AddMinutes(25);
            DateTime js = pdb.AddMinutes(30);
            static string At(DateTime time) => time.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            static string Build(int step, string csc, string extra) =>
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"Build\" Properties=\"SdkDir=$(SdkDir);Step=" + step +
                ";CscToolExe=$(MSBuildThisFileDirectory)" + csc + ".cmd;BuildProjectReferences=false" + extra +
                ";LanguageTargets=$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />\n"; // A .proj gets no C# targets by default.
            string driver =
                "<Project>\n" +
                "  <Target Name=\"Run\">\n" +
                "    <MSBuild Projects=\"App\\App.proj\" Targets=\"Restore\" Properties=\"SdkDir=$(SdkDir);Step=0\" />\n" +
                // The daemon patched the skin into the DLL: skin newer than the PDB, older than the DLL.
                Build(1, "csc-patched", string.Empty) +
                // The skin saved again after the patch: newer than the DLL.
                "    <Touch Files=\"App\\Title.skin\" Time=\"" + At(late) + "\" />\n" +
                Build(2, "csc-late", string.Empty) +
                // A .cs input newer than the PDB.
                "    <Touch Files=\"App\\Title.skin\" Time=\"" + At(skin) + "\" />\n" +
                "    <Touch Files=\"App\\Class.cs\" Time=\"" + At(pdb.AddMinutes(5)) + "\" />\n" +
                Build(3, "csc-cs", string.Empty) +
                // No sync: today's build.
                "    <Touch Files=\"App\\Class.cs\" />\n" +
                Build(4, "csc-off", ";NScriptWatchSync=false") +
                "  </Target>\n" +
                "</Project>\n";
            using var tree = new SyncTree(driver);
            string app = Path.Combine(tree.Dir, "App") + Path.DirectorySeparatorChar;
            // Lib as its last build left it (older than App's outputs).
            Directory.CreateDirectory(tree.LibObj + "ref");
            Directory.CreateDirectory(Path.Combine(tree.LibDir, "bin", "Debug", "netstandard2.1"));
            File.WriteAllText(tree.LibObj + @"ref\Lib.dll", "Lib's reference assembly");
            File.WriteAllText(Path.Combine(tree.LibDir, "bin", "Debug", "netstandard2.1", "Lib.dll"), "Lib");
            File.WriteAllText(app + "Class.cs", "class C { }\n");
            File.WriteAllText(app + "Title.skin", "<div>title</div>\n");
            File.SetLastWriteTime(app + "Title.skin", skin);
            File.WriteAllText(tree.AppObj + "App.pdb", "the watch compile's PDB");
            File.SetLastWriteTime(tree.AppObj + "App.pdb", pdb);
            File.WriteAllText(tree.AppObj + "App.dll", "the daemon's patched DLL");
            File.SetLastWriteTime(tree.AppObj + "App.dll", patched);
            File.WriteAllText(app + "App.js", "the daemon's dev JS");
            File.SetLastWriteTime(app + "App.js", js);
            foreach (string csc in new[] { "csc-patched", "csc-late", "csc-cs", "csc-off" })
            {
                File.WriteAllText(Path.Combine(tree.Dir, csc + ".cmd"), "@echo off\r\necho %~n0>>\"%~dp0csc-ran.txt\"\r\nexit /b 0\r\n");
            }

            string output = tree.Run();

            string ranFile = Path.Combine(tree.Dir, "csc-ran.txt");
            string ran = File.Exists(ranFile) ? string.Join(",", File.ReadAllLines(ranFile).Select(l => l.Trim())) : string.Empty;
            Assert.AreEqual("csc-late,csc-cs,csc-off", ran, "only the patched build skips csc\n" + output);
            Assert.AreEqual(3, Count(output, "NScript watch: App current, skipped"), "control: steps 1-3 sync\n" + output);
            Assert.AreEqual(1, Count(output, "NScript watch: App resources are in the watch's DLL; csc skipped"), output);
            Assert.AreEqual("the daemon's patched DLL", File.ReadAllText(Path.Combine(app, "bin", "Debug", "netstandard2.1", "App.dll")), "bin gets the patched DLL");
        }

        /// <summary>
        /// A1: the synced csc skip copies CoreCompile's Inputs (split at the embedded resources)
        /// and Outputs. An SDK that adds an input there would let a synced build skip csc when
        /// only that input changed, so the copies must equal the CoreCompile of the SDK the
        /// harness's MSBuild uses ($(RoslynTargetsPath), where CSharpCoreTargetsPath points by default).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // One dotnet msbuild run of an empty target (~1-2 s).
        public void SyncedSkipCsc_CopiesCoreCompileInputsAndOutputs_OfTheSdk()
        {
            const string driver =
                "<Project>\n" +
                "  <Target Name=\"Run\">\n" +
                "    <WriteLinesToFile File=\"$(MSBuildThisFileDirectory)roslyn.txt\" Lines=\"$(RoslynTargetsPath)\" Overwrite=\"true\" />\n" +
                "  </Target>\n" +
                "</Project>\n";
            using var tree = new SyncTree(driver);
            tree.Run();
            string core = Path.Combine(File.ReadAllText(Path.Combine(tree.Dir, "roslyn.txt")).Trim(), "Microsoft.CSharp.Core.targets");
            Assert.IsTrue(File.Exists(core), core);

            XElement coreCompile = Target(core, "CoreCompile");
            string sdkTargets = tree.SdkDir + "Sdk.targets";
            XElement compileCheck = Target(sdkTargets, "_NScriptWatchSyncedCompileCheck");
            const string resources = "@(_CoreCompileResourceInputs)";
            Assert.AreEqual(Items(coreCompile, "Inputs"), Items(compileCheck, "Inputs", resources), core + " vs " + sdkTargets);
            Assert.AreEqual(resources, Items(Target(sdkTargets, "_NScriptWatchSyncedResourceAfterPatchCheck"), "Inputs"));
            Assert.AreEqual(resources, Items(Target(sdkTargets, "_NScriptWatchSyncedResourcePatchCheck"), "Inputs"));
            Assert.AreEqual(Items(coreCompile, "Outputs"), Items(compileCheck, "Outputs"), core + " vs " + sdkTargets);
            Assert.AreEqual(Items(coreCompile, "Outputs"), Items(Target(sdkTargets, "_NScriptWatchSyncedResourcePatchCheck"), "Outputs"));
        }

        private static XElement Target(string file, string name)
            => XDocument.Load(file).Descendants().Single(e => e.Name.LocalName == "Target" && (string)e.Attribute("Name") == name);

        /// <summary>An Inputs/Outputs list as sorted, trimmed items, one per line.</summary>
        private static string Items(XElement target, string attribute, params string[] extra)
            => string.Join("\n", ((string)target.Attribute(attribute)).Split(';').Select(i => i.Trim()).Where(i => i.Length > 0).Concat(extra).OrderBy(i => i, StringComparer.Ordinal));

        private static int Count(string text, string part)
            => (text.Length - text.Replace(part, string.Empty).Length) / part.Length;

        /// <summary>
        /// A Lib project and an App that references it, both on the repo's Sdk.targets, a watch
        /// marker in App's obj, and a stand-in nscript that answers yes and vouches for Lib.
        /// </summary>
        private sealed class SyncTree : IDisposable
        {
            public SyncTree(string driver)
            {
                if (!OperatingSystem.IsWindows())
                {
                    Assert.Inconclusive("The stand-in nscript is a .cmd script.");
                }

                this.SdkDir = Path.Combine(GetRepoRoot(), "Sources", "Compiler", "NScript.Sdk", "Sdk") + Path.DirectorySeparatorChar;
                Assert.IsTrue(File.Exists(this.SdkDir + "Sdk.targets"), $"Expected SDK targets in {this.SdkDir}");

                this.Dir = Path.Combine(Path.GetTempPath(), "nscript-syncrefs-" + Guid.NewGuid().ToString("N"));
                this.LibDir = Path.Combine(this.Dir, "Lib") + Path.DirectorySeparatorChar;
                this.LibObj = Path.Combine(this.LibDir, "obj", "Debug", "netstandard2.1") + Path.DirectorySeparatorChar;
                this.AppObj = Path.Combine(this.Dir, "App", "obj", "Debug", "netstandard2.1") + Path.DirectorySeparatorChar;
                Directory.CreateDirectory(this.LibObj);
                Directory.CreateDirectory(this.AppObj);
                File.WriteAllText(this.AppObj + "nscript.watch", "pipe=fake\n");
                File.WriteAllText(Path.Combine(this.LibDir, "Lib.proj"), LibProject);
                File.WriteAllText(Path.Combine(this.Dir, "App", "App.proj"), AppProject);
                File.WriteAllText(Path.Combine(this.Dir, "Driver.proj"), driver);

                // The daemon's yes: Lib's directory, the reference csc reads (the reference
                // assembly) and Lib's intermediate directory, written by the daemon's own
                // formatter (BuildServiceTests.Sync_Yes_ReplyListsForeignJsAndVouchedReference
                // checks the daemon uses it), so the two sides cannot drift apart.
                File.WriteAllText(
                    Path.Combine(this.Dir, "fake-nscript.cmd"),
                    "@echo off\r\n" +
                    "echo nscript service: sync yes App.dll (1 ms)\r\n" +
                    "echo " + ServiceHost.SyncRefLine(this.LibDir, this.LibObj + @"ref\Lib.dll", this.LibObj).Replace("|", "^|") + "\r\n" +
                    "exit /b 0\r\n");
            }

            public string SdkDir { get; }

            public string Dir { get; }

            public string LibDir { get; }

            public string LibObj { get; }

            public string AppObj { get; }

            /// <summary>Runs the driver's Run target; asserts MSBuild succeeded and returns its output.</summary>
            public string Run()
            {
                var psi = new ProcessStartInfo("dotnet")
                {
                    WorkingDirectory = this.Dir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                foreach (string arg in new[] { "msbuild", "Driver.proj", "-t:Run", "-nologo", "-nr:false", "-v:n", "-p:SdkDir=" + this.SdkDir })
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
                return output;
            }

            public void Dispose() => Directory.Delete(this.Dir, recursive: true);

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
        }
    }
}
