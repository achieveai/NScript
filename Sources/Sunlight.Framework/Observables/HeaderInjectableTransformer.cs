//-----------------------------------------------------------------------
// <copyright file="HeaderInjectableTransformer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections.Generic;

    public class InjectedElement<I, H>
    {
        private H _header = default(H);
        private I _item = default(I);

        public H Header
        {
            get { return _header; }
            set { _header = value; }
        }
        public I Item
        {
            get { return _item; }
            set { _item = value; }
        }
    }

    // <summary>
    // Represents a dynamic data collection that groups the data and provides notifications
    // when items get added, removed, or when the entire list is refreshed.
    // </summary>

    public class HeaderInjectableTransformer<T, H> where H : class
    {
        private ObservableCollection<T> _inputCollection = new ObservableCollection<T>();
        private ObservableCollection<InjectedElement<T, H>> _transformedCollection = new ObservableCollection<InjectedElement<T, H>>();
        private List<int> _allHeaderIndexes = new List<int>();
        private Func<T, T, H> _headerDelegate;

        public HeaderInjectableTransformer(
            ObservableCollection<T> inputCollection,
            Func<T, T, H> headerDelegate)
        {
            this._inputCollection = inputCollection.Clone();
            this._headerDelegate = headerDelegate;
            this._inputCollection.CollectionChanged += this.OnSourceChanged;
            this.BuildCollection();
        }

        public ObservableCollection<T> InputCollection
        {
            get { return this._inputCollection; }

            set
            {
                if (this._inputCollection == value)
                { return; }

                if (this._inputCollection != null)
                {
                    this._inputCollection.CollectionChanged -= this.OnSourceChanged;
                    this._inputCollection = value;
                    this._inputCollection.CollectionChanged += this.OnSourceChanged;
                    this._transformedCollection.Clear();
                    this._allHeaderIndexes.Clear();
                    this.BuildCollection();
                }
            }
        }

        public ObservableCollection<InjectedElement<T, H>> TransformedCollection
        {
            get { return this._transformedCollection; }
        }

        private void BuildCollection()
        {
            var transformedCollection = new ObservableCollection<InjectedElement<T, H>>();
            var allHeaderIndexes = new List<int>();

            for (int idx = 0; idx < this._inputCollection.Count; idx++)
            {
                T item = this._inputCollection[idx];
                H header;

                if (idx != 0)
                {
                    header = _headerDelegate(
                        this._inputCollection[idx - 1],
                        item);
                }
                else
                {
                    header = _headerDelegate(
                        default(T),
                        item);
                }

                if (header != null)
                {
                    InjectedElement<T, H> headerEntry = new InjectedElement<T, H>();
                    headerEntry.Header = header;
                    transformedCollection.Add(headerEntry);
                    allHeaderIndexes.Add(transformedCollection.Count - 1);
                }

                InjectedElement<T, H> eachItem = new InjectedElement<T, H>();
                eachItem.Item = item;
                transformedCollection.Add(eachItem);
            }

            this._transformedCollection = transformedCollection;
            this._allHeaderIndexes = allHeaderIndexes;
        }

        public void OnSourceChanged(
            INotifyCollectionChanged<T> obj1,
            CollectionChangedEventArgs<T> obj2)
        {
            if (obj2.Action == CollectionChangedAction.Add)
            {
                this.InsertElements(obj2.NewItems, obj2.ChangeIndex);
            }
            else if (obj2.Action == CollectionChangedAction.Remove)
            {
                this.RemoveElements(obj2.OldItems, obj2.ChangeIndex);
            }
            else if (obj2.Action == CollectionChangedAction.Replace)
            {
                this.ReplaceElements(obj2.NewItems, obj2.ChangeIndex);
            }
        }

        private void InsertElements(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;
            var startIndexTuple = this.GetTransformedIndexes(changeIndex - 1);
            var insertIndex = startIndexTuple.ElementIndex + 1;
            var startHeaderIndex = startIndexTuple.HeaderIndex;

            if (insertIndex < this._transformedCollection.Count
                && this._transformedCollection[insertIndex].Item != null)
            {
                this._transformedCollection.InsertRangeAt(
                    insertIndex,
                    GetInsertedHeaderList(
                        changeList,
                        false,
                        true,
                        false,
                        false,
                        0).Items);

                this.MoveHeaderIndexes(
                    startHeaderIndex,
                    insertIndex,
                    changeList.Count);

                return;
            }

            List<T> wrappedList = WrapNeighbours(
                changeList,
                changeIndex,
                true,
                out addedBefore,
                out addedAfter);

            bool showFirstHead = changeIndex == 0;
            var tupleReturn = GetInsertedHeaderList(
                wrappedList,
                showFirstHead,
                false,
                addedBefore,
                addedAfter,
                insertIndex);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;
            this.RemoveExistingHeaders(
                insertIndex,
                startHeaderIndex,
                1);

            this._transformedCollection.InsertRangeAt(
                insertIndex,
                elementsToAdd);

            this.MoveHeaderIndexes(
                startHeaderIndex,
                insertIndex,
                elementsToAdd.Count - 1);

            this._allHeaderIndexes.InsertRange(
                startHeaderIndex,
                headersToAdd);

            return;
        }

        private void RemoveElements(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;
            var startIndexTuple = this.GetTransformedIndexes(changeIndex - 1);
            var insertIndex = startIndexTuple.ElementIndex + 1;
            var startHeaderIndex = startIndexTuple.HeaderIndex;
            var endIndexTuple = this.GetTransformedIndexes(changeIndex + changeList.Count);
            var endElementIndex = endIndexTuple.ElementIndex - 1;
            var endHeaderIndex = endIndexTuple.HeaderIndex;

            List<T> wrappedList = WrapNeighbours(
                changeList,
                changeIndex,
                false,
                out addedBefore,
                out addedAfter);

            bool showFirstHead = changeIndex == 0;
            var tupleReturn = GetInsertedHeaderList(
                wrappedList,
                showFirstHead,
                false,
                addedBefore,
                addedAfter,
                insertIndex);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;
            var countItemsRemoved = endElementIndex - insertIndex + 1;

            this._transformedCollection.RemoveRangeAt(
                insertIndex,
                countItemsRemoved);

            for (int ridx = startHeaderIndex; ridx < endHeaderIndex; ridx++)
            {
                this._allHeaderIndexes.RemoveAt(startHeaderIndex);
            }

            this._transformedCollection.InsertRangeAt(
                insertIndex,
                elementsToAdd);

            this.MoveHeaderIndexes(
                startHeaderIndex,
                insertIndex,
                headersToAdd.Count - countItemsRemoved);

            this._allHeaderIndexes.InsertRange(
                startHeaderIndex,
                headersToAdd);

            return;
        }

        private void ReplaceElements(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;
            var startIndexTuple = this.GetTransformedIndexes(changeIndex);
            var startElementIndex = startIndexTuple.ElementIndex;
            var startHeaderIndex = startIndexTuple.HeaderIndex;

            int insertionPoint;
            bool headerOnLeft;
            if (this._transformedCollection[startElementIndex - 1].Item == null)
            {
                insertionPoint = startElementIndex - 1;
                startHeaderIndex = startHeaderIndex - 1;
                headerOnLeft = true;
            }
            else if (startElementIndex + 1 >= this._transformedCollection.Count
                || this._transformedCollection[startElementIndex + 1].Item == null)
            {
                insertionPoint = startElementIndex;
                headerOnLeft = false;
            }
            else
            {
                this._transformedCollection.RemoveAt(startElementIndex);
                this._transformedCollection.InsertRangeAt(
                    startElementIndex,
                    GetInsertedHeaderList(
                        changeList,
                        false,
                        true,
                        false,
                        false,
                        0).Items);
                return;
            }

            List<T> wrappedList = WrapNeighbours(
                changeList,
                changeIndex,
                true,
                out addedBefore,
                out addedAfter);

            bool showFirstHead = changeIndex == 0;
            var tupleReturn = GetInsertedHeaderList(
                wrappedList,
                showFirstHead,
                false,
                addedBefore,
                addedAfter,
                insertionPoint);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;
            int removalCount = !headerOnLeft
                && startElementIndex == this._transformedCollection.Count - 1
                ? 1 : 2;

            this.RemoveExistingHeaders(
                insertionPoint,
                startHeaderIndex,
                removalCount);

            this._transformedCollection.InsertRangeAt(
                    insertionPoint,
                    elementsToAdd);

            this.MoveHeaderIndexes(
                startHeaderIndex,
                insertionPoint,
                elementsToAdd.Count - 2);

            this._allHeaderIndexes.InsertRange(
                startHeaderIndex,
                headersToAdd);

            return;
        }

        private List<T> WrapNeighbours(
            IList<T> changeList,
            int changeIdx,
            bool addChangeList,
            out bool addedBefore,
            out bool addedAfter)
        {
            List<T> wrappedList = new List<T>();
            addedBefore = changeIdx - 1 > 0;

            if (addedBefore)
            {
                wrappedList.Add(this._inputCollection[changeIdx - 1]);
            }

            if (addChangeList)
            {
                wrappedList.AddRange(changeList);
            }

            var endIndex = addChangeList
                    ? changeIdx + changeList.Count
                    : changeIdx;
            addedAfter = endIndex < this._inputCollection.Count;

            if (addedAfter)
            {
                wrappedList.Add(this._inputCollection[endIndex]);
            }

            return wrappedList;
        }

        private ElementsTuple GetInsertedHeaderList(
            IList<T> items,
            bool showFirstHead,
            bool skipCompare,
            bool addedBefore,
            bool addedAfter,
            int insertionCount)
        {
            var rv = new List<InjectedElement<T, H>>();
            var headerIdx = new List<int>();

            for (int idx = 0; idx < items.Count; idx++)
            {
                T item = items[idx];
                H header;

                if (idx != 0)
                {
                    if (!skipCompare)
                    {
                        header = _headerDelegate(items[idx - 1], item);

                        if (header != default(H))
                        {
                            InjectedElement<T, H> headerEntry = new InjectedElement<T, H>();
                            headerEntry.Header = header;
                            rv.Add(headerEntry);
                            headerIdx.Add(insertionCount);
                            insertionCount++;
                        }
                    }

                    if (!(addedAfter && idx == items.Count - 1))
                    {
                        InjectedElement<T, H> eachItem = new InjectedElement<T, H>();
                        eachItem.Item = item;
                        insertionCount++;
                        rv.Add(eachItem);
                    }
                }
                else
                {
                    if (!addedBefore || showFirstHead)
                    {
                        if (!skipCompare)
                        {
                            header = _headerDelegate(default(T), item);

                            InjectedElement<T, H> headerEntry = new InjectedElement<T, H>();
                            headerEntry.Header = header;
                            rv.Add(headerEntry);
                            headerIdx.Add(insertionCount);
                            insertionCount++;
                        }

                        InjectedElement<T, H> eachItem = new InjectedElement<T, H>();
                        eachItem.Item = item;
                        rv.Add(eachItem);
                        insertionCount++;
                    }
                }
            }

            return new ElementsTuple(rv, headerIdx);
        }

        private TransformedIndexTuple GetTransformedIndexes(int inIndex)
        {
            int elementIdx = 0;
            int headerCount = 0;
            bool headersExhausted = true;
            for (int idx = 0; idx < _allHeaderIndexes.Count; idx++)
            {
                var headIdx = _allHeaderIndexes[idx];
                elementIdx = inIndex + idx;
                if (elementIdx < headIdx)
                {
                    headerCount = idx;
                    headersExhausted = false;
                    break;
                }
            }

            if (headersExhausted)
            {
                headerCount = this._allHeaderIndexes.Count;
                elementIdx = inIndex + headerCount;
            }

            return new TransformedIndexTuple(elementIdx, headerCount);
        }

        private void MoveHeaderIndexes(int offset, int insertionIndex, int itemsInsertedCount)
        {
            if (insertionIndex >= _allHeaderIndexes[_allHeaderIndexes.Count - 1])
            {
                return;
            }

            for (int hid = 0; hid < this._allHeaderIndexes.Count; hid++)
            {
                if (hid >= offset)
                {
                    _allHeaderIndexes[hid] = _allHeaderIndexes[hid] + itemsInsertedCount;
                }
            }
        }

        private void RemoveExistingHeaders(int startElementIndex, int startHeaderIndex, int count)
        {
            if (startElementIndex > this._transformedCollection.Count - 1)
            {
                return;
            }
            else
            {
                this._transformedCollection.RemoveRangeAt(
                    startElementIndex,
                    count);
                this._allHeaderIndexes.RemoveAt(startHeaderIndex);
            }
        }

        private struct TransformedIndexTuple
        {
            private readonly int _elementIndex;
            private readonly int _headerCount;

            public TransformedIndexTuple(
                int elementIndex,
                int headerCount)
            {
                this._elementIndex = elementIndex;
                this._headerCount = headerCount;
            }

            public int ElementIndex
            {
                get { return _elementIndex; }
            }

            public int HeaderIndex
            {
                get { return _headerCount; }
            }

        }

        private struct ElementsTuple
        {
            private readonly List<int> _headerIndexes;
            private readonly List<InjectedElement<T, H>> _items;

            public ElementsTuple(
                List<InjectedElement<T, H>> items,
                List<int> headerIndexes)
            {
                this._items = items;
                this._headerIndexes = headerIndexes;
            }

            public List<int> HeaderIndexes
            {
                get { return _headerIndexes; }
            }

            public List<InjectedElement<T, H>> Items
            {
                get { return _items; }
            }

        }
    }
}