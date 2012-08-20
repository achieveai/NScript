//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for ObservableCollectionTests
    /// </summary>
    [TestFixture]
    public class ObservableCollectionTests
    {
        [Test]
        public static void TestCreateNewObservableCollection()
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();

            Assert.NotEqual(null, observableCollection, "ObservableCollection should be created");
            Assert.Equal(0, observableCollection.Count, "ObservableCollection's size should be 0");
        }

        [Test]
        public static void TestAddItemToObservableCollection()
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            bool eventRaised = false;

            observableCollection.CollectionChanged += (coll, evtArg) =>
                {
                    Assert.Equal(observableCollection, coll, "ObservableCollection");
                    Assert.Equal(1, evtArg.NewItems.Count, "evtArg.NewItems.Count");
                    Assert.IsTrue(Object.IsNullOrUndefined(evtArg.OldItems), "Object.IsNullOrUndefined(evtArg.OldItems)");
                    Assert.Equal(0, evtArg.ChangeIndex, "evtArg.changeIndex");
                    eventRaised = true;
                };

            observableCollection.Add(1);
            Assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestRemoveItemToObservableCollection()
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            bool eventRaised = false;
            observableCollection.Add(1);
            observableCollection.Add(2);
            observableCollection.Add(3);

            observableCollection.CollectionChanged += (coll, evtArg) =>
                {
                    Assert.Equal(observableCollection, coll, "ObservableCollection");
                    Assert.Equal(2, evtArg.OldItems.Count, "evtArg.OldItems.Count");
                    Assert.IsTrue(Object.IsNullOrUndefined(evtArg.NewItems), "Object.IsNullOrUndefined(evtArg.NewItems)");
                    Assert.Equal(1, evtArg.ChangeIndex, "evtArg.changeIndex");
                    eventRaised = true;
                };

            observableCollection.RemoveRangeAt(1, 2);
            Assert.IsTrue(eventRaised, "Change event raised");
        }
    }
}
