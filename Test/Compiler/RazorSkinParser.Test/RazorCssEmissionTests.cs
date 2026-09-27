using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RazorCssEmissionTests
    {
        [TestMethod]
        public void SharedStylesheetEmitsOnceAndDistinctStylesRemain()
        {
            var first = new RazorCssManager();
            first.AddStylesheet("Shared.css", ".shared { color: red; }");
            first.CompressNames(releaseNaming: false);

            var second = new RazorCssManager();
            second.AddStylesheet("Shared.css", ".shared { color: red; }");
            second.CompressNames(releaseNaming: false);

            var distinct = new RazorCssManager();
            distinct.AddStylesheet("Other.css", ".other { color: blue; }");
            distinct.CompressNames(releaseNaming: false);

            var css = RazorTemplatingPlugin.CollectCssForEmission(
                new[] { first, second, distinct });

            Regex.Matches(css, "color:red").Count.Should().Be(1);
            Regex.Matches(css, "color:blue").Count.Should().Be(1);
            css.Should().Contain(first.GetSerializedCss());
            css.Should().Contain(distinct.GetSerializedCss());
        }

        [TestMethod]
        public void SameResourceWithDifferentClassNamesKeepsBothVariants()
        {
            var first = new RazorCssManager();
            first.AddStylesheet("Shared.css", ".shared { color: red; }");
            first.CompressNames(releaseNaming: false);

            var second = new RazorCssManager();
            second.AddStylesheet("Other.css", ".other { color: blue; }");
            second.AddStylesheet("Shared.css", ".shared { color: red; }");
            second.CompressNames(releaseNaming: true);

            first.ReplaceCssClassNames("shared")
                .Should().NotBe(second.ReplaceCssClassNames("shared"));

            var css = RazorTemplatingPlugin.CollectCssForEmission(new[] { first, second });

            css.Should().Contain(first.GetSerializedCssForSheet(first.Sheets[0]));
            css.Should().Contain(second.GetSerializedCssForSheet(second.Sheets[1]));
            Regex.Matches(css, "color:red").Count.Should().Be(2);
            Regex.Matches(css, "color:blue").Count.Should().Be(1);
        }
    }
}
