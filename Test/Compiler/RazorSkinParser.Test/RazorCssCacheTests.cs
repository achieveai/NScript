using System;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;
using NScript.Utils;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// The parsed stylesheet cache hands one CssGrammer to every build with the same CSS text,
    /// without a copy. These tests fail if a build's CSS work changes that shared object.
    /// </summary>
    [TestClass]
    public class RazorCssCacheTests
    {
        private const string Html = "<div class=\"card card-title wide\"></div>";

        [TestMethod]
        public void SecondBuildFromTheCache_MatchesTheFirst_AndTheCachedGrammarIsUnchanged()
        {
            var css = UniqueCss();

            var first = RunBuild(css, releaseNaming: false, out var firstHit);
            var cached = RazorCssManager.ParsedSheets.GetOrCreate(
                ContentCache<CssParser.CssGrammer>.Key(css),
                () => throw new InvalidOperationException("the first build should have cached this stylesheet"));
            var before = Fingerprint(cached);

            RunBuild(css, releaseNaming: true, out _);
            var second = RunBuild(css, releaseNaming: false, out var secondHit);

            firstHit.Should().BeFalse();
            secondHit.Should().BeTrue("the second build must be served from the cache for this test to mean anything");
            second.Should().Be(first);
            Fingerprint(cached).Should().Be(before, "a build's CSS work must not change the shared grammar");
            before.Should().Be(Fingerprint(Parse(css)), "the cached grammar must match a fresh parse");
        }

        private static string UniqueCss()
            => "/* " + Guid.NewGuid().ToString("N") + " */\n"
                + ":root { --accent: red; }\n"
                + ".card { color: var(--accent); }\n"
                + ".card .card-title { font-weight: bold; }\n"
                + ".wide { width: 100%; }\n"
                + "@keyframes pulse { from { opacity: 0; } to { opacity: 1; } }\n"
                + "@media (max-width: 600px) { .card { padding: 0; } }\n";

        /// <summary>One build's CSS work, as RazorTemplatingPlugin does it.</summary>
        private static string RunBuild(string css, bool releaseNaming, out bool cacheHit)
        {
            var manager = new RazorCssManager();
            cacheHit = manager.AddStylesheet("App.css", css);
            manager.ValidateCssVariables();
            manager.CompressNames(releaseNaming);
            var output = new StringBuilder(manager.GetSerializedCss());
            foreach (var sheet in manager.Sheets)
            {
                output.Append('\n').Append(manager.GetSerializedCssForSheet(sheet));
            }

            output.Append('\n').Append(RazorCssManager.ReplaceCssClassNamesInHtml(Html, manager));
            return output.ToString();
        }

        private static CssParser.CssGrammer Parse(string css)
        {
            var grammar = new CssParser.CssGrammer(css, parseProperties: false);
            grammar.CollectCssVariablesFromRules();
            grammar.CollectUsedCssVariablesFromRules();
            return grammar;
        }

        /// <summary>Everything a build reads from the grammar, with class names unrenamed.</summary>
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
