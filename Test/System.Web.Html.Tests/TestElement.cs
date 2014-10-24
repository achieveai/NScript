//-----------------------------------------------------------------------
// <copyright file="Class.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Tests
{
    using System.Web.Html;
    using SunlightUnit;

    /// <summary>
    /// Definition for TestClass
    /// </summary>
    [TestFixture]
    public class TestElement
    {
        [TestSetup]
        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        public static void Setup()
        {
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestCreateElement()
        {
            Element element = Window.Instance.Document.CreateElement("div");
            Assert.NotEqual(null, element, "element should not be null.");
            Assert.Equal("DIV", element.TagName, "TagName of element");
        }

        [Test]
        public static void TestAttribute()
        {
            DivElement element = Window.Instance.Document.CreateElement("div").As<DivElement>();
            element.SetAttribute("_id", "test");

            var attributes = element.Attributes;
            Assert.Equal(1, attributes.Length, "attributes.Length");
            Assert.Equal("_id", attributes[0].Name, "attributes[0].Name");
            Assert.Equal("test", attributes[0].Value, "attributes[0].Value");
        }

        [Test]
        public static void TestEventBinding()
        {
            Element element = Window.Instance.Document.CreateElement("div");
            element.TextContent = "Foo";

            Window.Instance.Document.Body.AppendChild(element);

            bool handlerCalled = false;
            string eventType = null;
            Action<Element, ElementEvent> handler = delegate(Element elem, ElementEvent evt)
            {
                handlerCalled = true;
                eventType = evt.Type;
            };

            element.Bind("click", handler);

            MutableEvent testEvt = Window.Instance.Document.CreateEvent("MouseEvent");
            testEvt.InitMouseEvent(
                "click",
                true,
                true,
                Window.Instance,
                "",
                0,
                0,
                0,
                0,
                false,
                false,
                false,
                false,
                "",
                element);

            element.DispatchEvent(testEvt);

            Assert.IsTrue(handlerCalled, "Handler should be called");
            Assert.Equal("click", eventType, "EventType");

            element.UnBind("click", handler);
            handlerCalled = false;
            eventType = null;

            element.DispatchEvent(testEvt);
            Assert.IsTrue(!handlerCalled, "Handler should not be called");
        }
    }
}