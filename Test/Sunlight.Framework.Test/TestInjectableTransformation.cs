//-----------------------------------------------------------------------
// <copyright file="TestInjectableTransformation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;
    using Sunlight.Framework.Observables;
    using System.Collections.Generic;
    using System.Diagnostics;

    //using HeaderListItemModel = Observables.ListViewItemModel<System.Number, System.Number>;
    //using iiii = Observables.INotifyCollectionChanged<Observables.ListViewItemModel<System.Number, System.Number>>;
    //using headercollection = Observables.ObservableCollection<Observables.ListViewItemModel<System.Number, System.Number>>;

    /// <summary>
    /// Definition for TestInjectableTransformation
    /// </summary>
    [TestFixture]
    public class TestInjectableTransformation
    {

        [Test]
        public static void TestCollectionHeaderInjection(Assert assert)
        {
            ObservableCollection<Number> collection = new ObservableCollection<Number>();
            collection.Add(3);
            collection.Add(5);
            collection.Add(32);
            collection.Add(35);
            collection.Add(64);
            collection.Add(65);
            collection.Add(92);
            collection.Add(95);

            int newItemCount = 0;
            int itemsRemoved = 0;
            int insertIndex = 0;
            int expectedIndex = 0;
            List<Number> rangeToInsert = new List<Number>();
            bool eventRaised = false;
            var transformer = new HeaderInjectableTransformer<Number, Number>(collection, GenerateHeader);

            Action<Observables.INotifyCollectionChanged<Observables.ListViewItemModel<System.Number, System.Number>>
                , CollectionChangedEventArgs<ListViewItemModel<Number, Number>>> transformedCollChanged =
                (coll, evtArg) =>

                {
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "----------------------- " + rangeToInsert[0] + " " + rangeToInsert[1] + "----- Item Added");

                        assert.Equal(
                            evtArg.NewItems.Count,
                            newItemCount,
                            "Number of Items Added");

                        assert.Equal(
                            evtArg.ChangeIndex,
                            expectedIndex,
                            "Index changed");
                    }
                    else if (evtArg.Action == CollectionChangedAction.Remove)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.OldItems),
                            "----------------------- " + rangeToInsert[0] + " " + rangeToInsert[1] + "----- Item Removed");

                        assert.Equal(
                            itemsRemoved,
                            evtArg.OldItems.Count,
                            "Number of Items Removed");

                        assert.Equal(
                            evtArg.ChangeIndex,
                            expectedIndex,
                            "Index changed");

                    }

                    eventRaised = true;
                };

            transformer.TransformedCollection.CollectionChanged += transformedCollChanged;

            rangeToInsert.Clear();
            rangeToInsert.Add(1);
            rangeToInsert.Add(2);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(12);
            rangeToInsert.Add(22);
            insertIndex = 4;
            expectedIndex = 5;
            newItemCount = 5;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(33);
            rangeToInsert.Add(34);
            insertIndex = 7;
            expectedIndex = 11;
            newItemCount = 2;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(36);
            rangeToInsert.Add(37);
            insertIndex = 10;
            expectedIndex = 14;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(38);
            rangeToInsert.Add(61);
            insertIndex = 12;
            expectedIndex = 16;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(39);
            rangeToInsert.Add(43);
            insertIndex = 13;
            expectedIndex = 17;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(60);
            rangeToInsert.Add(61);
            insertIndex = 15;
            expectedIndex = 21;
            newItemCount = 3;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(76);
            rangeToInsert.Add(77);
            insertIndex = 19;
            expectedIndex = 24;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(82);
            rangeToInsert.Add(91);
            insertIndex = 21;
            expectedIndex = 27;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(97);
            rangeToInsert.Add(98);
            insertIndex = 25;
            expectedIndex = 33;
            newItemCount = 2;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            ////rangeToInsert.Clear();
            ////rangeToInsert.Add(45);
            ////rangeToInsert.Add(51);
            ////newItemCount = 2;
            ////insertIndex = 4;
            ////transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            transformer.TransformedCollection.CollectionChanged -= transformedCollChanged;

            assert.IsTrue(eventRaised, "Change event raised");

            ////Remove me
            ////Action<Observables.INotifyCollectionChanged<System.Number>
            ////    , CollectionChangedEventArgs<Number>> collChanged =
            ////    (coll, evtArg) =>

            ////    {
            ////        assert.Equal(
            ////            2,
            ////            evtArg.NewItems.Count,
            ////            "evtArg.NewItems.Count");

            ////        assert.IsTrue(
            ////            Object.IsNullOrUndefined(evtArg.OldItems),
            ////            "Object.IsNullOrUndefined(evtArg.OldItems)");

            ////        assert.Equal(
            ////            0,
            ////            evtArg.ChangeIndex,
            ////            "evtArg.changeIndex");
            ////    };

            ////collection.CollectionChanged += collChanged;
            ////Number[] rangeToInsert1 = { 1, 2 };
            ////collection.InsertRangeAt(0, rangeToInsert1);
            ////collection.InsertAt(0, 1);
        }

        public static Number GenerateHeader(Number first, Number second)
        {
            Number rv;
            int firstHead = first / 10;
            int secondHead = second / 10;

            if (first == null)
            {
                return secondHead;
            }

            if (firstHead == secondHead)
            {
                return null;
            }
            else {
                rv = secondHead;
            }

            return rv;
        }
    }
}
