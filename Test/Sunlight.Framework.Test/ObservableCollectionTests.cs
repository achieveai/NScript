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
    using System.Collections.Generic;

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

        [Test]
        public static void TestReplaceRangeObservableCollection(Assert assert)
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            bool eventRaised = false;
            observableCollection.Add(1);
            observableCollection.Add(2);
            observableCollection.Add(3);

            List<int> replaceList = new List<int>();
            replaceList.Add(12);
            replaceList.Add(13);

            observableCollection.CollectionChanged += (coll, evtArg) =>
                {
                    assert.Equal(evtArg.OldItems.Count, 1, "evtArg.OldItems.Count");
                    assert.IsTrue(!Object.IsNullOrUndefined(evtArg.NewItems), "Object.IsNullOrUndefined(evtArg.NewItems)");
                    assert.Equal(evtArg.NewItems, replaceList, "ObservableCollection");
                    assert.Equal(evtArg.ChangeIndex, 1, "evtArg.changeIndex");
                    eventRaised = true;
                };
            
            observableCollection.ReplaceRangeAt(1, replaceList);
            assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestGetRangeObservableCollection(Assert assert)
        {
            ObservableCollection<int> observableCollection = new ObservableCollection<int>();
            observableCollection.Add(1);
            observableCollection.Add(2);
            observableCollection.Add(3);
            observableCollection.Add(4);
            observableCollection.Add(5);
            observableCollection.Add(6);

            CheckGetAsserts(assert, observableCollection, 1, 0);//exception
            CheckGetAsserts(assert, observableCollection, 1, 1);
            CheckGetAsserts(assert, observableCollection, 1, 2);
            CheckGetAsserts(assert, observableCollection, 1, 5);
            CheckGetAsserts(assert, observableCollection, 1, 6);//exception
            CheckGetAsserts(assert, observableCollection, 1, -1);//exception
            CheckGetAsserts(assert, observableCollection, -1, 0);//exception
            CheckGetAsserts(assert, observableCollection, 0, 6);

        }

        public static void CheckGetAsserts(Assert assert, ObservableCollection<int> observableCollection, int index, int count) {
            try
            {
                var range = observableCollection.GetRangeAt(index, count);
                assert.Equal(range.Count, count, "count of items returned is equal for index: " + index + " ,count: " + count);
                for (int idx = 0; idx < count; idx++)
                {
                    assert.Equal(range[idx], observableCollection[index + idx], "Index: " + idx +" value: "+ range[idx] +" is equal");
                }
            }
            catch (Exception)
            {
                assert.IsTrue(
                    index < 0 
                    || index > observableCollection.Count - 1
                    || count <= 0
                    || count > observableCollection.Count
                    || index + count < 0
                    || index + count > observableCollection.Count - 1,
                    "Improper value of parameters for index : " + index + " ,count : " + count);
            }
        }
    }
}
