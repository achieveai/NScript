//-----------------------------------------------------------------------
// <copyright file="DomList.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Collections.Generic;

    /// <summary>
    /// Definition for DomList
    /// </summary>
    public sealed class DomList<T> : IList<T>
    {
        NativeArray<T> array;

        internal DomList(NativeArray<T> array)
        {
            this.array = array;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            for (int j = this.array.Length, i = 0; i < j; i++)
            {
                if (object.Equals(item, array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                if (index >= this.array.Length)
                {
                    throw new Exception("index not in range");
                }

                return this.array[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion IList<T> Members

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public void CopyTo(T[] arr, int index)
        {
            for (int i = 0; i < this.array.Length; i++)
            {
                arr[index + i] = this.array[i];
            }
        }

        public int Count
        {
            get { return this.array.Length; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion ICollection<T> Members

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new ListEnumerator<T>(this);
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable Members

        bool Collections.IList.IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        bool Collections.IList.IsReadOnly
        {
            get { return true; }
        }

        object Collections.IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void Collections.IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool Collections.IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int Collections.IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void Collections.IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void Collections.IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void Collections.IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void Collections.ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }
    }
}