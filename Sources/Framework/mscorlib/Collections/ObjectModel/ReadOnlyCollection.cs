//-----------------------------------------------------------------------
// <copyright file="ReadOnlyCollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    // Summary:
    //     Provides the base class for a generic read-only collection.
    //
    // Type parameters:
    //   T:
    //     The type of elements in the collection.
    public class ReadOnlyCollection<T> : IList<T>, ICollection<T>, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        private IList<T> list;

        // Summary:
        //     Initializes a new instance of the System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     class that is a read-only wrapper around the specified list.
        //
        // Parameters:
        //   list:
        //     The list to wrap.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     list is null.
        public ReadOnlyCollection(IList<T> list)
        {
            this.list = list;
        }

        // Summary:
        //     Gets the number of elements contained in the System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     instance.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     instance.
        public int Count { get { return this.list.Count; } }
        //
        // Summary:
        //     Returns the System.Collections.Generic.IList<T> that the System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     wraps.
        //
        // Returns:
        //     The System.Collections.Generic.IList<T> that the System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     wraps.
        protected IList<T> Items { get { return this.list; } }

        // Summary:
        //     Gets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get.
        //
        // Returns:
        //     The element at the specified index.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than zero.-or-index is equal to or greater than System.Collections.ObjectModel.ReadOnlyCollection<T>.Count.
        public T this[int index]
        {
            get
            { return this.list[index]; }
            set
            {
                throw new NotSupportedException();
            }
        }

        // Summary:
        //     Determines whether an element is in the System.Collections.ObjectModel.ReadOnlyCollection<T>.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.ObjectModel.ReadOnlyCollection<T>.
        //     The value can be null for reference types.
        //
        // Returns:
        //     true if value is found in the System.Collections.ObjectModel.ReadOnlyCollection<T>;
        //     otherwise, false.
        public bool Contains(T value)
        { return this.list.Contains(value); }
        //
        // Summary:
        //     Copies the entire System.Collections.ObjectModel.ReadOnlyCollection<T> to
        //     a compatible one-dimensional System.Array, starting at the specified index
        //     of the target array.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from System.Collections.ObjectModel.ReadOnlyCollection<T>. The System.Array
        //     must have zero-based indexing.
        //
        //   index:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     index is less than zero.
        //
        //   System.ArgumentException:
        //     The number of elements in the source System.Collections.ObjectModel.ReadOnlyCollection<T>
        //     is greater than the available space from index to the end of the destination
        //     array.
        public void CopyTo(T[] array, int index)
        {
            this.list.CopyTo(array, index);
        }

        //
        // Summary:
        //     Returns an enumerator that iterates through the System.Collections.ObjectModel.ReadOnlyCollection<T>.
        //
        // Returns:
        //     An System.Collections.Generic.IEnumerator<T> for the System.Collections.ObjectModel.ReadOnlyCollection<T>.
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     first occurrence within the entire System.Collections.ObjectModel.ReadOnlyCollection<T>.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.Generic.List<T>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     The zero-based index of the first occurrence of item within the entire System.Collections.ObjectModel.ReadOnlyCollection<T>,
        //     if found; otherwise, -1.
        public int IndexOf(T value)
        {
            return this.list.IndexOf(value);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { return this.list.IsFixedSize; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        object IList.this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return this.list.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.list.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }


        public void CopyTo(Array array, int index)
        {
            this.list.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }


        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }
    }
}
