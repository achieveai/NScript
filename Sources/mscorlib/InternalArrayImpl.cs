//-----------------------------------------------------------------------
// <copyright file="InternalArrayImpl.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Array[] implementation
    /// </summary>
    internal class ArrayG<T> : ArrayImpl, IList<T>
    {
        private readonly NativeArray<T> innerArray;

        public ArrayG(int size)
        {
            this.innerArray = new NativeArray<T>(size);

            T def = default(T);
            for (int i = 0; i < size; i++)
            {
                this.innerArray[i] = def;
            }
        }

        public ArrayG(NativeArray<T> nativeArray)
        {
            this.innerArray = nativeArray;
        }

        public override int Length
        {
            get { return this.innerArray.Length; }
        }

        public extern T this[int index]
        {
            [Script(@"
                var arr = this.@{[mscorlib]System.ArrayG`1::innerArray};
                if (index < 0 || index >= arr.length) throw ""index "" + index + "" out of range"";
                return arr[index];
                ")]
            get;

            [Script(@"
                var arr = this.@{[mscorlib]System.ArrayG`1::innerArray};
                if (index < 0 || index >= arr.length) throw ""index "" + index + "" out of range"";
                return arr[index] = value;
                ")]
            set;
        }

        public int Count
        {
            get { return this.innerArray.Length; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        internal NativeArray<T> InnerArray
        {
            get { return this.innerArray; }
        }

        public override ArrayImpl Clone()
        {
            return new ArrayG<T>(
                this.innerArray.Slice(0, this.innerArray.Length));
        }

        public override bool Contains(object item)
        {
            return this.IndexOf(item) >= 0;
        }

        public override int IndexOf(object item)
        {
            if (!(item is T))
            {
                return -1;
            }

            return this.innerArray.IndexOf((T)item, 0);
        }

        public override int IndexOf(object item, int startIndex)
        {
            if (!(item is T))
            {
                return -1;
            }

            return this.innerArray.IndexOf((T)item, startIndex);
        }

        public override void Reverse()
        {
            this.innerArray.Reverse();
        }

        [Script(@"
            this.@{[mscorlib]System.ArrayG`1::innerArray}.sort(compareCallback);
            ")]
        public extern void Sort(Func<T, int> compareCallback);

        public override object GetValue(int index)
        {
            return this[index];
        }

        public override void SetValue(int index, object value)
        {
            this[index] = (T)value;
        }

        public int IndexOf(T item)
        {
            return this.innerArray.IndexOf((T)item, 0);
        }

        public void Insert(int index, T item)
        {
            throw new Exception("Not Implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("Not Implemented.");
        }

        public void Add(T item)
        {
            throw new Exception("Not Implemented.");
        }

        public void Clear()
        {
            throw new Exception("Not Implemented.");
        }

        public bool Contains(T item)
        {
            return this.innerArray.IndexOf(item, 0) != -1;
        }

        public void CopyTo(T[] arr, int index)
        {
            for (int i = 0; i < this.innerArray.Length; i++)
            {
                arr[index + i] = this[i];
            }
        }

        public bool Remove(T item)
        {
            return this.innerArray.IndexOf(item, 0) != -1;
        }

        public override IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private class Enumerator : Collections.Generic.IEnumerator<T>
        {
            int currentIndex;
            ArrayG<T> array;

            public Enumerator(ArrayG<T> array)
            {
                this.currentIndex = -1;
                this.array = array;
            }

            public bool MoveNext()
            {
                return ++this.currentIndex < this.array.Length;
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }

            public T Current
            {
                get
                {
                    return this.array[this.currentIndex];
                }
            }

            object Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            { }
        }
    }
}