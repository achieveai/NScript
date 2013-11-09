//-----------------------------------------------------------------------
// <copyright file="LiveBinderTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.UI.Helpers;

    /// <summary>
    /// Definition for LiveBinderTests
    /// </summary>
    [TestFixture]
    public class LiveBinderTests
    {
        private static SkinBinderInfo oneWayBinder = new SkinBinderInfo(
            new Func<object, object>[]
            {
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).PropStr1;
                }
            },
            new string[]
            {
                "PropStr1"
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropStr1 = (string)val;
            },
            BinderType.PropertyBinder | BinderType.DataContext,
            0,
            0,
            null,
            null);

        private static SkinBinderInfo twoWayBinder = new SkinBinderInfo(
            new Func<object, object>[]
            {
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).PropStr1;
                }
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropStr1 = (string)val;
            },
            new string[]
            {
                "PropStr1"
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropStr1 = (string)val;
            },
            delegate(object obj)
            {
                return ((TestViewModelA)obj).PropStr1;
            },
            "PropStr1",
            BinderType.PropertyBinder | BinderType.DataContext,
            0,
            0,
            null,
            null,
            null);

        private static SkinBinderInfo oneWayMultiBinder = new SkinBinderInfo(
            new Func<object, object>[]
            {
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).TestVMA;
                },
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).PropStr1;
                }
            },
            new string[]
            {
                "TestVMA",
                "PropStr1"
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropStr1 = (string)val;
            },
            BinderType.PropertyBinder | BinderType.DataContext,
            0,
            0,
            null,
            null);

        private static SkinBinderInfo twoWayMultiBinder = new SkinBinderInfo(
            new Func<object, object>[]
            {
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).TestVMA;
                },
                delegate(object obj)
                {
                    return ((TestViewModelA)obj).PropInt1;
                }
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropInt1 = (int)val;
            },
            new string[]
            {
                "TestVMA",
                "PropInt1"
            },
            delegate(object tar, object val)
            {
                ((TestViewModelA)tar).PropStr1 = (string)val;
            },
            delegate(object obj)
            {
                return ((TestViewModelA)obj).PropStr1;
            },
            "PropStr1",
            BinderType.PropertyBinder | BinderType.DataContext,
            0,
            0,
            delegate(object obj)
            { return obj.ToString(); },
            delegate(object obj)
            { return int.Parse((string)obj);},
            null);

        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        [TestSetup]
        public static void Setup()
        {
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderOnActivate()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderOnChange()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.PropStr1 = "testChanged";
            Assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderMultiOnActivate()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderMultiOnChange()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.TestVMA.PropStr1 = "testChanged";
            Assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes on lastElement should have flowed.");

            src.TestVMA = new TestViewModelA() { PropStr1 = "ChangedTest" };
            Assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");

            src.TestVMA = null;
            Assert.Equal(null, target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");
        }
    }
}

