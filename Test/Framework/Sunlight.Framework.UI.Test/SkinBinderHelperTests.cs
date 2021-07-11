//-----------------------------------------------------------------------
// <copyright file="SkinBinderHelperTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.UI.Helpers;
    using System.Web.Html;

    /// <summary>
    /// Definition for SkinBinderHelperTests
    /// </summary>
    [TestClass]
    public class SkinBinderHelperTests
    {
        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestSimpleBinder(Assert assert)
        {
            TestViewModelA src = new TestViewModelA()
                {
                    PropStr1 = "Test",
                    PropInt1 = 1,
                    PropBool1 = true
                };
            TestViewModelA tar1 = new TestViewModelA();
            SkinBinderHelper.Bind(
                new SkinBinderInfo[]{
                    new SkinBinderInfo(
                        new Func<object,object>[] {TestViewModelA.PropStr1Getter},
                        TestViewModelA.PropStr1Setter,
                        BinderType.DataContext | BinderType.PropertyBinder,
                        0,
                        null,
                        null)
                },
                src,
                new Object[]{
                    tar1
                });

            assert.Equal(src.PropStr1, tar1.PropStr1, "After BindDataContext values should be equal");
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestAttrBinding(Assert assert)
        {
            TestViewModelA src = new TestViewModelA()
                {
                    PropStr1 = "Test",
                };
            System.Web.Html.Element target = Window.Instance.Document.CreateElement("div");
            SkinBinderHelper.Bind(
                new SkinBinderInfo[]{
                    new SkinBinderInfo(
                        new Func<object,object>[] {TestViewModelA.PropStr1Getter},
                        delegate(object o1, object o2, object o3) {
                            SkinBinderHelper.SetAttribute((Node)o1, (string)o2, (string)o3);
                        },
                        "tmp",
                        BinderType.DataContext | BinderType.AttributeBinder,
                        0,
                        null,
                        null,
                        0)
                },
                src,
                new Object[]{
                    target
                });

            assert.Equal(src.PropStr1, target.GetAttribute("tmp"), "After BindDataContext values should be equal");
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestStyleBinding(Assert assert)
        {
            TestViewModelA src = new TestViewModelA()
                {
                    PropBool1 = true,
                };
            System.Web.Html.Element target = Window.Instance.Document.CreateElement("div");
            target.ClassName = "t1";
            SkinBinderHelper.Bind(
                new SkinBinderInfo[]{
                    new SkinBinderInfo(
                        new Func<object,object>[] {TestViewModelA.PropBool1Getter},
                        delegate(object o1, object o2, object o3) {
                            SkinBinderHelper.SetCssClass((Element)o1, (bool)o2, (string)o3);
                        },
                        "test",
                        BinderType.DataContext | BinderType.AttributeBinder,
                        0,
                        null,
                        null,
                        0)
                },
                src,
                new Object[]{
                    target
                });

            assert.Equal("t1 test", target.ClassName, "After BindDataContext values should be equal");

            src.PropBool1 = false;
            SkinBinderHelper.Bind(
                new SkinBinderInfo[]{
                    new SkinBinderInfo(
                        new Func<object,object>[] {TestViewModelA.PropBool1Getter},
                        delegate(object o1, object o2, object o3) {
                            SkinBinderHelper.SetCssClass((Element)o1, (bool)o2, (string)o3);
                        },
                        "test",
                        BinderType.DataContext | BinderType.AttributeBinder,
                        0,
                        null,
                        null,
                        0)
                },
                src,
                new Object[]{
                    target
                });

            assert.Equal("t1", target.ClassName, "After BindDataContext values should be equal");
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestTextContentBinding(Assert assert)
        {
            TestViewModelA src = new TestViewModelA()
                {
                    PropStr1 = "Test",
                };
            System.Web.Html.Element target = Window.Instance.Document.CreateElement("div");
            SkinBinderHelper.Bind(
                new SkinBinderInfo[]{
                    new SkinBinderInfo(
                        new Func<object,object>[] {TestViewModelA.PropStr1Getter},
                        delegate(object o1, object o2) {
                            SkinBinderHelper.SetTextContent((Element)o1, (string)o2);
                        },
                        BinderType.DataContext | BinderType.PropertyBinder,
                        0,
                        null,
                        null)
                },
                src,
                new Object[]{
                    target
                });

            assert.Equal(src.PropStr1, target.TextContent, "After BindDataContext values should be equal");
        }

    }
}

