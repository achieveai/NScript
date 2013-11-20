//-----------------------------------------------------------------------
// <copyright file="TestListView.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for TestListView
    /// </summary>
    [TestFixture]
    public class TestListView
    {
        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void Test()
        {
            Document document = Window.Instance.Document;
            ListView listView = new ListView(document.CreateElement("div"));
            listView.ItemSkin = NScriptsTemplatesClass.TestTemplate1;

            TestViewModelB[] vmAs = new TestViewModelB[]
            {
                new TestViewModelB() { PropStr1 = "Str"},
                new TestViewModelB() { PropInt1 = 10},
                new TestViewModelB() { PropBool1 = true}
            };

            listView.FixedList = vmAs;
            listView.InactiveIfNullContext = false;
            Assert.Equal(0, listView.Element.Children.Length, "number of children should be 0");

            listView.Activate();
            Assert.Equal(3, listView.Element.Children.Length, "number of children should be 3");
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void TestChildSkin()
        {
            Document document = Window.Instance.Document;
            ListView listView = new ListView(document.CreateElement("div"));
            listView.ItemSkin = NScriptsTemplatesClass.TestTemplateVMB1;

            TestViewModelB[] vmAs = new TestViewModelB[]
            {
                new TestViewModelB() { PropStr1 = "StrTest"},
                new TestViewModelB() { PropStr1 = "TestStr"},
                new TestViewModelB() { PropStr1 = "TestTT1"}
            };

            listView.FixedList = vmAs;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            var elems = listView.Element.QuerySelectorAll("[test]");
            Assert.Equal(3, elems.Length, "number of children should be 3");

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(
                    vmAs[i].PropStr1,
                    elems[i].InnerText,
                    "Inner text for element should match property it's bound to.");
            }
        }
    }
}

