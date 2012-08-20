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
        }

        [Test]
        public static void TestCreateNewObservableObject()
        {
            ObservableTestObject observableObject = new ObservableTestObject();

            Assert.NotEqual(null, observableObject, "ObservableObject should be created");
        }

        [Test]
        public static void TestFirePropertyChanged()
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

            Assert.IsTrue(strChanged, "change callback called");

            strChanged = false;
            cbCalled = false;

            observableObject.IntProp = 1;
            Assert.IsTrue(!strChanged, "String change callback not called.");
            Assert.IsTrue(!cbCalled, "Callback should not be called for different property change");
        }

        [Test]
        public static void TestRemovePropertyChangeCallback()
        {
            ObservableTestObject observableObject = new ObservableTestObject();
            bool cbCalled = false;
            Action<INotifyPropertyChanged, string> cb1 =
                (sender, propName) => cbCalled = true;

            observableObject.AddPropertyChangedListener("StringProp", cb1);
            observableObject.StringProp = "1";

            Assert.IsTrue(cbCalled, "change callback called");
            cbCalled = false;

            observableObject.RemovePropertyChangedListener("StringProp", cb1);

            observableObject.StringProp = "2";
            Assert.IsTrue(!cbCalled, "after removing change listner, callback should not be called.");
        }
    }
}