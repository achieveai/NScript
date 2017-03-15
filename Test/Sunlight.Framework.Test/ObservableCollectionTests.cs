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
        public static void TestCreateNewObservableCollection(Assert assert)
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();

            assert.NotEqual(null, observableCollection, "ObservableCollection should be created");
            assert.Equal(0, observableCollection.Count, "ObservableCollection's size should be 0");
        }

        [Test]
        public static void TestAddItemToObservableCollection(Assert assert)
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            bool eventRaised = false;

            observableCollection.CollectionChanged += (coll, evtArg) =>
                {
                    assert.Equal(observableCollection, coll, "ObservableCollection");
                    assert.Equal(1, evtArg.NewItems.Count, "evtArg.NewItems.Count");
                    assert.IsTrue(Object.IsNullOrUndefined(evtArg.OldItems), "Object.IsNullOrUndefined(evtArg.OldItems)");
                    assert.Equal(0, evtArg.ChangeIndex, "evtArg.changeIndex");
                    eventRaised = true;
                };

            observableCollection.Add(1);
            assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestRemoveItemToObservableCollection(Assert assert)
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            bool eventRaised = false;
            observableCollection.Add(1);
            observableCollection.Add(2);
            observableCollection.Add(3);

            observableCollection.CollectionChanged += (coll, evtArg) =>
                {
                    assert.Equal(observableCollection, coll, "ObservableCollection");
                    assert.Equal(2, evtArg.OldItems.Count, "evtArg.OldItems.Count");
                    assert.IsTrue(Object.IsNullOrUndefined(evtArg.NewItems), "Object.IsNullOrUndefined(evtArg.NewItems)");
                    assert.Equal(1, evtArg.ChangeIndex, "evtArg.changeIndex");
                    eventRaised = true;
                };

            observableCollection.RemoveRangeAt(1, 2);
            assert.IsTrue(eventRaised, "Change event raised");
        }
    }
}
