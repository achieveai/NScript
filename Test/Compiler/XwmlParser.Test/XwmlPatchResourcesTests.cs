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
