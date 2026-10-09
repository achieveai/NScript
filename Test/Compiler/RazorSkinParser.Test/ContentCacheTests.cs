using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;
using NScript.Utils;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class ContentCacheTests
    {
        [TestMethod]
        public void Key_SeparatesParts_AndNullFromEmpty()
        {
            ContentCache<string>.Key("ab", "c").Should().NotBe(ContentCache<string>.Key("a", "bc"));
            ContentCache<string>.Key((string)null).Should().NotBe(ContentCache<string>.Key(string.Empty));
            ContentCache<string>.Key("a", "b").Should().Be(ContentCache<string>.Key("a", "b"));
        }

        [TestMethod]
        public void GetOrCreate_ReportsWhetherThisCallHit()
        {
            var cache = new ContentCache<string>(4);

            cache.GetOrCreate("k", () => "v", out var first);
            cache.GetOrCreate("k", () => "other", out var second).Should().Be("v");

            first.Should().BeFalse();
            second.Should().BeTrue();
        }

        [TestMethod]
        public void FailedCreate_StoresNothing()
        {
            var cache = new ContentCache<string>(4);

            Action failing = () => cache.GetOrCreate("k", () => throw new InvalidOperationException("boom"));

            failing.Should().Throw<InvalidOperationException>().WithMessage("boom");
            cache.Count.Should().Be(0);
            cache.GetOrCreate("k", () => "v").Should().Be("v");
        }

        [TestMethod]
        public void OverCapacity_EvictsTheLeastRecentlyUsed()
        {
            var cache = new ContentCache<string>(2);
            cache.GetOrCreate("a", () => "a1");
            cache.GetOrCreate("b", () => "b1");
            cache.GetOrCreate("a", () => "a2");
            cache.GetOrCreate("c", () => "c1");

            cache.GetOrCreate("a", () => "a3").Should().Be("a1", "a was used more recently than b");
            cache.GetOrCreate("b", () => "b2").Should().Be("b2", "b was evicted");
        }
    }
}
