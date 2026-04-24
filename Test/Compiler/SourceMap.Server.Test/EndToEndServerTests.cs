// -----------------------------------------------------------------------
// <copyright file="EndToEndServerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// End-to-end tests for <see cref="SourceMapFileHandler.MapSourceMapFiles"/>
    /// driven through <see cref="WebApplicationFactory{TEntryPoint}"/>. Unlike
    /// <see cref="SourceMapFileHandlerTests"/> (which drives
    /// <see cref="SourceMapFileHandler.HandleAsync"/> against a
    /// <c>DefaultHttpContext</c>), this suite exercises the real ASP.NET Core
    /// pipeline — routing, URL decoding, response streaming — so regressions in
    /// endpoint wiring are caught alongside handler logic.
    /// </summary>
    /// <remarks>
    /// Fixtures (<c>Program.cs</c>, <c>View.xwml</c>, <c>Skin.cshtml</c>) are
    /// copied into each test's workdir so <c>sourcesLong</c> entries point at
    /// real bytes the handler can stream back. Maps are produced by the real
    /// <see cref="OwaSourceMapper.SourceMap"/> class via
    /// <see cref="FixtureMapEmitter"/>, so each scenario exercises the JSON
    /// shape the compiler actually emits.
    /// </remarks>
    [TestClass]
    public class EndToEndServerTests
    {
        private string workDir;
        private string mapsDir;
        private string sourcesDir;
        private string programCsPath;
        private string viewXwmlPath;
        private string skinCshtmlPath;
        private SourceMapServerFactory factory;
        private HttpClient client;

        [TestInitialize]
        public void Init()
        {
            this.workDir = Path.Combine(Path.GetTempPath(), "SourceMapServerE2E_" + Guid.NewGuid().ToString("N"));
            this.mapsDir = Path.Combine(this.workDir, "maps");
            this.sourcesDir = Path.Combine(this.workDir, "src");
            Directory.CreateDirectory(this.mapsDir);
            Directory.CreateDirectory(this.sourcesDir);

            string fixtureRoot = Path.Combine(AppContext.BaseDirectory, "Fixtures");
            this.programCsPath = CopyFixture(fixtureRoot, "Program.cs");
            this.viewXwmlPath = CopyFixture(fixtureRoot, "View.xwml");
            this.skinCshtmlPath = CopyFixture(fixtureRoot, "Skin.cshtml");

            FixtureMapEmitter.Emit(
                FixtureMapEmitter.DefaultMapName,
                this.mapsDir,
                new[] { this.programCsPath, this.viewXwmlPath, this.skinCshtmlPath },
                sourceRoot: TestStartup.PathPrefix + "/" + FixtureMapEmitter.DefaultMapName,
                emitLegacyAshxHandler: false);

            TestStartup.CurrentOptions = new SourceMapFileHandlerOptions
            {
                MapsDirectory = this.mapsDir,
                AllowedSourceRoots = new List<string> { this.sourcesDir },
            };

            this.factory = new SourceMapServerFactory();
            this.client = this.factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.client?.Dispose();
            this.factory?.Dispose();
            TestStartup.CurrentOptions = null;

            if (Directory.Exists(this.workDir))
            {
                try
                {
                    Directory.Delete(this.workDir, recursive: true);
                }
                catch (IOException)
                {
                    // Best-effort; a transient antivirus hold on Windows should
                    // not fail the test itself.
                }
            }
        }

        [TestMethod]
        public async Task GET_ShortName_ReturnsMappedSource_Cs()
        {
            await AssertFixtureRoundTripAsync(this.programCsPath);
        }

        [TestMethod]
        public async Task GET_ShortName_ReturnsMappedSource_Xwml()
        {
            await AssertFixtureRoundTripAsync(this.viewXwmlPath);
        }

        [TestMethod]
        public async Task GET_ShortName_ReturnsMappedSource_Cshtml()
        {
            await AssertFixtureRoundTripAsync(this.skinCshtmlPath);
        }

        [TestMethod]
        public async Task GET_UnknownShort_Returns404()
        {
            var resp = await this.client.GetAsync(
                TestStartup.PathPrefix + "/" + FixtureMapEmitter.DefaultMapName + "/C$/does-not-exist/Phantom.cs");

            Assert.AreEqual(HttpStatusCode.NotFound, resp.StatusCode);
        }

        [TestMethod]
        [DataRow("a:b")]
        [DataRow("a b")]
        [DataRow(".hidden")]
        public async Task GET_MapNameWithDisallowedChars_Returns400(string mapName)
        {
            // These map-name patterns survive URL normalization and reach the
            // endpoint as-is, so the handler's whitelist must reject them with
            // 400. Dotted-traversal (".." segment) is handled one layer up by
            // ASP.NET Core path normalization and never reaches the endpoint,
            // so it is covered by GET_PathTraversalNormalization_DoesNotServeContent.
            string url = TestStartup.PathPrefix + "/"
                + Uri.EscapeDataString(mapName) + "/"
                + FixtureMapEmitter.ToShortName(this.programCsPath);

            var resp = await this.client.GetAsync(url);

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                resp.StatusCode,
                "Tampered mapName must be refused with 400 before any filesystem lookup");
        }

        [TestMethod]
        public async Task GET_PathTraversalNormalization_DoesNotServeContent()
        {
            // ASP.NET Core normalizes ".." out of the path before routing, so
            // the request either misses the endpoint entirely (yielding the
            // normalized URL which is not a configured route) or hits it with
            // a collapsed mapName that does not exist. Either outcome is
            // acceptable — the contract being verified here is simply "no
            // fixture content can be coerced out via traversal".
            string url = TestStartup.PathPrefix + "/../"
                + FixtureMapEmitter.DefaultMapName + "/"
                + FixtureMapEmitter.ToShortName(this.programCsPath);

            var resp = await this.client.GetAsync(url);

            int status = (int)resp.StatusCode;
            Assert.IsTrue(
                status >= 400 && status < 500,
                "Dotted traversal must fail closed with a 4xx, got " + status);
            string body = await resp.Content.ReadAsStringAsync();
            Assert.IsFalse(
                body.Contains("namespace FixtureApp"),
                "Traversal response must not leak fixture content");
        }

        [TestMethod]
        public async Task GET_UnknownMap_Returns404()
        {
            var resp = await this.client.GetAsync(
                TestStartup.PathPrefix + "/missing/"
                + FixtureMapEmitter.ToShortName(this.programCsPath));

            Assert.AreEqual(
                HttpStatusCode.NotFound,
                resp.StatusCode,
                "Missing map file must 404 rather than serving any content");
        }

        [TestMethod]
        public async Task GET_SiblingPrefixRoot_Returns404_NoBodyLeak()
        {
            // Classic containment pitfall: allow-list is <work>/src, attacker
            // aims at <work>/src-evil/Secret.cs. Repeats the HandleAsync-level
            // regression coverage at the full HTTP-transport level so future
            // wiring changes (middleware, filters) cannot reintroduce the leak.
            string evilDir = Path.Combine(this.workDir, "src-evil");
            Directory.CreateDirectory(evilDir);
            string secretPath = Path.Combine(evilDir, "Secret.cs");
            File.WriteAllText(secretPath, "sibling-evil-payload");

            FixtureMapEmitter.Emit(
                "sibling",
                this.mapsDir,
                new[] { secretPath },
                sourceRoot: TestStartup.PathPrefix + "/sibling",
                emitLegacyAshxHandler: false);

            var resp = await this.client.GetAsync(
                TestStartup.PathPrefix + "/sibling/" + FixtureMapEmitter.ToShortName(secretPath));

            Assert.AreEqual(HttpStatusCode.NotFound, resp.StatusCode);
            string body = await resp.Content.ReadAsStringAsync();
            Assert.IsFalse(
                body.Contains("sibling-evil-payload"),
                "Containment must fail closed — body must not leak file content");
        }

        [TestMethod]
        public void SourceRoot_IsWiredIntoMap_AndNoLegacyAshxSidecar()
        {
            // Opens the file the emitter wrote in Init and asserts the two
            // configuration knobs PR #36 added flowed through end-to-end.
            string mapPath = Path.Combine(this.mapsDir, FixtureMapEmitter.DefaultMapName + ".map");
            using var doc = JsonDocument.Parse(File.ReadAllText(mapPath));
            string sourceRoot = doc.RootElement.GetProperty("sourceRoot").GetString();

            Assert.AreEqual(
                TestStartup.PathPrefix + "/" + FixtureMapEmitter.DefaultMapName,
                sourceRoot,
                "Emitted map's sourceRoot must point at the new handler route, not the legacy .ashx");

            string ashxSidecar = Path.Combine(this.mapsDir, FixtureMapEmitter.DefaultMapName + ".ashx");
            Assert.IsFalse(
                File.Exists(ashxSidecar),
                "EmitLegacyAshxHandler=false must suppress the .ashx sidecar next to the map");
        }

        [TestMethod]
        public async Task GET_SourceRootPlusShortName_ResolvesLikeDevTools()
        {
            // DevTools composes GET {sourceRoot}/{shortName}. Asserts the two
            // together form a valid URL that routes into the handler and
            // streams back the original bytes — the canonical AC #1 flow.
            string mapPath = Path.Combine(this.mapsDir, FixtureMapEmitter.DefaultMapName + ".map");
            using var doc = JsonDocument.Parse(File.ReadAllText(mapPath));
            string sourceRoot = doc.RootElement.GetProperty("sourceRoot").GetString();
            string shortName = doc.RootElement.GetProperty("sources")[0].GetString();

            string composed = sourceRoot.TrimEnd('/') + "/" + shortName;

            var resp = await this.client.GetAsync(composed);

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
            Assert.AreEqual("text/plain; charset=utf-8", resp.Content.Headers.ContentType?.ToString());
            byte[] expected = File.ReadAllBytes(this.programCsPath);
            byte[] actual = await resp.Content.ReadAsByteArrayAsync();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Body returned by the real HTTP pipeline must be byte-exact with the fixture on disk");
        }

        private async Task AssertFixtureRoundTripAsync(string fixturePath)
        {
            string shortName = FixtureMapEmitter.ToShortName(fixturePath);
            string url = TestStartup.PathPrefix + "/" + FixtureMapEmitter.DefaultMapName + "/" + shortName;

            var resp = await this.client.GetAsync(url);

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode, "Expected 200 for " + url);
            Assert.AreEqual("text/plain; charset=utf-8", resp.Content.Headers.ContentType?.ToString());
            byte[] expected = File.ReadAllBytes(fixturePath);
            byte[] actual = await resp.Content.ReadAsByteArrayAsync();
            CollectionAssert.AreEqual(
                expected,
                actual,
                "Response body must be byte-identical to the fixture on disk");
        }

        private string CopyFixture(string fixtureRoot, string relative)
        {
            string source = Path.Combine(fixtureRoot, relative);
            string dest = Path.Combine(this.sourcesDir, relative);
            string destDir = Path.GetDirectoryName(dest);
            if (!string.IsNullOrEmpty(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(source, dest, overwrite: true);
            return dest;
        }
    }
}
