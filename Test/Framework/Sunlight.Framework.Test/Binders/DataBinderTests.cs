//-----------------------------------------------------------------------
// <copyright file="DataBinderTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Binders
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for DataBinderTests
    /// </summary>
    [TestClass]
    public class DataBinderTests
    {
        [Test]
        public static void BasicBinderSimpleObjectTest(Assert assert)
        {
            DataBinder dataBinder = new DataBinder(
                new SourcePropertyBinder(
                    new string[] { "IntProp" },
                    new Func<object, object>[]
                    {
                        delegate(object obj) { return (object)((SimpleObjectWithProperty)obj).IntProp; }
                    },
                    null),
                new TargetBinder(
                    "IntProp",
                    null,
                    delegate(INotifyPropertyChanged obj, object value) { ((SimpleNotifiableClass)obj).IntProp = (int)value; },
                    -1),
                DataBindingMode.OneWay,
                null);

            var target = SourcePropertyBinderTests.PrepNotifiableObject();
            var source = SourcePropertyBinderTests.PrepSimpleObject();
            source.IntProp = 101;

            dataBinder.SetTarget(target);
            assert.Equal(-1, target.IntProp, "source.IntProp == target.IntProp");

            dataBinder.SetSource(source);
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");

            dataBinder.SetSource(null);
            assert.Equal(-1, target.IntProp, "source.IntProp == target.IntProp");
        }

        [Test]
        public static void BasicBinderSimpleObjectTestReverseOrder(Assert assert)
        {
            DataBinder dataBinder = new DataBinder(
                new SourcePropertyBinder(
                    new string[] { "IntProp" },
                    new Func<object, object>[]
                    {
                        delegate(object obj) { return (object)((SimpleObjectWithProperty)obj).IntProp; }
                    },
                    null),
                new TargetBinder(
                    "IntProp",
                    null,
                    delegate(INotifyPropertyChanged obj, object value) { ((SimpleNotifiableClass)obj).IntProp = (int)value; },
                    -1),
                DataBindingMode.OneWay,
                null);

            var target = SourcePropertyBinderTests.PrepNotifiableObject();
            var source = SourcePropertyBinderTests.PrepSimpleObject();
            source.IntProp = 101;

            dataBinder.SetSource(source);

            dataBinder.SetTarget(target);
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");

            dataBinder.SetSource(null);
            assert.Equal(-1, target.IntProp, "source.IntProp == target.IntProp");
        }

        [Test]
        public static void BasicBinderOneWayNotifiableObjectTest(Assert assert)
        {
            DataBinder dataBinder = new DataBinder(
                new SourcePropertyBinder(
                    new string[] { "IntProp" },
                    new Func<object, object>[]
                    {
                        delegate(object obj) { return (object)((SimpleNotifiableClass)obj).IntProp; }
                    },
                    null),
                new TargetBinder(
                    "IntProp",
                    null,
                    delegate(INotifyPropertyChanged obj, object value) { ((SimpleNotifiableClass)obj).IntProp = (int)value; },
                    -1),
                DataBindingMode.OneWay,
                null);

            var target = SourcePropertyBinderTests.PrepNotifiableObject();
            var source = SourcePropertyBinderTests.PrepNotifiableObject();
            source.IntProp = 101;

            dataBinder.SetTarget(target);
            assert.Equal(-1, target.IntProp, "source.IntProp == target.IntProp");

            dataBinder.SetSource(source);
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");

            source.IntProp = 102;
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");
        }

        [Test]
        public static void BasicBinderTwoWayNotifiableObjectTest(Assert assert)
        {
            DataBinder dataBinder = new DataBinder(
                new SourcePropertyBinder(
                    new string[] { "IntProp" },
                    new Func<object, object>[]
                    {
                        delegate(object obj) { return (object)((SimpleNotifiableClass)obj).IntProp; }
                    },
                    delegate(object obj, object value) { ((SimpleNotifiableClass)obj).IntProp = (int)value; }),
                new TargetBinder(
                    "IntProp",
                    delegate(INotifyPropertyChanged obj) { return (object)((SimpleNotifiableClass)obj).IntProp; },
                    delegate(INotifyPropertyChanged obj, object value) { ((SimpleNotifiableClass)obj).IntProp = (int)value; },
                    -1),
                DataBindingMode.TwoWay,
                null);

            var target = SourcePropertyBinderTests.PrepNotifiableObject();
            var source = SourcePropertyBinderTests.PrepNotifiableObject();
            source.IntProp = 101;

            dataBinder.SetTarget(target);
            assert.Equal(-1, target.IntProp, "source.IntProp == target.IntProp");

            dataBinder.SetSource(source);
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");

            target.IntProp++;
            assert.Equal(source.IntProp, target.IntProp, "source.IntProp == target.IntProp");
        }
    }
}
