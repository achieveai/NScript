//-----------------------------------------------------------------------
// <copyright file="XwmlParseReuseTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.Converter;
    using System.Text;

    /// <summary>
    /// A build session keeps the XWML parse for its next build (dev mode). A build that reuses
    /// it must equal one that parses afresh, byte for byte. Both documents here define and use
    /// the class <c>card</c>, so the document order sets the CSS class names.
    /// </summary>
    [TestClass]
    public class XwmlParseReuseTests
    {
        private const string First = "Reuse.First.html";

        private const string Second = "Reuse.Second.html";

        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }

        private static string Document(string text)
            => "<!DOCTYPE html>\n"
                + "<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"\n"
                + "    xmlns:vm=\"Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test\"\n"
                + "    xmlns:ctrl=\"Sunlight.Framework.UI!Sunlight.Framework.UI\">\n"
                + "<head><style> .card { color: red; } </style></head>\n"
                + "<body>\n"
                + "  <skin ControlType=\"ctrl:UISkinableElement\" DataContextType=\"vm:TestViewModelB\">\n"
                + "    <div class=\"card\">" + text + " {PropStr1}</div>\n"
                + "  </skin>\n"
                + "</body>\n"
                + "</html>\n";

        private static ConverterContext NewSession()
            => new ConverterContext(Helper.GetClrContext(), null, null) { DevMode = true };

        /// <summary>One build on <paramref name="session"/>: asks for the documents in order, returns the code.</summary>
        private static string Build(
            ConverterContext session,
            out XwmlTemplatingPlugin plugin,
            string secondText,
            params string[] documents)
        {
            session.BeginBuild(null, null);
            plugin = Helper.CreatePlugin(null, session);
            plugin.CodeGenerator.AddResource(Resource(First, Document("first")), First);
            plugin.CodeGenerator.AddResource(Resource(Second, Document(secondText)), Second);
            foreach (var document in documents)
            {
                Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(document));
            }

            plugin.GetMethodsToEmitPass1();
            var code = Helper.ConvertCodeToString(plugin.GetPostJavascript());
            Assert.AreEqual(0, session.Errors.Count, "the build must not fail");
            return code;
        }

        private static EmbeddedResource Resource(string name, string text)
            => new EmbeddedResource(name, ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(text));

        private static string Fresh(string secondText, params string[] documents)
            => Build(NewSession(), out _, secondText, documents);

        [TestMethod]
        public void KeptParse_IsReusedWhenUnchanged_RedoneOnChangeOrNewOrder_AlwaysEqualsAFreshParse()
        {
            var session = NewSession();

            var cold = Build(session, out var plugin, "second", First, Second);
            Assert.IsFalse(plugin.CodeGenerator.ReusedParse);
            Assert.AreEqual(Fresh("second", First, Second), cold);

            var warm = Build(session, out plugin, "second", First, Second);
            Assert.IsTrue(plugin.CodeGenerator.ReusedParse, "an unchanged build must reuse the kept parse: " + plugin.CodeGenerator.ReuseMiss);
            Assert.AreEqual(2, plugin.CodeGenerator.ProbeReusedDocuments);
            Assert.AreEqual(0, plugin.CodeGenerator.ProbeDocuments, "no document is read again");
            Assert.AreEqual(cold, warm, "a reused parse must give the code a fresh parse gives");
            Assert.IsNull(session.RetryReason);

            // The order sets the class names, so this case means something.
            var swappedFresh = Fresh("second", Second, First);
            Assert.AreNotEqual(cold, swappedFresh);
            Build(session, out plugin, "second", Second, First);
            Assert.IsTrue(plugin.CodeGenerator.ReusedParse);
            Assert.AreEqual("xwml-document-order", session.RetryReason, "a reused parse in another order must ask the builder to retry");
            var retried = Build(session, out plugin, "second", Second, First);
            Assert.IsFalse(plugin.CodeGenerator.ReusedParse, "the retry must not reuse the parse again");
            Assert.AreEqual(swappedFresh, retried);

            var changed = Build(session, out plugin, "changed", Second, First);
            Assert.IsFalse(plugin.CodeGenerator.ReusedParse, "a changed document must be parsed afresh");
            Assert.AreEqual("changed " + Second, plugin.CodeGenerator.ReuseMiss);
            Assert.AreEqual(Fresh("changed", Second, First), changed);
        }
    }
}
