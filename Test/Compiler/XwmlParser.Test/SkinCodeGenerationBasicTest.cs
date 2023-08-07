//-----------------------------------------------------------------------
// <copyright file="SkinCodeGenerationBasicTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Definition for SkinCodeGenerationBasicTest
    /// </summary>
    [TestClass]
    public class SkinCodeGenerationBasicTest
    {
        private XwmlParser.XwmlTemplatingPlugin plugin;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }
        [DataTestMethod]
        [DataRow("TestDuplicateCss", null)]
        public void TestDuplicateCss(string name, string templateId)
        {
            plugin = Helper.CreatePlugin(null);
            Helper.LoadHtmlParser(name + ".html", plugin.ParserContext);
            var identifier = plugin.CodeGenerator.GetTemplateGetterIdentifier(
                (name + ".html")
                + (templateId == null
                    ? ""
                    : ":" + templateId));
            Assert.IsNotNull(identifier);
            plugin.CodeGenerator.IterateParsing();
            plugin.CodeGenerator.GetAllTemplateStatements();
            Assert.IsTrue(plugin.CodeGenerator.HasDuplicateCssRule);
        }

        /// <summary>
        /// Tests parser 1.
        /// </summary>
        /// <param name="bindingStr"> The binding string. </param>
        [DataTestMethod]
        // [DataRow("TestAttributeBinding", null)]
        // [DataRow("TestAttributeBindingNonObservable", null)]
        // [DataRow("TestSkinWithId", "test")]
        // [DataRow("TestTemplate1", null)]
        // [DataRow("TestTextBinding1", null)]
        // [DataRow("TestCssBinding1", null)]
        // [DataRow("TestDomEventBinding1", null)]
        // [DataRow("TestStaticValues", null)]
        // [DataRow("TestSkinPartIds", null)]
        // [DataRow("TestSkinWithDomPartId", null)]
        // [DataRow("TestSkinWithUIElementDomId", null)]
        // [DataRow("TestUIElement", null)]
        // [DataRow("TestUIElementWithTag", null)]
        // [DataRow("TestUIElementWithAttr", null)]
        // [DataRow("TestUIElementPropertyBinding", null)]
        // [DataRow("TestUIElementDataContextBinding", null)]
        // [DataRow("TestUIElementPropertyBinding_TwoWay", null)]
        // [DataRow("TestUIElementPropertyAttrBinding", null)]
        // [DataRow("TestStaticBinding", null)]
        // [DataRow("TestDomEventInFormBinding1", null)]
        // [DataRow("TestTextBinding2", null)]
        // [DataRow("TestTextBindingWithConverter", null)]
        // [DataRow("TestTemplateBinding", null)]
        [DataRow("TestArrayBinding", null)]
        [DataRow("TestCss", null)]
        public void TestParser1(string name, string templateId)
        {
            plugin = Helper.CreatePlugin(null);

            Helper.LoadHtmlParser(name + ".html", plugin.ParserContext);
            var identifier = plugin.CodeGenerator.GetTemplateGetterIdentifier(
                (name + ".html")
                + (templateId == null
                    ? ""
                    : ":" + templateId));
            Assert.IsNotNull(identifier);
            plugin.CodeGenerator.IterateParsing();
            var code = plugin.CodeGenerator.GetAllTemplateStatements();

            //For debugging
            File.WriteAllText("../../XwmlParser.Test/TemplateCode/GeneratedCode.js", Helper.ConvertCodeToString(code).Trim());
            Helper.CheckCode(name + ".js", code);
        }

        // [DataRow("JsName", array of html files, templateID)]
        [DataTestMethod]
        [DataRow("TestRemoveUnusedCss", new string[] { "TestRemoveUnusedCss1.html", "TestRemoveUnusedCss2.html" }, null)]
        public void TestRemoveUnusedCss(string jsName, string[] htmlFiles, string templateId)
        {
            plugin = Helper.CreatePlugin(null);

            foreach (var fileName in htmlFiles)
            {
                var identifier = plugin.CodeGenerator.GetTemplateGetterIdentifier(
                    "Sunlight.Framework.UI.Test.Templates." + fileName
                    + (templateId == null
                        ? ""
                        : ":" + templateId));
                Assert.IsNotNull(identifier);
            }

            plugin.CodeGenerator.IterateParsing();
            var code = plugin.CodeGenerator.GetAllTemplateStatements();

            //For debugging
            File.WriteAllText("../../XwmlParser.Test/TemplateCode/GeneratedCode.js", Helper.ConvertCodeToString(code).Trim());

            Helper.CheckCode(jsName + ".js", code);
        }
    }
}