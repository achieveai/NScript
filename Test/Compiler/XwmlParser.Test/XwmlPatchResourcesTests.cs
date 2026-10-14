//-----------------------------------------------------------------------
// <copyright file="XwmlPatchResourcesTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.Converter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Dev patch emit of a skin save: after a full build, a changed document is parsed again
    /// and only its code is written. The code keeps the names the bundle already calls, the
    /// classes keep their names, and the skin takes a storage index past the old ones. A
    /// change a full build would refuse is a miss, and the next full build parses afresh.
    /// </summary>
    [TestClass]
    public class XwmlPatchResourcesTests
    {
        private const string First = "Patch.First.html";

        private const string Second = "Patch.Second.html";

        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }

        private static string Document(string text, string style = ".card { color: red; }")
            => "<!DOCTYPE html>\n"
                + "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"\n"
                + "    xmlns:vm=\"Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test\"\n"
                + "    xmlns:ctrl=\"Sunlight.Framework.UI!Sunlight.Framework.UI\">\n"
                + "<head><style> " + style + " </style></head>\n"
                + "<body>\n"
                + "  <skin ControlType=\"ctrl:UISkinableElement\" DataContextType=\"vm:TestViewModelB\">\n"
                + "    <div class=\"card\">" + text + " {PropStr1}</div>\n"
                + "  </skin>\n"
                + "</body>\n"
                + "</html>\n";

        private static ConverterContext NewSession()
            => new ConverterContext(Helper.GetClrContext(), null, null) { DevMode = true };

        private static string Build(ConverterContext session, out XwmlTemplatingPlugin plugin, string secondText)
        {
            session.BeginBuild(null, null);
            plugin = Helper.CreatePlugin(null, session);
            plugin.CodeGenerator.AddResource(Resource(First, Document("first")), First);
            plugin.CodeGenerator.AddResource(Resource(Second, Document(secondText)), Second);
            Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(First));
            Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(Second));
            plugin.GetMethodsToEmitPass1();
            var code = Helper.ConvertCodeToString(plugin.GetPostJavascript());
            Assert.AreEqual(0, session.Errors.Count, "the build must not fail");
            return code;
        }

        private static EmbeddedResource Resource(string name, string text)
            => new EmbeddedResource(name, ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(text));

        /// <summary>The build's resources with <see cref="Second"/> replaced.</summary>
        private static Dictionary<string, Tuple<EmbeddedResource, string>> WithSecond(XwmlTemplatingPlugin plugin, string document)
            => new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [Second] = Tuple.Create(Resource(Second, document), Second),
            };

        private static string Css(string code)
            => Regex.Match(code, "textContent = (\".*?\");").Groups[1].Value;

        /// <summary>The CSS a patch writes again in place, as the bundle's literal.</summary>
        private static string Css(Dictionary<string, string> literals)
            => literals.TryGetValue(CodeGenerator.CssMarker, out var css) ? "\"" + Utils.ToJSString(css) + "\"" : string.Empty;

        [TestMethod]
        public void ChangedDocument_PatchKeepsNamesAndClasses_RefusedChangeMisses_NextFullBuildIsFresh()
        {
            var session = NewSession();
            var cold = Build(session, out var plugin, "second");

            Assert.IsTrue(
                Regex.IsMatch(cold, Regex.Escape(CodeGenerator.CssMarker) + @"\r?\n *style\.textContent = """),
                "the CSS line is marked");
            var statements = new List<Statement>();
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(statements, literals, WithSecond(plugin, Document("changed"))));
            var fragment = Helper.ConvertCodeToString(statements);

            StringAssert.Contains(fragment, "changed");
            Assert.IsFalse(fragment.Contains("First_factory"), "an unchanged document is not written again");
            var declared = Regex.Matches(fragment, @"function [\w$]+\(");
            Assert.AreEqual(2, declared.Count, "the factory and the getter: " + fragment);
            foreach (Match function in declared)
            {
                Assert.IsTrue(cold.Contains(function.Value), "the bundle must already call " + function.Value);
            }

            StringAssert.Contains(fragment, "\"2\")", "the skin takes the storage index past the old ones");
            Assert.IsFalse(fragment.Contains("textContent"), "the document initializer stands");
            Assert.AreEqual(0, literals.Count, "the CSS did not change: it is not written again");

            // A changed style writes the CSS again in place, the classes keeping their names.
            statements = new List<Statement>();
            Assert.IsNull(plugin.TryPatchResources(statements, literals, WithSecond(plugin, Document("changed", ".card { color: green; }"))));
            Assert.IsFalse(Helper.ConvertCodeToString(statements).Contains("textContent"), "the document initializer stands");
            Assert.AreNotEqual(string.Empty, Css(cold));
            Assert.AreNotEqual(Css(cold), Css(literals));
            Assert.AreEqual(Css(cold), Css(literals).Replace("green", "red"), "the classes must keep their names");

            // The skin uses a class its style no longer declares: a full build reports it.
            Assert.IsNotNull(plugin.TryPatchResources(
                new List<Statement>(),
                new Dictionary<string, string>(),
                WithSecond(plugin, Document("changed", ".other { color: blue; }"))));

            var full = Build(session, out plugin, "changed");
            Assert.IsFalse(plugin.CodeGenerator.ReusedParse, "a patched parse is never kept for a full build");
            Assert.AreEqual(Build(NewSession(), out _, "changed"), full);
        }

        /// <summary>
        /// S5: with the live patch hook, a linked sheet's colour edit changes no skin code, and
        /// its live form restyles the documents open now and the ones made later. The CSS line
        /// keeps its one literal, so the bundle is still patched in place.
        /// </summary>
        [TestMethod]
        public void SkinEdits_PatchUntilTheNextIndexIsRazors_ThenMiss()
        {
            // The two skins take 0 and 1; each patch of the second takes the next index. Razor's
            // skins share the per-document DOM cache from LiveSkinHook.RazorStorageBase up.
            var session = NewSession();
            Build(session, out var plugin, "second");
            string last = null, miss = null;
            int patch = 0;
            while (miss == null && patch < LiveSkinHook.RazorStorageBase)
            {
                var statements = new List<Statement>();
                miss = plugin.TryPatchResources(statements, new Dictionary<string, string>(), WithSecond(plugin, Document("edit " + ++patch)));
                last = miss == null ? Helper.ConvertCodeToString(statements) : last;
            }

            Assert.AreEqual(LiveSkinHook.RazorStorageBase - 1, patch, "the patch that would take index " + LiveSkinHook.RazorStorageBase + " misses");
            Assert.AreEqual("xwml storage index " + LiveSkinHook.RazorStorageBase + " reaches Razor's", miss);
            StringAssert.Contains(last, "\"" + (LiveSkinHook.RazorStorageBase - 1) + "\")", "the last patch took the index below Razor's");
        }

        [TestMethod]
        public void LinkedSheet_ColourEdit_WithLiveHook_GoesLiveForOpenAndLaterDocuments()
        {
            const string A = "Patch.Live.css";
            var linked = Document("linked").Replace(
                "<style> .card { color: red; } </style>",
                "<link rel=\"stylesheet\" href=\"" + A + "\" />");
            string ColdBuild(bool liveHook, out XwmlTemplatingPlugin built)
            {
                var session = NewSession();
                session.LivePatchHook = liveHook;
                session.BeginBuild(null, null);
                built = Helper.CreatePlugin(null, session);
                built.CodeGenerator.AddResource(Resource(A, ".card { color: red; }"), A);
                built.CodeGenerator.AddResource(Resource(First, linked), First);
                Assert.IsNotNull(built.CodeGenerator.GetTemplateGetterIdentifier(First));
                built.GetMethodsToEmitPass1();
                var code = Helper.ConvertCodeToString(built.GetPostJavascript());
                Assert.AreEqual(0, session.Errors.Count, "the build must not fail");
                return code;
            }

            var cold = ColdBuild(true, out var plugin);
            var getter = Regex.Match(cold, @"function (DocStorageGetter[\w$]*)\(").Groups[1].Value;
            Assert.AreNotEqual(string.Empty, getter, cold);
            Assert.IsTrue(
                Regex.IsMatch(cold, Regex.Escape(CodeGenerator.CssMarker) + @"\r?\n *style\.textContent = " + Regex.Escape(getter) + @"\.__nsCss \|\| ""[^""\r\n]*red[^""\r\n]*"";\r?\n"),
                "the marked CSS line reads a live patch's CSS first, and keeps one literal: " + cold);
            StringAssert.Contains(cold, "(" + getter + ".__nsStyles || (" + getter + ".__nsStyles = [])).push(style);");

            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [A] = Tuple.Create(Resource(A, ".card { color: green; }"), A),
            };
            var statements = new List<Statement>();
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(statements, literals, resources));
            Assert.AreEqual(0, statements.Count, "a linked sheet's colour edit changes no skin code");
            StringAssert.Contains(literals[CodeGenerator.CssMarker], "green");

            var root = ((SimpleIdentifier)plugin.CodeGenerator.GetDocumentStorageGetter()).OwnerScope;
            Assert.IsNull(plugin.LiveLiteral("/*@L Other*/", "x", root), "another plugin's literal is not XWML's to take live");
            var live = Helper.ConvertCodeToString(plugin.LiveLiteral(CodeGenerator.CssMarker, literals[CodeGenerator.CssMarker], root));
            StringAssert.Contains(live, getter + ".__nsCss = \"" + Utils.ToJSString(literals[CodeGenerator.CssMarker]) + "\";");
            StringAssert.Contains(live, "(" + getter + ".__nsStyles || []).forEach(function(s){s.textContent=String(this);}, " + getter + ".__nsCss);");

            // A plain dev bundle has no hook, so its CSS edit reloads.
            var plain = ColdBuild(false, out var plainPlugin);
            Assert.IsFalse(plain.Contains("__ns"), "no hook without live patch emit: " + plain);
            Assert.IsNull(plainPlugin.LiveLiteral(CodeGenerator.CssMarker, "x", root));
        }

        /// <summary>
        /// S7: with the live hook a skin edit runs live. Each factory's instance is kept on its
        /// skin, and the patch's <c>Name_var</c> statement gives the skin made before it the
        /// new factory and skins its open controls again. Plain dev bundles are as before.
        /// </summary>
        [TestMethod]
        public void SkinEdit_WithLiveHook_SkinsOpenControlsAgain()
        {
            string ColdBuild(bool liveHook, out XwmlTemplatingPlugin built)
            {
                var session = NewSession();
                session.LivePatchHook = liveHook;
                session.BeginBuild(null, null);
                built = Helper.CreatePlugin(null, session);
                built.CodeGenerator.AddResource(Resource(First, Document("first")), First);
                Assert.IsNotNull(built.CodeGenerator.GetTemplateGetterIdentifier(First));
                built.GetMethodsToEmitPass1();
                var code = Helper.ConvertCodeToString(built.GetPostJavascript());
                Assert.AreEqual(0, session.Errors.Count, "the build must not fail");
                return code;
            }

            var cold = ColdBuild(true, out var plugin);
            var getter = Regex.Match(cold, @"function (DocStorageGetter[\w$]*)\(").Groups[1].Value;
            var reskin = Regex.Match(cold, @"if \(([\w$]+_var[\w$]*)\)\s*\{?\s*" + Regex.Escape(getter) + @"\.__nsReskin\(\1, ([\w$]+_factory[\w$]*), ""(\d+)"", ""First""\);");
            Assert.IsTrue(reskin.Success, "the skin variable's statement skins open controls again: " + cold);
            Assert.IsFalse(cold.Contains(reskin.Groups[1].Value + " = null;"), "the hook takes the place of the null start");
            StringAssert.Contains(cold, "return " + getter + ".__nsTrack(");
            Assert.IsTrue(
                Regex.IsMatch(cold, Regex.Escape(getter) + @"\.__nsReskin = \(function\(F,I,P,K,set\)[^\n]*\}\)\(""factoryMethod[\w$]*"", ""id[\w$]*"", ""skinableParent[\w$]*"", ""skinInstance[\w$]*"", function\("),
                "the helper gets the framework's field names as resolved, and a Skin setter: " + cold);
            Assert.IsTrue(plugin.PatchSectionLive);

            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [First] = Tuple.Create(Resource(First, Document("edited")), First),
            };
            var statements = new List<Statement>();
            Assert.IsNull(plugin.TryPatchResources(statements, new Dictionary<string, string>(), resources));
            var patch = Helper.ConvertCodeToString(statements);
            StringAssert.Contains(patch, "edited");
            StringAssert.Contains(patch, getter + ".__nsReskin(" + reskin.Groups[1].Value + ", " + reskin.Groups[2].Value + ", \"");
            StringAssert.Contains(patch, "return " + getter + ".__nsTrack(", "the patch's factory keeps its instances too");

            // A plain dev bundle has no hook, so its skin edit reloads.
            var plain = ColdBuild(false, out var plainPlugin);
            Assert.IsFalse(plain.Contains("__ns"), "no hook without live patch emit: " + plain);
            StringAssert.Contains(plain, "_var = null;");
            Assert.IsFalse(plainPlugin.PatchSectionLive);
        }

        [TestMethod]
        public void LinkedSheet_ClassAddedThatALaterSheetDeclares_Misses()
        {
            // A cold build parses A first, so B's .card would redeclare it: the patch must not
            // give A a new name for .card while B and the skin keep theirs.
            const string A = "Patch.A.css", B = "Patch.B.css";
            var linked = Document("linked").Replace(
                "<style> .card { color: red; } </style>",
                "<link rel=\"stylesheet\" href=\"" + A + "\" /><link rel=\"stylesheet\" href=\"" + B + "\" />");
            var session = NewSession();
            session.BeginBuild(null, null);
            var plugin = Helper.CreatePlugin(null, session);
            plugin.CodeGenerator.AddResource(Resource(A, ".a { color: red; }"), A);
            plugin.CodeGenerator.AddResource(Resource(B, ".card { color: blue; }"), B);
            plugin.CodeGenerator.AddResource(Resource(First, linked), First);
            Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(First));
            plugin.GetMethodsToEmitPass1();
            Helper.ConvertCodeToString(plugin.GetPostJavascript());
            Assert.AreEqual(0, session.Errors.Count, "the build must not fail");

            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [A] = Tuple.Create(Resource(A, ".a { color: red; } .card { color: green; }"), A),
            };
            Assert.AreEqual(
                "xwml class added " + A,
                plugin.TryPatchResources(new List<Statement>(), new Dictionary<string, string>(), resources));

            // The full build that follows reports what a cold build reports.
            var cold = NewSession();
            cold.BeginBuild(null, null);
            var coldPlugin = Helper.CreatePlugin(null, cold);
            string coldError = null;
            try
            {
                coldPlugin.CodeGenerator.AddResource(resources[A].Item1, A);
                coldPlugin.CodeGenerator.AddResource(Resource(B, ".card { color: blue; }"), B);
                coldPlugin.CodeGenerator.AddResource(Resource(First, linked), First);
                coldPlugin.CodeGenerator.GetTemplateGetterIdentifier(First);
                coldPlugin.GetMethodsToEmitPass1();
                Helper.ConvertCodeToString(coldPlugin.GetPostJavascript());
            }
            catch (Exception ex)
            {
                coldError = ex.Message;
            }

            Assert.IsTrue(coldError != null || cold.Errors.Count > 0, "a cold build refuses .card declared in both sheets");
        }

        [TestMethod]
        public void ChangedStyle_PatchPrefixesOnlyItsCss()
        {
            // More pieces of CSS than the prefixer once kept results for.
            var session = NewSession();
            session.BeginBuild(null, null);
            var plugin = Helper.CreatePlugin(null, session);
            var names = new List<string>();
            for (int i = 0; i < 12; i++)
            {
                names.Add("Patch.Many" + i + ".html");
                plugin.CodeGenerator.AddResource(Resource(names[i], Document("many" + i)), names[i]);
                Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(names[i]));
            }

            plugin.GetMethodsToEmitPass1();
            Helper.ConvertCodeToString(plugin.GetPostJavascript());
            Assert.AreEqual(0, session.Errors.Count, "the build must not fail");

            int runs = CssStyleSheet.Compiler.Runs;
            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [names[5]] = Tuple.Create(Resource(names[5], Document("many5", ".card { color: green; }")), names[5]),
            };
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, resources));
            StringAssert.Contains(Css(literals), "green");
            Assert.AreEqual(1, CssStyleSheet.Compiler.Runs - runs, "only the changed style is prefixed again");
        }

    }
}
