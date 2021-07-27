//-----------------------------------------------------------------------
// <copyright file="TestObservabelCollectionTransformer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using Sunlight.Framework.Observables;
    using SunlightUnit;
    using System;
    using System.Collections.Generic;
    //using InjectedNumber = Sunlight.Framework.Observables.InjectedElement<System.Number, System.Number>;
    //using NumberColection = Sunlight.Framework.Observables.CollectionChangedEventArgs
        //<Sunlight.Framework.Observables.InjectedElement<System.Number, System.Number>>;
    /// <summary>
    /// Definition for TestInjectableTransformation
    /// </summary>
    [TestFixture]
    public class TestObservabelCollectionTransformer
    {
        private static ObservableCollectionGenerator<Number, string> _listTransformer;
        private static int _notificationCount = 0;
        private static List<int> _expectedCounts = new List<int>();
        private static List<int> _expectedIndexes = new List<int>();
        private static List<Number> _list = new List<Number>();

        [Test]
        public static void TestObservableCollectionTransformer(Assert assert)
        {
            _listTransformer = new ObservableCollectionGenerator<Number,string>(ChangeDataType);

            Action<Observables.INotifyCollectionChanged<string>, CollectionChangedEventArgs<string>> transformedCollChanged =
                (coll, evtArg) =>
                {
                    var listBounds = _list!=null?_list[0]+" - "+_list[_list.Count-1]:"null";
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "------------------------" + _notificationCount + " Add in Reset from "+listBounds);

                        assert.Equal(
                            evtArg.NewItems.Count,
                            _expectedCounts[_notificationCount],
                            "Number of Items Added" + evtArg.NewItems.Count);

                        assert.Equal(
                            evtArg.ChangeIndex,
                            _expectedIndexes[_notificationCount],
                            "Index changed" + evtArg.ChangeIndex);
                    }
                    else if (evtArg.Action == CollectionChangedAction.Remove)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.OldItems),
                            "------------------------" + _notificationCount + "Remove in Reset from "+listBounds);

                        assert.Equal(
                            evtArg.OldItems.Count,
                            _expectedCounts[_notificationCount],
                            "Number of Items Removed" + evtArg.ChangeIndex);

                        assert.Equal(
                            evtArg.ChangeIndex,
                            _expectedIndexes[_notificationCount],
                            "Index changed" + evtArg.ChangeIndex);
                    }
                    else if (evtArg.Action == CollectionChangedAction.Replace)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.OldItems) && !Object.IsNullOrUndefined(evtArg.NewItems),
                            "------------------------" + _notificationCount + "Replace in Reset from "+listBounds);

                        assert.Equal(
                            evtArg.NewItems.Count,
                            _expectedCounts[_notificationCount],
                            "Number of Items Added in replace" + evtArg.NewItems.Count);

                        assert.Equal(
                            evtArg.ChangeIndex,
                            _expectedIndexes[_notificationCount],
                            "Index changed" + evtArg.NewItems.Count);
                    }

                    _notificationCount++;
                };

            _listTransformer.OutputCollection.CollectionChanged += transformedCollChanged;

            TestWithNewList(
                assert,
                new List<Number> { 11 , 12 },
                new List<int>() { 2 },
                new List<int>() { 0 });
            TestWithNewList(
                assert,
                new List<Number> { 21 , 31 },
                new List<int>() { 2 },
                new List<int>() { 0 });
            TestWithNewList(
                assert,
                null,
                new List<int>() { 2 },
                new List<int>() { 0 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23, 33 },
                new List<int>() { 3 },
                new List<int>() { 0 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23 },
                new List<int>() { 1 },
                new List<int>() { 2 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23, 25 },
                new List<int>() { 1 },
                new List<int>() { 2 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23 },
                new List<int>() { 1 },
                new List<int>() { 2 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23, 33 },
                new List<int>() { 1 },
                new List<int>() { 2 });
            TestWithNewList(
                assert,
                new List<Number> { 13, 23, 33 },
                new List<int>() { },
                new List<int>() { });
            TestWithNewList(
                assert,
                new List<Number> { 12, 23, 33 },
                new List<int>() { 1 },
                new List<int>() { 0 });
            return;
        }

        private static void TestWithNewList(Assert assert, List<Number> list, List<int> expectedCount, List<int> expectedIndex)
        {
            _list = list;
            var length = _expectedCounts.Count;
            var lastIndex = Math.Max(length, 0);
            _expectedCounts.InsertRange(lastIndex,expectedCount);
            _expectedIndexes.InsertRange(lastIndex,expectedIndex);

            _listTransformer.InputCollection = list;
        }

        public static string ChangeDataType(Number first)
        {
            return "This is number : "+first.ToString();
        }
    }
}