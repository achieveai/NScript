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

        /// <summary>
        /// A daemon watching two projects whose compiles are faked by <c>run</c>: A.cs in
        /// <see cref="Project"/>, and B.cs in <see cref="Dependent"/>, which references A's
        /// output. Each faked compile reports its folder's .cs file as its only input.
        /// </summary>
        private sealed class WatchHost : IDisposable
        {
            private readonly string pipeName = "nscript-test-" + Guid.NewGuid().ToString("N");

            public WatchHost(Func<ServiceRequest, ServiceResponse> run, string toolsetHash = null, TimeSpan? registrationQuiet = null)
            {
                this.Identity = ServiceIdentity.FromKnown(NewTempDir(), new string('0', 64));
                this.Project = NewTempDir();
                this.Dependent = NewTempDir();
                File.WriteAllText(Path.Combine(this.Project, "A.cs"), "class A { }");
                File.WriteAllText(Path.Combine(this.Dependent, "B.cs"), "class B : A { }");
                var options = new ServiceHostOptions
                {
                    ToolsetHash = () => toolsetHash ?? this.Identity.ToolsetHash,
                    RunRequest = run,
                    RunRequestInputs = request => string.Equals(request.Cwd, this.Dependent, StringComparison.OrdinalIgnoreCase)
                        ? Inputs(this.Dependent, "B", Path.Combine(this.Project, "obj", "A.dll"))
                        : Inputs(this.Project, "A"),
                    RunRequestEmitOptions = request => ParseOptions.ParseArgs(request.Args),
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

            public void RegisterEmit(params string[] args)
            {
                var response = Send(this.pipeName, new ServiceRequest
                {
                    Kind = ServiceProtocol.KindEmitJs,
                    ClientPid = Environment.ProcessId,
                    Cwd = this.Project,
                    Args = args,
                    Watch = true,
                });
                Assert.AreEqual(0, response.ExitCode, response.Message);
            }

            public IDictionary<string, string> Status() => Send(this.pipeName, ServiceProtocol.KindStatus, this.Project).Status;

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
