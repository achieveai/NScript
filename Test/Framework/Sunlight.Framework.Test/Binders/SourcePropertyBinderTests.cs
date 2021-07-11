//-----------------------------------------------------------------------
// <copyright file="SourcePropertyBinderTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Binders
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.Binders;

    /// <summary>
    /// Definition for SourcePropertyBinderTests
    /// </summary>
    [TestClass]
    public class SourcePropertyBinderTests
    {
        public class BinderTestHelper : ISourceDataBinder
        {
            public bool SourceValueUpdateCalled
            {
                get;
                set;
            }

            public void SourceValueUpdated()
            {
                this.SourceValueUpdateCalled = true;
            }
        }

        public static SimpleNotifiableClass PrepNotifiableObject()
        {
            SimpleNotifiableClass rv = new SimpleNotifiableClass()
            {
                IntProp = 10,
                StrProp = "Ten",
                ObjProp = new SimpleObjectWithProperty()
                {
                    IntProp = 11,
                    StringProp = "Eleven"
                }
            };

            rv.SelfProp = rv;
            rv.ObjProp.SelfProp = rv.ObjProp;
            rv.ObjProp.NotifiableProp = rv;

            return rv;
        }

        public static SimpleObjectWithProperty PrepSimpleObject()
        {
            return SourcePropertyBinderTests.PrepNotifiableObject().ObjProp;
        }

        [Test]
        public static void BasicValueTest(Assert assert)
        {
            SourcePropertyBinder sourceBinder = new SourcePropertyBinder(
                new string[] { null },
                new Func<object, object>[]
                {
                    delegate(object obj) { return ((SimpleObjectWithProperty)obj).IntProp; }
                },
                null);

            BinderTestHelper helper = new BinderTestHelper();
            sourceBinder.UseDataBinder(helper);

            var src = SourcePropertyBinderTests.PrepSimpleObject();
            sourceBinder.Source = src;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");
        }

        [Test]
        public static void BasicValueTestWithNotification(Assert assert)
        {
            SourcePropertyBinder sourceBinder = new SourcePropertyBinder(
                new string[] { "IntProp" },
                new Func<object, object>[]
                {
                    delegate(object obj) { return ((SimpleNotifiableClass)obj).IntProp; }
                },
                null);

            BinderTestHelper helper = new BinderTestHelper();
            sourceBinder.UseDataBinder(helper);

            var src = SourcePropertyBinderTests.PrepNotifiableObject();
            sourceBinder.Source = src;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");

            helper.SourceValueUpdateCalled = false;
            src.IntProp++;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");
        }

        [Test]
        public static void PropertyPathValueNotifiableTest(Assert assert)
        {
            SourcePropertyBinder sourceBinder = new SourcePropertyBinder(
                new string[] { "NotifiableProp", "IntProp" },
                new Func<object, object>[]
                {
                    delegate(object obj) { return ((SimpleObjectWithProperty)obj).NotifiableProp; },
                    delegate(object obj) { return ((SimpleNotifiableClass)obj).IntProp; }
                },
                null);

            BinderTestHelper helper = new BinderTestHelper();
            sourceBinder.UseDataBinder(helper);

            var src = SourcePropertyBinderTests.PrepSimpleObject();
            sourceBinder.Source = src;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.NotifiableProp.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");

            helper.SourceValueUpdateCalled = false;
            src.NotifiableProp.IntProp = -1;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.NotifiableProp.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");
        }

        [Test]
        public static void PropertyPathValueTest(Assert assert)
        {
            SourcePropertyBinder sourceBinder = new SourcePropertyBinder(
                new string[] { "SelfProp", "IntProp" },
                new Func<object, object>[]
                {
                    delegate(object obj) { return ((SimpleObjectWithProperty)obj).SelfProp; },
                    delegate(object obj) { return ((SimpleObjectWithProperty)obj).IntProp; }
                },
                null);

            BinderTestHelper helper = new BinderTestHelper();
            sourceBinder.UseDataBinder(helper);

            var src = SourcePropertyBinderTests.PrepSimpleObject();
            sourceBinder.Source = src;

            assert.IsTrue(helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(src.SelfProp.IntProp, (int)sourceBinder.Value, "SourceBinder.Value");

            helper.SourceValueUpdateCalled = false;
            int lastValue = src.SelfProp.IntProp;
            src.SelfProp.IntProp = -1;

            assert.IsTrue(!helper.SourceValueUpdateCalled, "SourceValueUpdate called");
            assert.Equal(lastValue, (int)sourceBinder.Value, "SourceBinder.Value");
        }
    }
}
