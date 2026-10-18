using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Dev patch emit of a skin or stylesheet save (docs/build/build-service.md, "Live patching
    /// in the open page"). A changed skin is written again with the names the bundle has and a
    /// storage index past the old ones; in a patch-emit bundle its storage line re-skins the
    /// open controls. A changed stylesheet keeps its class names and is written again on the
    /// marked CSS line. Anything the bundle lacks is a miss, so the build emits in full.
    /// </summary>
    [TestClass]
    public class RazorLivePatchTests
    {
        private const string Simple = "GraphSimpleText.skin.cshtml";
        private const string SimpleOld = "<span>@Model.PropStr1</span>";
        private const string Styles = "RazorStyles.css";

        private static (RazorTemplatingPlugin Plugin, ClrContext Context, RuntimeScopeManager Manager, string Js) BuildSkins(bool liveHook)
        {
            var clrContext = UiTestAssemblies.Load();
            var context = new ConverterContext(clrContext, null, null) { DevMode = true, LivePatchHook = liveHook };
            var manager = new RuntimeScopeManager(context);
            var plugin = new RazorTemplatingPlugin();
            var js = UiTestAssemblies.Build(plugin, clrContext, manager);

            // As the patch session does once the full emit is written.
            manager.AcceptDemand();
            return (plugin, clrContext, manager, js);
        }

        /// <summary>Gives the embedded resource whose name ends in <paramref name="suffix"/> new text.</summary>
        private static void Edit(ClrContext clrContext, string suffix, Func<string, string> edit)
        {
            foreach (var module in clrContext.Modules)
            {
                for (int i = 0; i < module.Resources.Count; i++)
                {
                    if (module.Resources[i] is EmbeddedResource resource && resource.Name.EndsWith(suffix, StringComparison.Ordinal))
                    {
                        var text = Encoding.UTF8.GetString(resource.GetResourceData());
                        var edited = edit(text);
                        edited.Should().NotBe(text, "the edit must change " + suffix);
                        module.Resources[i] = new EmbeddedResource(resource.Name, resource.Attributes, Encoding.UTF8.GetBytes(edited));
                        return;
                    }
                }
            }

            Assert.Fail("No resource " + suffix);
        }

        private static string Write(IEnumerable<Statement> statements)
        {
            var writer = new StringWriter();
            var serializer = new JSWriter(true, false);
            foreach (var statement in statements)
            {
                serializer.Write(statement);
            }

            serializer.Write(writer);
            return writer.ToString();
        }

        private static string Patch(RazorTemplatingPlugin plugin, RuntimeScopeManager manager, Dictionary<string, string> literals, out string miss)
        {
            var statements = new List<Statement>();
            miss = plugin.TryPatchResources(statements, literals);
            manager.TakeNewDemand(manager.VirtualMethodsUsedCount)
                .Should().BeNull("a patch of a skin the bundle has asks for nothing the bundle lacks");
            return Write(statements);
        }

        /// <summary>The storage indexes the skins' Skin factory calls name.</summary>
        private static IEnumerable<int> Indexes(string js)
            => Regex.Matches(js, @"__nsReskin\([\w$]+, [\w$]+, ""(\d+)""").Select(match => int.Parse(match.Groups[1].Value));

        [TestMethod]
        public void SkinEdit_WithLiveHook_KeepsItsNames_TakesANewIndex_SkinsOpenControlsAgain()
        {
            var (plugin, clrContext, manager, cold) = BuildSkins(liveHook: true);
            cold.Should().Contain(".__nsTrack = function(si,s)", "a Razor-only bundle writes the live helpers with its document storage getter");
            cold.Should().Contain(".__nsReskin = (function(F,I,P,K,set)");
            cold.Should().NotMatchRegex(@"GraphSimpleText_var[\w$]* = null", "the storage line is the re-skin");
            var reskin = new Regex(@"if \((GraphSimpleText_var[\w$]*)\)\s*\{?\s*([\w$]+)\.__nsReskin\(\1, (GraphSimpleText_factory[\w$]*), ""(\d+)"", ""GraphSimpleText""\);");
            var before = reskin.Match(cold);
            before.Success.Should().BeTrue("each skin's storage line re-skins when a patch runs it again");
            cold.Should().Contain(before.Groups[2].Value + ".__nsTrack(", "each factory keeps what it made");
            plugin.PatchSectionName.Should().Be("RazorSkinParser");
            plugin.PatchSectionLive.Should().BeTrue();

            Edit(clrContext, Simple, text => text.Replace(SimpleOld, SimpleOld + "<i>live</i>"));
            var literals = new Dictionary<string, string>();
            var patch = Patch(plugin, manager, literals, out var miss);
            miss.Should().BeNull();
            patch.Should().Contain("<i>live</i>");
            var after = reskin.Match(patch);
            after.Success.Should().BeTrue("the patch writes the storage line again");
            after.Groups[1].Value.Should().Be(before.Groups[1].Value, "the patch keeps the storage variable");
            after.Groups[3].Value.Should().Be(before.Groups[3].Value, "the patch keeps the factory's name");
            int.Parse(after.Groups[4].Value).Should().BeGreaterThan(Indexes(cold).Max(), "each document caches a skin's DOM by its index");
            patch.Should().Contain("function " + before.Groups[3].Value + "(");
            patch.Should().Contain(before.Groups[2].Value + ".__nsTrack(");
            patch.Should().NotContain("RazorStyledTemplate", "only the changed skin is written");
            literals.Should().BeEmpty("a skin edit leaves the CSS");

            // A second edit takes the next index again.
            Edit(clrContext, Simple, text => text.Replace("<i>live</i>", "<i>again</i>"));
            var again = reskin.Match(Patch(plugin, manager, literals, out miss));
            miss.Should().BeNull();
            int.Parse(again.Groups[4].Value).Should().BeGreaterThan(int.Parse(after.Groups[4].Value));
        }

        [TestMethod]
        public void SkinEdit_AfterTheAssemblyIsCompiledAgain_IsStillAPatch()
        {
            // A .cs save compiles the assembly again: same skins, a new Mvid.
            var (plugin, clrContext, manager, _) = BuildSkins(liveHook: true);
            foreach (var module in clrContext.Modules)
            {
                module.Mvid = Guid.NewGuid();
            }

            Patch(plugin, manager, new Dictionary<string, string>(), out var miss).Should().BeEmpty();
            miss.Should().BeNull("no skin changed");
            Edit(clrContext, Simple, text => text.Replace(SimpleOld, SimpleOld + "<i>live</i>"));
            Patch(plugin, manager, new Dictionary<string, string>(), out miss).Should().Contain("<i>live</i>");
            miss.Should().BeNull("the bundle has the skin");
        }

        [TestMethod]
        public void SkinEdit_InABundleWithNoRazorCss_IsStillAPatch()
        {
            // An app whose skins name no stylesheet (AgentWorkBench) writes no CSS line.
            var clrContext = UiTestAssemblies.Load();
            foreach (var module in clrContext.Modules)
            {
                for (int i = 0; i < module.Resources.Count; i++)
                {
                    if (module.Resources[i] is EmbeddedResource resource && resource.Name.EndsWith(".css", StringComparison.Ordinal))
                    {
                        module.Resources[i] = new EmbeddedResource(resource.Name, resource.Attributes, Array.Empty<byte>());
                    }
                }
            }

            var context = new ConverterContext(clrContext, null, null) { DevMode = true, LivePatchHook = true };
            var manager = new RuntimeScopeManager(context);
            var plugin = new RazorTemplatingPlugin();
            var cold = UiTestAssemblies.Build(plugin, clrContext, manager);
            manager.AcceptDemand();
            cold.Should().NotContain("/*@L RazorSkinParser.Css*/", "no Razor CSS, no CSS line");

            Edit(clrContext, Simple, text => text.Replace(SimpleOld, SimpleOld + "<i>live</i>"));
            var literals = new Dictionary<string, string>();
            Patch(plugin, manager, literals, out var miss).Should().Contain("<i>live</i>");
            miss.Should().BeNull("no CSS before and none after is no CSS change");
            literals.Should().BeEmpty();
        }

        [TestMethod]
        public void SkinEdit_WithoutLiveHook_PatchesTheBundle_PageReloads()
        {
            var (plugin, clrContext, manager, cold) = BuildSkins(liveHook: false);
            cold.Should().NotContain("__ns");
            cold.Should().MatchRegex(@"GraphSimpleText_var[\w$]* = null");
            plugin.PatchSectionLive.Should().BeFalse("without the hook an open page reloads");

            Edit(clrContext, Simple, text => text.Replace(SimpleOld, SimpleOld + "<i>live</i>"));
            var patch = Patch(plugin, manager, new Dictionary<string, string>(), out var miss);
            miss.Should().BeNull("the bundle still takes the patch, for the next load");
            patch.Should().Contain("<i>live</i>").And.NotContain("__ns");
        }

        [TestMethod]
        public void SkinEdit_TheBundleCannotTake_IsAMiss()
        {
            var (plugin, clrContext, manager, _) = BuildSkins(liveHook: true);
            Edit(clrContext, Simple, text => text.Replace(SimpleOld, SimpleOld + "<b onclick=\"@Model.ToString\">x</b>"));
            Patch(plugin, manager, new Dictionary<string, string>(), out var miss);
            miss.Should().StartWith("razor skin calls ", "a method no skin called is not in the bundle");

            (plugin, clrContext, manager, _) = BuildSkins(liveHook: true);
            Edit(clrContext, Simple, text => text.Replace("TestViewModelA", "RazorTestVM").Replace("PropStr1", "Name"));
            Patch(plugin, manager, new Dictionary<string, string>(), out miss);
            miss.Should().StartWith("razor skin types or styles ");

            (plugin, clrContext, manager, _) = BuildSkins(liveHook: true);
            Edit(clrContext, "RazorStyledTemplate.skin.cshtml", text => text.Replace("class=\"styled-footer\"", "class=\"styled-footer not-in-css\""));
            Patch(plugin, manager, new Dictionary<string, string>(), out miss);
            miss.Should().StartWith("razor skin css ", "a full build would refuse the class");
        }

        [TestMethod]
        public void StylesheetEdit_KeepsClassNames_WritesTheCssLineAgain_GoesLive()
        {
            var (plugin, clrContext, manager, cold) = BuildSkins(liveHook: true);
            cold.Should().Contain(RazorTemplatingPlugin.CssMarker, "dev bundles mark the CSS line");
            cold.Should().MatchRegex(@"textContent = [\w$]+\.__nsRazorCss \|\| """, "a load after a live patch takes the patch's CSS");
            cold.Should().MatchRegex(@"[\w$]+\.__nsRazorStyle = ");

            Edit(clrContext, Styles, text => text.Replace("color: red;", "color: rgb(1, 2, 3);"));
            var literals = new Dictionary<string, string>();
            var patch = Patch(plugin, manager, literals, out var miss);
            miss.Should().BeNull();
            patch.Should().BeEmpty("no skin changed");
            literals.Should().ContainKey(RazorTemplatingPlugin.CssMarker);
            var css = literals[RazorTemplatingPlugin.CssMarker];
            css.Should().Contain("color:rgb(1,2,3)").And.NotContain("color:red");
            var headerName = Regex.Match(cold, @"\.(styled-header[\w-]*)\{").Groups[1].Value;
            headerName.Should().NotBeEmpty();
            css.Should().Contain("." + headerName + "{", "the class keeps its name, which the open skins carry");

            var live = Write(plugin.LiveLiteral(RazorTemplatingPlugin.CssMarker, css, manager.Scope));
            live.Should().Contain(".__nsRazorCss = ").And.Contain(".__nsRazorStyle.textContent = ");
            plugin.LiveLiteral("/*@L Other*/", css, manager.Scope).Should().BeNull();

            // A skin save after it still finds the sheet's classes.
            Edit(clrContext, "RazorStyledTemplate.skin.cshtml", text => text.Replace("Footer", "Footer!"));
            Patch(plugin, manager, new Dictionary<string, string>(), out miss).Should().Contain("Footer!");
            miss.Should().BeNull();

            // A second colour save compares against the kept classes.
            Edit(clrContext, Styles, text => text.Replace("rgb(1, 2, 3)", "rgb(4, 5, 6)"));
            literals.Clear();
            Patch(plugin, manager, literals, out miss);
            miss.Should().BeNull();
            literals[RazorTemplatingPlugin.CssMarker].Should().Contain("color:rgb(4,5,6)");

            // Adding a class would need a new name: a miss, so the build emits in full.
            Edit(clrContext, Styles, text => text + "\n.styled-new { color: green; }");
            Patch(plugin, manager, new Dictionary<string, string>(), out miss);
            miss.Should().StartWith("razor css classes changed ");
        }
    }
}
