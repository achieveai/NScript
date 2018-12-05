//-----------------------------------------------------------------------
// <copyright file="TestInjectableTransformation.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using Sunlight.Framework.Observables;
    using SunlightUnit;
    using System;
    using System.Collections.Generic;
    using InjectedNumber = Sunlight.Framework.Observables.InjectedElement<System.Number, System.Number>;
    using NumberColection = Sunlight.Framework.Observables.CollectionChangedEventArgs
        <Sunlight.Framework.Observables.InjectedElement<System.Number, System.Number>>;

    /// <summary>
    /// Definition for TestInjectableTransformation
    /// </summary>
    [TestFixture]
    public class TestInjectableTransformation
    {
        [Test]
        public static void TestHeaderInjectionInsert(Assert assert)
        {
            ObservableCollection<Number> collection = new ObservableCollection<Number>();
            collection.Add(110);
            collection.Add(111);
            collection.Add(198);
            collection.Add(199);

            int newItemCount = 0;
            int itemsRemoved = 0;
            int insertIndex = 0;
            int expectedIndex = 0;
            List<Number> rangeToInsert = new List<Number>();
            bool eventRaised = false;
            var transformer = new HeaderInjectableTransformer<Number, Number>(collection, GenerateHeader);

            Action<Observables.INotifyCollectionChanged<InjectedNumber>, NumberColection> transformedCollChanged =
                (coll, evtArg) =>

                {
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "----- " + rangeToInsert[0] + " " + rangeToInsert[1] + "---- Item Added");

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
                            "----- " + rangeToInsert[0] + " " + rangeToInsert[1] + "----- Item Removed");

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
            rangeToInsert.Add(112);
            rangeToInsert.Add(113);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(196);
            rangeToInsert.Add(197);
            insertIndex = 4;
            expectedIndex = 5;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(119);
            rangeToInsert.Add(120);
            insertIndex = 4;
            expectedIndex = 5;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(189);
            rangeToInsert.Add(191);
            insertIndex = 6;
            expectedIndex = 8;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(142);
            rangeToInsert.Add(143);
            insertIndex = 6;
            expectedIndex = 8;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(158);
            rangeToInsert.Add(159);
            insertIndex = 8;
            expectedIndex = 11;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(139);
            rangeToInsert.Add(140);
            insertIndex = 6;
            expectedIndex = 8;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(169);
            rangeToInsert.Add(170);
            insertIndex = 12;
            expectedIndex = 17;
            newItemCount = 5;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(149);
            rangeToInsert.Add(150);
            insertIndex = 10;
            expectedIndex = 14;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            //Left edge cases
            rangeToInsert.Clear();
            rangeToInsert.Add(109);
            rangeToInsert.Add(111);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(108);
            rangeToInsert.Add(107);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 3;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(95);
            rangeToInsert.Add(99);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 4;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(71);
            rangeToInsert.Add(89);
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 5;
            itemsRemoved = 1;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            //Right edge cases
            rangeToInsert.Clear();
            rangeToInsert.Add(200);
            rangeToInsert.Add(201);
            insertIndex = 30;
            expectedIndex = 43;
            newItemCount = 3;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(211);
            rangeToInsert.Add(221);
            insertIndex = 32;
            expectedIndex = 46;
            newItemCount = 4;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(222);
            rangeToInsert.Add(225);
            insertIndex = 34;
            expectedIndex = 50;
            newItemCount = 2;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            rangeToInsert.Clear();
            rangeToInsert.Add(229);
            rangeToInsert.Add(231);
            insertIndex = 36;
            expectedIndex = 52;
            newItemCount = 3;
            itemsRemoved = 0;
            transformer.InputCollection.InsertRangeAt(insertIndex, rangeToInsert);

            transformer.TransformedCollection.CollectionChanged -= transformedCollChanged;

            assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestHeaderInjectionRemove(Assert assert)
        {
            ObservableCollection<Number> collection = new ObservableCollection<Number>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);
            collection.Add(5);
            collection.Add(12);
            collection.Add(22);
            collection.Add(32);
            collection.Add(33);
            collection.Add(34);
            collection.Add(35);
            collection.Add(36);
            collection.Add(37);
            collection.Add(38);
            collection.Add(39);
            collection.Add(43);
            collection.Add(60);
            collection.Add(61);
            collection.Add(61);
            collection.Add(64);
            collection.Add(65);
            collection.Add(76);
            collection.Add(77);
            collection.Add(82);
            collection.Add(91);
            collection.Add(92);
            collection.Add(97);
            collection.Add(98);
            collection.Add(99);

            int itemsAdded = 0;
            int itemsRemoved = 0;
            int removeIndex = 0;
            int expectedIndex = 0;
            List<Number> rangeToRemove = new List<Number>();
            bool eventRaised = false;
            var transformer = new HeaderInjectableTransformer<Number, Number>(collection, GenerateHeader);

            Action<Observables.INotifyCollectionChanged<InjectedNumber>, NumberColection> transformedCollChanged =
                (coll, evtArg) =>

                {
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "----- " + rangeToRemove[0] + " - "
                            + rangeToRemove[rangeToRemove.Count - 1] + "----- Item Added");

                        assert.Equal(

                            evtArg.NewItems.Count,
                            itemsAdded,
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
                            "----- " + rangeToRemove[0] + " - " + rangeToRemove[rangeToRemove.Count - 1] + "----- Item Removed");

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

            rangeToRemove.Clear();
            rangeToRemove.Add(12);
            rangeToRemove.Add(22);
            removeIndex = 4;
            expectedIndex = 5;
            itemsAdded = 1;
            itemsRemoved = 5;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(33);
            rangeToRemove.Add(34);
            removeIndex = 5;
            expectedIndex = 7;
            itemsAdded = 0;
            itemsRemoved = 2;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(36);
            rangeToRemove.Add(37);
            removeIndex = 6;
            expectedIndex = 8;
            itemsAdded = 0;
            itemsRemoved = 2;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(39);
            rangeToRemove.Add(43);
            removeIndex = 7;
            expectedIndex = 9;
            itemsAdded = 1;
            itemsRemoved = 4;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(38);
            rangeToRemove.Add(60);
            removeIndex = 6;
            expectedIndex = 8;
            itemsAdded = 1;
            itemsRemoved = 3;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(61);
            rangeToRemove.Add(61);
            removeIndex = 6;
            expectedIndex = 8;
            itemsAdded = 1;
            itemsRemoved = 3;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(64);
            rangeToRemove.Add(65);
            removeIndex = 6;
            expectedIndex = 8;
            itemsAdded = 1;
            itemsRemoved = 4;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(32);
            rangeToRemove.Add(35);
            rangeToRemove.Add(76);
            rangeToRemove.Add(77);
            rangeToRemove.Add(82);
            removeIndex = 4;
            expectedIndex = 5;
            itemsAdded = 1;
            itemsRemoved = 9;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(98);
            rangeToRemove.Add(99);
            removeIndex = 7;
            expectedIndex = 9;
            itemsAdded = 0;
            itemsRemoved = 2;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);

            rangeToRemove.Clear();
            rangeToRemove.Add(91);
            rangeToRemove.Add(92);
            removeIndex = 4;
            expectedIndex = 5;
            itemsAdded = 1;
            itemsRemoved = 3;
            transformer.InputCollection.RemoveRangeAt(removeIndex, rangeToRemove.Count);


            transformer.TransformedCollection.CollectionChanged -= transformedCollChanged;

            assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestHeaderInjectionReplace(Assert assert)
        {
            ObservableCollection<Number> collection = new ObservableCollection<Number>();
            collection.Add(11);
            collection.Add(12);
            collection.Add(13);
            collection.Add(14);
            collection.Add(97);
            collection.Add(98);
            collection.Add(99);

            int newItemCount = 0;
            int itemsRemoved = 0;
            int insertIndex = 0;
            int expectedIndex = 0;
            Number replaceWithValue = 0;
            bool eventRaised = false;
            var transformer = new HeaderInjectableTransformer<Number, Number>(collection, GenerateHeader);

            Action<Observables.INotifyCollectionChanged<InjectedNumber>, NumberColection> transformedCollChanged =
                (coll, evtArg) =>

                {
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "----------------------- " + " -- " + replaceWithValue + "----- Item Added");

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
                            "----------------------- " + " -- " + replaceWithValue + "----- Item Added");

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

            replaceWithValue = 10;
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 9;
            insertIndex = 0;
            expectedIndex = 0;
            newItemCount = 3;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 19;
            insertIndex = 2;
            expectedIndex = 4;
            newItemCount = 1;
            itemsRemoved = 1;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 15;
            insertIndex = 3;
            expectedIndex = 5;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 91;
            insertIndex = 3;
            expectedIndex = 5;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 22;
            insertIndex = 3;
            expectedIndex = 5;
            newItemCount = 3;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 93;
            insertIndex = 4;
            expectedIndex = 7;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 29;
            insertIndex = 4;
            expectedIndex = 7;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 85;
            insertIndex = 5;
            expectedIndex = 8;
            newItemCount = 3;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 95;
            insertIndex = 6;
            expectedIndex = 10;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 102;
            insertIndex = 6;
            expectedIndex = 10;
            newItemCount = 2;
            itemsRemoved = 2;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 103;
            insertIndex = 7;
            expectedIndex = 12;
            newItemCount = 1;
            itemsRemoved = 1;
            transformer.InputCollection.Add(replaceWithValue);

            replaceWithValue = 105;
            insertIndex = 7;
            expectedIndex = 12;
            newItemCount = 1;
            itemsRemoved = 1;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            replaceWithValue = 111;
            insertIndex = 7;
            expectedIndex = 12;
            newItemCount = 2;
            itemsRemoved = 1;
            transformer.InputCollection[insertIndex] = replaceWithValue;

            transformer.TransformedCollection.CollectionChanged -= transformedCollChanged;

            assert.IsTrue(eventRaised, "Change event raised");
        }

        [Test]
        public static void TestHeaderInjectionReset(Assert assert)
        {
            ObservableCollection<Number> collection = new ObservableCollection<Number>();
            collection.Add(111);
            collection.Add(112);
            collection.Add(113);
            collection.Add(114);
            collection.Add(197);
            collection.Add(198);
            collection.Add(199);

            int newItemCount = 18;
            int itemsRemoved = 9;
            int expectedIndex = 0;
            bool eventRaised = false;

            var transformer = new HeaderInjectableTransformer<Number, Number>(collection, GenerateHeader);

            Action<Observables.INotifyCollectionChanged<InjectedNumber>, NumberColection> transformedCollChanged =
                (coll, evtArg) =>

                {
                    if (evtArg.Action == CollectionChangedAction.Add)
                    {
                        assert.IsTrue(
                            !Object.IsNullOrUndefined(evtArg.NewItems),
                            "Item Added");

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
                            "Item Removed");

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
            ObservableCollection<Number> resetCollection = new ObservableCollection<Number>();
            resetCollection.Add(11);
            resetCollection.Add(12);
            resetCollection.Add(13);
            resetCollection.Add(14);
            resetCollection.Add(97);
            resetCollection.Add(98);
            resetCollection.Add(99);
            resetCollection.Add(111);
            resetCollection.Add(112);
            resetCollection.Add(113);
            resetCollection.Add(114);
            resetCollection.Add(197);
            resetCollection.Add(198);
            resetCollection.Add(199);

            transformer.InputCollection = resetCollection;
            assert.IsTrue(eventRaised, "Change event raised");
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
            else
            {
                rv = secondHead;
            }

            return rv;
        }
    }
}