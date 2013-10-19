//-----------------------------------------------------------------------
// <copyright file="ListComparer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ListComparer
    /// </summary>
    public class ListComparer<T> : IComparer<IList<T>>
    {
        /// <summary>
        /// The item comparer.
        /// </summary>
        private IComparer<T> itemComparer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer"> The item comparer. </param>
        public ListComparer(IComparer<T> itemComparer)
        {
            this.itemComparer = itemComparer;
        }

        /// <summary>
        /// Compares two IList&lt;T&gt; objects to determine their relative ordering.
        /// </summary>
        /// <param name="x"> I list&lt; t&gt; to be compared. </param>
        /// <param name="y"> I list&lt; t&gt; to be compared. </param>
        /// <returns>
        /// Negative if 'x' is less than 'y', 0 if they are equal, or positive if it is greater.
        /// </returns>
        public int Compare(IList<T> x, IList<T> y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }

            for (int i = 0, minLength = Math.Min(x.Count, y.Count); i < minLength; i++)
            {
                var res = this.itemComparer.Compare(x[i], y[i]);
                if (res != 0)
                {
                    return res;
                }
            }

            if (x.Count > y.Count)
            {
                return -1;
            }
            else if (y.Count == x.Count)
            {
                return 0;
            }

            return 1;
        }
    }

    public class ListEqualityComparer<T> : IEqualityComparer<IList<T>>
    {
        /// <summary>
        /// The item comparer.
        /// </summary>
        private IEqualityComparer<T> itemComparer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemComparer"> The item comparer. </param>
        public ListEqualityComparer(IEqualityComparer<T> itemComparer)
        {
            this.itemComparer = itemComparer;
        }

        /// <summary>
        /// Tests if two IList&lt;T&gt; objects are considered equal.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="x"> I list&lt; t&gt; to be compared. </param>
        /// <param name="y"> I list&lt; t&gt; to be compared. </param>
        /// <returns>
        /// true if the objects are considered equal, false if they are not.
        /// </returns>
        public bool Equals(IList<T> x, IList<T> y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            for (int i = 0, minLength = Math.Min(x.Count, y.Count); i < minLength; i++)
            {
                var res = this.itemComparer.Equals(x[i], y[i]);
                if (!res)
                {
                    return res;
                }
            }

            return x.Count == y.Count;
        }

        /// <summary>
        /// Calculates the hash code for this object.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="obj"> The object. </param>
        /// <returns>
        /// The hash code for this object.
        /// </returns>
        public int GetHashCode(IList<T> obj)
        {
            if (obj == null)
            {
                return -1;
            }

            int rv = obj.Count;
            for (int i = 0; i < obj.Count; i++)
            {
                rv = rv << 1 | rv >> 31;
                rv |= this.itemComparer.GetHashCode(obj[i]);
            }

            return rv;
        }
    }
}
