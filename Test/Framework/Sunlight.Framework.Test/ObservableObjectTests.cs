//-----------------------------------------------------------------------
// <copyright file="ObservableObjectTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using System.Collections.Generic;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using SunlightUnit;

    /// <summary>
    /// Definition for ObservableObjectTests
    /// </summary>
    [TestFixture]
    public class ObservableObjectTests
    {
        private class ObservableTestObject : ObservableObject
        {
            string strField;
            int intProp;

            public int IntProp
            {
                get { return this.intProp; }

                set
                {
                    if (value != this.intProp)
                    {
                        this.intProp = value;
                        this.FirePropertyChanged("IntProp");
                    }
                }
            }

            public string StringProp
            {
                get { return this.strField; }

                set
                {
                    if (value != this.strField)
                    {
                        this.strField = value;
                        this.FirePropertyChanged("StringProp");
                    }
                }
            }

            public KeyValuePair<int, string> DualProp
            {
                get { return new KeyValuePair<int, string>(this.intProp, this.strField); }

                set
                {
                    if (this.strField != value.Value
                        || this.intProp != value.Key)
                    {
                        this.intProp = value.Key;
                        this.strField = value.Value;

                        this.FirePropertyChanged("StringProp", "IntProp");
                    }
                }
            }

            [AutoFire]
            public string AutoFiredProp { get; set; }

            [AutoFire(nameof(AutoFiredProp))]
            public string AutoFiredProp1 { get; set; }

            [AutoFire]
            public List<int> GenericProp { get; set; }
        }

        [Test]
        public static void TestCreateNewObservableObject(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();

            assert.NotEqual(null, observableObject, "ObservableObject should be created");
        }

        [Test]
        public static void TestFirePropertyChanged(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            bool strChanged = false;
            bool cbCalled = false;
            Action<INotifyPropertyChanged, string> cb1 =
                (sender, propName) =>
                {
                    strChanged = propName == "StringProp" && sender == observableObject;
                    cbCalled = true;
                };

            observableObject.AddPropertyChangedListener("StringProp", cb1);
            observableObject.StringProp = "1";

            assert.IsTrue(strChanged, "change callback called");

            strChanged = false;
            cbCalled = false;

            observableObject.IntProp = 1;
            assert.IsTrue(!strChanged, "String change callback not called.");
            assert.IsTrue(!cbCalled, "Callback should not be called for different property change");
        }

        [Test]
        public static void TestRemovePropertyChangeCallback(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            bool cbCalled = false;
            Action<INotifyPropertyChanged, string> cb1 =
                (sender, propName) => cbCalled = true;

            observableObject.AddPropertyChangedListener("StringProp", cb1);
            observableObject.StringProp = "1";

            assert.IsTrue(cbCalled, "change callback called");
            cbCalled = false;

            observableObject.RemovePropertyChangedListener("StringProp", cb1);

            observableObject.StringProp = "2";
            assert.IsTrue(!cbCalled, "after removing change listner, callback should not be called.");
        }

        [Test]
        public static void TestAutoFirePropertyChanged(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            bool strChanged = false;
            bool cbCalled = false;
            Action<INotifyPropertyChanged, string> cb1 =
                (sender, propName) =>
                {
                    strChanged = propName == "AutoFiredProp" && sender == observableObject;
                    cbCalled = true;
                };

            observableObject.AddPropertyChangedListener("AutoFiredProp", cb1);
            observableObject.AutoFiredProp = "1";

            assert.IsTrue(strChanged, "change callback called");

            strChanged = false;
            cbCalled = false;

            observableObject.IntProp = 1;
            assert.IsTrue(!strChanged, "Auto fire change callback not called.");
        }

        [Test]
        public static void TestAlsoFirePropertyChanged(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            var strChanged = false;
            var strChanged1 = false;

            observableObject.GenericProp = null;
            var yyy = observableObject.GenericProp;

            observableObject.AddPropertyChangedListener(
                "AutoFiredProp",
                (sender, propName) =>
                {
                    strChanged = propName == "AutoFiredProp" && sender == observableObject;
                });

            observableObject.AddPropertyChangedListener(
                "AutoFiredProp1",
                (sender, propName) =>
                {
                    strChanged1 = propName == "AutoFiredProp1" && sender == observableObject;
                });

            observableObject.AutoFiredProp1 = "2";

            assert.IsTrue(strChanged, "change callback called");
            assert.IsTrue(strChanged1, "change callback called");

            strChanged = false;
            strChanged1 = false;

            observableObject.IntProp = 1;
            assert.IsTrue(!strChanged, "Auto fire change callback not called.");
            assert.IsTrue(!strChanged1, "Auto fire change callback not called.");
        }

        [Test]
        public static void TestGenericPropertyFirePropertyChanged(Assert assert)
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            var changed = false;

            observableObject.AddPropertyChangedListener(
                "GenericProp",
                (sender, propName) =>
                {
                    changed = propName == "GenericProp" && sender == observableObject;
                });

            observableObject.GenericProp = new List<int>();

            assert.IsTrue(changed, "change callback called");

            changed = false;

            observableObject.IntProp = 1;
            assert.IsTrue(!changed, "Auto fire change callback not called.");
        }
    }
}