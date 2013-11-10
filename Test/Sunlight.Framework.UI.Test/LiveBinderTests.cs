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
        private static SkinBinderInfo oneWayBinder;

        private static SkinBinderInfo twoWayBinder;

        private static SkinBinderInfo oneWayMultiBinder;

        private static SkinBinderInfo twoWayMultiBinder;

        /// <summary>
        /// Sets up the data/environment to run all the test cases.
        /// </summary>
        [TestSetup]
        public static void Setup()
        {
            LiveBinderTests.oneWayBinder = new SkinBinderInfo(
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

            LiveBinderTests.twoWayBinder = new SkinBinderInfo(
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

            LiveBinderTests.oneWayMultiBinder = new SkinBinderInfo(
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

            LiveBinderTests.twoWayMultiBinder = new SkinBinderInfo(
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

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestTwoWayLiveBinderOnChange()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.twoWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.PropStr1 = "testChanged";
            Assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            target.PropStr1 = "Changed Again";
            Assert.Equal(target.PropStr1, src.PropStr1, "if liveBinder is active in twoWay changes on target should flow back.");
        }

        /// <summary>
        /// Tests two way live binder multi on change with converters.
        /// </summary>
        [Test]
        public static void TestTwoWayLiveBinderMultiOnChangeWithConverters()
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropInt1 = 1;
            var liveBinder = new LiveBinder(LiveBinderTests.twoWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            Assert.NotEqual(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            Assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.TestVMA.PropInt1 = 2;
            Assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes on lastElement should have flowed.");

            src.TestVMA = new TestViewModelA() { PropInt1 = 3 };
            Assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");

            target.PropStr1 = "21";
            Assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active in twoWay changes on target should flow back.");
        }

    }
}

