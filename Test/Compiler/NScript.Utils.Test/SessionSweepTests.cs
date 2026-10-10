namespace NScript.Utils.Test
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Converter;
    using NScript.Lib;
    using NScript.Lib.Service;
    using NScript.Utils;

    /// <summary>
    /// Inc 5: the daemon releases build sessions not built for a while (a bundle watched all
    /// day but not edited), by NSCRIPT_SESSION_IDLE_SECONDS (default 30 min, 0 = off).
    /// </summary>
    [TestClass]
    public class SessionSweepTests
    {
        private static readonly string Reference = typeof(SessionSweepTests).Assembly.Location;

        private static long Minutes(int minutes) => (long)TimeSpan.FromMinutes(minutes).TotalMilliseconds;

        [TestMethod]
        public void DropIdle_ReleasesOnlySessionsUnusedForTheTimeout()
        {
            WithFakeClock((dir, setNow) =>
            {
                Assert.IsFalse(Build(dir, "a"));
                setNow(Minutes(20));
                Assert.IsFalse(Build(dir, "b"));
                Assert.AreEqual(2, BuilderSessions.Count, "A failed build keeps its entry (the builder dropped its modules).");

                setNow(Minutes(29));
                Assert.AreEqual(0, BuilderSessions.DropIdle(TimeSpan.FromMinutes(30)));

                setNow(Minutes(31));
                Assert.AreEqual(1, BuilderSessions.DropIdle(TimeSpan.FromMinutes(30)), "Only a (unused 31 min); b was used 11 min ago.");
                Assert.AreEqual(1, BuilderSessions.Count);

                setNow(Minutes(50));
                Assert.AreEqual(1, BuilderSessions.DropIdle(TimeSpan.FromMinutes(30)));
                Assert.AreEqual(0, BuilderSessions.Count);
            });
        }

        [TestMethod]
        public void LastUsed_IsStampedWhenTheBuildEnds_SoALongBuildIsNotSweptRightAfter()
        {
            WithFakeClock((dir, setNow) =>
            {
                // The build reports its missing entry assembly through the logger; that moves the
                // clock 40 minutes, as if the build took that long.
                Assert.IsFalse(Build(dir, "slow", new ClockLogger(() => setNow(Minutes(40)))));
                Assert.AreEqual(0, BuilderSessions.DropIdle(TimeSpan.FromMinutes(30)), "Used at 40 min (end), not 0 (start).");
                Assert.AreEqual(1, BuilderSessions.Count);
            });
        }

        [TestMethod]
        public void Count_AnswersFromAnotherThreadWhileABuildRuns()
        {
            // --status reads Count outside the request lock; it must not wait for the build.
            WithFakeClock((dir, setNow) =>
            {
                bool answered = false;
                Assert.IsFalse(Build(dir, "busy", new ClockLogger(() =>
                {
                    var read = System.Threading.Tasks.Task.Run(() => BuilderSessions.Count);
                    answered = read.Wait(TimeSpan.FromSeconds(1));
                })));
                Assert.IsTrue(answered, "Count blocked until the build ended.");
            });
        }

        [TestMethod]
        public void SessionIdleSetting_ZeroIsOff_BadValuesWarnAndUseTheDefault()
        {
            Assert.AreEqual(TimeSpan.FromMinutes(30), ServiceHost.ParseSessionIdleTimeout(null, out var warning));
            Assert.IsNull(warning);
            Assert.AreEqual(TimeSpan.FromSeconds(600), ServiceHost.ParseSessionIdleTimeout("600", out warning));
            Assert.IsNull(warning);
            Assert.AreEqual(TimeSpan.Zero, ServiceHost.ParseSessionIdleTimeout("0", out warning), "0 = off.");
            Assert.IsNull(warning);

            foreach (var bad in new[] { "abc", "-5", "1.5", "99999999999" })
            {
                Assert.AreEqual(TimeSpan.FromMinutes(30), ServiceHost.ParseSessionIdleTimeout(bad, out warning), bad);
                StringAssert.Contains(warning, ServiceHost.SessionIdleSecondsEnvVar, bad);
                StringAssert.Contains(warning, "'" + bad + "'", bad);
            }
        }

        private static void WithFakeClock(Action<string, Action<long>> test)
        {
            // Sessions other tests left are stamped on the real clock: clear them before faking it.
            BuilderSessions.DropIdle(TimeSpan.Zero);
            var originalClock = BuilderSessions.Clock;
            long now = 0;
            BuilderSessions.Clock = () => now;
            var dir = Path.Combine(Path.GetTempPath(), "nscript-sweep-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                Assert.AreEqual(0, BuilderSessions.Count);
                test(dir, value => now = value);
            }
            finally
            {
                BuilderSessions.Clock = originalClock;
                BuilderSessions.DropIdle(TimeSpan.Zero);
                Logger.Instance = new Logger();
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// Builds an output whose entry assembly is missing: the build fails fast in VerifyPaths
        /// and the session entry stays.
        /// </summary>
        private static bool Build(string dir, string name, ILog buildLog = null)
        {
            Logger.Instance = new Logger();
            var entry = Path.Combine(dir, name + ".dll");
            var options = ParseOptions.ParseArgs(new[] { "-outJs", Path.Combine(dir, name + ".js"), "-entryAssembly", entry, "-references", Reference, "-devMode" });
            Assert.IsNotNull(options);
            Logger.Instance = buildLog ?? new Logger();
            return BuilderSessions.Execute(
                options,
                () => new Builder(options.JsFileName, 1, entry, Array.Empty<string>(), Array.Empty<IConverterPlugin>(), (false, false, false), devMode: true),
                Array.Empty<IConverterPlugin>());
        }

        private sealed class ClockLogger : ILog
        {
            private readonly Action onError;

            public ClockLogger(Action onError) => this.onError = onError;

            public bool HasErrors { get; private set; }

            public void LogError(string error)
            {
                this.HasErrors = true;
                this.onError();
            }

            public void LogError(ErrorInfo errorInfo) => this.LogError(errorInfo.ToString());

            public void LogWarning(string warning)
            {
            }

            public void LogWarning(ErrorInfo warningInfo)
            {
            }
        }
    }
}
