//-----------------------------------------------------------------------
// <copyright file="ParseOptionsSecondaryRepoTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Utils.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Lib;
    using NScript.Utils;

    /// <summary>
    /// Validation tests for the issue #85 secondary-repo command-line flags
    /// (<c>-secondarySourceRoot</c>, <c>-secondaryRepoRoot</c>) parsed by
    /// <see cref="ParseOptions.ParseArgs"/>. The shared <see cref="Logger"/> singleton is
    /// reset in <see cref="Setup"/> so a previous test's error state cannot pollute
    /// subsequent assertions (HasErrors is what determines ParseArgs' null return).
    /// </summary>
    [TestClass]
    public class ParseOptionsSecondaryRepoTests
    {
        [TestInitialize]
        public void Setup()
        {
            // ParseArgs returns null when Logger.Instance.HasErrors. The default Logger
            // implementation accumulates errors across tests; swap in a fresh instance so
            // each test starts from a clean state.
            Logger.Instance = new Logger();
        }

        /// <summary>
        /// Returns the path to a DLL that exists on disk so the <c>-references</c> argument
        /// passes the file-existence check inside <see cref="ParseOptions.ParseArgs"/>.
        /// Using the running test assembly's own location keeps the test self-contained and
        /// works on every platform.
        /// </summary>
        private static string ExistingDllPath
            => typeof(ParseOptionsSecondaryRepoTests).Assembly.Location;

        /// <summary>
        /// Sanity check: the four-arg "valid baseline" that the failure tests below extend
        /// with bad secondary flags must, by itself, parse successfully. Without this
        /// assertion the failure tests could pass for the wrong reason (e.g. missing
        /// references file rather than the new validation).
        /// </summary>
        [TestMethod]
        public void CommandLine_ValidBaseline_ParsesSuccessfully()
        {
            string dll = ExistingDllPath;
            var result = ParseOptions.ParseArgs(new[]
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
                "-sourceMapRoot", "https://primary.example/",
            });

            Assert.IsNotNull(result, "Baseline args must parse successfully — failure tests rely on this.");
        }

        /// <summary>
        /// -secondarySourceRoot is only meaningful when paired with the primary
        /// -sourceMapRoot — without it, the secondary URL would side-step a sourceRoot that
        /// doesn't exist, producing maps whose secondary sources resolve fine but whose
        /// primary sources are stuck in the legacy <c>.ashx</c> fallback. ParseArgs must
        /// reject the partial configuration loudly (return null).
        /// </summary>
        [TestMethod]
        public void CommandLine_SecondarySourceRoot_RequiresPrimarySourceMapRoot()
        {
            string dll = ExistingDllPath;
            var result = ParseOptions.ParseArgs(new[]
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
                "-secondarySourceRoot", "https://example.com/",
            });

            Assert.IsNull(result, "ParseArgs must return null when -secondarySourceRoot is supplied without -sourceMapRoot");
        }

        /// <summary>
        /// -secondarySourceRoot must be an https:// URL — the whole point is to embed an
        /// absolute URL in <c>sources[]</c> so DevTools fetches it directly. http://, file://
        /// or any other scheme would either fail to load in the browser or defeat the
        /// bypass-sourceRoot mechanism.
        /// </summary>
        [TestMethod]
        public void CommandLine_SecondarySourceRoot_MustBeHttps()
        {
            string dll = ExistingDllPath;
            var result = ParseOptions.ParseArgs(new[]
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
                "-sourceMapRoot", "https://example.com/",
                "-secondarySourceRoot", "http://example.com/",
            });

            Assert.IsNull(result, "ParseArgs must return null when -secondarySourceRoot is not https://");
        }

        /// <summary>
        /// Companion check to the partial-configuration validation: <c>-secondaryRepoRoot</c>
        /// without <c>-secondarySourceRoot</c> has nothing to absolutize against, so the
        /// command line is rejected.
        /// </summary>
        [TestMethod]
        public void CommandLine_SecondaryRepoRoot_RequiresSecondarySourceRoot()
        {
            string dll = ExistingDllPath;
            var result = ParseOptions.ParseArgs(new[]
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
                "-sourceMapRoot", "https://example.com/",
                "-secondaryRepoRoot", System.IO.Path.GetTempPath(),
            });

            Assert.IsNull(result, "ParseArgs must return null when -secondaryRepoRoot is supplied without -secondarySourceRoot");
        }

        /// <summary>
        /// Happy path: when all secondary flags are supplied alongside the primary
        /// <c>-sourceMapRoot</c>, ParseArgs must succeed and surface both values on the
        /// returned <see cref="ParseOptions"/> instance.
        /// </summary>
        [TestMethod]
        public void CommandLine_SecondaryFlags_FullyConfigured_PropagatesValues()
        {
            string dll = ExistingDllPath;
            var result = ParseOptions.ParseArgs(new[]
            {
                "-outJs", "app.js",
                "-entryAssembly", dll,
                "-references", dll,
                "-sourceMapRoot", "https://primary.example/",
                "-secondarySourceRoot", "https://secondary.example/lib/",
                "-secondaryRepoRoot", System.IO.Path.GetTempPath(),
            });

            Assert.IsNotNull(result, "ParseArgs must succeed when secondary flags are fully configured");
            Assert.AreEqual("https://secondary.example/lib/", result.SecondarySourceRoot);
            Assert.AreEqual(System.IO.Path.GetTempPath(), result.SecondaryRepoRoot);
        }
    }
}
