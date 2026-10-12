//-----------------------------------------------------------------------
// <copyright file="XwmlCssCacheTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// XWML stylesheets share one parse per content through <see cref="CssStyleSheet.ParsedSheets"/>.
    /// A sheet built from the cache must match one built from a fresh parse, and building it
    /// must leave the shared grammar unchanged.
    /// </summary>
    [TestClass]
    public class XwmlCssCacheTests
    {
        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }

        [TestMethod]
        public void SecondSheetFromTheCache_MatchesTheFirst_AndTheCachedGrammarIsUnchanged()
        {
            var css = UniqueCss();

            var first = BuildSheet(css, out var firstHit);
            var cached = CssStyleSheet.ParsedSheets.GetOrCreate(
                ContentCache<CssParser.CssGrammer>.Key(css),
                () => throw new InvalidOperationException("the first sheet should have cached this stylesheet"));
            var before = Fingerprint(cached);

            var second = BuildSheet(css, out var secondHit);

            Assert.IsFalse(firstHit);
            Assert.IsTrue(secondHit, "the second sheet must come from the cache for this test to mean anything");
            Assert.AreEqual(first, second);
            Assert.AreEqual(before, Fingerprint(cached), "building a sheet must not change the shared grammar");
            Assert.AreEqual(Fingerprint(Parse(css)), before, "the cached grammar must match a fresh parse");
        }

        private static string UniqueCss()
            => "/* " + Guid.NewGuid().ToString("N") + " */\n"
                + ".card { color: red; }\n"
                + ".card .cardTitle { font-weight: bold; }\n"
                + ".wide { width: 100%; }\n"
                + "@keyframes pulse { from { opacity: 0; } to { opacity: 1; } }\n"
                + "@media (max-width: 600px) { .card { padding: 0; } }\n";

        /// <summary>One sheet's CSS work, as CodeGenerator.GetStyleSheet does it.</summary>
        private static string BuildSheet(string css, out bool cacheHit)
        {
            var sheet = new CssStyleSheet(Helper.GetParserContext(), "App.css");
            cacheHit = sheet.AddCss(css, new Location("App.css", 0, 0), new List<CssStyleSheet>());
            return sheet.GetCssString()
                + "|classes:" + string.Join(",", sheet.ClassNames.OrderBy(n => n, StringComparer.Ordinal));
        }

        private static CssParser.CssGrammer Parse(string css)
        {
            var grammar = new CssParser.CssGrammer(css, parseProperties: false);
            grammar.CollectCssVariablesFromRules();
            grammar.CollectUsedCssVariablesFromRules();
            return grammar;
        }

        /// <summary>Everything a sheet reads from the grammar, with class names unrenamed.</summary>
        private static string Fingerprint(CssParser.CssGrammer grammar)
        {
            var sb = new StringBuilder();
            foreach (var rule in grammar.Rules)
            {
                CssParser.CssSerializerVisitor.Instance.Process(sb, rule, cn => cn.ClassName, id => id.Id);
            }

            sb.Append("|keyframes:").Append(grammar.KeyFrames.Count).Append('|');
            foreach (var keyFrames in grammar.KeyFrames)
            {
                CssParser.CssSerializerVisitor.Instance.Process(sb, keyFrames);
            }

            sb.Append("|media:").Append(grammar.MediaRules.Count).Append('|');
            foreach (var media in grammar.MediaRules)
            {
                CssParser.CssSerializerVisitor.Instance.Process(sb, media, cn => cn.ClassName, id => id.Id);
            }

            sb.Append("|defined:").Append(string.Join(",", grammar.DefinedCssVariables));
            sb.Append("|used:").Append(string.Join(",", grammar.UsedCssVariables));
            return sb.ToString();
        }
    }
}
