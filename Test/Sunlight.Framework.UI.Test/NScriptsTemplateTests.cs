//-----------------------------------------------------------------------
// <copyright file="NScriptsTemplateTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Definition for NScriptsTemplateTests
    /// </summary>
    [TestFixture]
    public class NScriptsTemplateTests
    {
        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(
                new TestWindowTimer(),
                10,
                10);
        }

        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        [Test]
        public static void Test()
        {
            Assert.NotEqual(null, NScriptsTemplatesClass.TestTemplate1, "Template should not be null");
            Assert.IsTrue(true, "true should be true");
        }

        /// <summary>
        /// Tests apply skin.
        /// </summary>
        [Test]
        public static void TestApplySkin()
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            control.DataContext = new TestViewModelA();
            control.Skin = NScriptsTemplatesClass.TestTemplate1;

            control.Activate();

            Assert.NotEqual(null, element.QuerySelector("[test]"), "After applying skin, skin element should be loaded");
        }

        /// <summary>
        /// Tests CSS binder.
        /// </summary>
        [Test]
        public static void TestCssBinder()
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_CssBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            Assert.NotEqual(null, elem, "After applying skin, skin element should be loaded");
            Assert.Equal("", elem.ClassName, "Class should not be set if PropBool1 is not set.");

            vm.PropBool1 = true;
            Assert.NotEqual("", elem.ClassName, "Class should be set if PropBool1 is set.");
        }

        /// <summary>
        /// Tests style binder.
        /// </summary>
        [Test]
        public static void TestStyleBinder()
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_StyleBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            Assert.NotEqual(null, elem, "After applying skin, skin element should be loaded");
            Assert.Equal("", elem.Style.Height, "Style should not be set if PropStr1 is not set.");

            vm.PropStr1 = "10px";
            Assert.Equal("10px", elem.Style.Height, "Style should be set if PropStr1 is set.");
        }

        /// <summary>
        /// Tests attribute binder.
        /// </summary>
        [Test]
        public static void TestAttrBinder()
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_AttrBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            Assert.NotEqual(elem, null, "After applying skin, skin element should be loaded");
            Assert.Equal(elem.GetAttribute("test1"), null, "Attribute 'test' should not be set if PropStr1 is not set.");

            vm.PropStr1 = "TTTest";
            Assert.Equal("TTTest", elem.GetAttribute("test1"), "Attribute 'test' should be set if PropStr1 is set.");
        }
    }
}

