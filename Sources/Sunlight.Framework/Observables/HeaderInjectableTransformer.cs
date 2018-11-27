//-----------------------------------------------------------------------
// <copyright file="HeaderInjectableTransformer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections.Generic;

    public class ListViewItemModel<I, H>
    {
        private H _header = default(H);
        private I _item = default(I);

        public H Header { get { return _header; } set { _header = value; } }
        public I Item { get { return _item; } set { _item = value; } }

        //public ListViewItemModel(I item, H header) {
        //    this.Item = item;
        //    this.Header = header;
        //    //this.
        //}
        //public Number GenerateHeader(int first, int second)
        //{
        //    if (first % 100 == second % 100)
        //    { return null; }

        //    return 100 * (second % 100);
        //}
    }

    /// <summary>
    /// Represents a dynamic data collection that provides notifications
    /// when items get added, removed, or when the entire list is refreshed.
    /// </summary>

    public class HeaderInjectableTransformer<T, H> where H : class
    {
        private ObservableCollection<T> _inputCollection;
        private ObservableCollection<ListViewItemModel<T, H>> _transformedCollection;
        private Func<T, T, H> _headerDelegate;
        private List<int> _allHeaderIndexes;

        public ObservableCollection<T> InputCollection
        {
            get { return this._inputCollection; }
            set { this._inputCollection = value; }
        }

        public ObservableCollection<ListViewItemModel<T, H>> TransformedCollection
        {
            get { return this._transformedCollection; }
            set { this._transformedCollection = value; }
        }

        public HeaderInjectableTransformer(
            ObservableCollection<T> inputCollection,
            Func<T, T, H> headerDelegate
            )
        {
            this.InputCollection = new ObservableCollection<T>();
            this.InputCollection = inputCollection.Clone();
            this._headerDelegate = headerDelegate;
            this._inputCollection.CollectionChanged += this.OnSourceChanged;
            this.BuildCollection();
        }

        private void BuildCollection()
        {
            this._transformedCollection = new ObservableCollection<ListViewItemModel<T, H>>();

            this._allHeaderIndexes = new List<int>();

            for (int idx = 0; idx < this.InputCollection.Count; idx++)
            {
                T item = InputCollection[idx];
                H header;

                if (idx != 0)
                {
                    header = _headerDelegate(InputCollection[idx - 1], item);
                }
                else
                {
                    header = _headerDelegate(default(T), item);
                }

                if (header != null)
                {
                    ListViewItemModel<T, H> headerEntry = new ListViewItemModel<T, H>();
                    headerEntry.Header = header;
                    this.TransformedCollection.Add(headerEntry);
                    this._allHeaderIndexes.Add(this.TransformedCollection.Count - 1);
                }

                ListViewItemModel<T, H> eachItem = new ListViewItemModel<T, H>();
                eachItem.Item = item;
                this.TransformedCollection.Add(eachItem);
            }
        }

        public void OnSourceChanged(
            INotifyCollectionChanged<T> obj1,
            CollectionChangedEventArgs<T> obj2)
        {
            int changeIndex = obj2.ChangeIndex;
            IList<T> changeList = obj2.NewItems;

            //int type = 0;
            //if (changeList[0] is int) {
            //    type = 1;
            //}
            //if (changeList[0] is Number)
            //{
            //    type = 2;
            //}

            //if (changeList[0] is String)
            //{
            //    type = 3;
            //}

            //if (changeList[0] is Object)
            //{
            //    type = 4;
            //}

            //if (type != 0) {

            //}

            if (obj2.Action == CollectionChangedAction.Add)
            {
                InsertNewItems(changeList, changeIndex);
                if (TransformedCollection != null)
                {
                }
            }
            //switch (obj2.Action)
            //{
            //    case CollectionChangedAction.Add:
            //        var newItems = obj2.NewItems;
            //        //var oldItems = obj2.OldItems;
            //        //var listToInsert = GetInsertedHeaderList(obj2.NewItems[0], newItems);
            //        //// TODO: Take care of headers at the end and start of the list
            //        //this.TransformedCollection.InsertRangeAt(
            //        //    this.GetOutPosition(obj2.ChangeIndex),
            //        //    listToInsert);
            //        //////for (int idx = 0; idx < obj2.NewItems.Count; idx++)
            //        //////{
            //        //////    ListViewItemModel<T, H> eachNewItem = new ListViewItemModel<T, H>();
            //        //////    eachNewItem.Item = obj2.NewItems[idx];
            //        //////    this.TransformedCollection.InsertAt(
            //        //////        3,
            //        //////        eachNewItem);
            //        //////}
            //        break;
            //    case CollectionChangedAction.Remove:
            //        //// TODO: Fix headers at boundaries.
            //        //this.TransformedCollection.RemoveRangeAt(
            //        //    this.GetOutPosition(obj2.ChangeIndex),
            //        //    this.GetOutPosition(obj2.ChangeIndex + obj2.OldItems.Count)
            //        //        - this.GetOutPosition(obj2.ChangeIndex));
            //        break;
            //    case CollectionChangedAction.Replace:
            //        //for (int idx = 0; idx < obj2.NewItems.Count; idx++)
            //        //{
            //        //    ListViewItemModel<T, H> eachNewItem = new ListViewItemModel<T, H>();
            //        //    eachNewItem.Item = obj2.NewItems[idx];
            //        //    this.TransformedCollection[idx + obj2.ChangeIndex] =
            //        //        eachNewItem;
            //        //}
            //        break;
            //    case CollectionChangedAction.Reset:
            //        //this.BuildCollection();
            //        break;
            //}
        }

        private void InsertNewItems(IList<T> changeList, int changeIndex)
        {
            bool addedBefore, addedAfter;
            var positionTuple = GetOutPosition(changeIndex);
            var outPosition = positionTuple.ElementIndex;
            var outHeaderPosition = positionTuple.HeaderIndex;
            var outHead = TransformedCollection[outPosition];
            if (outHead.Header == default(H))
            {
                this.TransformedCollection.InsertRangeAt(
                    outPosition+1,
                    GetInsertedHeaderList(
                        changeList,
                        false,
                        true,
                        false,
                        false,
                        0).Items);

                moveHeaderIndexes(outHeaderPosition,
                    changeList.Count);
            }
            else
            {
                List<T> wrappedList = WrapNeighbours(
                changeList,
                changeIndex,
                out addedBefore,
                out addedAfter);

                bool showFirstHead = changeIndex == 0;
                var tupleReturn = GetInsertedHeaderList(
                    wrappedList,
                    showFirstHead,
                    false,
                    addedBefore,
                    addedAfter,
                    outPosition-1);

                var listToInsert = tupleReturn.Items;
                var headersInserted = tupleReturn.HeaderIndexes;

                this.TransformedCollection.RemoveAt(outPosition);
                this.TransformedCollection.InsertRangeAt(
                  outPosition,
                  listToInsert);

                this._allHeaderIndexes.RemoveAt(outHeaderPosition);
                this._allHeaderIndexes.InsertRange(
                    outHeaderPosition,
                    headersInserted);

                int endHeadersOffset = outHeaderPosition + headersInserted.Count - 1;
                this.moveHeaderIndexes(endHeadersOffset
                    ,listToInsert.Count - 1);

                if (this._transformedCollection!=null) {

                }
            }
        }

        private List<T> WrapNeighbours(
            IList<T> changeList,
            int changeIdx,
            out bool addedBefore,
            out bool addedAfter)
        {
            List<T> wrappedList = new List<T>();
            addedBefore = changeIdx - 1 > 0;
            addedAfter = changeIdx + changeList.Count < _inputCollection.Count;

            if (addedBefore)
            {
                wrappedList.Add(_inputCollection[changeIdx - 1]);
            }

            wrappedList.AddRange(changeList);

            if (addedAfter)
            {
                wrappedList.Add(_inputCollection[changeIdx + changeList.Count]);
            }

            return wrappedList;
        }

        private TupleReturn GetInsertedHeaderList(
            IList<T> items,
            bool showFirstHead,
            bool skipCompare,
            bool addedBefore,
            bool addedAfter,
            int headerOffset)
        {
            var rv = new List<ListViewItemModel<T, H>>();
            var headerIdx = new List<int>();
            int insertionCount = headerOffset;

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
                            ListViewItemModel<T, H> headerEntry = new ListViewItemModel<T, H>();
                            headerEntry.Header = header;
                            rv.Add(headerEntry);
                            insertionCount++;
                            headerIdx.Add(insertionCount);
                        }
                    }

                    if (!(addedAfter && idx == items.Count - 1))
                    {
                        ListViewItemModel<T, H> eachItem = new ListViewItemModel<T, H>();
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

                            ListViewItemModel<T, H> headerEntry = new ListViewItemModel<T, H>();
                            headerEntry.Header = header;
                            rv.Add(headerEntry);
                            insertionCount++;
                            headerIdx.Add(insertionCount);
                        }

                        ListViewItemModel<T, H> eachItem = new ListViewItemModel<T, H>();
                        eachItem.Item = item;
                        rv.Add(eachItem);
                        insertionCount++;
                    }
                }
            }

            return new TupleReturn(headerIdx, rv);
        }

        /// <summary>
        /// replace rangeat - obs coll
        /// </summary>
        /// <param name="inIndex"></param>
        /// <returns></returns>

        private OutIndexTuple GetOutPosition(int inIndex)
        {
            if (inIndex == 0) {
                return new OutIndexTuple(0, 0);
            }

            if (_allHeaderIndexes.Count == 0)
            {
                return new OutIndexTuple(0, 0); // doubleCheck
            }
            else {

            }

            int outIdx = 0;
            int headerCount = 0;
            for (int idx = 0; idx < _allHeaderIndexes.Count; idx++)
            {
                var headIdx = _allHeaderIndexes[idx];
                if (headIdx > inIndex)
                {
                    outIdx = idx + inIndex;//1 for going before next header
                    headerCount = idx;
                    break;
                }
                // _outHeaderIndex[i] == _inHeaderIndex[i] + i
            }
            return new OutIndexTuple(outIdx, headerCount);

        }

        private void moveHeaderIndexes(int offset,int itemsInsertedCount) {
            //Can i use linq for this to change it to a one liner lambda function?
            for (int hid = 0; hid < this._allHeaderIndexes.Count; hid++)
            {
                if (hid > offset)
                {
                    _allHeaderIndexes[hid] = _allHeaderIndexes[hid] + itemsInsertedCount;
                }
            }
        }

        private struct OutIndexTuple
        {
            private readonly int _elementIndex;
            private readonly int _headerIndex;

            public int ElementIndex
            { get { return _elementIndex; } }

            public int HeaderIndex
            { get { return _headerIndex; } }

            public OutIndexTuple(int elementIndex, int headerIndex)
            {
                this._elementIndex = elementIndex;
                this._headerIndex = headerIndex;
            }
        }

        private struct TupleReturn
        {
            private readonly List<int> _headerIndexes;
            private readonly List<ListViewItemModel<T, H>> _items;

            public List<int> HeaderIndexes
            { get { return _headerIndexes; } }

            public List<ListViewItemModel<T, H>> Items
            { get { return _items; } }

            public TupleReturn(List<int> headerIndexes, List<ListViewItemModel<T, H>> items)
            {
                this._headerIndexes = headerIndexes;
                this._items = items;
            }
        }
    }
}