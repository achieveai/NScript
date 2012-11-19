//-----------------------------------------------------------------------
// <copyright file="List.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for List
    /// </summary>
    public class List<T> : IList<T>
    {
        NativeArray nativeArray;

        public List()
        {
            this.nativeArray = new NativeArray(0);
        }

        public List(NativeArray nativeArray)
        {
            this.nativeArray = nativeArray;
        }

        public List(T[] array)
        {
            this.nativeArray = NativeArray.GetNativeArray(array).Slice(0, 0);
        }

        public extern T this[int index]
        {
            [Script(@"
                var arr = this.@{[mscorlib]System.Collections.Generic.List`1::nativeArray};
                if (index < 0 || index >= arr.length) throw ""index "" + index + "" out of range"";
                return arr[index];
                ")]
            get;

            [Script(@"
                var arr = this.@{[mscorlib]System.Collections.Generic.List`1::nativeArray};
                if (index < 0 || index >= arr.length) throw ""index "" + index + "" out of range"";
                return arr[index] = value;
                ")]
            set;
        }

        public int IndexOf(T item)
        {
            return this.nativeArray.IndexOf(item, 0);
        }

        public void Insert(int index, T item)
        {
            this.nativeArray.InsertAt(index, item);
        }

        public void InsertRange(int index, T[] items)
        {
            this.nativeArray.InsertRangeAt(index, NativeArray.GetNativeArray(items));
        }

        public void InsertRange(int index, List<T> items)
        {
            this.nativeArray.InsertRangeAt(index, items.nativeArray);
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.nativeArray.InsertAt(index++, item);
            }
        }

        public void RemoveAt(int index)
        {
            this.nativeArray.RemoveAt(index);
        }

        public int Count
        {
            get { return this.nativeArray.Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            this.nativeArray.Push(item);
        }

        public void AddRange(T[] items)
        {
            this.nativeArray =
                this.nativeArray.Concat(NativeArray.GetNativeArray(items));
        }

        public void AddRange(List<T> items)
        {
            this.nativeArray =
                this.nativeArray.Concat(items.nativeArray);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.nativeArray.Push(item);
            }
        }

        public void Clear()
        {
            this.nativeArray.Length = 0;
        }

        public bool Contains(T item)
        {
            return this.nativeArray.IndexOf(item, 0) >= 0;
        }

        public void CopyTo(T[] arr, int index)
        {
            for (int i = 0; i < this.nativeArray.Length; i++)
            {
                arr[i + index] = this[i];
            }
        }

        public bool Remove(T item)
        {
            int index = this.nativeArray.IndexOf(item, 0);
            if (index >= 0)
            {
                this.nativeArray.RemoveAt(index);
            }

            return index >= 0;
        }

        public void Sort(Func<T, T, int> sortFunction)
        {
            this.nativeArray.Sort(sortFunction);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class ListEnumerator<T> : IEnumerator<T>
    {
        private IList<T> innerList;
        private int index = -1;

        public ListEnumerator(IList<T> list)
        {
            this.innerList = list;
        }

        public T Current
        {
            get { return this.innerList[this.index]; }
        }

        public bool MoveNext()
        {
            return ++this.index < this.innerList.Count;
        }

        public void Reset()
        {
            this.index = -1;
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public void Dispose()
        { }
    }
}