//-----------------------------------------------------------------------
// <copyright file="XwmlSourceMapTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.JST;
    using NScript.Utils;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Phase 3a coverage: XWML templates must flow the originating template
    /// <see cref="HtmlAgilityPack.HtmlNode"/> positions through the emitted JST so
    /// the final source map points back at the .html template rather than reporting
    /// null positions. These tests drive a known template through the real
    /// <see cref="XwmlTemplatingPlugin"/> pipeline and assert that the produced
    /// JST statements carry template Locations end-to-end.
    /// </summary>
    [TestClass]
    public class XwmlSourceMapTests
    {
        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }

        /// <summary>
        /// End-to-end: a template must produce at least one JST node that carries
        /// a Location pointing at the template file. Prior to Phase 3 the XWML
        /// emitter passed null everywhere, so this test fails without the Location
        /// threading added in <c>SkinCodeGenerator</c>, <c>HtmlNodeInfo</c>,
        /// <c>UIElementNodeInfo</c>, <c>TypeNodeInfo</c>, and <c>BinderInfo</c>.
        /// </summary>
        [TestMethod]
        public void Generate_TemplateStatementsCarryTemplateLocation()
        {
            IList<Statement> statements = GenerateTemplateStatements("TestArrayBinding");

            IReadOnlyList<Location> templateLocations =
                CollectLocationsPointingAt(statements, "TestArrayBinding.html");

            Assert.IsTrue(
                templateLocations.Count > 0,
                "Expected at least one JST node to carry a Location pointing at TestArrayBinding.html, " +
                "but no template-sourced locations were produced.");
        }

        /// <summary>
        /// The <c>&lt;skin&gt;</c> root in <c>TestArrayBinding.html</c> sits on line 11
        /// and its two bound <c>&lt;div&gt;</c> children on lines 12 and 13. The
        /// skin-factory / skin-getter functions anchor to the skin line, so we assert
        /// at least one statement reports a line within that span — protecting
        /// against future drift where locations default to line 1 (document start).
        /// </summary>
        [TestMethod]
        public void Generate_LocationsPointAtSkinOrChildLines()
        {
            IList<Statement> statements = GenerateTemplateStatements("TestArrayBinding");

            IReadOnlyList<Location> templateLocations =
                CollectLocationsPointingAt(statements, "TestArrayBinding.html");

            Assert.IsTrue(
                templateLocations.Any(loc => loc.StartLine >= 11 && loc.StartLine <= 13),
                "Expected a template Location pointing at the <skin> / bound <div> lines (11-13). " +
                "Got: " + string.Join(", ", templateLocations.Select(l => l.StartLine.ToString())));
        }

        /// <summary>
        /// End-to-end source-map emission: writing the template JST via
        /// <see cref="JSWriter.WriteWithMap"/> must produce a source-map JSON whose
        /// <c>sources</c> array includes the originating .html template. Without this,
        /// browser debuggers can't step from generated JS back to the XWML file.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_IncludesTemplateFileInSources()
        {
            IList<Statement> statements = GenerateTemplateStatements("TestArrayBinding");

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            foreach (var stmt in statements)
            {
                writer.Write(stmt);
            }

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "TestArrayBinding.js");

            string mapJson = map.ToString();
            StringAssert.Contains(
                mapJson,
                "TestArrayBinding.html",
                "Expected the template filename to appear in the source-map sources.\nMap:\n" + mapJson);
        }

        private static IList<Statement> GenerateTemplateStatements(string templateBaseName)
        {
            var plugin = Helper.CreatePlugin(null);
            Helper.LoadHtmlParser(templateBaseName + ".html", plugin.ParserContext);

            var identifier = plugin.CodeGenerator.GetTemplateGetterIdentifier(templateBaseName + ".html");
            Assert.IsNotNull(identifier, "Template getter identifier should resolve for " + templateBaseName);

            plugin.CodeGenerator.IterateParsing();
            return plugin.CodeGenerator.GetAllTemplateStatements();
        }

        /// <summary>
        /// Walks the JST looking for every <see cref="Node.Location"/> whose file name
        /// ends with <paramref name="templateFileName"/>. Uses reflection-based node
        /// traversal because the visitor extension methods are internal to NScript.JST;
        /// a reflection walk is a cheap, test-only way to cover every child regardless
        /// of future node-type additions.
        /// </summary>
        private static IReadOnlyList<Location> CollectLocationsPointingAt(
            IEnumerable<Statement> statements,
            string templateFileName)
        {
            var collected = new List<Location>();
            var visited = new HashSet<object>();
            foreach (var stmt in statements)
            {
                WalkNode(stmt, templateFileName, collected, visited);
            }

            return collected;
        }

        private static void WalkNode(
            object node,
            string templateFileName,
            List<Location> collected,
            HashSet<object> visited)
        {
            if (node == null || !visited.Add(node))
            {
                return;
            }

            if (node is Node jstNode && jstNode.Location != null)
            {
                var fileName = jstNode.Location.FileName;
                if (fileName != null
                    && fileName.EndsWith(templateFileName, System.StringComparison.Ordinal))
                {
                    collected.Add(jstNode.Location);
                }
            }

            var type = node.GetType();
            foreach (var field in GetAllFields(type))
            {
                if (field.IsStatic)
                {
                    continue;
                }

                var fieldType = field.FieldType;
                if (fieldType.IsPrimitive || fieldType == typeof(string) || fieldType.IsEnum)
                {
                    continue;
                }

                object value;
                try
                {
                    value = field.GetValue(node);
                }
                catch
                {
                    continue;
                }

                if (value == null)
                {
                    continue;
                }

                if (value is Node childNode)
                {
                    WalkNode(childNode, templateFileName, collected, visited);
                }
                else if (value is System.Collections.IEnumerable enumerable
                    && !(value is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item is Node itemNode)
                        {
                            WalkNode(itemNode, templateFileName, collected, visited);
                        }
                    }
                }
            }
        }

        private static IEnumerable<FieldInfo> GetAllFields(System.Type type)
        {
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                foreach (var f in t.GetFields(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    yield return f;
                }
            }
        }
    }
}
