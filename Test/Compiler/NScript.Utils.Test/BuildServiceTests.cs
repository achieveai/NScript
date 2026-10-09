namespace NScript.Utils.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Text.Json;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Service;
    using NScript.Lib;
    using NScript.Lib.Service;
    using NScript.Utils;

    /// <summary>
    /// Contracts of the build service (nscript.exe service) that a regression would break
    /// silently: the MSBuild flag handling, the -devMode guard, and a daemon that must
    /// survive bad requests and exit when idle.
    /// </summary>
    [TestClass]
    public class BuildServiceTests
    {
        private string logPath;
        private string savedLogEnv;

        [TestInitialize]
        public void Setup()
        {
            Logger.Instance = new Logger();
            this.logPath = Path.Combine(Path.GetTempPath(), "nscript-service-test-" + Guid.NewGuid().ToString("N") + ".jsonl");
            this.savedLogEnv = Environment.GetEnvironmentVariable(ServiceIdentity.LogPathEnvVar);
            Environment.SetEnvironmentVariable(ServiceIdentity.LogPathEnvVar, this.logPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable(ServiceIdentity.LogPathEnvVar, this.savedLogEnv);
            CompilerLog.Shutdown();
            File.Delete(this.logPath);
        }

        [TestMethod]
        public void ServiceFlag_StrippedInAnyCase_DevModeAppendedOnce()
        {
            Assert.IsTrue(ServiceArgs.TryStripServiceFlag(new[] { "-outJs", "a.js", "-SERVICE", "/service" }, out var stripped));
            CollectionAssert.AreEqual(new[] { "-outJs", "a.js" }, stripped);

            Assert.IsFalse(ServiceArgs.TryStripServiceFlag(new[] { "-outJs", "service" }, out stripped));
            CollectionAssert.AreEqual(new[] { "-outJs", "service" }, stripped);

            CollectionAssert.AreEqual(new[] { "a", "-devMode" }, ServiceArgs.EnsureDevMode(new[] { "a" }));
            CollectionAssert.AreEqual(new[] { "-DEVMODE", "a" }, ServiceArgs.EnsureDevMode(new[] { "-DEVMODE", "a" }));
        }

        [TestMethod]
        public void ServiceFlag_ReleaseFlagsKeepTheBuildLocal()
        {
            Assert.IsFalse(ServiceArgs.HasReleaseFlags(new[] { "-outJs", "a.js", "-devMode" }));
            Assert.IsTrue(ServiceArgs.HasReleaseFlags(new[] { "-outJs", "a.js", "-Minify" }));
            Assert.IsTrue(ServiceArgs.HasReleaseFlags(new[] { "-uglify" }));
            Assert.IsTrue(ServiceArgs.HasReleaseFlags(new[] { "-optimize" }));
        }

        [DataTestMethod]
        [DataRow("-minify")]
        [DataRow("-uglify")]
        [DataRow("-optimize")]
        public void DevMode_WithReleaseFlag_IsRejected(string releaseFlag)
        {
            Assert.IsNull(ParseOptions.ParseArgs(ValidArgs("-devMode", releaseFlag)));
            Assert.IsTrue(Logger.Instance.HasErrors);
        }

        [TestMethod]
        public void DevMode_Alone_Parses()
        {
            var options = ParseOptions.ParseArgs(ValidArgs("-devMode"));
            Assert.IsNotNull(options);
            Assert.IsTrue(options.DevMode);
            Assert.IsFalse(options.Minify || options.Uglify || options.Optimize);
        }

        [TestMethod]
        public void Daemon_BadRequest_ReturnsExitOne_AndKeepsServing()
        {
            var host = NewHost(TimeSpan.FromMinutes(5), out var pipeName);
            int serveExit = -1;
            var serve = new Thread(() => serveExit = host.Serve());
            serve.Start();

            var requestCwd = Path.GetTempPath();
            var cwdBefore = Directory.GetCurrentDirectory();
            var bad = Send(pipeName, ServiceProtocol.KindEmitJs, requestCwd, "-bogus");
            Assert.AreEqual(1, bad.ExitCode, "Bad args are a real result, exit 1 like batch.");
            Assert.IsFalse(bad.InternalError, bad.Message);
            StringAssert.Contains(bad.Stdout, ParseOptions.Usage, "Batch prints the usage for bad args.");
            Assert.AreEqual(cwdBefore, Directory.GetCurrentDirectory(), "The request cwd must be restored.");

            // A request that fails argument validation leaves the process-wide Logger with
            // errors; batch exits there, so the daemon must reset it before the next request.
            var rejected = Send(pipeName, ServiceProtocol.KindEmitJs, requestCwd, ValidArgs("-devMode", "-minify"));
            Assert.AreEqual(1, rejected.ExitCode);
            var next = Send(pipeName, ServiceProtocol.KindEmitJs, requestCwd, ValidArgs("-devMode"));
            Assert.IsFalse(
                next.Stdout.Contains(ParseOptions.Usage),
                "Valid args were rejected: errors from the previous request leaked into this one: " + next.Stdout);

            var status = Send(pipeName, ServiceProtocol.KindStatus, requestCwd);
            Assert.AreEqual("3", status.Status["Requests"]);
            Assert.AreEqual("none", status.Status["CurrentRequest"]);

            Send(pipeName, ServiceProtocol.KindStop, requestCwd);
            Assert.IsTrue(serve.Join(TimeSpan.FromSeconds(5)), "Stop must end Serve.");
            Assert.AreEqual(0, serveExit);
        }

        [TestMethod]
        public void Daemon_WithNoRequests_ExitsWhenIdle()
        {
            var host = NewHost(TimeSpan.FromMilliseconds(200), out _);
            var sw = Stopwatch.StartNew();

            Assert.AreEqual(0, host.Serve());

            Assert.IsTrue(sw.Elapsed < TimeSpan.FromSeconds(3), "Idle exit took " + sw.Elapsed);
            StringAssert.Contains(File.ReadAllText(this.logPath), "\"Reason\":\"idle\"");
        }

        [TestMethod]
        public void ToolsetHash_CoversOnlyTheRuntimeClosure()
        {
            var dir = NewTempDir();
            try
            {
                string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                    : "linux";
                var closure = new[] { "NScript.deps.json", "NScript.dll", "NScript.exe", "PluginConfig.xml", Path.Combine("runtimes", os, "native.bin") };
                foreach (var file in closure)
                {
                    WriteFile(dir, file, "closure " + file);
                }

                var outside = new[] { "NScript.pdb", "notes.txt", Path.Combine("publish", "NScript.dll"), Path.Combine("runtimes", "other-rid", "native.bin") };
                foreach (var file in outside)
                {
                    WriteFile(dir, file, "outside " + file);
                }

                CollectionAssert.AreEqual(closure.OrderBy(f => f, StringComparer.Ordinal).ToList(), ServiceIdentity.RuntimeClosure(dir));

                var hash = ServiceIdentity.ComputeToolsetHash(dir);
                Assert.AreEqual(hash, ServiceIdentity.ComputeToolsetHash(dir), "The hash must be stable.");

                foreach (var file in outside)
                {
                    WriteFile(dir, file, "rewritten outside the closure, longer than before");
                }

                Assert.AreEqual(hash, ServiceIdentity.ComputeToolsetHash(dir), "Files the daemon never loads must not change the hash.");

                var dll = new FileInfo(Path.Combine(dir, "NScript.dll"));
                var mtime = dll.LastWriteTimeUtc;
                dll.LastWriteTimeUtc = mtime.AddSeconds(1);
                Assert.AreNotEqual(hash, ServiceIdentity.ComputeToolsetHash(dir), "A rebuilt (newer) assembly must change the hash.");
                dll.LastWriteTimeUtc = mtime;
                Assert.AreEqual(hash, ServiceIdentity.ComputeToolsetHash(dir));

                var config = new FileInfo(Path.Combine(dir, "PluginConfig.xml"));
                var configTime = config.LastWriteTimeUtc;
                File.AppendAllText(config.FullName, "x");
                config.LastWriteTimeUtc = configTime;
                Assert.AreNotEqual(hash, ServiceIdentity.ComputeToolsetHash(dir), "A size change with the same mtime must change the hash.");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        [TestMethod]
        public void Daemon_Compile_ResolvesRelativeResourcesAgainstRequestCwd()
        {
            var project = NewTempDir();
            var host = NewHost(TimeSpan.FromMinutes(5), out var pipeName);
            var serve = new Thread(() => host.Serve());
            serve.Start();
            try
            {
                WriteFile(project, "C.cs", "public class C { public int M() { return 1; } }");
                WriteFile(project, Path.Combine("Views", "a.html"), "<div></div>");
                Assert.AreNotEqual(
                    Path.TrimEndingDirectorySeparator(project),
                    Path.TrimEndingDirectorySeparator(Directory.GetCurrentDirectory()),
                    "The request cwd must differ from the daemon's, or the test proves nothing.");

                // Every path is relative to the request cwd, as MSBuild passes them.
                var response = Send(
                    pipeName,
                    ServiceProtocol.KindCompile,
                    project,
                    "/noconfig",
                    "/nostdlib+",
                    "/target:library",
                    "/out:out.dll",
                    "/reference:" + typeof(object).Assembly.Location,
                    "-resource:" + Path.Combine("Views", "a.html") + ",Test.a.html",
                    "C.cs");
                Assert.AreEqual(0, response.ExitCode, response.Stdout + response.Stderr + response.Message);

                var resInfo = ReadResource(Path.Combine(project, "out.dll"), "$$ResInfo$$");
                Assert.AreEqual(
                    Path.Combine(project, "Views", "a.html"),
                    JsonDocument.Parse(resInfo).RootElement.GetProperty("Test.a.html").GetString(),
                    "Stage 2 reads skins from $$ResInfo$$, so the path must be absolute in the request cwd.");
            }
            finally
            {
                Send(pipeName, ServiceProtocol.KindStop, project);
                Assert.IsTrue(serve.Join(TimeSpan.FromSeconds(5)), "Stop must end Serve.");
                Directory.Delete(project, recursive: true);
            }
        }

        [TestMethod]
        public void StaleShadowCleanup_DeletesOnlyOldUnlockedCopiesOfOtherBuilds()
        {
            // A toolset dir no real daemon uses, so the service root (keyed by it) is private.
            var identity = ServiceIdentity.FromKnown(NewTempDir(), new string('0', 64));
            Directory.CreateDirectory(identity.ServiceRoot);
            try
            {
                string Shadow(string name, bool old)
                {
                    var dir = Path.Combine(identity.ServiceRoot, name);
                    WriteFile(dir, "NScript.dll", name);
                    if (old)
                    {
                        Directory.SetLastWriteTimeUtc(dir, DateTime.UtcNow.AddHours(-1));
                    }

                    return dir;
                }

                var current = Shadow("current", old: true);
                var stale = Shadow("stale", old: true);
                var fresh = Shadow("fresh", old: false);
                var live = Shadow("live", old: true);
                using (new FileStream(Path.Combine(live, ServiceLauncher.DaemonLockFile), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    Directory.SetLastWriteTimeUtc(live, DateTime.UtcNow.AddHours(-1));
                    ServiceLauncher.DeleteStaleShadowCopies(identity, current + Path.DirectorySeparatorChar);
                }

                Assert.IsFalse(Directory.Exists(stale), "An old, unlocked copy of another build is garbage.");
                Assert.IsTrue(File.Exists(Path.Combine(current, "NScript.dll")), "The caller's own copy must survive.");
                Assert.IsTrue(File.Exists(Path.Combine(fresh, "NScript.dll")), "A young copy may be a launch in progress.");
                Assert.IsTrue(File.Exists(Path.Combine(live, "NScript.dll")), "A live daemon holds its lock file; its copy must survive.");
            }
            finally
            {
                Directory.Delete(identity.ServiceRoot, recursive: true);
                Directory.Delete(identity.ToolsetDir, recursive: true);
            }
        }

        /// <summary>
        /// Ruling (M5 risk list): cleanup also covers the shadow copies of other toolset keys
        /// (a Release toolset, another checkout), not only the caller's key; a live daemon's
        /// copy under another key survives.
        /// </summary>
        [TestMethod]
        public void StaleShadowCleanup_CoversOtherToolsetKeys()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), new string('0', 64));
            var otherKey = Path.Combine(Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(identity.ServiceRoot)), "test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(identity.ServiceRoot);
            try
            {
                string Shadow(string root, string name)
                {
                    var dir = Path.Combine(root, name);
                    WriteFile(dir, "NScript.dll", name);
                    Directory.SetLastWriteTimeUtc(dir, DateTime.UtcNow.AddHours(-1));
                    return dir;
                }

                var current = Shadow(identity.ServiceRoot, "current");
                var stale = Shadow(otherKey, "stale");
                var live = Shadow(otherKey, "live");
                using (new FileStream(Path.Combine(live, ServiceLauncher.DaemonLockFile), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    Directory.SetLastWriteTimeUtc(live, DateTime.UtcNow.AddHours(-1));
                    ServiceLauncher.DeleteStaleShadowCopies(identity, current);
                }

                Assert.IsFalse(Directory.Exists(stale), "An old, unlocked copy under another toolset key is garbage.");
                Assert.IsTrue(File.Exists(Path.Combine(live, "NScript.dll")), "A live daemon under another key keeps its copy.");
                Assert.IsTrue(File.Exists(Path.Combine(current, "NScript.dll")), "The caller's own copy must survive.");
            }
            finally
            {
                Directory.Delete(identity.ServiceRoot, recursive: true);
                Directory.Delete(otherKey, recursive: true);
                Directory.Delete(identity.ToolsetDir, recursive: true);
            }
        }

        [TestMethod]
        public void ServiceEnvVar_OnlyOneOrTrueOptsStageOneIn()
        {
            var saved = Environment.GetEnvironmentVariable(ServiceArgs.ServiceEnvVar);
            try
            {
                foreach (var on in new[] { "1", "true", "TRUE" })
                {
                    Environment.SetEnvironmentVariable(ServiceArgs.ServiceEnvVar, on);
                    Assert.IsTrue(ServiceArgs.IsServiceRequestedByEnvironment(), on);
                }

                foreach (var off in new[] { null, "0", "false", "yes" })
                {
                    Environment.SetEnvironmentVariable(ServiceArgs.ServiceEnvVar, off);
                    Assert.IsFalse(ServiceArgs.IsServiceRequestedByEnvironment(), off ?? "<unset>");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(ServiceArgs.ServiceEnvVar, saved);
            }
        }

        [TestMethod]
        public void Daemon_InternalError_IsReported_StateRestored_AndKeepsServing()
        {
            var host = NewHost(TimeSpan.FromMinutes(5), out var pipeName);
            var serve = new Thread(() => host.Serve());
            serve.Start();
            var consoleOut = Console.Out;
            var consoleErr = Console.Error;
            var cwdBefore = Directory.GetCurrentDirectory();
            try
            {
                // A cwd that does not exist fails inside the isolated run, before any stage code:
                // an internal error, which the client answers with a local compile.
                var missingCwd = Path.Combine(Path.GetTempPath(), "nscript-missing-" + Guid.NewGuid().ToString("N"));
                var response = Send(pipeName, ServiceProtocol.KindEmitJs, missingCwd, ValidArgs("-devMode"));
                Assert.IsTrue(response.InternalError, "Expected an internal error, got exit " + response.ExitCode);
                StringAssert.Contains(response.Message, nameof(DirectoryNotFoundException));

                Assert.AreSame(consoleOut, Console.Out, "Console.Out must be restored.");
                Assert.AreSame(consoleErr, Console.Error, "Console.Error must be restored.");
                Assert.AreEqual(cwdBefore, Directory.GetCurrentDirectory(), "The cwd must be restored.");

                var status = Send(pipeName, ServiceProtocol.KindStatus, cwdBefore);
                Assert.AreEqual("1", status.Status["Requests"]);
                Assert.AreEqual("none", status.Status["CurrentRequest"]);
            }
            finally
            {
                Send(pipeName, ServiceProtocol.KindStop, cwdBefore);
                Assert.IsTrue(serve.Join(TimeSpan.FromSeconds(5)), "Stop must end Serve.");
            }
        }

        private static string NewTempDir()
        {
            var dir = Path.Combine(Path.GetTempPath(), "nscript-service-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void WriteFile(string dir, string relativePath, string text)
        {
            var path = Path.Combine(dir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text);
        }

        private static string ReadResource(string assemblyPath, string resourceName)
        {
            var context = new AssemblyLoadContext("resinfo", isCollectible: true);
            try
            {
                using var file = new MemoryStream(File.ReadAllBytes(assemblyPath));
                using var stream = context.LoadFromStream(file).GetManifestResourceStream(resourceName);
                Assert.IsNotNull(stream, resourceName + " is missing from " + assemblyPath);
                return new StreamReader(stream).ReadToEnd();
            }
            finally
            {
                context.Unload();
            }
        }

        /// <summary>
        /// Regression (M5 D3): a compile that failed on an unreadable reference printed
        /// "error CS0009: ..." with no file prefix, and watch.log showed no reason at all.
        /// </summary>
        /// <summary>
        /// Defect D6: a directory deleted between the Directory.Exists check and its enumeration
        /// (a test's flood folder) threw out of RunBatch and stopped watch for good.
        /// </summary>
        [TestMethod]
        public void AddFilesOfNewDirectories_DirectoryGoneWhileRead_IsSkippedAndReported()
        {
            var gone = Path.Combine(Path.GetTempPath(), "nscript-gone-" + Guid.NewGuid().ToString("N"));
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { gone };

            // It still existed when the batch checked it.
            var skipped = ServiceHost.AddFilesOfNewDirectories(paths, _ => true);

            CollectionAssert.AreEqual(new[] { gone }, skipped);
            CollectionAssert.AreEqual(new[] { gone }, paths.ToList());
        }

        [TestMethod]
        public void DiagnosticLines_KeepsErrorsWithAndWithoutFilePrefix()
        {
            var output = string.Join(
                "\r\n",
                "Microsoft (R) Visual C# Compiler",
                @"error CS0009: Metadata file 'B:\lib\Sunlight.Framework.dll' could not be opened -- in use",
                @"B:\App\Program.cs(28,62): error CS0029: Cannot implicitly convert type 'string' to 'int'",
                @"NScript.Exe(0,0): error UNK0001: cannot replace B:\web\App.js: in use",
                "warning CS0168: unused",
                "");

            CollectionAssert.AreEqual(
                new[]
                {
                    @"error CS0009: Metadata file 'B:\lib\Sunlight.Framework.dll' could not be opened -- in use",
                    @"B:\App\Program.cs(28,62): error CS0029: Cannot implicitly convert type 'string' to 'int'",
                    @"NScript.Exe(0,0): error UNK0001: cannot replace B:\web\App.js: in use",
                },
                ServiceHost.DiagnosticLines(output).ToArray());
        }

        /// <summary>
        /// Regression (M5 P3): --status against a daemon that accepts but never answers (a
        /// suspended or wedged daemon) hung forever. It now gives up and names the way out.
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SendControl_DaemonNeverAnswers_TimesOutWithHint()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            using var server = new System.IO.Pipes.NamedPipeServerStream(
                identity.PipeName,
                System.IO.Pipes.PipeDirection.InOut,
                1,
                System.IO.Pipes.PipeTransmissionMode.Byte,
                System.IO.Pipes.PipeOptions.Asynchronous);
            var accepted = server.WaitForConnectionAsync();
            var output = new StringWriter();

            int exitCode = ServiceHost.SendControl(identity, ServiceProtocol.KindStatus, output, TimeSpan.FromMilliseconds(300));

            Assert.IsTrue(accepted.IsCompleted, "the fake daemon never saw the connection");
            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(output.ToString(), "--stop --force");
        }

        /// <summary>
        /// P11: a --status against a hung daemon takes its only waiting pipe instance, so the
        /// --stop that follows cannot connect. The daemon still holds its run lock: --stop must
        /// say it is not answering and point to --force, not report "no daemon running".
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // Waits out the 1 s pipe connect timeout of --stop.
        public void SendControl_StopAfterHungStatus_PointsToForceNotNoDaemon()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(identity.RunDir);
            try
            {
                using (new FileStream(Path.Combine(identity.RunDir, ServiceLauncher.DaemonLockFile), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (var server = new System.IO.Pipes.NamedPipeServerStream(
                    identity.PipeName,
                    System.IO.Pipes.PipeDirection.InOut,
                    1,
                    System.IO.Pipes.PipeTransmissionMode.Byte,
                    System.IO.Pipes.PipeOptions.Asynchronous))
                {
                    var accepted = server.WaitForConnectionAsync();
                    ServiceHost.SendControl(identity, ServiceProtocol.KindStatus, new StringWriter(), TimeSpan.FromMilliseconds(300));
                    Assert.IsTrue(accepted.IsCompleted, "the hung daemon never saw the --status connection");

                    var output = new StringWriter();
                    int exitCode = ServiceHost.SendControl(identity, ServiceProtocol.KindStop, output, Timeout.InfiniteTimeSpan);

                    Assert.AreEqual(1, exitCode);
                    StringAssert.Contains(output.ToString(), "--stop --force");
                    Assert.IsFalse(output.ToString().Contains("no daemon running", StringComparison.Ordinal), output.ToString());
                }
            }
            finally
            {
                Directory.Delete(identity.ServiceRoot, recursive: true);
                Directory.Delete(identity.ToolsetDir, recursive: true);
            }
        }

        /// <summary>
        /// S2: the --sync client prints the answer line and only the reference and JS lines
        /// after it (Sdk.targets reads every line it prints), asks with the obj DLL's full path
        /// and the default 30 s wait, and passes the answer through as its exit code.
        /// </summary>
        [TestMethod]
        [Timeout(5000)]
        public void SyncClient_PrintsAnswerAndReferenceLinesOnly()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            var key = Path.Combine(NewTempDir(), "obj", "A.dll");
            ServiceRequest seen = null;
            var output = RunSyncClient(identity, key, server =>
            {
                seen = ServiceProtocol.ReadMessage<ServiceRequest>(server);
                ServiceProtocol.WriteMessage(server, new ServiceResponse
                {
                    ExitCode = ServiceHost.SyncExitYes,
                    Message = "nscript service: sync yes A.dll (1 ms)\nnoise\r\nnscript-ref C:\\p\\|C:\\p\\obj\\ref\\P.dll|C:\\p\\obj\\\nnscript-jsforeign C:\\web\\a.js",
                });
            }, out int exitCode);

            Assert.AreEqual(ServiceHost.SyncExitYes, exitCode, output);
            Assert.AreEqual(
                "nscript service: sync yes A.dll (1 ms)\nnscript-ref C:\\p\\|C:\\p\\obj\\ref\\P.dll|C:\\p\\obj\\\nnscript-jsforeign C:\\web\\a.js\n",
                output.Replace("\r\n", "\n"));
            CollectionAssert.AreEqual(new[] { key, "30" }, seen.Args);
        }

        /// <summary>S2: a daemon that hangs up without a reply is a no (exit 1) with one line saying so.</summary>
        [TestMethod]
        [Timeout(5000)]
        public void SyncClient_DaemonClosesWithoutReply_AnswersNo()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            var output = RunSyncClient(identity, Path.Combine(NewTempDir(), "A.dll"), server => ServiceProtocol.ReadMessage<ServiceRequest>(server), out int exitCode);

            Assert.AreEqual(ServiceHost.SyncExitNo, exitCode, output);
            Assert.AreEqual("nscript service: the daemon closed the connection without replying\n", output.Replace("\r\n", "\n"));
        }

        /// <summary>
        /// S3 P4: an unreadable NSCRIPT_SYNC_WAIT_SECONDS is one line and exit 1 (MSBuild shows
        /// it and builds as today), not a stack trace; no daemon is asked.
        /// </summary>
        [TestMethod]
        public void SyncClient_BadWaitSetting_OneLineNo()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            var saved = Environment.GetEnvironmentVariable(ServiceHost.SyncWaitSecondsEnvVar);
            try
            {
                foreach (var value in new[] { "soon", "0" })
                {
                    Environment.SetEnvironmentVariable(ServiceHost.SyncWaitSecondsEnvVar, value);
                    var output = RunSyncClient(identity, Path.Combine(NewTempDir(), "A.dll"), null, out int exitCode);

                    Assert.AreEqual(1, exitCode, output);
                    Assert.AreEqual(
                        "nscript service: " + ServiceHost.SyncWaitSecondsEnvVar + " must be a positive integer, got '" + value + "'\n",
                        output.Replace("\r\n", "\n"));
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(ServiceHost.SyncWaitSecondsEnvVar, saved);
            }
        }

        /// <summary>
        /// S2 P2: a killed daemon leaves its marker, and every build would pay the connect
        /// timeout. With no daemon, the client deletes a marker naming its own pipe, and keeps
        /// one naming another daemon's pipe.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // Waits out the 1 s pipe connect timeout twice.
        public void SyncClient_NoDaemon_DeletesOnlyItsOwnDeadMarker()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));
            var key = Path.Combine(NewTempDir(), "A.dll");
            var marker = ServiceHost.WatchMarkerPath(key);

            File.WriteAllText(marker, "pipe=another-daemon\n");
            var kept = RunSyncClient(identity, key, null, out int keptExit);
            Assert.AreEqual(ServiceHost.SyncExitNo, keptExit, kept);
            Assert.IsTrue(File.Exists(marker), "Another daemon's marker was deleted.");
            StringAssert.Contains(kept, "no daemon running");

            File.WriteAllText(marker, "pipe=" + identity.PipeName + "\npid=1\n");
            var deleted = RunSyncClient(identity, key, null, out int deletedExit);
            Assert.AreEqual(ServiceHost.SyncExitNo, deletedExit, deleted);
            Assert.IsFalse(File.Exists(marker), "The dead daemon's marker was kept.");
            StringAssert.StartsWith(deleted, "nscript service: deleted nscript.watch: its daemon is gone");
        }

        /// <summary>
        /// Runs <c>nscript service --sync <paramref name="key"/></c> for <paramref name="identity"/>
        /// and returns what it printed. With <paramref name="daemon"/>, a fake daemon on the
        /// identity's pipe serves the one connection.
        /// </summary>
        private static string RunSyncClient(ServiceIdentity identity, string key, Action<Stream> daemon, out int exitCode)
        {
            System.Threading.Tasks.Task served = System.Threading.Tasks.Task.CompletedTask;
            System.IO.Pipes.NamedPipeServerStream server = null;
            if (daemon != null)
            {
                server = new System.IO.Pipes.NamedPipeServerStream(
                    identity.PipeName,
                    System.IO.Pipes.PipeDirection.InOut,
                    1,
                    System.IO.Pipes.PipeTransmissionMode.Byte,
                    System.IO.Pipes.PipeOptions.Asynchronous);
                served = server.WaitForConnectionAsync().ContinueWith(_ =>
                {
                    using (server)
                    {
                        daemon(server);
                    }
                });
            }

            var output = new StringWriter();
            var saved = Console.Out;
            Console.SetOut(output);
            try
            {
                exitCode = ServiceHost.Run(new[] { "--sync", key, "--toolset-dir", identity.ToolsetDir, "--toolset-hash", identity.ToolsetHash });
            }
            finally
            {
                Console.SetOut(saved);
                server?.Dispose();
            }

            served.Wait(TimeSpan.FromSeconds(2));
            return output.ToString();
        }

        /// <summary>
        /// Ruling (M5.1): a plain --stop that connects to a hung daemon waited for a reply
        /// forever. It must give up after <see cref="ServiceHost.StopReplyTimeout"/> and point
        /// to --stop --force.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // Waits out the real 10 s --stop reply timeout.
        public void Stop_HungDaemon_TimesOutWithForceHint()
        {
            var identity = ServiceIdentity.ForToolset(AppContext.BaseDirectory);
            using var server = new System.IO.Pipes.NamedPipeServerStream(
                identity.PipeName,
                System.IO.Pipes.PipeDirection.InOut,
                1,
                System.IO.Pipes.PipeTransmissionMode.Byte,
                System.IO.Pipes.PipeOptions.Asynchronous | System.IO.Pipes.PipeOptions.CurrentUserOnly);
            var accepted = server.WaitForConnectionAsync();
            var output = new StringWriter();
            var savedOut = Console.Out;
            int exitCode = -1;
            var stop = new Thread(() => exitCode = ServiceHost.Run(new[] { "--stop" })) { IsBackground = true };
            Console.SetOut(output);
            try
            {
                stop.Start();
                Assert.IsTrue(stop.Join(TimeSpan.FromSeconds(20)), "--stop never gave up on the hung daemon.");
            }
            finally
            {
                Console.SetOut(savedOut);
            }

            Assert.IsTrue(accepted.IsCompleted, "--stop never connected to the hung daemon.");
            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(output.ToString(), "--stop --force");
        }

        /// <summary>
        /// Contract 9: a step that runs past the request timeout makes the watchdog end the
        /// process with reason "watchdog" (the exit itself is injected here).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Watchdog_StepOverTimeout_ExitsWithWatchdogReason()
        {
            string exitReason = null;
            var exited = new ManualResetEventSlim();
            var options = new ServiceHostOptions
            {
                RequestTimeout = TimeSpan.FromMilliseconds(200),
                RunRequest = request =>
                {
                    Thread.Sleep(400);
                    return new ServiceResponse { ExitCode = 0 };
                },
                ExitProcess = reason =>
                {
                    exitReason = reason;
                    exited.Set();
                },
            };
            var pipeName = "nscript-test-" + Guid.NewGuid().ToString("N");
            var host = new ServiceHost(ServiceIdentity.FromKnown(AppContext.BaseDirectory, new string('0', 64)), pipeName, TimeSpan.FromMinutes(5), false, options);
            var serve = new Thread(() => host.Serve());
            serve.Start();

            Send(pipeName, ServiceProtocol.KindCompile, Path.GetTempPath(), "x.cs");

            Assert.IsTrue(exited.Wait(TimeSpan.FromSeconds(2)), "The watchdog did not fire.");
            Assert.AreEqual("watchdog", exitReason);
            host.RequestStop("test");
            Assert.IsTrue(serve.Join(TimeSpan.FromSeconds(5)));
            StringAssert.Contains(File.ReadAllText(this.logPath), "\"Reason\":\"watchdog\"");
        }

        /// <summary>
        /// Ruling N1: --stop --force kills only a process that holds the run lock and whose
        /// start time matches the pid file. A stale pid file (no lock, or a reused pid) kills
        /// nothing.
        /// </summary>
        [TestMethod]
        public void ForceStop_KillsOnlyLockHolderWithMatchingStartTime()
        {
            var identity = ServiceIdentity.FromKnown(NewTempDir(), new string('0', 64));
            Directory.CreateDirectory(identity.RunDir);
            var victim = Process.Start(new ProcessStartInfo("ping", "-n 30 127.0.0.1") { CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true });
            try
            {
                var pidFile = Path.Combine(identity.RunDir, ServiceHost.PidFileName);
                var lockFile = Path.Combine(identity.RunDir, ServiceLauncher.DaemonLockFile);
                long startTicks = victim.StartTime.ToUniversalTime().Ticks;
                int kills = 0;
                void Kill(Process p)
                {
                    kills++;
                    p.Kill();
                }

                File.WriteAllText(pidFile, victim.Id + "\n" + startTicks + "\n");
                File.WriteAllText(lockFile, string.Empty);
                Assert.AreEqual(1, ServiceHost.ForceStop(identity, new StringWriter(), Kill), "No lock holder: stale pid file.");

                using (new FileStream(lockFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    File.WriteAllText(pidFile, victim.Id + "\n" + (startTicks + 1) + "\n");
                    Assert.AreEqual(1, ServiceHost.ForceStop(identity, new StringWriter(), Kill), "Start time differs: the pid was reused.");
                    Assert.AreEqual(0, kills);
                    Assert.IsFalse(victim.HasExited);

                    File.WriteAllText(pidFile, victim.Id + "\n" + startTicks + "\n");
                    var output = new StringWriter();
                    Assert.AreEqual(0, ServiceHost.ForceStop(identity, output, Kill));
                    Assert.AreEqual(1, kills);
                    StringAssert.Contains(output.ToString(), "killed daemon pid=" + victim.Id);
                }

                Assert.IsTrue(victim.WaitForExit(2000));
            }
            finally
            {
                if (!victim.HasExited)
                {
                    victim.Kill();
                }

                Directory.Delete(identity.ServiceRoot, recursive: true);
            }
        }

        /// <summary>
        /// Contract 5: a registered compile is replayed from a byte-identical copy of its @rsp
        /// in the run dir, so the replay still works after MSBuild deleted the original.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void WatchRegister_RspSnapshot_ReplaysAfterOriginalIsDeleted()
        {
            var calls = new List<string[]>();
            var replayed = new ManualResetEventSlim();
            using var watch = new WatchHost(request =>
            {
                lock (calls)
                {
                    calls.Add(request.Args);
                    if (calls.Count == 2)
                    {
                        replayed.Set();
                    }
                }

                return new ServiceResponse { ExitCode = 0 };
            });
            var rsp = Path.Combine(watch.Project, "csc.rsp");
            File.WriteAllText(rsp, "/out:obj\\A.dll A.cs");
            var rspBytes = File.ReadAllBytes(rsp);

            watch.Register("@" + rsp);
            File.Delete(rsp);
            watch.EditSource();

            Assert.IsTrue(replayed.Wait(TimeSpan.FromSeconds(5)), "The edit did not replay the compile.");
            var snapshot = calls[1].Single();
            StringAssert.StartsWith(snapshot, "@" + Path.Combine(watch.Identity.RunDir, "rsp") + Path.DirectorySeparatorChar);
            CollectionAssert.AreEqual(rspBytes, File.ReadAllBytes(snapshot.Substring(1)));
            CollectionAssert.AreEqual(calls[0], calls[1], "Registration and replay run the same snapshot.");
        }

        /// <summary>
        /// Contract 8: a batch that finds a rebuilt toolset runs no step and stops the daemon
        /// with reason toolsetChanged (one writer per output).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void WatchBatch_ToolsetChanged_RunsNoStepAndStops()
        {
            int calls = 0;
            using var watch = new WatchHost(
                request =>
                {
                    Interlocked.Increment(ref calls);
                    return new ServiceResponse { ExitCode = 0 };
                },
                toolsetHash: new string('f', 64));

            watch.Register("A.cs");
            watch.EditSource();

            Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)), "The daemon kept running on a changed toolset.");
            Assert.AreEqual(1, calls, "Only the registration ran; the batch ran no step.");
            StringAssert.Contains(File.ReadAllText(this.logPath), "\"Reason\":\"toolsetChanged\"");
        }

        /// <summary>
        /// Contract 10: --stop during a watch batch waits for it; ServiceStop is logged after
        /// WatchBatchEnd, so no output is left half-written by the stop.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Stop_DuringWatchBatch_ServiceStopFollowsBatchEnd()
        {
            int calls = 0;
            var replaying = new ManualResetEventSlim();
            var release = new ManualResetEventSlim();
            using var watch = new WatchHost(request =>
            {
                if (Interlocked.Increment(ref calls) == 2)
                {
                    replaying.Set();
                    release.Wait(TimeSpan.FromSeconds(5));
                }

                return new ServiceResponse { ExitCode = 0 };
            });

            watch.Register("A.cs");
            watch.EditSource();
            Assert.IsTrue(replaying.Wait(TimeSpan.FromSeconds(5)), "The edit did not replay the compile.");
            watch.Host.RequestStop("test");
            Assert.IsFalse(watch.Serve.Join(TimeSpan.FromMilliseconds(200)), "The daemon stopped in the middle of a batch.");
            release.Set();

            Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)));
            var log = File.ReadAllText(this.logPath);
            int batchEnd = log.IndexOf("WatchBatchEnd", StringComparison.Ordinal);
            int stop = log.IndexOf("ServiceStop", StringComparison.Ordinal);
            Assert.IsTrue(batchEnd >= 0, "No WatchBatchEnd logged.");
            Assert.IsTrue(stop > batchEnd, "ServiceStop was logged before WatchBatchEnd.");
        }

        /// <summary>
        /// Defect D5: after NEEDS BUILD, the dotnet build the log asks for re-registers only the
        /// rebuilt project; its dependents stayed dirty with no batch until an unrelated save.
        /// The registration must start a batch once the build is quiet.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Register_AfterNeedsBuild_ReplaysDirtyDependents()
        {
            var dependentCompiled = new ManualResetEventSlim();
            WatchHost watch = null;
            using (watch = new WatchHost(
                request =>
                {
                    if (string.Equals(request.Cwd, watch?.Dependent, StringComparison.OrdinalIgnoreCase) && watch.Registered >= 2)
                    {
                        dependentCompiled.Set();
                    }

                    return new ServiceResponse { ExitCode = 0 };
                },
                registrationQuiet: TimeSpan.FromMilliseconds(100)))
            {
                watch.Register("A.cs");
                watch.RegisterIn(watch.Dependent, "B.cs");

                File.WriteAllText(Path.Combine(watch.Project, "New.cs"), "class N { }");
                WaitForLog("WatchNeedsBuild", 1);
                watch.EditSource();
                WaitForLog("\"Result\":\"blocked\"", 2);
                Assert.IsFalse(dependentCompiled.IsSet);

                // dotnet build -p:NScriptWatch=true of the first project only.
                watch.Register("A.cs", "New.cs");

                Assert.IsTrue(dependentCompiled.Wait(TimeSpan.FromSeconds(3)), "The dirty dependent was not replayed after the registration.");
            }
        }

        /// <summary>
        /// The core loop: a save recompiles the project and re-emits its bundle; a failed
        /// compile keeps the last good bundle without running the emit; a revert restores the
        /// original bytes. The faked compile copies A.cs into obj\A.dll (exit 1 when the source
        /// says "broken"); the faked emit copies obj\A.dll into app.js.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Watch_EditSource_ReemitsBundle_RevertRestoresBytes()
        {
            int emits = 0;
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                var source = Path.Combine(watch.Project, "A.cs");
                var dll = Path.Combine(watch.Project, "obj", "A.dll");
                if (request.Kind == ServiceProtocol.KindCompile)
                {
                    var text = File.ReadAllText(source);
                    if (text.Contains("broken", StringComparison.Ordinal))
                    {
                        // A real compile always reports its output; watch.log shows the errors.
                        return new ServiceResponse { ExitCode = 1, Stdout = "A.cs(1,11): error CS1519: broken", Stderr = string.Empty };
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.WriteAllText(dll, text);
                    return new ServiceResponse { ExitCode = 0 };
                }

                Interlocked.Increment(ref emits);
                File.WriteAllText(Path.Combine(watch.Project, "app.js"), "js:" + File.ReadAllText(dll));
                return new ServiceResponse { ExitCode = 0 };
            }))
            {
                var appJs = Path.Combine(watch.Project, "app.js");
                watch.Register("A.cs");
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"obj\A.dll", "-references", typeof(BuildServiceTests).Assembly.Location);
                var original = File.ReadAllBytes(appJs);

                watch.EditSource();
                WaitFor(() => File.ReadAllText(appJs).Contains("int x", StringComparison.Ordinal), "The save did not re-emit app.js.");
                int emitsAfterEdit = Volatile.Read(ref emits);

                File.WriteAllText(Path.Combine(watch.Project, "A.cs"), "class A { broken }");
                WaitForLog("\"Result\":\"kept\"", 1);
                Assert.AreEqual(emitsAfterEdit, Volatile.Read(ref emits), "A red project's bundle was emitted.");
                StringAssert.Contains(File.ReadAllText(appJs), "int x");

                File.WriteAllText(Path.Combine(watch.Project, "A.cs"), "class A { }");
                WaitFor(() => File.ReadAllBytes(appJs).AsSpan().SequenceEqual(original), "The revert did not restore app.js.");
            }
        }

        /// <summary>
        /// Trace B: a signature change in A breaks its dependent B. B goes red; app.js (reads A
        /// and B) keeps its last good bytes and write time without running the emit, obj\B.dll
        /// is untouched, and watch.log names the error, while ctl.js (reads only A) is emitted
        /// with the change. While red, another edit of A recompiles B and reports the error
        /// again. Fixing B clears red and emits app.js in the same batch. The faked compile of
        /// A copies A.cs into obj\A.dll; the faked compile of B fails while obj\A.dll says Sig2
        /// and B.cs does not, else copies B.cs into obj\B.dll (Roslyn writes no DLL on errors);
        /// the faked emit writes the DLLs its bundle reads. Both bundles write outside the
        /// watched folders, so their writes start no batch.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Watch_TraceB_DependentRedKeepsBundle_FixEmits()
        {
            int appEmits = 0;
            int controlEmits = 0;
            var web = NewTempDir();
            var appJs = Path.Combine(web, "app.js");
            var controlJs = Path.Combine(web, "ctl.js");
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                var aDll = Path.Combine(watch.Project, "obj", "A.dll");
                var bDll = Path.Combine(watch.Dependent, "obj", "B.dll");
                if (request.Kind != ServiceProtocol.KindCompile)
                {
                    if (string.Equals(request.Cwd, watch.Project, StringComparison.OrdinalIgnoreCase))
                    {
                        Interlocked.Increment(ref controlEmits);
                        File.WriteAllText(controlJs, "js:" + File.ReadAllText(aDll));
                    }
                    else
                    {
                        Interlocked.Increment(ref appEmits);
                        File.WriteAllText(appJs, "js:" + File.ReadAllText(aDll) + "|" + File.ReadAllText(bDll));
                    }

                    return new ServiceResponse { ExitCode = 0 };
                }

                bool dependent = string.Equals(request.Cwd, watch.Dependent, StringComparison.OrdinalIgnoreCase);
                var text = File.ReadAllText(dependent ? Path.Combine(watch.Dependent, "B.cs") : Path.Combine(watch.Project, "A.cs"));
                if (dependent
                    && File.ReadAllText(aDll).Contains("Sig2", StringComparison.Ordinal)
                    && !text.Contains("Sig2", StringComparison.Ordinal))
                {
                    return new ServiceResponse { ExitCode = 1, Stdout = "B.cs(1,17): error CS7036: no argument for b", Stderr = string.Empty };
                }

                var dll = dependent ? bDll : aDll;
                Directory.CreateDirectory(Path.GetDirectoryName(dll));
                File.WriteAllText(dll, text);
                return new ServiceResponse { ExitCode = 0 };
            }))
            {
                var bDll = Path.Combine(watch.Dependent, "obj", "B.dll");
                watch.Register("A.cs");
                watch.RegisterIn(watch.Dependent, "B.cs");
                watch.RegisterEmit("-outJs", controlJs, "-entryAssembly", @"obj\A.dll", "-references", typeof(BuildServiceTests).Assembly.Location);
                watch.RegisterEmitIn(watch.Dependent, "-outJs", appJs, "-entryAssembly", @"obj\B.dll", "-references", Path.Combine(watch.Project, "obj", "A.dll"));
                var goodJs = File.ReadAllBytes(appJs);
                var goodJsTime = File.GetLastWriteTimeUtc(appJs);
                var goodDll = File.ReadAllBytes(bDll);
                var goodDllTime = File.GetLastWriteTimeUtc(bDll);
                Assert.AreEqual(1, Volatile.Read(ref appEmits));
                var watchLog = watch.Status()["WatchLog"];

                void AssertLastGoodKept(string when)
                {
                    Assert.AreEqual(1, Volatile.Read(ref appEmits), when + ": app.js was emitted although it reads a red project.");
                    CollectionAssert.AreEqual(goodJs, File.ReadAllBytes(appJs), when + ": app.js changed.");
                    Assert.AreEqual(goodJsTime, File.GetLastWriteTimeUtc(appJs), when + ": app.js was rewritten.");
                    CollectionAssert.AreEqual(goodDll, File.ReadAllBytes(bDll), when + ": obj\\B.dll changed.");
                    Assert.AreEqual(goodDllTime, File.GetLastWriteTimeUtc(bDll), when + ": obj\\B.dll was rewritten.");
                    Assert.AreEqual("B.dll", watch.Status()["WatchRed"], when);
                }

                // B1: the signature change. A compiles and ctl.js gets it; B fails; app.js is kept.
                var sig2 = "class A { void M(int a, int b) { } } // Sig2";
                File.WriteAllText(Path.Combine(watch.Project, "A.cs"), sig2);
                this.WaitForLogLines(1, "WatchBatchEnd", "\"Result\":\"failed\"");
                AssertLastGoodKept("B1");
                Assert.AreEqual(2, Volatile.Read(ref controlEmits), "B1: ctl.js, which reads only A, was not emitted.");
                Assert.AreEqual("js:" + sig2, File.ReadAllText(controlJs));
                Assert.AreEqual(1, CountOf(ReadShared(watchLog), "error CS7036"), "watch.log does not name the error once.");

                // B2: another edit of A while B is red recompiles B; it fails again; still kept.
                var sig2y = "class A { void M(int a, int b) { } int y; } // Sig2";
                File.WriteAllText(Path.Combine(watch.Project, "A.cs"), sig2y);
                this.WaitForLogLines(2, "WatchBatchEnd", "\"Result\":\"failed\"");
                AssertLastGoodKept("B2");
                Assert.AreEqual(2, CountOf(ReadShared(watchLog), "error CS7036"), "The red re-entry did not report the error again.");

                // B5: the fix. B compiles, red clears, app.js is emitted once, in that batch.
                var fixedB = "class B : A { } // Sig2";
                File.WriteAllText(Path.Combine(watch.Dependent, "B.cs"), fixedB);
                this.WaitForLogLines(1, "WatchBatchEnd", "\"Result\":\"ok\"");
                Assert.AreEqual(2, Volatile.Read(ref appEmits));
                Assert.AreEqual("js:" + sig2y + "|" + fixedB, File.ReadAllText(appJs));
                Assert.AreEqual(string.Empty, watch.Status()["WatchRed"]);
            }

            Directory.Delete(web, recursive: true);
        }

        /// <summary>
        /// P14: a slow checkout trickles files in while a batch runs. A save that lands
        /// mid-batch supersedes the batch's remaining steps; they stay dirty and the next batch
        /// runs them, so a storm does not rebuild the whole chain once per batch.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 2-3 s.
        public void WatchBatch_SaveDuringBatch_SupersedesRemainingSteps()
        {
            int projectReplays = 0;
            int dependentReplays = 0;
            var firstReplay = new ManualResetEventSlim();
            var release = new ManualResetEventSlim();
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (watch.Registered < 2)
                {
                    return new ServiceResponse { ExitCode = 0 };
                }

                if (string.Equals(request.Cwd, watch.Dependent, StringComparison.OrdinalIgnoreCase))
                {
                    Interlocked.Increment(ref dependentReplays);
                }
                else if (Interlocked.Increment(ref projectReplays) == 1)
                {
                    firstReplay.Set();
                    release.Wait(TimeSpan.FromSeconds(5));
                }

                return new ServiceResponse { ExitCode = 0 };
            }))
            {
                watch.Register("A.cs");
                watch.RegisterIn(watch.Dependent, "B.cs");

                watch.EditSource();
                Assert.IsTrue(firstReplay.Wait(TimeSpan.FromSeconds(5)), "The edit did not replay the compile.");
                File.WriteAllText(Path.Combine(watch.Project, "A.cs"), "class A { int y; }");

                // The second save is observable only through the batch it starts; give the
                // watcher time to deliver it before the first batch moves on.
                Thread.Sleep(1000);
                release.Set();

                // Batch 1 stops after A; batch 2 runs the dependent it left dirty. (The faked
                // compile hashes its inputs after it returns, so A already counts the second save
                // and batch 2 does not recompile it; a real compile snapshots before.)
                WaitForLog("WatchBatchSuperseded", 1);
                WaitForLog("WatchBatchEnd", 2);
                Assert.AreEqual(1, Volatile.Read(ref projectReplays));
                Assert.AreEqual(1, Volatile.Read(ref dependentReplays), "The dependent should compile once, in the batch after the superseded one.");
            }
        }

        /// <summary>
        /// The locked-copy path: MSBuild's copy of a project's DLL (bin\A.dll, read by a bundle)
        /// is held open by another process, so the refresh after a compile cannot overwrite it.
        /// The copy is marked pending, its bundle is kept, and a timed retry copies it once the
        /// holder lets go, with no further save. The faked compile writes a real assembly to
        /// obj\A.dll: the test assembly at registration, NScript.Lib after that (a new MVID).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe, plus the 1 s copy retry: 2-3 s.
        public void Watch_CopyTargetLocked_PendingThenTimedRetryCopies()
        {
            string first = typeof(BuildServiceTests).Assembly.Location;
            string second = typeof(ServiceHost).Assembly.Location;
            int compiles = 0;
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindCompile)
                {
                    var dll = Path.Combine(watch.Project, "obj", "A.dll");
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.Copy(Interlocked.Increment(ref compiles) == 1 ? first : second, dll, overwrite: true);
                }

                return new ServiceResponse { ExitCode = 0, Stdout = string.Empty, Stderr = string.Empty };
            }))
            {
                watch.Register("A.cs");
                var copy = Path.Combine(watch.Project, "bin", "A.dll");
                Directory.CreateDirectory(Path.GetDirectoryName(copy));
                File.Copy(first, copy);
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"bin\A.dll", "-references", first);

                using (new FileStream(copy, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    watch.EditSource();
                    WaitForLog("\"Result\":\"pending\"", 1);
                    WaitForLog("\"Result\":\"kept\"", 1);
                    StringAssert.Contains(watch.Status()["WatchCopyPending"], copy);
                }

                // Released: no save follows; the timed retry must refresh the copy.
                WaitForLog("\"Step\":\"copy\"", 2);
                WaitFor(() => watch.Status()["WatchCopyPending"].Length == 0, "The copy stayed pending after its holder let go.");
                CollectionAssert.AreEqual(File.ReadAllBytes(second), File.ReadAllBytes(copy), "bin\\A.dll is not the new build.");
            }
        }

        /// <summary>
        /// P12: a stop that leaves a bundle kept and a bin copy pending (its holder still has it
        /// open) left both stale with nothing in watch.log naming them. The stop must list each
        /// one. Setup as in <see cref="Watch_CopyTargetLocked_PendingThenTimedRetryCopies"/>.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Stop_WithPendingCopyAndKeptBundle_LogsBothStale()
        {
            string first = typeof(BuildServiceTests).Assembly.Location;
            string second = typeof(ServiceHost).Assembly.Location;
            int compiles = 0;
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindCompile)
                {
                    var dll = Path.Combine(watch.Project, "obj", "A.dll");
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.Copy(Interlocked.Increment(ref compiles) == 1 ? first : second, dll, overwrite: true);
                }

                return new ServiceResponse { ExitCode = 0, Stdout = string.Empty, Stderr = string.Empty };
            }))
            {
                watch.Register("A.cs");
                var copy = Path.Combine(watch.Project, "bin", "A.dll");
                Directory.CreateDirectory(Path.GetDirectoryName(copy));
                File.Copy(first, copy);
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"bin\A.dll", "-references", first);
                var watchLog = watch.Status()["WatchLog"];

                using (new FileStream(copy, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    watch.EditSource();
                    WaitForLog("\"Result\":\"kept\"", 1);
                    watch.Host.RequestStop("test");
                    Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)), "The daemon did not stop.");
                }

                var log = ReadShared(watchLog);
                StringAssert.Contains(log, "STALE   " + Path.Combine(watch.Project, "app.js"));
                StringAssert.Contains(log, "STALE   " + copy);
            }
        }

        /// <summary>
        /// S3 D-S3-3: a bundle kept for a pending copy, whose copy a full build then refreshed
        /// (here: the copy already equals the obj DLL while it is held open), has no blocker
        /// left once sync clears the copy. Sync must emit it and answer yes, not "not emitted".
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Sync_CopyPendingClearedAtSync_EmitsKeptBundle_AnswersYes()
        {
            string first = typeof(BuildServiceTests).Assembly.Location;
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindCompile)
                {
                    // Same bytes and write time every compile: bin\A.dll equals obj\A.dll throughout.
                    var dll = Path.Combine(watch.Project, "obj", "A.dll");
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.Copy(first, dll, overwrite: true);
                }

                return new ServiceResponse { ExitCode = 0, Stdout = string.Empty, Stderr = string.Empty };
            }))
            {
                watch.Register("A.cs");
                var copy = Path.Combine(watch.Project, "bin", "A.dll");
                Directory.CreateDirectory(Path.GetDirectoryName(copy));
                File.Copy(first, copy);
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"bin\A.dll", "-references", first);

                using (new FileStream(copy, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    watch.EditSource();
                    WaitForLog("\"Result\":\"kept\"", 1);

                    var answer = watch.Sync(Path.Combine(watch.Project, "obj", "A.dll"));
                    Assert.AreEqual(ServiceHost.SyncExitYes, answer.ExitCode, answer.Message);
                    Assert.AreEqual(string.Empty, watch.Status()["WatchCopyPending"]);
                }
            }
        }

        /// <summary>
        /// S2/S3: a yes lists, after the answer line, each JS file of the project's bundles the
        /// daemon did not write last (here rewritten by another build) and each vouched
        /// reference as "project dir|path csc reads|its intermediate dir". Sdk.targets parses
        /// exactly this text.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Sync_Yes_ReplyListsForeignJsAndVouchedReference()
        {
            var appJs = Path.Combine(NewTempDir(), "app.js");
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindEmitJs)
                {
                    File.WriteAllText(appJs, "daemon");
                }
                else
                {
                    // The emit's options name the obj DLLs, so they must exist.
                    var dll = Path.Combine(request.Cwd, "obj", request.Cwd == watch.Project ? "A.dll" : "B.dll");
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.WriteAllText(dll, "dll");
                }

                return new ServiceResponse { ExitCode = 0 };
            }))
            {
                var aDll = Path.Combine(watch.Project, "obj", "A.dll");
                watch.Register("A.cs");
                watch.RegisterIn(watch.Dependent, "B.cs");
                watch.RegisterEmitIn(watch.Dependent, "-outJs", appJs, "-entryAssembly", @"obj\B.dll", "-references", aDll);
                File.WriteAllText(appJs, "another build");

                var answer = watch.Sync(Path.Combine(watch.Dependent, "obj", "B.dll"));

                Assert.AreEqual(ServiceHost.SyncExitYes, answer.ExitCode, answer.Message);
                Assert.AreEqual(
                    "nscript service: sync yes B.dll (N ms)\n"
                        + ServiceHost.SyncJsForeignPrefix + appJs + "\n"
                        + ServiceHost.SyncRefPrefix + watch.Project + @"\|" + aDll + "|" + Path.Combine(watch.Project, "obj") + @"\",
                    System.Text.RegularExpressions.Regex.Replace(answer.Message, @"\(\d+ ms\)", "(N ms)"));

                // WatchSyncReferencesTests feeds Sdk.targets this formatter's line.
                StringAssert.EndsWith(answer.Message, "\n" + ServiceHost.SyncRefLine(watch.Project, aDll, Path.Combine(watch.Project, "obj")));
            }
        }

        /// <summary>
        /// S2: sync answers no, never yes, for a key it does not watch (with the watched key
        /// when only the folder is wrong, in full: the path is the hint) and for bad arguments.
        /// With the watcher's .cs events lost, a new source file and a deleted input are still
        /// found by sync's own scan and answer no. A reason naming a file keeps only its name,
        /// folders with spaces included (P5).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Sync_UnwatchedBadArgsOrUnseenFile_AnswersNo()
        {
            WatchHost watch = null;
            using (watch = new WatchHost(_ => new ServiceResponse { ExitCode = 0 }, dropWatchEvents: new[] { ".cs" }))
            {
                var key = Path.Combine(watch.Project, "obj", "A.dll");
                watch.Register("A.cs");

                var binCopy = watch.Sync(Path.Combine(watch.Project, "bin", "A.dll"));
                Assert.AreEqual(ServiceHost.SyncExitNo, binCopy.ExitCode);
                StringAssert.Contains(binCopy.Message, "not watched; the watched key for that name is " + key + " (");

                var other = watch.Sync(Path.Combine(watch.Project, "obj", "Other.dll"));
                Assert.AreEqual(ServiceHost.SyncExitNo, other.ExitCode);
                StringAssert.Contains(other.Message, "Other.dll: not watched (pass the obj DLL csc writes)");

                foreach (var args in new[] { new[] { key }, new[] { key, "0" }, new[] { key, "x" } })
                {
                    var bad = watch.SyncArgs(args);
                    Assert.AreEqual(ServiceHost.SyncExitNo, bad.ExitCode, string.Join(" ", args));
                    Assert.AreEqual("nscript service: sync needs <obj dll> <wait seconds>", bad.Message);
                }

                var bKey = Path.Combine(watch.Dependent, "obj", "B.dll");
                watch.RegisterIn(watch.Dependent, "B.cs");
                Assert.AreEqual(ServiceHost.SyncExitYes, watch.Sync(bKey).ExitCode, "control: nothing changed");

                // A deleted input (B reads A, which stays current).
                File.Delete(Path.Combine(watch.Dependent, "B.cs"));
                var deleted = watch.Sync(bKey);
                Assert.AreEqual(ServiceHost.SyncExitNo, deleted.ExitCode, deleted.Message);
                StringAssert.Contains(deleted.Message, "sync no B.dll: B.dll needs dotnet build -p:NScriptWatch=true: deleted B.cs (");

                // A new source file in the project folder: csc must be told by a dotnet build.
                File.WriteAllText(Path.Combine(watch.Project, "New.cs"), "class N { }");
                var newFile = watch.Sync(key);
                Assert.AreEqual(ServiceHost.SyncExitNo, newFile.ExitCode, newFile.Message);
                Assert.AreEqual(
                    "nscript service: sync no A.dll: A.dll needs dotnet build -p:NScriptWatch=true: new file New.cs (N ms)",
                    System.Text.RegularExpressions.Regex.Replace(newFile.Message, @"\(\d+ ms\)", "(N ms)"));

            }
        }

        /// <summary>
        /// S2: while a batch runs, sync waits for it and answers busy when its wait runs out;
        /// a sync waiting when the daemon stops answers busy at once (MSBuild then builds as
        /// today), not yes or no.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe; waits out a 1 s sync wait.
        public void Sync_BatchRunning_BusyAfterWait_BusyWhenStopping()
        {
            using var hold = new ManualResetEventSlim(false);
            using var held = new ManualResetEventSlim(false);
            using var release = new ManualResetEventSlim(false);
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindCompile && hold.IsSet)
                {
                    held.Set();
                    release.Wait(TimeSpan.FromSeconds(10));
                }

                return new ServiceResponse { ExitCode = 0 };
            }))
            {
                var key = Path.Combine(watch.Project, "obj", "A.dll");
                watch.Register("A.cs");
                hold.Set();
                watch.EditSource();
                Assert.IsTrue(held.Wait(TimeSpan.FromSeconds(5)), "The batch never compiled the save.");
                try
                {
                    var busy = watch.Sync(key, "1");
                    Assert.AreEqual(ServiceHost.SyncExitBusy, busy.ExitCode, busy.Message);
                    StringAssert.Contains(busy.Message, "a batch still running after 1 s");

                    // Connected and asked before the stop: the daemon still answers it.
                    using var pipe = ServiceClient.TryConnect(watch.PipeName, 5000);
                    ServiceProtocol.WriteMessage(pipe, new ServiceRequest
                    {
                        Kind = ServiceProtocol.KindSync,
                        ClientPid = Environment.ProcessId,
                        Cwd = watch.Project,
                        Args = new[] { key, "10" },
                    });
                    watch.Host.RequestStop("test");
                    var stopping = ServiceProtocol.ReadMessage<ServiceResponse>(pipe);
                    Assert.AreEqual(ServiceHost.SyncExitBusy, stopping.ExitCode, stopping.Message);
                    StringAssert.Contains(stopping.Message, "daemon stopping");
                }
                finally
                {
                    release.Set();
                }
            }
        }

        /// <summary>
        /// Critic F8: the toolset-change stop is the likeliest stop and bypassed the stop path.
        /// It must name the bundle the save that found the new toolset leaves stale. The
        /// toolset changes only after the watcher-start rescan batch: a batch that finds it
        /// earlier stops the daemon before the save.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void WatchBatch_ToolsetChanged_LogsStaleBundle()
        {
            WatchHost watch = null;
            using (watch = new WatchHost(_ => new ServiceResponse { ExitCode = 0 }))
            {
                watch.Register("A.cs");
                WaitForLogLines(1, "WatchChange");
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"obj\A.dll", "-references", typeof(BuildServiceTests).Assembly.Location);
                var watchLog = watch.Status()["WatchLog"];

                watch.ToolsetHashNow = new string('f', 64);
                watch.EditSource();

                Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)), "The daemon kept running on a changed toolset.");
                StringAssert.Contains(ReadShared(watchLog), "STALE   " + Path.Combine(watch.Project, "app.js"));
            }
        }

        /// <summary>
        /// D-E3-1 (a): a bin copy held share-none from before the first save cannot be read,
        /// so the batch found no copy edge: the bundle reading the copy was never planned and
        /// the stop named nothing. Edges known from registration keep that bundle, and the
        /// stop names it and the copy. As in a real build, the bundle registers after the
        /// rescan batch that the first registration's watcher start runs.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Stop_FirstSaveWithCopyLockedShareNone_LogsReaderBundleAndCopyStale()
        {
            string first = typeof(BuildServiceTests).Assembly.Location;
            string second = typeof(ServiceHost).Assembly.Location;
            int compiles = 0;
            WatchHost watch = null;
            using (watch = new WatchHost(request =>
            {
                if (request.Kind == ServiceProtocol.KindCompile)
                {
                    var dll = Path.Combine(watch.Project, "obj", "A.dll");
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.Copy(Interlocked.Increment(ref compiles) == 1 ? first : second, dll, overwrite: true);
                }

                return new ServiceResponse { ExitCode = 0, Stdout = string.Empty, Stderr = string.Empty };
            }))
            {
                watch.Register("A.cs");
                WaitForLogLines(1, "WatchChange");
                var copy = Path.Combine(watch.Project, "bin", "A.dll");
                Directory.CreateDirectory(Path.GetDirectoryName(copy));
                File.Copy(first, copy);
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"bin\A.dll", "-references", first);
                var watchLog = watch.Status()["WatchLog"];

                using (new FileStream(copy, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    watch.EditSource();
                    WaitForLogLines(1, "WatchBatchEnd");
                    watch.Host.RequestStop("test");
                    Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)), "The daemon did not stop.");
                }

                var log = ReadShared(watchLog);
                StringAssert.Contains(log, "KEPT    " + Path.Combine(watch.Project, "app.js"));
                StringAssert.Contains(log, "STALE   " + Path.Combine(watch.Project, "app.js"));
                StringAssert.Contains(log, "STALE   " + copy);
            }
        }

        /// <summary>
        /// D-E3-1 (b): a toolset change found by the first save stops before that batch finds
        /// copy edges, and the only earlier batch (the rescan at watcher start) ran before the
        /// bundle registered; so the stop named only bundles reading obj. Edges known from
        /// registration make it name the bundle reading the bin copy too.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void WatchBatch_ToolsetChangedOnFirstSave_LogsCopyReaderStale()
        {
            string first = typeof(BuildServiceTests).Assembly.Location;
            using (var watch = new WatchHost(_ => new ServiceResponse { ExitCode = 0 }))
            {
                foreach (var dll in new[] { Path.Combine(watch.Project, "obj", "A.dll"), Path.Combine(watch.Project, "bin", "A.dll") })
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dll));
                    File.Copy(first, dll);
                }

                watch.Register("A.cs");
                WaitForLogLines(1, "WatchChange");
                watch.RegisterEmit("-outJs", "app.js", "-entryAssembly", @"bin\A.dll", "-references", first);
                var watchLog = watch.Status()["WatchLog"];

                watch.ToolsetHashNow = new string('f', 64);
                watch.EditSource();

                Assert.IsTrue(watch.Serve.Join(TimeSpan.FromSeconds(5)), "The daemon kept running on a changed toolset.");
                StringAssert.Contains(ReadShared(watchLog), "STALE   " + Path.Combine(watch.Project, "app.js"));
            }
        }

        private static void WaitFor(Func<bool> condition, string message)
        {
            var clock = Stopwatch.StartNew();
            while (!condition())
            {
                Assert.IsTrue(clock.Elapsed < TimeSpan.FromSeconds(5), message);
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Ruling 2 (tester P8): --status kept reporting the last batch as blocked after the
        /// dotnet build it asked for cleared NEEDS BUILD, so it read as still broken.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // A real daemon on a named pipe with file watchers: 1-2 s.
        public void Status_AfterBlockedBatchIsResolved_SaysSo()
        {
            using (var watch = new WatchHost(_ => new ServiceResponse { ExitCode = 0 }))
            {
                watch.Register("A.cs");
                File.WriteAllText(Path.Combine(watch.Project, "New.cs"), "class N { }");
                WaitForLog("WatchNeedsBuild", 1);
                watch.EditSource();
                WaitForLog("\"Result\":\"blocked\"", 1);
                var blocked = watch.Status();
                StringAssert.Contains(blocked["WatchLastBatch"], "blocked");
                Assert.IsFalse(blocked["WatchLastBatch"].Contains("resolved", StringComparison.Ordinal), blocked["WatchLastBatch"]);

                // dotnet build -p:NScriptWatch=true picks up New.cs.
                watch.Register("A.cs", "New.cs");

                var resolved = watch.Status();
                Assert.AreEqual(string.Empty, resolved["WatchNeedsBuild"]);
                StringAssert.Contains(resolved["WatchLastBatch"], "resolved");
            }
        }

        private void WaitForLog(string text, int count)
        {
            var clock = Stopwatch.StartNew();
            while (true)
            {
                string log;
                using (var stream = new FileStream(this.logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var reader = new StreamReader(stream))
                {
                    log = reader.ReadToEnd();
                }

                int found = 0;
                for (int at = log.IndexOf(text, StringComparison.Ordinal); at >= 0; at = log.IndexOf(text, at + 1, StringComparison.Ordinal))
                {
                    found++;
                }

                if (found >= count)
                {
                    return;
                }

                Assert.IsTrue(clock.Elapsed < TimeSpan.FromSeconds(5), "Timed out waiting for " + count + " x " + text);
                Thread.Sleep(20);
            }
        }

        /// <summary>Waits until <paramref name="count"/> lines of the JSONL log each contain every one of <paramref name="parts"/>.</summary>
        private void WaitForLogLines(int count, params string[] parts)
        {
            var clock = Stopwatch.StartNew();
            while (ReadShared(this.logPath).Split('\n').Count(line => parts.All(p => line.Contains(p, StringComparison.Ordinal))) < count)
            {
                Assert.IsTrue(clock.Elapsed < TimeSpan.FromSeconds(5), "Timed out waiting for " + count + " log lines with " + string.Join(" + ", parts));
                Thread.Sleep(20);
            }
        }

        private static string ReadShared(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static int CountOf(string text, string value)
        {
            int found = 0;
            for (int at = text.IndexOf(value, StringComparison.Ordinal); at >= 0; at = text.IndexOf(value, at + 1, StringComparison.Ordinal))
            {
                found++;
            }

            return found;
        }

        /// <summary>
        /// A daemon watching two projects whose compiles are faked by <c>run</c>: A.cs in
        /// <see cref="Project"/>, and B.cs in <see cref="Dependent"/>, which references A's
        /// output. Each faked compile reports its folder's .cs file as its only input.
        /// </summary>
        private sealed class WatchHost : IDisposable
        {
            private readonly string pipeName = "nscript-test-" + Guid.NewGuid().ToString("N");

            public WatchHost(Func<ServiceRequest, ServiceResponse> run, string toolsetHash = null, TimeSpan? registrationQuiet = null, string[] dropWatchEvents = null)
            {
                this.Identity = ServiceIdentity.FromKnown(NewTempDir(), new string('0', 64));
                this.Project = NewTempDir();
                this.Dependent = NewTempDir();
                File.WriteAllText(Path.Combine(this.Project, "A.cs"), "class A { }");
                File.WriteAllText(Path.Combine(this.Dependent, "B.cs"), "class B : A { }");
                var options = new ServiceHostOptions
                {
                    ToolsetHash = () => this.ToolsetHashNow ?? toolsetHash ?? this.Identity.ToolsetHash,
                    RunRequest = run,
                    RunRequestInputs = request => string.Equals(request.Cwd, this.Dependent, StringComparison.OrdinalIgnoreCase)
                        ? Inputs(this.Dependent, "B", Path.Combine(this.Project, "obj", "A.dll"))
                        : Inputs(this.Project, "A"),
                    RunRequestEmitOptions = request => ParseOptions.ParseArgs(request.Args),
                    DropWatchEvents = dropWatchEvents ?? Array.Empty<string>(),
                };
                if (registrationQuiet != null)
                {
                    options.RegistrationQuiet = registrationQuiet.Value;
                }

                this.Host = new ServiceHost(this.Identity, this.pipeName, TimeSpan.FromMinutes(5), false, options);
                this.Serve = new Thread(() => this.Host.Serve());
                this.Serve.Start();
            }

            public ServiceIdentity Identity { get; }

            /// <summary>When set, the toolset hash the daemon sees from now on.</summary>
            public string ToolsetHashNow { get; set; }

            public string Project { get; }

            public string Dependent { get; }

            /// <summary>Registrations sent so far.</summary>
            public int Registered { get; private set; }

            public ServiceHost Host { get; }

            public Thread Serve { get; }

            public void Register(params string[] args) => this.RegisterIn(this.Project, args);

            public void RegisterIn(string cwd, params string[] args)
            {
                var response = Send(this.pipeName, new ServiceRequest
                {
                    Kind = ServiceProtocol.KindCompile,
                    ClientPid = Environment.ProcessId,
                    Cwd = cwd,
                    Args = args,
                    Watch = true,
                });
                Assert.AreEqual(0, response.ExitCode, response.Message);
                this.Registered++;
            }

            public void RegisterEmit(params string[] args) => this.RegisterEmitIn(this.Project, args);

            public void RegisterEmitIn(string cwd, params string[] args)
            {
                var response = Send(this.pipeName, new ServiceRequest
                {
                    Kind = ServiceProtocol.KindEmitJs,
                    ClientPid = Environment.ProcessId,
                    Cwd = cwd,
                    Args = args,
                    Watch = true,
                });
                Assert.AreEqual(0, response.ExitCode, response.Message);
            }

            public IDictionary<string, string> Status() => Send(this.pipeName, ServiceProtocol.KindStatus, this.Project).Status;

            public string PipeName => this.pipeName;

            /// <summary>The <c>--sync</c> request for the obj DLL <paramref name="key"/>, waiting at most <paramref name="waitSeconds"/>.</summary>
            public ServiceResponse Sync(string key, string waitSeconds = "10") => this.SyncArgs(key, waitSeconds);

            /// <summary>A <c>--sync</c> request with exactly <paramref name="args"/>.</summary>
            public ServiceResponse SyncArgs(params string[] args) => Send(this.pipeName, ServiceProtocol.KindSync, this.Project, args);

            public void EditSource() => File.WriteAllText(Path.Combine(this.Project, "A.cs"), "class A { int x; }");

            public void Dispose()
            {
                this.Host.RequestStop("test");
                Assert.IsTrue(this.Serve.Join(TimeSpan.FromSeconds(10)), "The daemon did not stop.");
                Directory.Delete(this.Identity.ServiceRoot, recursive: true);
                Directory.Delete(this.Identity.ToolsetDir, recursive: true);
                Directory.Delete(this.Project, recursive: true);
                Directory.Delete(this.Dependent, recursive: true);
            }

            private static CompileInputs Inputs(string dir, string name, params string[] references)
            {
                var source = Path.Combine(dir, name + ".cs");
                return new CompileInputs(
                    Path.Combine(dir, "obj", name + ".dll"),
                    null,
                    null,
                    new[] { source },
                    Array.Empty<string>(),
                    references,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { [source] = CompileInputs.HashFile(source) });
            }
        }

        private static ServiceHost NewHost(TimeSpan idle, out string pipeName)
        {
            pipeName = "nscript-test-" + Guid.NewGuid().ToString("N");
            var identity = ServiceIdentity.FromKnown(AppContext.BaseDirectory, new string('0', 64));
            return new ServiceHost(identity, pipeName, idle, echoToConsole: false);
        }

        private static ServiceResponse Send(string pipeName, string kind, string cwd, params string[] args)
            => Send(pipeName, new ServiceRequest
            {
                Kind = kind,
                ClientPid = Environment.ProcessId,
                Cwd = cwd,
                Args = args,
            });

        private static ServiceResponse Send(string pipeName, ServiceRequest request)
        {
            using var pipe = ServiceClient.TryConnect(pipeName, 5000);
            Assert.IsNotNull(pipe, "The daemon is not listening on " + pipeName);
            ServiceProtocol.WriteMessage(pipe, request);
            var response = ServiceProtocol.ReadMessage<ServiceResponse>(pipe);
            Assert.IsNotNull(response, "The daemon hung up without replying.");
            return response;
        }

        private static string[] ValidArgs(params string[] extra)
        {
            // ParseArgs needs more than five args and existing reference files.
            string dll = typeof(BuildServiceTests).Assembly.Location;
            var args = new System.Collections.Generic.List<string>
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
            };
            args.AddRange(extra);
            return args.ToArray();
        }
    }
}
