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
        public static void Test(Assert assert)
        {
            assert.NotEqual(null, NScriptsTemplatesClass.TestTemplate1, "Template should not be null");
            assert.IsTrue(true, "true should be true");
        }

        /// <summary>
        /// Tests apply skin.
        /// </summary>
        [Test]
        public static void TestApplySkin(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            control.DataContext = new TestViewModelA();
            control.Skin = NScriptsTemplatesClass.TestTemplate1;

            control.Activate();

            assert.NotEqual(null, element.QuerySelector("[test]"), "After applying skin, skin element should be loaded");
        }

        /// <summary>
        /// Tests CSS binder.
        /// </summary>
        [Test]
        public static void TestCssBinder(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_CssBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            assert.NotEqual(null, elem, "After applying skin, skin element should be loaded");
            assert.Equal("", elem.ClassName, "Class should not be set if PropBool1 is not set.");

            vm.PropBool1 = true;
            assert.NotEqual("", elem.ClassName, "Class should be set if PropBool1 is set.");
        }

        /// <summary>
        /// Tests style binder.
        /// </summary>
        [Test]
        public static void TestStyleBinder(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_StyleBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            assert.NotEqual(null, elem, "After applying skin, skin element should be loaded");
            assert.Equal("", elem.Style.Height, "Style should not be set if PropStr1 is not set.");

            vm.PropStr1 = "10px";
            assert.Equal("10px", elem.Style.Height, "Style should be set if PropStr1 is set.");
        }

        /// <summary>
        /// Tests attribute binder.
        /// </summary>
        [Test]
        public static void TestAttrBinder(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            UISkinableElement control = new UISkinableElement(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateVMB_AttrBinding;

            control.Activate();

            var elem = element.QuerySelector("[test]");
            assert.NotEqual(elem, null, "After applying skin, skin element should be loaded");
            assert.Equal(elem.GetAttribute("test1"), null, "Attribute 'test' should not be set if PropStr1 is not set.");

            vm.PropStr1 = "TTTest";
            assert.Equal("TTTest", elem.GetAttribute("test1"), "Attribute 'test' should be set if PropStr1 is set.");
        }

        [Test]
        public static void TestPropertyBinder(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            TestSkinableWithTestUIElementPart control = new TestSkinableWithTestUIElementPart(element);

            var vm = new TestViewModelB();
            control.DataContext = vm;
            control.Skin = NScriptsTemplatesClass.TestTemplateB_PropertyBinding;

            control.Activate();

            assert.IsTrue(control.Part != null, "templatePart should not be null.");
            assert.Equal(control.Part.OneWayStrictBinding, vm.PropStr1, "vmPropStr1 should be equal to OnewayStrictBinding.");
            vm.PropStr1 = "T1";
            assert.Equal(control.Part.OneWayStrictBinding, vm.PropStr1, "vmPropStr1 should be equal to OnewayStrictBinding.");

            assert.Equal(control.Part.TwoWayLooseBinding, vm.PropInt1, "TwoWayLooseBinding and bound property PropInt1 should be equal.");
            vm.PropInt1 = 11;
            assert.Equal(control.Part.TwoWayLooseBinding, vm.PropInt1, "TwoWayLooseBinding and bound property PropInt1 should be equal.");
            control.Part.TwoWayLooseBinding = 101;
            assert.Equal(control.Part.TwoWayLooseBinding, vm.PropInt1, "TwoWayLooseBinding and bound property PropInt1 should be equal.");
        }
    }
}

