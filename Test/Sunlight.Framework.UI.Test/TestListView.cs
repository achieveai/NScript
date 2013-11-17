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

            TestViewModelA[] vmAs = new TestViewModelA[]
            {
                new TestViewModelA() { PropStr1 = "Str"},
                new TestViewModelA() { PropInt1 = 10},
                new TestViewModelA() { PropBool1 = true}
            };

            listView.FixedList = vmAs;
            listView.InactiveIfNullContext = false;
            listView.Activate();
            Assert.Equal(4,listView.Element.Children.Length, "true should be true");
        }
    }
}

