//-----------------------------------------------------------------------
// <copyright file="CompilerLogTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils.Test
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using NScript.Lib;
    using NScript.Utils;

    /// <summary>
    /// Tests for the shared <see cref="CompilerLog"/> facility. Each test
    /// isolates itself by constructing a unique temp path and calling
    /// <see cref="CompilerLog.Shutdown"/> in cleanup so the global singleton
    /// does not leak between tests.
    /// </summary>
    [TestClass]
    public class CompilerLogTests
    {
        private string tempLogPath;

        [TestInitialize]
        public void Setup()
        {
            // Ensure no lingering state from a previous test.
            CompilerLog.Shutdown();
            this.tempLogPath = Path.Combine(
                Path.GetTempPath(),
                "nscript-compilerlog-test-" + Guid.NewGuid().ToString("N") + ".jsonl");
        }

        [TestCleanup]
        public void Cleanup()
        {
            CompilerLog.Shutdown();
            if (!string.IsNullOrEmpty(this.tempLogPath) && File.Exists(this.tempLogPath))
            {
                try { File.Delete(this.tempLogPath); } catch { /* best effort */ }
            }
        }

        [TestMethod]
        public void ForComponent_ReturnsSilentLogger_WhenNotInitialized()
        {
            Assert.IsFalse(CompilerLog.IsEnabled);

            var log = CompilerLog.ForComponent("test");
            Assert.IsNotNull(log);

            // Writing to the silent logger must not throw and must not create a file.
            log.Information("hello");

            Assert.IsFalse(File.Exists(this.tempLogPath));
        }

        [TestMethod]
        public void Initialize_WritesJsonlFile_WithExpectedFields()
        {
            CompilerLog.Initialize(this.tempLogPath, "test-stage", "run-abc");
            Assert.IsTrue(CompilerLog.IsEnabled);
            Assert.AreEqual("test-stage", CompilerLog.Stage);
            Assert.AreEqual("run-abc", CompilerLog.RunId);

            CompilerLog.ForComponent("TestComponent").Information("event {X}", 42);
            CompilerLog.Shutdown();

            Assert.IsFalse(CompilerLog.IsEnabled);
            Assert.IsTrue(File.Exists(this.tempLogPath), "Log file should exist after Initialize + write.");

            var content = File.ReadAllText(this.tempLogPath);
            Assert.IsTrue(content.Length > 0, "Log file should not be empty.");

            var line = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            var obj = JObject.Parse(line);

            Assert.IsNotNull(obj["@t"], "missing @t");
            Assert.IsNotNull(obj["@l"] ?? obj["@mt"], "missing @l or @mt");
            Assert.AreEqual("TestComponent", (string)obj["Component"]);
            Assert.AreEqual("test-stage", (string)obj["Stage"]);
            Assert.AreEqual("run-abc", (string)obj["RunId"]);
            Assert.IsNotNull(obj["Pid"]);
            Assert.IsNotNull(obj["MachineName"]);
        }

        [TestMethod]
        public void Initialize_WithNullPath_KeepsLoggingDisabled()
        {
            // Snapshot & clear env var in case it leaks from the host environment.
            var originalPathEnv = Environment.GetEnvironmentVariable(CompilerLog.LogPathEnvVar);
            Environment.SetEnvironmentVariable(CompilerLog.LogPathEnvVar, null);
            try
            {
                CompilerLog.Initialize(null, "test-stage");
                Assert.IsFalse(CompilerLog.IsEnabled);
                Assert.IsNull(CompilerLog.LogPath);
            }
            finally
            {
                Environment.SetEnvironmentVariable(CompilerLog.LogPathEnvVar, originalPathEnv);
            }
        }

        [TestMethod]
        public void Initialize_IsIdempotent()
        {
            CompilerLog.Initialize(this.tempLogPath, "stage1", "first-run");
            var firstRunId = CompilerLog.RunId;
            Assert.AreEqual("first-run", firstRunId);

            // Second call with different values must be a no-op.
            var secondPath = this.tempLogPath + ".second";
            CompilerLog.Initialize(secondPath, "stage2", "second-run");

            Assert.AreEqual("first-run", CompilerLog.RunId);
            Assert.AreEqual("stage1", CompilerLog.Stage);
            Assert.AreEqual(this.tempLogPath, CompilerLog.LogPath);
            Assert.IsFalse(File.Exists(secondPath), "Second path must not be created.");
        }

        [TestMethod]
        public void ResolveRunId_PrefersExplicit_OverEnvAndGenerated()
        {
            var originalEnv = Environment.GetEnvironmentVariable(CompilerLog.RunIdEnvVar);
            try
            {
                Environment.SetEnvironmentVariable(CompilerLog.RunIdEnvVar, "env-run");

                var explicitResult = CompilerLog.ResolveRunId("explicit-run");
                Assert.AreEqual("explicit-run", explicitResult);

                var envResult = CompilerLog.ResolveRunId(null);
                Assert.AreEqual("env-run", envResult);

                Environment.SetEnvironmentVariable(CompilerLog.RunIdEnvVar, null);
                var generated = CompilerLog.ResolveRunId(null);
                Assert.IsFalse(string.IsNullOrWhiteSpace(generated));
                Assert.AreNotEqual("explicit-run", generated);
                Assert.AreNotEqual("env-run", generated);
            }
            finally
            {
                Environment.SetEnvironmentVariable(CompilerLog.RunIdEnvVar, originalEnv);
            }
        }

        [TestMethod]
        public void ResolveLogPath_PrefersExplicit_OverEnv()
        {
            var originalEnv = Environment.GetEnvironmentVariable(CompilerLog.LogPathEnvVar);
            try
            {
                Environment.SetEnvironmentVariable(CompilerLog.LogPathEnvVar, @"C:\from-env.jsonl");

                Assert.AreEqual(@"C:\from-arg.jsonl", CompilerLog.ResolveLogPath(@"C:\from-arg.jsonl"));
                Assert.AreEqual(@"C:\from-env.jsonl", CompilerLog.ResolveLogPath(null));

                Environment.SetEnvironmentVariable(CompilerLog.LogPathEnvVar, null);
                Assert.IsNull(CompilerLog.ResolveLogPath(null));
            }
            finally
            {
                Environment.SetEnvironmentVariable(CompilerLog.LogPathEnvVar, originalEnv);
            }
        }

        [TestMethod]
        public void Initialize_CreatesMissingDirectory()
        {
            var nestedDir = Path.Combine(Path.GetTempPath(), "nscript-log-dir-" + Guid.NewGuid().ToString("N"));
            var nestedPath = Path.Combine(nestedDir, "nested.jsonl");
            try
            {
                Assert.IsFalse(Directory.Exists(nestedDir));
                CompilerLog.Initialize(nestedPath, "dir-stage");
                Assert.IsTrue(CompilerLog.IsEnabled);
                Assert.IsTrue(Directory.Exists(nestedDir));
                CompilerLog.Shutdown();
            }
            finally
            {
                try
                {
                    if (File.Exists(nestedPath)) File.Delete(nestedPath);
                    if (Directory.Exists(nestedDir)) Directory.Delete(nestedDir, recursive: true);
                }
                catch { /* best effort */ }
            }
        }

        [TestMethod]
        public void Shutdown_FlushesAndReleasesHandle()
        {
            CompilerLog.Initialize(this.tempLogPath, "flush-stage");
            CompilerLog.ForComponent("flush-test").Information("will-be-flushed");
            CompilerLog.Shutdown();

            // If the handle were still open we would get an IOException here.
            using var fs = File.Open(this.tempLogPath, FileMode.Open, FileAccess.Read, FileShare.None);
            var len = fs.Length;
            Assert.IsTrue(len > 0);
        }
    }

    /// <summary>
    /// Tests for the CLI flag stripping used by the Stage-1 entry point.
    /// </summary>
    [TestClass]
    public class CscFlagExtractionTests
    {
        [TestMethod]
        public void ExtractNScriptFlags_SpaceDelimited_RemovesFlagsAndValues()
        {
            var args = new[] { "/reference:foo.dll", "--log", "log.jsonl", "--run-id", "run-123", "File.cs" };
            var filtered = NScript.Csc.Lib.CscCompiler.ExtractNScriptFlags(args, out var logPath, out var runId);
            Assert.AreEqual("log.jsonl", logPath);
            Assert.AreEqual("run-123", runId);
            CollectionAssert.AreEqual(new[] { "/reference:foo.dll", "File.cs" }, filtered);
        }

        [TestMethod]
        public void ExtractNScriptFlags_ColonDelimited_RemovesFlag()
        {
            var args = new[] { "/reference:foo.dll", "--log:log.jsonl", "--run-id=run-42", "File.cs" };
            var filtered = NScript.Csc.Lib.CscCompiler.ExtractNScriptFlags(args, out var logPath, out var runId);
            Assert.AreEqual("log.jsonl", logPath);
            Assert.AreEqual("run-42", runId);
            CollectionAssert.AreEqual(new[] { "/reference:foo.dll", "File.cs" }, filtered);
        }

        [TestMethod]
        public void ExtractNScriptFlags_NoFlags_ReturnsArgsUnchanged()
        {
            var args = new[] { "/reference:foo.dll", "File.cs" };
            var filtered = NScript.Csc.Lib.CscCompiler.ExtractNScriptFlags(args, out var logPath, out var runId);
            Assert.IsNull(logPath);
            Assert.IsNull(runId);
            CollectionAssert.AreEqual(args, filtered);
        }

        [TestMethod]
        public void ExtractNScriptFlags_EmptyArgs_ReturnsEmpty()
        {
            var filtered = NScript.Csc.Lib.CscCompiler.ExtractNScriptFlags(null, out var logPath, out var runId);
            Assert.IsNull(logPath);
            Assert.IsNull(runId);
            Assert.AreEqual(0, filtered.Length);
        }
    }

    /// <summary>
    /// Tests for Stage-2 (<c>ParseOptions.ParseArgs</c>) log/runid flag parsing.
    /// Uses temp files to satisfy <c>ParseOptions</c> file-existence validation.
    /// </summary>
    [TestClass]
    public class ParseOptionsLogFlagTests
    {
        private string tempRefDll;

        [TestInitialize]
        public void Setup()
        {
            // Reset Logger so HasErrors doesn't leak between tests.
            Logger.Instance = new Logger();
            this.tempRefDll = Path.GetTempFileName();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (!string.IsNullOrEmpty(this.tempRefDll) && File.Exists(this.tempRefDll))
            {
                try { File.Delete(this.tempRefDll); } catch { /* best effort */ }
            }
        }

        private string[] BaseArgs => new[]
        {
            "-outJs", "out.js",
            "-entryAssembly", "fake.dll",
            "-references", this.tempRefDll,
        };

        private string[] ArgsWithSuffix(params string[] suffix)
        {
            var list = new System.Collections.Generic.List<string>(this.BaseArgs);
            list.AddRange(suffix);
            return list.ToArray();
        }

        [TestMethod]
        public void ParseArgs_SpaceDelimited_LogAndRunId()
        {
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("--log", "build.jsonl", "--run-id", "run-42"));
            Assert.IsNotNull(options, "ParseArgs should succeed");
            Assert.AreEqual("build.jsonl", options.LogPath);
            Assert.AreEqual("run-42", options.RunId);
        }

        [TestMethod]
        public void ParseArgs_ColonDelimited_LogAndRunId()
        {
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("--log:build.jsonl", "-runid:run-99"));
            Assert.IsNotNull(options, "ParseArgs should succeed");
            Assert.AreEqual("build.jsonl", options.LogPath);
            Assert.AreEqual("run-99", options.RunId);
        }

        [TestMethod]
        public void ParseArgs_EqualsDelimited_LogAndRunId()
        {
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("-log=my.jsonl", "--run-id=abc"));
            Assert.IsNotNull(options, "ParseArgs should succeed");
            Assert.AreEqual("my.jsonl", options.LogPath);
            Assert.AreEqual("abc", options.RunId);
        }

        [TestMethod]
        public void ParseArgs_SlashForm_LogAndRunId()
        {
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("/log", "slash.jsonl", "/runid", "slash-run"));
            Assert.IsNotNull(options, "ParseArgs should succeed");
            Assert.AreEqual("slash.jsonl", options.LogPath);
            Assert.AreEqual("slash-run", options.RunId);
        }

        [TestMethod]
        public void ParseArgs_LogAsLastArg_GracefulNoAbort()
        {
            // --log with no value should not abort; logPath stays null.
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("--log"));
            Assert.IsNotNull(options, "ParseArgs should not abort when --log has no value");
            Assert.IsNull(options.LogPath);
        }

        [TestMethod]
        public void ParseArgs_RunIdAsLastArg_GracefulNoAbort()
        {
            var options = ParseOptions.ParseArgs(
                ArgsWithSuffix("--run-id"));
            Assert.IsNotNull(options, "ParseArgs should not abort when --run-id has no value");
            Assert.IsNull(options.RunId);
        }

        [TestMethod]
        public void ParseArgs_NoLogFlags_PathAndRunIdAreNull()
        {
            var options = ParseOptions.ParseArgs(this.BaseArgs);
            Assert.IsNotNull(options, "ParseArgs should succeed");
            Assert.IsNull(options.LogPath);
            Assert.IsNull(options.RunId);
        }
    }
}
