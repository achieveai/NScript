using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// The compiled skin IR cache a warm build service keeps across builds. The cache lives
    /// for the process, so each test uses its own template name.
    /// </summary>
    [TestClass]
    public class RazorSkinIRCacheTests
    {
        private const string FrameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
}";

        private const string PlainVM = @"
public class TestVM
{
    public string Name { get; set; }
}";

        private const string ObservableVM = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
}";

        [TestMethod]
        public void EditedSkin_IsCompiledAgain()
        {
            var name = UniqueName();

            var first = Get(name, "@model TestVM\n\n<div>Alpha</div>", PlainVM);
            var edited = Get(name, "@model TestVM\n\n<div>Beta</div>", PlainVM);

            Html(first).Should().Contain("Alpha");
            Html(edited).Should().Contain("Beta").And.NotContain("Alpha");
        }

        [TestMethod]
        public void EditedBoundType_IsCompiledAgain()
        {
            var name = UniqueName();
            const string skin = "@model TestVM\n\n<div>@Model.Name</div>";

            var before = Get(name, skin, PlainVM);
            var after = Get(name, skin, ObservableVM);

            Find<ExpressionBindingNode>(before).Single().Classification.Mode.Should().Be(BindingMode.OneTime);
            Find<ExpressionBindingNode>(after).Single().Classification.Mode.Should().Be(BindingMode.OneWay,
                "the model type became observable, so the cached OneTime IR must not be reused");
        }

        [TestMethod]
        public void UnchangedSkin_IsAHit_AndWritesToAnEarlierCopyDoNotLeak()
        {
            var name = UniqueName();
            const string skin = "@model TestVM\n\n<div><SearchBox Query=\"Model.Name\" /></div>";

            var first = Get(name, skin, ObservableVM);
            var sub = Find<SubControlNode>(first).Single();
            var binding = sub.PropertyBindings.Single();
            var original = (sub.TagName, sub.RuntimeMarkerIdx, sub.ResolvedTypeName, Attributes: sub.DomAttributes?.Count,
                binding.IsDelegate, binding.IsLiteral, Count: first.Children.Count);
            var hits = RazorTemplatingPlugin.SkinIRs.Hits;

            // What codegen writes into the IR from Cecil data during a build.
            sub.TagName = "section";
            sub.RuntimeMarkerIdx = 5;
            sub.ResolvedTypeName = "App.Changed";
            sub.DomAttributes = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("role", "search") };
            binding.IsDelegate = !binding.IsDelegate;
            binding.IsLiteral = !binding.IsLiteral;
            first.Children.Add(new HtmlNode { HtmlContent = "stale" });

            var second = Get(name, skin, ObservableVM);

            RazorTemplatingPlugin.SkinIRs.Hits.Should().Be(hits + 1, "an unchanged skin is served from the cache");
            var fresh = Find<SubControlNode>(second).Single();
            var freshBinding = fresh.PropertyBindings.Single();
            (fresh.TagName, fresh.RuntimeMarkerIdx, fresh.ResolvedTypeName, Attributes: fresh.DomAttributes?.Count,
                freshBinding.IsDelegate, freshBinding.IsLiteral, Count: second.Children.Count)
                .Should().Be(original);
            Html(second).Should().NotContain("stale");
        }

        private static string UniqueName() => "CacheSkin" + Guid.NewGuid().ToString("N");

        private static SkinTemplateNode Get(string name, string skin, string vmSource)
            => RazorTemplatingPlugin.GetSkinIR(name, skin, new[] { FrameworkStubs, vmSource }, name + ".skin.cshtml");

        private static string Html(IRNode root)
            => string.Concat(Find<HtmlNode>(root).Select(node => node.HtmlContent));

        private static IEnumerable<T> Find<T>(IRNode root)
            where T : IRNode
        {
            if (root is T match)
            {
                yield return match;
            }

            foreach (var child in root.Children)
            {
                foreach (var found in Find<T>(child))
                {
                    yield return found;
                }
            }
        }
    }
}
