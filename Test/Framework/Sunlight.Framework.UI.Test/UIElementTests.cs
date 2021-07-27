//-----------------------------------------------------------------------
// <copyright file="UIElementTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Definition for UIElementTests
    /// </summary>
    [TestFixture]
    public class UIElementTests
    {
        [Test]
        public static void TestNewUIElement(Assert assert)
        {
            Document doc = Window.Instance.Document;
            var element = new UIElement(
                doc.CreateElement("div"));

            assert.NotEqual(
                element.Element,
                null,
                "element.Element != null");

            assert.Equal(
                element.Element.TagName,
                "DIV",
                "element.Element.TagName == 'DIV'");
        }
    }
}
