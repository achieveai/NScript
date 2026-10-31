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

        /// <summary>A dev bundle's CSS part, as its literal.</summary>
        private static string CssPart(string code, int part)
            => Regex.Match(code, Regex.Escape(CodeGenerator.CssPartMarker(part)) + "\\r?\\n *style\\.textContent = (\".*?\");").Groups[1].Value;

        /// <summary>The one CSS part a patch writes again in place, as the bundle's literal.</summary>
        private static string Css(Dictionary<string, string> literals)
        {
            Assert.IsTrue(literals.Count <= 1, "one CSS part changed");
            foreach (var css in literals.Values)
            {
                return "\"" + Utils.ToJSString(css) + "\"";
            }

            return string.Empty;
        }

        [TestMethod]
        public void ChangedDocument_PatchKeepsNamesAndClasses_RefusedChangeMisses_NextFullBuildIsFresh()
        {
            var session = NewSession();
            var cold = Build(session, out var plugin, "second");

            Assert.IsTrue(
                Regex.IsMatch(cold, Regex.Escape(CodeGenerator.CssPartMarker(1)) + @"\r?\n *style\.textContent = """),
                "each document's CSS line is marked");
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
            Assert.AreNotEqual(string.Empty, CssPart(cold, 1));
            Assert.AreNotEqual(CssPart(cold, 1), Css(literals));
            Assert.AreEqual(CssPart(cold, 1), Css(literals).Replace("green", "red"), "the classes must keep their names");

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
        public void SkinEdits_PatchPastAHundred_EachTakesTheNextIndex()
        {
            // The two skins take 0 and 1; each patch of the second takes the next index. The
            // per-document DOM cache is XWML's alone, so the numbering has no ceiling.
            var session = NewSession();
            Build(session, out var plugin, "second");
            for (int patch = 1; patch <= 150; patch++)
            {
                var statements = new List<Statement>();
                Assert.IsNull(
                    plugin.TryPatchResources(statements, new Dictionary<string, string>(), WithSecond(plugin, Document("edit " + patch))),
                    "patch " + patch);
                StringAssert.Contains(Helper.ConvertCodeToString(statements), "\"" + (1 + patch) + "\")", "patch " + patch + " takes the next index");
            }
        }

        /// <summary>
        /// A large app's CSS save: a bundle with more than a hundred skins takes a linked
        /// sheet's colour edit as a patch.
        /// </summary>
        [TestMethod]
        public void LinkedSheet_ColourEdit_InABundleOfMoreThanAHundredSkins_IsAPatch()
        {
            const string A = "Patch.Large.css";
            var session = NewSession();
            session.BeginBuild(null, null);
            var plugin = Helper.CreatePlugin(null, session);
            plugin.CodeGenerator.AddResource(Resource(A, ".card { color: red; }"), A);
            for (int i = 0; i < 101; i++)
            {
                var name = "Patch.Large" + i + ".html";
                plugin.CodeGenerator.AddResource(
                    Resource(name, Document("large" + i).Replace(
                        "<style> .card { color: red; } </style>",
                        "<link rel=\"stylesheet\" href=\"" + A + "\" />")),
                    name);
                Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(name));
            }

            plugin.GetMethodsToEmitPass1();
            StringAssert.Contains(Helper.ConvertCodeToString(plugin.GetPostJavascript()), "\"100\")", "the 101st skin takes index 100");
            Assert.AreEqual(0, session.Errors.Count, "the build must not fail");

            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [A] = Tuple.Create(Resource(A, ".card { color: green; }"), A),
            };
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, resources));
            StringAssert.Contains(Css(literals), "green");
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
                Regex.IsMatch(cold, Regex.Escape(CodeGenerator.CssPartMarker(0)) + @"\r?\n *style\.textContent = " + Regex.Escape(getter) + @"\.__nsCss && 0 in " + Regex.Escape(getter) + @"\.__nsCss \? " + Regex.Escape(getter) + @"\.__nsCss\[0\] : ""[^""\r\n]*red[^""\r\n]*"";\r?\n"),
                "the marked CSS line reads a live patch's CSS first, even an emptied part, and keeps one literal: " + cold);
            StringAssert.Contains(cold, "(" + getter + ".__nsStyles || (" + getter + ".__nsStyles = [])).push(style);");

            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [A] = Tuple.Create(Resource(A, ".card { color: green; }"), A),
            };
            var statements = new List<Statement>();
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(statements, literals, resources));
            Assert.AreEqual(0, statements.Count, "a linked sheet's colour edit changes no skin code");
            var marker = CodeGenerator.CssPartMarker(0);
            StringAssert.Contains(literals[marker], "green");

            var root = ((SimpleIdentifier)plugin.CodeGenerator.GetDocumentStorageGetter()).OwnerScope;
            Assert.IsNull(plugin.LiveLiteral("/*@L Other*/", "x", root), "another plugin's literal is not XWML's to take live");
            var live = Helper.ConvertCodeToString(plugin.LiveLiteral(marker, literals[marker], root));
            StringAssert.Contains(live, getter + ".__nsCss[0] = \"" + Utils.ToJSString(literals[marker]) + "\";");
            StringAssert.Contains(live, "(" + getter + ".__nsStyles || []).forEach(function(s){if(s.__nsPart===0)s.textContent=this[0];}, " + getter + ".__nsCss);");

            // A plain dev bundle has no hook, so its CSS edit reloads.
            var plain = ColdBuild(false, out var plainPlugin);
            Assert.IsFalse(plain.Contains("__ns"), "no hook without live patch emit: " + plain);
            Assert.IsNull(plainPlugin.LiveLiteral(marker, "x", root));
        }

        /// <summary>
        /// #123: a dev bundle writes each style sheet and document's CSS in its own style
        /// element, in cascade order, so a sheet's save sends that sheet's CSS alone. A release
        /// bundle keeps one style element.
        /// </summary>
        [TestMethod]
        public void TwoLinkedSheets_DevBundleWritesAPartEach_ASaveSendsItsPartAlone()
        {
            const string A = "Patch.PartA.css";
            const string B = "Patch.PartB.css";
            string Linked(string text, string sheet)
                => Document(text, ".inline" + text + " { margin: 0; }").Replace(
                    "</style>",
                    "</style><link rel=\"stylesheet\" href=\"" + sheet + "\" />");
            string ColdBuild(bool devMode, out XwmlTemplatingPlugin built)
            {
                var session = NewSession();
                session.DevMode = devMode;
                session.LivePatchHook = devMode;
                session.BeginBuild(null, null);
                built = Helper.CreatePlugin(null, session);
                built.CodeGenerator.AddResource(Resource(A, ".card { color: red; }"), A);
                built.CodeGenerator.AddResource(Resource(B, ".card { border: solid blue; }"), B);
                built.CodeGenerator.AddResource(Resource(First, Linked("first", A)), First);
                built.CodeGenerator.AddResource(Resource(Second, Linked("second", B)), Second);
                Assert.IsNotNull(built.CodeGenerator.GetTemplateGetterIdentifier(First));
                Assert.IsNotNull(built.CodeGenerator.GetTemplateGetterIdentifier(Second));
                built.GetMethodsToEmitPass1();
                var code = Helper.ConvertCodeToString(built.GetPostJavascript());
                Assert.AreEqual(0, session.Errors.Count, "the build must not fail");
                return code;
            }

            var cold = ColdBuild(true, out var plugin);
            var getter = Regex.Match(cold, @"function (DocStorageGetter[\w$]*)\(").Groups[1].Value;
            var parts = Regex.Matches(
                cold,
                @"/\*@L XwmlParser\.Css\.(\d+)\*/\r?\n *style\.textContent = " + Regex.Escape(getter)
                    + @"\.__nsCss && \1 in " + Regex.Escape(getter) + @"\.__nsCss \? " + Regex.Escape(getter) + @"\.__nsCss\[(\d+)\] : ""([^""\r\n]*)"";\r?\n *style\.__nsPart = (\d+);");
            Assert.AreEqual(4, parts.Count, "a part for each sheet and each document: " + cold);
            var texts = new List<string>();
            for (int i = 0; i < parts.Count; i++)
            {
                Assert.AreEqual(i.ToString(), parts[i].Groups[1].Value);
                Assert.AreEqual(i.ToString(), parts[i].Groups[2].Value);
                Assert.AreEqual(i.ToString(), parts[i].Groups[4].Value);
                texts.Add(parts[i].Groups[3].Value);
            }

            Assert.IsTrue(texts[0].Contains("red") && !texts[0].Contains("blue"), "sheets first, in order: " + texts[0]);
            Assert.IsTrue(texts[1].Contains("blue") && !texts[1].Contains("red"), texts[1]);
            StringAssert.Contains(texts[2], "inlinefirst");
            StringAssert.Contains(texts[3], "inlinesecond");
            Assert.AreEqual(4, Regex.Matches(cold, Regex.Escape(".push(style);")).Count, "each part's element is kept");

            // B's save sends B's part alone.
            var resources = new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [B] = Tuple.Create(Resource(B, ".card { border: solid green; }"), B),
            };
            var literals = new Dictionary<string, string>();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, resources));
            Assert.AreEqual(1, literals.Count, "only the changed sheet's part");
            var marker = MarkerStatement.ForLiteral("XwmlParser.Css.1");
            StringAssert.Contains(literals[marker], "green");
            Assert.IsFalse(literals[marker].Contains("red"), "A's CSS is not sent again");

            var root = ((SimpleIdentifier)plugin.CodeGenerator.GetDocumentStorageGetter()).OwnerScope;
            var live = Helper.ConvertCodeToString(plugin.LiveLiteral(marker, literals[marker], root));
            StringAssert.Contains(live, getter + ".__nsCss = " + getter + ".__nsCss || { };");
            StringAssert.Contains(live, getter + ".__nsCss[1] = \"" + Utils.ToJSString(literals[marker]) + "\";");
            StringAssert.Contains(live, "(" + getter + ".__nsStyles || []).forEach(function(s){if(s.__nsPart===1)s.textContent=this[1];}, " + getter + ".__nsCss);");
            Assert.IsNull(plugin.LiveLiteral(MarkerStatement.ForLiteral("XwmlParser.Css.9"), "x", root), "no such part");

            // A save that leaves the CSS as it was sends nothing.
            literals.Clear();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [Second] = Tuple.Create(Resource(Second, Linked("second", B).Replace("second {PropStr1}", "edited {PropStr1}")), Second),
            }));
            Assert.AreEqual(0, literals.Count, "no CSS changed");

            // A document keeps its part when its CSS is gone, and takes CSS into it again: both live.
            literals.Clear();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [Second] = Tuple.Create(Resource(Second, Document("second", " ").Replace("</style>", "</style><link rel=\"stylesheet\" href=\"" + B + "\" />")), Second),
            }));
            Assert.AreEqual(1, literals.Count, "the emptied document's part alone");
            Assert.AreEqual(string.Empty, literals[MarkerStatement.ForLiteral("XwmlParser.Css.3")]);
            literals.Clear();
            Assert.IsNull(plugin.TryPatchResources(new List<Statement>(), literals, new Dictionary<string, Tuple<EmbeddedResource, string>>(plugin.CodeGenerator.ResourceMap)
            {
                [Second] = Tuple.Create(Resource(Second, Linked("second", B).Replace(".inlinesecond", ".inlineagain")), Second),
            }));
            Assert.AreEqual(1, literals.Count, "the document's part takes its CSS again");
            StringAssert.Contains(literals[MarkerStatement.ForLiteral("XwmlParser.Css.3")], "inlineagain");

            // A release bundle keeps one unmarked style element with all the CSS, in order.
            var release = ColdBuild(false, out _);
            Assert.IsFalse(release.Contains("/*@L"), release);
            Assert.AreEqual(1, Regex.Matches(release, Regex.Escape(".textContent = ")).Count, release);
            var all = Css(release);
            Assert.IsTrue(all.IndexOf("red") < all.IndexOf("blue") && all.IndexOf("blue") < all.IndexOf("inlinefirst"), all);
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
                Regex.IsMatch(cold, Regex.Escape(getter) + @"\.__nsReskin = \(function\(F,I,P,K,E,set\)[^\n]*\}\)\(""factoryMethod[\w$]*"", ""id[\w$]*"", ""skinableParent[\w$]*"", ""skinInstance[\w$]*"", ""element[\w$]*"", function\("),
                "the helper gets the framework's field names as resolved, and a Skin setter: " + cold);
            Assert.IsTrue(plugin.PatchSectionLive);

            // The console line says what a re-skin resets, and names the template, not its path.
            Assert.IsTrue(cold.Contains("(n?' (reset: focus, caret, scroll, unbound typed text)':'')"), "the console line names what a re-skin resets, only when it re-skinned something");
            Assert.IsTrue(cold.Contains("globalThis.__nsFlush=globalThis.__nsFlush||function(){"), "the helpers define the flush that skins the queued controls again");
            var getterId = plugin.CodeGenerator.GetDocumentStorageGetter();
            var named = Helper.ConvertCodeToString(new List<Statement>
            {
                plugin.CodeGenerator.CreateReskin(((SimpleIdentifier)getterId).OwnerScope, getterId, getterId, "0", @"Test\Templates\L60"),
            });
            Assert.IsTrue(named.Contains(", \"0\", \"L60\");"), "a watch build's template id is its path: " + named.Replace("{", "{{").Replace("}", "}}"));

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
