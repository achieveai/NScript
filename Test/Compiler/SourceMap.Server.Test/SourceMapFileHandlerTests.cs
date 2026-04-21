// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
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
        private string sourceFilePath;

        [TestInitialize]
        public void Init()
        {
            this.workDir = Path.Combine(Path.GetTempPath(), "SourceMapServerTests_" + System.Guid.NewGuid().ToString("N"));
            this.mapsDir = Path.Combine(this.workDir, "maps");
            string sourcesDir = Path.Combine(this.workDir, "src");
            Directory.CreateDirectory(this.mapsDir);
            Directory.CreateDirectory(sourcesDir);

            this.sourceFilePath = Path.Combine(sourcesDir, "Program.cs");
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
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

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
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "NotListed.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_MapFileMissing_Returns404()
        {
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "nonexistent", "Program.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_LongPathDoesNotExistOnDisk_Returns404()
        {
            string missingLong = Path.Combine(this.workDir, "src", "Gone.cs").Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Gone.cs", missingLong);
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, "app", "C$/sources/Gone.cs", options);

            Assert.AreEqual((int)HttpStatusCode.NotFound, ctx.Response.StatusCode);
        }

        [TestMethod]
        [DataRow("../../../../etc/passwd")]
        [DataRow("..")]
        [DataRow("foo/../bar")]
        public async Task HandleAsync_MapNameWithTraversal_Returns400(string mapName)
        {
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

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
            var options = new SourceMapFileHandlerOptions { MapsDirectory = this.mapsDir };

            HttpContext ctx = BuildContext();
            await SourceMapFileHandler.HandleAsync(ctx, mapName, sourceName, options);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, ctx.Response.StatusCode);
        }

        [TestMethod]
        public async Task HandleAsync_ServeFromSourcesLongFalse_Returns404EvenOnHit()
        {
            string escapedLong = this.sourceFilePath.Replace("\\", "\\\\");
            WriteMap("app", "C$/sources/Program.cs", escapedLong);
            var options = new SourceMapFileHandlerOptions
            {
                MapsDirectory = this.mapsDir,
                ServeFromSourcesLong = false,
            };

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
