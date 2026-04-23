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
    }
}
