//-----------------------------------------------------------------------
// <copyright file="SkinCodeGenerationBasicTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for SkinCodeGenerationBasicTest
    /// </summary>
    [TestFixture]
    public class SkinCodeGenerationBasicTest
    {
        XwmlParser.XwmlTemplatingPlugin plugin;
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Helper.Initialize();
        }

        /// <summary>
        /// Tests parser 1.
        /// </summary>
        /// <param name="bindingStr"> The binding string. </param>
        [Test]
        [TestCase("TestCss", null)]
        [TestCase("TestAttributeBinding", null)]
        [TestCase("TestSkinWithId", "test")]
        [TestCase("TestTemplate1", null)]
        [TestCase("TestTextBinding1", null)]
        [TestCase("TestCssBinding1", null)]
        [TestCase("TestDomEventBinding1", null)]
        [TestCase("TestUIElement", null)]
        [TestCase("TestUIElementWithTag", null)]
        [TestCase("TestUIElementWithAttr", null)]
        [TestCase("TestUIElementPropertyBinding", null)]
        [TestCase("TestUIElementPropertyAttrBinding", null)]
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
            Helper.CheckCode(name + ".js", code);
        }
    }
}
