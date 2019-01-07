//-----------------------------------------------------------------------
// <copyright file="HeaderInjectableTransformer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections.Generic;

    public interface IHeaderedElement
    {
        bool IsHeader { get; }
        object Header { get; }
        object Item { get; }
    }

    public class InjectedElement<I, H> : IHeaderedElement
        where H : class
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

        bool IHeaderedElement.IsHeader
        { get { return this.Header != null; } }

        object IHeaderedElement.Header
        { get { return this.Header; } }

        object IHeaderedElement.Item
        { get { return this.Item; } }
    }

    // <summary>
    // Represents a dynamic data collection that groups the data and provides notifications
    // when items get added, removed, or when the entire list is refreshed.
    // </summary>

    public class HeaderInjectableTransformer<T, H> where H : class
    {
        private ObservableCollection<T> _inputCollection = null;
        private readonly ObservableCollection<InjectedElement<T, H>> _transformedCollection = new ObservableCollection<InjectedElement<T, H>>();
        private List<int> _allHeaderIndexes = new List<int>();
        private Func<T, T, H> _headerDelegate;

        public HeaderInjectableTransformer(
            Func<T, T, H> headerDelegate,
            ObservableCollection<T> inputCollection = null)
        {
            this._headerDelegate = headerDelegate;
            this.InputCollection = inputCollection;
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
                }
                else
                {
                    this._inputCollection = new ObservableCollection<T>();
                }

                var inputCollection = this._inputCollection;
                this._inputCollection = value;

                if (this._inputCollection != null)
                {
                    this._inputCollection.CollectionChanged += this.OnSourceChanged;
                }

                // Remove (if applicable, i.e. new list is null or is smaller than previous list).
                // Replace Elements
                // Add, if new list is bigger than older list or older list was null.
                // default null to 0 and check 0 length inside methods
                var newLength = value != null
                    ? value.Count
                    : 0;
                var oldLength = inputCollection
                    != null
                    ? inputCollection.Count
                    : 0;
                var lengthDifference = newLength - oldLength;

                if (lengthDifference < 0)
                {
                    //Overall extra elements are removed
                    this.RemoveAfterKItems(
                        inputCollection,
                        newLength,
                        -lengthDifference);
                    this.ReplaceFirstKItems(
                        value,
                        newLength);
                }
                else
                {
                    //Overall extra elements are replaced and/or added
                    this.ReplaceFirstKItems(
                        value,
                        oldLength);
                    this.InsertAfterFirstKItems(
                        value,
                        oldLength,
                        lengthDifference);
                }
                return;
            }
        }

        public ObservableCollection<InjectedElement<T, H>> TransformedCollection
        {
            get { return this._transformedCollection; }
        }

        public void OnSourceChanged(
            INotifyCollectionChanged<T> obj1,
            CollectionChangedEventArgs<T> obj2)
        {
            if (obj2.Action == CollectionChangedAction.Add)
            {
                this.InsertElements(
                    obj2.NewItems,
                    obj2.ChangeIndex);
            }
            else if (obj2.Action == CollectionChangedAction.Remove)
            {
                this.RemoveElements(
                    obj2.OldItems,
                    obj2.ChangeIndex);
            }
            else if (obj2.Action == CollectionChangedAction.Replace)
            {
                this.ReplaceElements(
                    obj2.NewItems,
                    obj2.ChangeIndex);
            }
            return;
        }

        private void InsertElements(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;

            //index previous to changeindex
            var leftNeighbourTuple = this.GetTransformedIndexes(changeIndex - 1);
            var leftElementIndex = leftNeighbourTuple.ElementIndex;
            var leftHeaderCount = leftNeighbourTuple.HeaderCount;

            //index of element after last element of changeList in the future transformed collection. Insertion happens at one point
            var rightNeighbourTuple = this.GetTransformedIndexes(changeIndex);
            var rightElementIndex = rightNeighbourTuple.ElementIndex;
            var rightHeaderCount = rightNeighbourTuple.HeaderCount;

            //point and count of insertion
            var boundaryElementIndex = leftElementIndex + 1;
            var boundaryHeaderIndex = leftHeaderCount;
            var boundaryElementCount = rightHeaderCount - leftHeaderCount;//will either be 1 or 0 always
            var boundaryHeaderCount = rightHeaderCount - leftHeaderCount;

            //using number of headers instead of null check
            if (leftHeaderCount == rightHeaderCount
                && boundaryElementIndex < this._transformedCollection.Count)
            {
                this._transformedCollection.InsertRangeAt(
                    boundaryElementIndex,
                    GetInsertedHeaderList(
                        changeList,
                        false,
                        true,
                        false,
                        false,
                        0).Items);

                this.MoveHeaderIndexes(
                    boundaryHeaderIndex,
                    boundaryElementIndex,
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
                boundaryElementIndex);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;

            this.RemoveElementsAndHeaderIndexes(
                boundaryElementIndex,
                boundaryHeaderIndex,
                boundaryElementCount,
                boundaryHeaderCount);

            this.MoveHeaderIndexes(
                boundaryHeaderIndex,
                boundaryElementIndex,
                elementsToAdd.Count - boundaryElementCount);

            this._transformedCollection.InsertRangeAt(
                boundaryElementIndex,
                elementsToAdd);

            this._allHeaderIndexes.InsertRange(
                boundaryHeaderIndex,
                headersToAdd);

            return;
        }

        private void RemoveElements(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;
            var leftNeighbourTuple = this.GetTransformedIndexes(changeIndex - 1);//index previous to changeindex
            var leftElementIndex = leftNeighbourTuple.ElementIndex;
            var leftHeaderCount = leftNeighbourTuple.HeaderCount;

            var rightNeighbourTuple = this.GetTransformedIndexes(changeIndex + changeList.Count);//index of element after last element of changeList
            var rightElementIndex = rightNeighbourTuple.ElementIndex;
            var rightHeaderCount = rightNeighbourTuple.HeaderCount;

            var boundaryElementIndex = leftElementIndex + 1;//index of transformed change
            var boundaryHeaderIndex = leftHeaderCount;
            var boundaryElementCount = rightElementIndex - leftElementIndex - 1;//will either be 1 or 0 always using header cound as only headers need to be removed
            var boundaryHeaderCount = rightHeaderCount - leftHeaderCount;

            if (boundaryElementCount == this._transformedCollection.Count)
            {
                //array being emptied
                this._transformedCollection.RemoveRangeAt(
                    0,
                    this._transformedCollection.Count);
                this._allHeaderIndexes.Clear();
                return;
            }

            if (leftHeaderCount == rightHeaderCount
                && boundaryElementIndex < this._transformedCollection.Count)
            {
                this._transformedCollection.RemoveRangeAt(
                    boundaryElementIndex,
                    changeList.Count);

                this.MoveHeaderIndexes(
                    boundaryHeaderIndex,
                    boundaryElementIndex,
                    -changeList.Count);

                return;
            }

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
                boundaryElementIndex);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;

            this.RemoveElementsAndHeaderIndexes(
                boundaryElementIndex,
                boundaryHeaderIndex,
                boundaryElementCount,
                boundaryHeaderCount);

            this.MoveHeaderIndexes(
                boundaryHeaderIndex,
                boundaryElementIndex,
                elementsToAdd.Count - boundaryElementCount);

            this.InsertElementsAndHeaderIndexes(
                boundaryElementIndex,
                boundaryHeaderIndex,
                elementsToAdd,
                headersToAdd);
            return;
        }

        private void ReplaceElements(IList<T> changeList, int changeIndex)
        {
            // Just replace, else assume headers can be anywhere (incl the edges)

            bool addedBefore, addedAfter;

            //index previous to changeindex
            var leftNeighbourTuple = this.GetTransformedIndexes(changeIndex - 1);
            var leftElementIndex = leftNeighbourTuple.ElementIndex;
            var leftHeaderCount = leftNeighbourTuple.HeaderCount;

            //index of element after last element of changeList. Insertion happens over a range of value
            var rightNeighbourTuple = this.GetTransformedIndexes(changeIndex + changeList.Count);
            var rightElementIndex = rightNeighbourTuple.ElementIndex;
            var rightHeaderCount = rightNeighbourTuple.HeaderCount;

            //point and count of insertion
            var boundaryElementIndex = leftElementIndex + 1;//point of insertion
            var boundaryHeaderIndex = leftHeaderCount;
            var boundaryElementCount = rightElementIndex - leftElementIndex - 1;
            var boundaryHeaderCount = rightHeaderCount - leftHeaderCount;

            if (leftHeaderCount == rightHeaderCount
                && boundaryElementIndex < this._transformedCollection.Count - 1)
            {
                this._transformedCollection.ReplaceRangeAt(
                    boundaryElementIndex,
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
                boundaryElementIndex);

            var elementsToAdd = tupleReturn.Items;
            var headersToAdd = tupleReturn.HeaderIndexes;

            this.RemoveElementsAndHeaderIndexes(
                boundaryElementIndex,
                boundaryHeaderIndex,
                boundaryElementCount,
                boundaryHeaderCount);

            this._transformedCollection.InsertRangeAt(
                boundaryElementIndex,
                elementsToAdd);

            this.MoveHeaderIndexes(
                boundaryHeaderIndex,
                boundaryElementIndex,
                elementsToAdd.Count - boundaryElementCount);

            this._allHeaderIndexes.InsertRange(
                boundaryHeaderIndex,
                headersToAdd);

            return;
        }

        private void ReplaceFirstKItems(ObservableCollection<T> collection, int k)
        {
            if (k == 0 || collection == null)
            {
                return;
            }
            else
            {
                var elementsToReplace = collection.GetRangeAt(
                    0,
                    k);

                this.ReplaceElements(
                elementsToReplace,
                0);
            }
        }

        private void InsertAfterFirstKItems(ObservableCollection<T> collection, int k, int count)
        {
            if (count == 0 || collection == null)
            {
                return;
            }
            else
            {
                var elementsToInsert = collection.GetRangeAt(
                        k,
                        count);

                this.InsertElements(
                    elementsToInsert,
                    k);
            }
        }

        private void RemoveAfterKItems(ObservableCollection<T> collection, int k, int count)
        {
            if (count == 0 || collection == null)
            {
                return;
            }
            else
            {
                var elementsToRemove = collection.GetRangeAt(
                    k,
                    count);

                this.RemoveElements(
                    elementsToRemove,
                    k);
            }
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

                            if (header != default(H))
                            {
                                InjectedElement<T, H> headerEntry = new InjectedElement<T, H>();
                                headerEntry.Header = header;
                                rv.Add(headerEntry);
                                headerIdx.Add(insertionCount);
                                insertionCount++;
                            }
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
            if (_allHeaderIndexes.Count == 0
                || insertionIndex >= _allHeaderIndexes[_allHeaderIndexes.Count - 1])
            {
                return;
            }

            for (int hid = offset; hid < this._allHeaderIndexes.Count; hid++)
            {
                _allHeaderIndexes[hid] = _allHeaderIndexes[hid] + itemsInsertedCount;
            }
        }

        private void RemoveElementsAndHeaderIndexes(int startElementIndex, int startHeaderIndex, int elementcount, int headerCount)
        {
            if (startElementIndex == this._transformedCollection.Count
                || elementcount == 0)
            {
                return;
            }
            else
            {
                this._transformedCollection.RemoveRangeAt(
                    startElementIndex,
                    elementcount);
                this._allHeaderIndexes.RemoveRangeAt(startHeaderIndex, headerCount);
            }
        }

        private void InsertElementsAndHeaderIndexes(int startElementIndex, int startHeaderIndex, List<InjectedElement<T, H>> elementsToAdd, List<int> headersToAdd)
        {
            if (startElementIndex == this._transformedCollection.Count
                || elementsToAdd.Count == 0
                || headersToAdd.Count == 0)
            {
                return;
            }
            else
            {
                this._transformedCollection.InsertRangeAt(
                    startElementIndex,
                    elementsToAdd);

                this._allHeaderIndexes.InsertRange(
                    startHeaderIndex,
                    headersToAdd);
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

            public int HeaderCount
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