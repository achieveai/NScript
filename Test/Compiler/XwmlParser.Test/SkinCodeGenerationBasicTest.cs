//-----------------------------------------------------------------------
// <copyright file="SkinCodeGenerationBasicTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using FluentAssertions;

    /// <summary>
    /// Definition for SkinCodeGenerationBasicTest
    /// </summary>
    [TestClass]
    public class SkinCodeGenerationBasicTest
    {
        XwmlParser.XwmlTemplatingPlugin plugin;
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
        }

        /// <summary>
        /// Tests parser 1.
        /// </summary>
        /// <param name="bindingStr"> The binding string. </param>
        [DataTestMethod]
        // [DataRow("TestCss", null)]
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
