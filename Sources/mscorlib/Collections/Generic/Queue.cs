//-----------------------------------------------------------------------
// <copyright file="Queue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;

    /// <summary>
    /// Definition for Queue
    /// </summary>
    public class Queue<T> : IEnumerable<T>
    {
        NativeArray nativeArray = new NativeArray(0);

        public void Clear()
        {
            this.nativeArray = new NativeArray(0);
        }

        public bool Contains(T item)
        {
            return this.nativeArray.IndexOf<T>(item, 0) >= 0;
        }

        public T Peek()
        {
            if (this.Count > 0)
            {
                return this.nativeArray.GetFrom<T>(0);
            }

            throw new Exception("No elements in stack");
        }

        public T Dequeue()
        {
            if (this.Count > 0)
            {
                return this.nativeArray.Pop<T>();
            }

            throw new Exception("No elements in stack");
        }

        public void Enqueue(T item)
        {
            this.nativeArray.InsertAt<T>(this.nativeArray.Length, item);
        }

        public int Count
        {
            get
            {
                return this.nativeArray.Length;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class QueueEnumerator : IEnumerator<T>
        {
            Queue<T> queue;
            int currentIndex;

            public QueueEnumerator(Queue<T> queue)
            {
                this.queue = queue;
                this.currentIndex = -1;
            }

            public T Current
            {
                get
                {
                    if (this.currentIndex < 0
                        || this.currentIndex >= this.queue.nativeArray.Length)
                    {
                        throw new Exception("Out of range");
                    }

                    return this.queue.nativeArray.GetFrom<T>(this.currentIndex);
                }
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                this.currentIndex++;
                return this.currentIndex < this.queue.nativeArray.Length;
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}
