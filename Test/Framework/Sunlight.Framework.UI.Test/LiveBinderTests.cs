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
        public static void TestLiveBinderOnActivate(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderOnChange(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.PropStr1 = "testChanged";
            assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderMultiOnActivate(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestLiveBinderMultiOnChange(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.oneWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.TestVMA.PropStr1 = "testChanged";
            assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes on lastElement should have flowed.");

            src.TestVMA = new TestViewModelA() { PropStr1 = "ChangedTest" };
            assert.Equal(src.TestVMA.PropStr1, target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");

            src.TestVMA = null;
            assert.Equal(null, target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");
        }

        /// <summary>
        /// A null first name marks a plain (non-observable) root: read it, never listen to it.
        /// </summary>
        [Test]
        public static void TestLiveBinderPlainRootObservableChild(Assert assert)
        {
            var binderInfo = new SkinBinderInfo(
                new Func<object, object>[]
                {
                    delegate(object obj)
                    {
                        return ((NonObservableObject)obj).Child;
                    },
                    delegate(object obj)
                    {
                        return ((TestViewModelA)obj).PropStr1;
                    }
                },
                new string[]
                {
                    null,
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

            NonObservableObject src = new NonObservableObject();
            src.Child = new TestViewModelA();
            src.Child.PropStr1 = "test";
            TestViewModelA target = new TestViewModelA();
            var liveBinder = new LiveBinder(binderInfo);
            liveBinder.Source = src;
            liveBinder.Target = target;
            liveBinder.IsActive = true;
            assert.Equal(target.PropStr1, "test", "value from a plain root should flow on activate.");

            src.Child.PropStr1 = "testChanged";
            assert.Equal(target.PropStr1, "testChanged", "changes on the observable child should flow.");

            NonObservableObject src2 = new NonObservableObject();
            src2.Child = new TestViewModelA();
            src2.Child.PropStr1 = "second";
            liveBinder.Source = src2;
            assert.Equal(target.PropStr1, "second", "a new plain root should flow.");
        }

        /// <summary>
        /// A binder on Child.Child.Name stops listening to the objects its path no longer
        /// reaches, keeps listening to the ones it still reaches, and to none once disposed.
        /// </summary>
        [Test]
        public static void TestLiveBinderRemovesListenersOffThePath(Assert assert)
        {
            var binderInfo = new SkinBinderInfo(
                new Func<object, object>[]
                {
                    delegate(object obj)
                    {
                        return ((ListenerProbeVM)obj).Child;
                    },
                    delegate(object obj)
                    {
                        return ((ListenerProbeVM)obj).Child;
                    },
                    delegate(object obj)
                    {
                        return ((ListenerProbeVM)obj).Name;
                    }
                },
                new string[]
                {
                    "Child",
                    "Child",
                    "Name"
                },
                delegate(object tar, object val)
                {
                    ((ListenerProbeVM)tar).Name = (string)val;
                },
                BinderType.PropertyBinder | BinderType.DataContext,
                0,
                0,
                null,
                null);

            var src = new ListenerProbeVM();
            var child = new ListenerProbeVM();
            var grandChild = new ListenerProbeVM();
            src.Child = child;
            child.Child = grandChild;
            grandChild.Name = "test";
            var target = new ListenerProbeVM();
            var liveBinder = new LiveBinder(binderInfo);
            liveBinder.Source = src;
            liveBinder.Target = target;
            liveBinder.IsActive = true;
            assert.Equal(target.Name, "test", "the value flows on activate.");
            assert.Equal(grandChild.Listeners, 1, "the binder listens to the last object on the path.");

            src.Child = null;
            assert.Equal(child.Listeners, 0, "a cut path stops listening to the old child.");
            assert.Equal(grandChild.Listeners, 0, "a cut path stops listening to the old grandchild.");

            src.Child = child;
            assert.Equal(target.Name, "test", "a restored path flows again.");
            assert.Equal(child.Listeners, 1, "a restored path listens to the child again.");
            assert.Equal(grandChild.Listeners, 1, "a restored path listens to the grandchild again.");
            grandChild.Name = "changed";
            assert.Equal(target.Name, "changed", "the restored path's last object still flows.");

            var otherChild = new ListenerProbeVM();
            var otherGrandChild = new ListenerProbeVM();
            otherChild.Child = otherGrandChild;
            otherGrandChild.Name = "other";
            src.Child = otherChild;
            assert.Equal(target.Name, "other", "a new mid-path object flows.");
            assert.Equal(child.Listeners + grandChild.Listeners, 0, "the old mid-path objects are let go.");
            assert.Equal(otherChild.Listeners, 1, "the new child is listened to.");
            assert.Equal(otherGrandChild.Listeners, 1, "the new grandchild is listened to.");
            otherGrandChild.Name = "other changed";
            assert.Equal(target.Name, "other changed", "the new path's last object flows.");
            assert.Equal(src.Listeners, 1, "the source keeps its one listener throughout.");

            liveBinder.IsActive = false;
            liveBinder.Source = null;
            liveBinder.Target = null;
            liveBinder.Cleanup();
            assert.Equal(src.Listeners, 0, "a disposed binder stops listening to the source.");
            assert.Equal(otherChild.Listeners, 0, "a disposed binder stops listening to the child.");
            assert.Equal(otherGrandChild.Listeners, 0, "a disposed binder stops listening to the grandchild.");
        }

        /// <summary>
        /// Tests live binder on activate.
        /// </summary>
        [Test]
        public static void TestTwoWayLiveBinderOnChange(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.PropStr1 = "test";
            var liveBinder = new LiveBinder(LiveBinderTests.twoWayBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.PropStr1, target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.PropStr1 = "testChanged";
            assert.Equal(src.PropStr1, target.PropStr1, "if liveBinder is active and changes should have flowed.");

            target.PropStr1 = "Changed Again";
            assert.Equal(target.PropStr1, src.PropStr1, "if liveBinder is active in twoWay changes on target should flow back.");
        }

        /// <summary>
        /// Tests two way live binder multi on change with converters.
        /// </summary>
        [Test]
        public static void TestTwoWayLiveBinderMultiOnChangeWithConverters(Assert assert)
        {
            TestViewModelA src = new TestViewModelA();
            TestViewModelA target = new TestViewModelA();
            src.TestVMA = new TestViewModelA();
            src.TestVMA.PropInt1 = 1;
            var liveBinder = new LiveBinder(LiveBinderTests.twoWayMultiBinder);
            liveBinder.Source = src;
            liveBinder.Target = target;

            assert.NotEqual(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is notActive, changes should not flow");

            liveBinder.IsActive = true;
            assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes should have flowed.");

            src.TestVMA.PropInt1 = 2;
            assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes on lastElement should have flowed.");

            src.TestVMA = new TestViewModelA() { PropInt1 = 3 };
            assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active and changes on firstElement should have flowed.");

            target.PropStr1 = "21";
            assert.Equal(src.TestVMA.PropInt1.ToString(), target.PropStr1, "if liveBinder is active in twoWay changes on target should flow back.");
        }

    }
}

