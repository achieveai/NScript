// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="SourceMapFileHandler.HandleAsync"/>. Drives the handler
    /// directly against a <see cref="DefaultHttpContext"/> so the tests do not depend on
    /// Kestrel/WebApplicationFactory — keeps the suite in-process and deterministic.
    /// </summary>
    [TestClass]
    public class SourceMapFileHandlerTests
    {
        private string workDir;
        private string mapsDir;
        private string sourcesDir;
        private string sourceFilePath;

        [TestInitialize]
        public void Init()
        {
            this.workDir = Path.Combine(Path.GetTempPath(), "SourceMapServerTests_" + System.Guid.NewGuid().ToString("N"));
            this.mapsDir = Path.Combine(this.workDir, "maps");
            this.sourcesDir = Path.Combine(this.workDir, "src");
            Directory.CreateDirectory(this.mapsDir);
            Directory.CreateDirectory(this.sourcesDir);

            this.sourceFilePath = Path.Combine(this.sourcesDir, "Program.cs");
            File.WriteAllText(this.sourceFilePath, "// body of Program.cs\nclass Foo {}\n");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.workDir))
            {
                Directory.Delete(this.workDir, recursive: true);
            }
        }

        [TestMethod]
        public async Task HandleAsync_HitOnShortName_StreamsFileContent()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Program.cs", options);

            Assert.AreEqual((int)HttpStatusCode.OK, ctx.Response.StatusCode);
            Assert.AreEqual("text/plain; charset=utf-8", ctx.Response.ContentType);
            string body = ReadBody(ctx);
            StringAssert.Contains(body, "class Foo");
        }

        [TestMethod]
        public async Task HandleAsync_ShortNameNotInMap_Returns404()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "NotListed.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_MapFileMissing_Returns404()
        {
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "nonexistent", "Program.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_LongPathDoesNotExistOnDisk_Returns404()
        {
            string missingLong = Path.Combine(this.sourcesDir, "Gone.cs").Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Gone.cs", missingLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Gone.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        [DataRow("../../../../etc/passwd")]
        [DataRow("..")]
        [DataRow("foo/../bar")]
        [DataRow("foo/bar")]
        [DataRow("a b")]
        [DataRow("a:b")]
        public async Task HandleAsync_MapNameWithTraversal_Returns400(string mapName)
        {
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, mapName, "Program.cs", options);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, ctx.Response.StatusCode);
        }

        [TestMethod]
        [DataRow("", "Program.cs")]
        [DataRow("app", "")]
        [DataRow(null, "Program.cs")]
        [DataRow("app", null)]
        public async Task HandleAsync_EmptySegment_Returns400(string mapName, string sourceName)
        {
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, mapName, sourceName, options);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_ServeFromSourcesLongFalse_Returns404EvenOnHit()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.ServeFromSourcesLong = false;

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Program.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Opting out of sourcesLong must produce 404 instead of leaking local file contents");
        }

        [TestMethod]
        public async Task HandleAsync_MapsDirectoryMissing_Returns404()
        {
            var options = new SourceMapFileHandlerOptions
            {
                MapsDirectory = Path.Combine(this.workDir, "does-not-exist"),
            };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "Program.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_LongPathOutsideAllowedRoot_Returns404()
        {
            // Source sits in workDir/rogue, but allow-list only permits workDir/src. Even though
            // the file exists on disk and the short name matches, containment must fail closed.
            string rogueDir = Path.Combine(this.workDir, "rogue");
            Directory.CreateDirectory(rogueDir);
            string roguePath = Path.Combine(rogueDir, "Secret.cs");
            File.WriteAllText(roguePath, "secret contents");

            string escapedLong = roguePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/x/Secret.cs", escapedLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/x/Secret.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Allow-list containment must refuse files outside AllowedSourceRoots even when sourcesLong points at them");
            string body = ReadBody(ctx);
            Assert.IsFalse(body.Contains("secret contents"), "Body must not leak file content on containment failure");
        }

        [TestMethod]
        public async Task HandleAsync_AllowedRootsUnsetFallsBackToMapsDir_Returns404ForOutsideFiles()
        {
            // No AllowedSourceRoots configured — the handler must fail closed by using MapsDirectory
            // as the sole allowed root. The source file lives under workDir/src, NOT under MapsDir.
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);
            var options = new SourceMapFileHandlerOptions
            {
                MapsDirectory = this.mapsDir,
            };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Program.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "When AllowedSourceRoots is unset the handler must default to maps-dir containment and refuse outside paths");
        }

        [TestMethod]
        public async Task HandleAsync_LongPathInSiblingPrefixDirectory_Returns404()
        {
            // Classic string-prefix containment pitfall: allow-list is <work>/src, attacker aims
            // at <work>/src-evil/secret.cs. Without the trailing-separator normalization the naive
            // StartsWith("<work>/src") would return true and leak the file.
            string siblingDir = Path.Combine(this.workDir, "src-evil");
            Directory.CreateDirectory(siblingDir);
            string rogue = Path.Combine(siblingDir, "Secret.cs");
            File.WriteAllText(rogue, "sibling-evil");

            string escapedLong = rogue.Replace("\\", "\\\\");
            WriteMap("app", "C$/x/Secret.cs", escapedLong);
            var options = BuildOptions(allowedRoot: this.sourcesDir);

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/x/Secret.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Sibling directory whose name shares the allowed root as a prefix must not match the allow-list");
            Assert.IsFalse(ReadBody(ctx).Contains("sibling-evil"));
        }

        [TestMethod]
        public async Task HandleAsync_MapFileExceedsSizeCap_Returns404()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);

            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.MaxMapFileSizeBytes = 10; // much smaller than any real map

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Program.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Maps larger than MaxMapFileSizeBytes must be rejected instead of being fully loaded");
        }

        [TestMethod]
        public async Task HandleAsync_SourceFileExceedsSizeCap_Returns404()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);

            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.MaxSourceFileSizeBytes = 5; // source file is ~35 bytes

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Program.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Sources larger than MaxSourceFileSizeBytes must be refused instead of streamed");
        }

        [TestMethod]
        public async Task HandleAsync_RedirectEnabled_HttpsSourceRoot_LocalFileMissing_Returns302()
        {
            // Map points at a non-existent file, but `sources[]` contains the short name and
            // `sourceRoot` is an https URL → handler should 302 to {sourceRoot}{sourceName}.
            string missingLong = Path.Combine(this.sourcesDir, "Gone.cs").Replace("\\", "\\\\");
            WriteMapWithSourceRoot(
                "app",
                "Sources/Gone.cs",
                missingLong,
                "https://raw.githubusercontent.com/owner/repo/sha/");
            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.RepoUrlRedirectOnMiss = true;

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "Sources/Gone.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.Found,
                ctx.Response.StatusCode,
                "Opt-in redirect must turn local-miss into 302 when sourceRoot is an http(s) URL");
            Assert.AreEqual(
                "https://raw.githubusercontent.com/owner/repo/sha/Sources/Gone.cs",
                ctx.Response.Headers["Location"].ToString());
        }

        [TestMethod]
        public async Task HandleAsync_RedirectEnabled_RelativeSourceRoot_Returns404()
        {
            // sourceRoot is a relative path / legacy ashx — must NOT redirect; only http(s) URLs
            // are accepted as redirect targets, otherwise a tampered map could send the browser
            // anywhere (javascript:, data:, file://, …).
            string missingLong = Path.Combine(this.sourcesDir, "Gone.cs").Replace("\\", "\\\\");
            WriteMapWithSourceRoot(
                "app",
                "Sources/Gone.cs",
                missingLong,
                "/legacy/SrcMapper.ashx?map=app&file=");
            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.RepoUrlRedirectOnMiss = true;

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "Sources/Gone.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Non-http(s) sourceRoot must fall through to 404 even when redirect is enabled");
            Assert.IsFalse(
                ctx.Response.Headers.ContainsKey("Location"),
                "No Location header should be set when refusing to redirect");
        }

        [TestMethod]
        public async Task HandleAsync_RedirectDisabled_HttpsSourceRoot_Returns404()
        {
            // Default behaviour preserved: even with an http(s) sourceRoot, the absence of the
            // opt-in flag must keep the response a plain 404.
            string missingLong = Path.Combine(this.sourcesDir, "Gone.cs").Replace("\\", "\\\\");
            WriteMapWithSourceRoot(
                "app",
                "Sources/Gone.cs",
                missingLong,
                "https://raw.githubusercontent.com/owner/repo/sha/");
            var options = BuildOptions(allowedRoot: this.sourcesDir);
            // RepoUrlRedirectOnMiss left at its default (false)

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "Sources/Gone.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "RepoUrlRedirectOnMiss=false must preserve the legacy 404 behaviour");
            Assert.IsFalse(
                ctx.Response.Headers.ContainsKey("Location"),
                "Default-disabled redirect must not emit a Location header");
        }

        [TestMethod]
        public async Task HandleAsync_RedirectEnabled_ShortNameNotInMap_Returns404()
        {
            // Even with redirect enabled, a short name that doesn't appear in `sources[]` must
            // NOT trigger a 302 — that would let an attacker dangle arbitrary paths after the
            // prefix and cause the browser to be redirected to attacker-chosen URLs.
            string missingLong = Path.Combine(this.sourcesDir, "Gone.cs").Replace("\\", "\\\\");
            WriteMapWithSourceRoot(
                "app",
                "Sources/Gone.cs",
                missingLong,
                "https://raw.githubusercontent.com/owner/repo/sha/");
            var options = BuildOptions(allowedRoot: this.sourcesDir);
            options.RepoUrlRedirectOnMiss = true;

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "NotInSources.cs", options);

            Assert.AreEqual(
                (int)HttpStatusCode.NotFound,
                ctx.Response.StatusCode,
                "Short name absent from sources[] must produce 404 even with redirect enabled");
        }

        // ---------------------------------------------------------------------------------
        // TryBuildRepoRedirect — direct unit tests.
        //
        // The redirect helper is the security-critical branch (a tampered sourceRoot could
        // send the browser to an attacker-controlled URL). The black-box HandleAsync tests
        // above cover happy / sad paths, but the hostile-scheme rejection (javascript:,
        // data:, file://) needs explicit assertions. Drive the helper directly so failures
        // there can never be masked by an unrelated 404 path higher up in HandleAsync.
        // ---------------------------------------------------------------------------------

        [TestMethod]
        public void TryBuildRepoRedirect_OptedOut_ReturnsFalse()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = false };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Foo.cs", options, out string url);

            Assert.IsFalse(result, "Default-disabled redirect must always return false");
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_ValidGitHubSourceRoot_BuildsHttpsUrl()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Sources/Foo.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Sources/Foo.cs", options, out string url);

            Assert.IsTrue(result);
            Assert.AreEqual("https://raw.githubusercontent.com/o/r/sha/Sources/Foo.cs", url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_ValidAdoSourceRoot_BuildsItemsApiUrl()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Sources/Foo.cs\"],"
                + "\"sourceRoot\":\"https://dev.azure.com/org/proj/_apis/git/repositories/repo/items?api-version=7.1&versionDescriptor.version=sha&versionDescriptor.versionType=commit&path=/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Sources/Foo.cs", options, out string url);

            Assert.IsTrue(result);
            StringAssert.EndsWith(url, "path=/Sources/Foo.cs");
        }

        [TestMethod]
        [DataRow("javascript:alert(1)/")]
        [DataRow("data:text/html,foo")]
        [DataRow("file:///etc/passwd/")]
        [DataRow("vbscript:msgbox(1)/")]
        [DataRow("ftp://example.com/")]
        [DataRow("/legacy/SrcMapper.ashx?file=")]
        [DataRow("relative/path/")]
        public void TryBuildRepoRedirect_HostileOrNonHttpsSourceRoot_ReturnsFalse(string hostileRoot)
        {
            string json = "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"" + hostileRoot + "\"}";
            var sources = SourceMapSources.TryParseContent(json);
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Foo.cs", options, out string url);

            Assert.IsFalse(
                result,
                "Hostile / non-https sourceRoot must NOT produce a redirect: " + hostileRoot);
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_HttpSourceRoot_ReturnsFalse()
        {
            // Plain http:// is rejected — both GitHub raw and the ADO Items API are HTTPS-only,
            // and we never want to silently downgrade DevTools to a cleartext fetch.
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"http://example.com/raw/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Foo.cs", options, out string url);

            Assert.IsFalse(result, "http:// sourceRoot must be refused — only https:// is acceptable");
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_NullSources_ReturnsFalse()
        {
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(null, "Foo.cs", options, out string url);

            Assert.IsFalse(result);
            Assert.IsNull(url);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void TryBuildRepoRedirect_NullOrEmptySourceName_ReturnsFalse(string sourceName)
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, sourceName, options, out string url);

            Assert.IsFalse(result);
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_EmptySourceRoot_ReturnsFalse()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Foo.cs", options, out string url);

            Assert.IsFalse(result);
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_SourceNameNotInSources_ReturnsFalse()
        {
            // Membership gate: even if sourceRoot is a valid https URL, redirecting an arbitrary
            // dangling path lets an attacker turn the handler into an open redirect.
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "AttackerControlled.cs", options, out string url);

            Assert.IsFalse(
                result,
                "Membership gate: short names absent from sources[] must never be redirected");
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_NullOptions_ReturnsFalse()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"Foo.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "Foo.cs", null, out string url);

            Assert.IsFalse(result);
            Assert.IsNull(url);
        }

        [TestMethod]
        public void TryBuildRepoRedirect_SourceNameContainsHash_EncodesFragmentBoundary()
        {
            // A `#` inside a file path would truncate the ADO `path=/` query at the fragment
            // boundary (everything after `#` is fragment, not path). URI-encoding the path
            // component prevents that — `#` becomes `%23` and the redirect lands intact.
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"foo#bar.cs\"],"
                + "\"sourceRoot\":\"https://dev.azure.com/o/p/_apis/git/repositories/r/items?api-version=7.1&versionDescriptor.version=sha&versionDescriptor.versionType=commit&path=/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "foo#bar.cs", options, out string url);

            Assert.IsTrue(result);
            Assert.IsFalse(
                url.Contains("#"),
                "URL must not contain a literal '#' after encoding — found: " + url);
            StringAssert.Contains(url, "foo%23bar.cs");
        }

        [TestMethod]
        public void TryBuildRepoRedirect_SourceNameContainsSpace_EncodesSpace()
        {
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"my file.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "my file.cs", options, out string url);

            Assert.IsTrue(result);
            StringAssert.Contains(url, "my%20file.cs");
        }

        [TestMethod]
        public void TryBuildRepoRedirect_SourceNamePreservesForwardSlashes()
        {
            // `/` is a path separator at both GitHub raw and the ADO Items API — encoding it
            // to `%2F` would break the lookup. Only segment-internal characters get encoded.
            var sources = SourceMapSources.TryParseContent(
                "{\"sources\":[\"a/b/c.cs\"],\"sourceRoot\":\"https://raw.githubusercontent.com/o/r/sha/\"}");
            var options = new SourceMapFileHandlerOptions { RepoUrlRedirectOnMiss = true };

            bool result = SourceMapFileHandler.TryBuildRepoRedirect(sources, "a/b/c.cs", options, out string url);

            Assert.IsTrue(result);
            Assert.AreEqual("https://raw.githubusercontent.com/o/r/sha/a/b/c.cs", url);
        }

        private SourceMapFileHandlerOptions BuildOptions(string allowedRoot)
        {
            return new SourceMapFileHandlerOptions
            {
                MapsDirectory = this.mapsDir,
                AllowedSourceRoots = new List<string> { allowedRoot },
            };
        }

        private static HttpContext BuildContext()
        {
            var ctx = new DefaultHttpContext();
            ctx.Response.Body = new MemoryStream();
            return ctx;
        }

        private static string ReadBody(HttpContext ctx)
        {
            ctx.Response.Body.Position = 0;
            using var reader = new StreamReader(ctx.Response.Body, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private void WriteMap(string mapName, string shortSource, string longSourceEscaped)
        {
            string json = "{"
                + "\"version\":\"3\","
                + "\"file\":\"" + mapName + ".js\","
                + "\"sources\":[\"" + shortSource + "\"],"
                + "\"sourcesLong\":[\"" + longSourceEscaped + "\"],"
                + "\"mappings\":\"\""
                + "}";
            File.WriteAllText(Path.Combine(this.mapsDir, mapName + ".map"), json);
        }

        private void WriteMapWithSourceRoot(
            string mapName,
            string shortSource,
            string longSourceEscaped,
            string sourceRoot)
        {
            string json = "{"
                + "\"version\":\"3\","
                + "\"file\":\"" + mapName + ".js\","
                + "\"sourceRoot\":\"" + sourceRoot + "\","
                + "\"sources\":[\"" + shortSource + "\"],"
                + "\"sourcesLong\":[\"" + longSourceEscaped + "\"],"
                + "\"mappings\":\"\""
                + "}";
            File.WriteAllText(Path.Combine(this.mapsDir, mapName + ".map"), json);
        }
    }
}
