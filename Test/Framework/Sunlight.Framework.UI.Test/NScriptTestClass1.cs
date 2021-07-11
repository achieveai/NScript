//-----------------------------------------------------------------------
// <copyright file="NScriptTestClass1.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using System.Runtime.CompilerServices;
    using Sunlight.Framework.UI.Helpers;

    public class AppViewModel
    {
        public bool FullContact
        { get; set; }

        public string FirstName
        { get; set; }

        public string LastName
        { get; set; }

        public string Name
        { get; set; }

        public bool HasLastName
        { get { return string.IsNullOrEmpty(this.LastName); } }
    }

    public class ValueIfTrue<T>
    {
        T value;
        T defaultValue;
        public ValueIfTrue(T value)
        {
            this.value = value;
            this.defaultValue = default(T);
        }

        public T Convert(bool isTrue)
        {
            return isTrue ? this.value : this.defaultValue;
        }
    }

    /// <summary>
    /// Definition for NScriptTestClass1
    /// </summary>
    [TestClass]
    public class ManualTemplateTests
    {
        public static ValueIfTrue<string> noneValue = new ValueIfTrue<string>("none");
        [IgnoreGenericArguments]
        public static T GetValue<T>(Func<T> func, T defaultValue)
        {
            try
            {
                return func();
            }
            catch
            {
                return defaultValue;
            }
        }

        [TestSetup]
        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        public static void Setup()
        {
        }

        [Test]
        /// <summary>
        /// Test case to test a unit of functionality.
        /// </summary>
        public static void Test(Assert assert)
        {
            /*
            Func<Document, SkinInstance> instanceFactory = delegate(Document document)
            {
                Element elem = document.CreateElement("div");
                elem.InnerHTML = "<div class='x y'>Hi <span></span>!!!</div><div class='x'>Your last name is: <span></span></div>";
                Element elem1 = elem.Children[0].As<Element>();
                Element elem2 = elem1.Children[0].As<Element>();
                Element elem3 = elem.Children[1].As<Element>();
                Element elem4 = elem3.Children[0].As<Element>();

                Action<object> binder = delegate(object obj)
                {
                    AppViewModel appViewModel = (AppViewModel)obj;

                    Helpers.CssTargetBinder.Bind(elem1,
                        "z",
                        ManualTemplateTests.GetValue<bool>(
                            () => appViewModel.FullContact,
                            false));

                    elem2.TextContent =
                        ManualTemplateTests.GetValue<string>(
                            () => appViewModel.Name,
                            null);

                    elem3.Style.Display =
                        ManualTemplateTests.GetValue<string>(
                            () => noneValue.Convert(appViewModel.HasLastName),
                            null);

                    elem4.TextContent =
                        ManualTemplateTests.GetValue<string>(
                            () => appViewModel.LastName,
                            null);

                    TaskScheduler.Instance.EnqueueTask(
                        () => { },
                        "initializing binders");
                };

                return new SkinInstance(
                    elem,
                    binder,
                    null);
            };
            */
            assert.IsTrue(true, "true should be true");
        }
    }
}

