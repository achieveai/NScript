//-----------------------------------------------------------------------
// <copyright file="List.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for List.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public class List<T> : IList<T>
    {
        /// <summary>
        /// Array of natives.
        /// </summary>
        NativeArray<T> nativeArray;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public List()
        {
            this.nativeArray = new NativeArray<T>(0);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nativeArray"> Array of natives. </param>
        public List(NativeArray<T> nativeArray)
        {
            this.nativeArray = nativeArray;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="array"> The array. </param>
        public List(T[] array)
        {
            // The problem is that not all arrays contain NativeArray, some of them (specially DOM stuff)
            // contain DOMList of DOMCollection which do not have splice methond on them
            var arrayNativeArray = NativeArray<T>.GetNativeArray(array);
            if (((object)arrayNativeArray) is NativeArray<T>)
            {
                this.nativeArray = NativeArray<T>.GetNativeArray(array).Slice(0, arrayNativeArray.Length);
            }
            else
            {
                this.nativeArray = new NativeArray<T>(arrayNativeArray.Length);
                for (int i = arrayNativeArray.Length - 1; i >= 0; i--)
                {
                    this.nativeArray[i] = arrayNativeArray[i];
                }
            }
        }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public extern T this[int index]
        {
            [Script(@"
                var arr = this.@{[mscorlib]System.Collections.Generic.List`1::nativeArray};
                if (index < 0 || index >= arr.length) throw new @{[mscorlib]System.Exception}(""index "" + index + "" out of range"");
                return arr[index];
                ")]
            get;

            [Script(@"
                var arr = this.@{[mscorlib]System.Collections.Generic.List`1::nativeArray};
                if (index < 0 || index >= arr.length) throw new @{[mscorlib]System.Exception}(""index "" + index + "" out of range"");
                return arr[index] = value;
                ")]
            set;
        }

        /// <summary>
        /// Gets an array of inners.
        /// </summary>
        /// <value>
        /// An Array of inners.
        /// </value>
        internal NativeArray<T> InnerArray
        {
            get { return this.nativeArray; }
        }

        /// <summary>
        /// NativeArray casting operator.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="n"> The List&lt;T&gt; to process. </param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator NativeArray<T>(List<T> n)
        {
            return n.nativeArray;
        }

        /// <summary>
        /// List casting operator.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="n"> The List&lt;T&gt; to process. </param>
        public static implicit operator List<T>(NativeArray<T> n)
        {
            return n == null ? (List<T>)null : new List<T>(n);
        }

        /// <summary>
        /// Index of the given item.
        /// </summary>
        /// <param name="item"> The item. </param>
        /// <returns>
        /// .
        /// </returns>
        public int IndexOf(T item)
        {
            return this.nativeArray.IndexOf(item, 0);
        }

        /// <summary>
        /// Index of the given item.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        /// index of value, -1 if not found.
        /// </returns>
        int System.Collections.IList.IndexOf(object value)
        {
            if (value == null
                && value is T)
            {
                return this.nativeArray.IndexOf((T)value, 0);
            }

            return -1;
        }

        /// <summary>
        /// Inserts.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="item">  The item. </param>
        public void Insert(int index, T item)
        {
            this.nativeArray.InsertAt(index, item);
        }

        /// <summary>
        /// Inserts.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="value"> The value. </param>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        /// <summary>
        /// Inserts a range.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="items"> The items. </param>
        public void InsertRange(int index, T[] items)
        {
            this.nativeArray.InsertRangeAt(index, NativeArray<T>.GetNativeArray(items));
        }

        /// <summary>
        /// Inserts a range.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="items"> The items. </param>
        public void InsertRange(int index, List<T> items)
        {
            this.nativeArray.InsertRangeAt(index, items.nativeArray);
        }

        /// <summary>
        /// Inserts a range.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <param name="items"> The items. </param>
        public void InsertRange(int index, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.nativeArray.InsertAt(index++, item);
            }
        }

        /// <summary>
        /// Removes at described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        public void RemoveAt(int index)
        {
            this.nativeArray.RemoveAt(index);
        }

        /// <summary>
        /// Removes at described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        public void RemoveRangeAt(int index, int count)
        {
            while (count-- > 0)
            { this.nativeArray.RemoveAt(index); }
        }

        public List<T> GetRangeAt(int index, int count)
        {
            var rv = new List<T>();
            for (int idx = index; idx < this.Count; idx++)
            {
                if (idx < index + count)
                {
                    rv.Add(this.nativeArray[idx]);
                }
            }

            return rv;
        }

        /// <summary>
        /// Gets the number of. 
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return this.nativeArray.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is read only.
        /// </summary>
        /// <value>
        /// true if this object is read only, false if not.
        /// </value>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds item.
        /// </summary>
        /// <param name="item"> The item. </param>
        public void Add(T item)
        {
            this.nativeArray.Push(item);
        }

        void System.Collections.IList.Add(object value)
        {
            this.Add((T)value);
        }

        /// <summary>
        /// Adds a range.
        /// </summary>
        /// <param name="items"> The items. </param>
        public void AddRange(T[] items)
        {
            this.nativeArray =
                this.nativeArray.Concat(NativeArray<T>.GetNativeArray(items));
        }

        /// <summary>
        /// Adds a range.
        /// </summary>
        /// <param name="items"> The items. </param>
        public void AddRange(List<T> items)
        {
            this.nativeArray =
                this.nativeArray.Concat(items.nativeArray);
        }

        /// <summary>
        /// Adds a range.
        /// </summary>
        /// <param name="items"> The items. </param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.nativeArray.Push(item);
            }
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            this.nativeArray.Length = 0;
        }

        /// <summary>
        /// Query if this object contains the given item.
        /// </summary>
        /// <param name="item"> The item. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        public bool Contains(T item)
        {
            return this.nativeArray.IndexOf(item, 0) >= 0;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="arr">   The arr. </param>
        /// <param name="index"> Zero-based index of the. </param>
        public void CopyTo(T[] arr, int index)
        {
            var nativeArray = this.nativeArray;
            var length = nativeArray.Length;
            for (int i = 0; i < length; i++)
            {
                arr[i + index] = nativeArray[i];
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var nativeArray = this.nativeArray;
            var length = nativeArray.Length;
            for (int i = 0; i < length; i++)
            {
                array.SetValue(i + index, nativeArray[i]);
            }
        }

        /// <summary>
        /// Convert this object into an array representation.
        /// </summary>
        /// <returns>
        /// This object as a T[].
        /// </returns>
        public T[] ToArray()
        {
            return
                (T[])(object)new ArrayG<T>(this.nativeArray.Slice(0, this.nativeArray.Length));
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item"> The item. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Remove(T item)
        {
            int index = this.nativeArray.IndexOf(item, 0);
            if (index >= 0)
            {
                this.nativeArray.RemoveAt(index);
            }

            return index >= 0;
        }

        void IList.Remove(object value)
        {
            if (value == null
                && value is T)
            {
                this.Remove((T)value);
            }
        }

        /// <summary>
        /// Sorts the given sort function.
        /// </summary>
        /// <param name="sortFunction"> The sort function. </param>
        public void Sort(Func<T, T, int> sortFunction)
        {
            this.nativeArray.Sort(sortFunction);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>(this);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets a value indicating whether this object is fixed size.
        /// </summary>
        /// <value>
        /// true if this object is fixed size, false if not.
        /// </value>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                this[index] = (T)value;
            }
        }

        /// <summary>
        /// Query if this object contains the given item.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        bool IList.Contains(object value)
        {
            return ((IList)this).IndexOf(value) >= 0;
        }
    }

    /// <summary>
    /// List enumerator.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public class ListEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// List of inners.
        /// </summary>
        private IList<T> innerList;

        /// <summary>
        /// Zero-based index of the.
        /// </summary>
        private int index = -1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="list"> The list. </param>
        public ListEnumerator(IList<T> list)
        {
            this.innerList = list;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public T Current
        {
            get { return this.innerList[this.index]; }
        }

        /// <summary>
        /// Determines if we can move next.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool MoveNext()
        {
            return ++this.index < this.innerList.Count;
        }

        /// <summary>
        /// Resets this object.
        /// </summary>
        public void Reset()
        {
            this.index = -1;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        { }
    }
}