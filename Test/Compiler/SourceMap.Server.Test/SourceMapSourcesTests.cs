// -----------------------------------------------------------------------
// <copyright file="SourceMapSourcesTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="SourceMapSources"/>. Exercises JSON parsing edge
    /// cases so the server-side handler can rely on a correct short → long mapping.
    /// </summary>
    [TestClass]
    public class SourceMapSourcesTests
    {
        [TestMethod]
        public void TryParseContent_ValidMapWithSourcesLong_ResolvesLongPath()
        {
            string json = "{"
                + "\"sources\":[\"C$/a/Foo.cs\",\"C$/b/Bar.cs\"],"
                + "\"sourcesLong\":[\"C:\\\\a\\\\Foo.cs\",\"C:\\\\b\\\\Bar.cs\"]"
                + "}";

            var result = SourceMapSources.TryParseContent(json);

            Assert.IsNotNull(result);
            Assert.AreEqual("C:\\a\\Foo.cs", result.ResolveLongPath("C$/a/Foo.cs"));
            Assert.AreEqual("C:\\b\\Bar.cs", result.ResolveLongPath("C$/b/Bar.cs"));
        }

        [TestMethod]
        public void TryParseContent_MissingSourcesLong_FallsBackToShortName()
        {
            string json = "{\"sources\":[\"repo/Foo.cs\"]}";

            var result = SourceMapSources.TryParseContent(json);

            Assert.IsNotNull(result);
            Assert.AreEqual(
                "repo/Foo.cs",
                result.ResolveLongPath("repo/Foo.cs"),
                "When sourcesLong is absent, the short name itself is treated as the resolvable path");
        }

        [TestMethod]
        public void TryParseContent_SourcesLongShorterThanSources_FallsBackForOverrun()
        {
            string json = "{"
                + "\"sources\":[\"short1\",\"short2\"],"
                + "\"sourcesLong\":[\"long1\"]"
                + "}";

            var result = SourceMapSources.TryParseContent(json);

            Assert.IsNotNull(result);
            Assert.AreEqual("long1", result.ResolveLongPath("short1"));
            Assert.AreEqual(
                "short2",
                result.ResolveLongPath("short2"),
                "Indices past the end of sourcesLong must fall back to the short name, not throw");
        }

        [TestMethod]
        public void TryParseContent_MalformedJson_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{this is not json");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TryParseContent_EmptyPayload_ReturnsNull()
        {
            Assert.IsNull(SourceMapSources.TryParseContent(string.Empty));
            Assert.IsNull(SourceMapSources.TryParseContent("   "));
            Assert.IsNull(SourceMapSources.TryParseContent(null));
        }

        [TestMethod]
        public void TryParseContent_NoSourcesArray_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{\"mappings\":\"\"}");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TryParseContent_SourcesNotArray_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{\"sources\":\"nope\"}");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TryParseContent_EmptySourcesArray_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{\"sources\":[]}");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ResolveLongPath_UnknownShortName_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{\"sources\":[\"known\"]}");

            Assert.IsNotNull(result);
            Assert.IsNull(result.ResolveLongPath("unknown"));
        }

        [TestMethod]
        public void ResolveLongPath_NullOrEmptyInput_ReturnsNull()
        {
            var result = SourceMapSources.TryParseContent("{\"sources\":[\"x\"]}");

            Assert.IsNotNull(result);
            Assert.IsNull(result.ResolveLongPath(null));
            Assert.IsNull(result.ResolveLongPath(string.Empty));
        }

        [TestMethod]
        public void ShortNames_ReturnsAllRecordedShortNames()
        {
            string json = "{\"sources\":[\"a\",\"b\",\"c\"]}";

            var result = SourceMapSources.TryParseContent(json);

            Assert.IsNotNull(result);
            CollectionAssert.AreEquivalent(new[] { "a", "b", "c" }, System.Linq.Enumerable.ToArray(result.ShortNames));
        }
    }
}
