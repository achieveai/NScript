//-----------------------------------------------------------------------
// <copyright file="GenericCollections.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    public interface IList<T>
    {
        T this[int i]
        {
            get;
            set;
        }

        int Count
        {
            get;
            set;
        }

        void Add(T elem);

        void RemoveAt(int index);

        void InsertAt(T elem, int index);
    }

    public class List<T> : IList<T>
    {
        public T this[int i]
        {
            get
            {
                throw new Exception("Not Implemented");
            }
            set
            {
                throw new Exception("Not Implemented");
            }
        }

        public int Count
        {
            get
            {
                throw new Exception("Not Implemented");
            }
            set
            {
                throw new Exception("Not Implemented");
            }
        }

        public virtual void Add(T elem)
        {
            throw new Exception("Not Implemented");
        }

        public virtual void RemoveAt(int index)
        {
            throw new Exception("Not Implemented");
        }

        public virtual void InsertAt(T elem, int index)
        {
            throw new Exception("Not Implemented");
        }
    }
}
