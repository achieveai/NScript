//-----------------------------------------------------------------------
// <copyright file="Stack.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;

    /// <summary>
    /// Definition for Stack
    /// </summary>
    public class Stack<T> : IEnumerable<T>
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

        public T Pop()
        {
            if (this.Count > 0)
            {
                return this.nativeArray.Pop<T>();
            }

            throw new Exception("No elements in stack");
        }

        public void Push(T item)
        {
            this.nativeArray.Push<T>(item);
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
            return new StackEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class StackEnumerator : IEnumerator<T>
        {
            Stack<T> stack;
            int currentIndex;

            public StackEnumerator(Stack<T> stack)
            {
                this.stack = stack;
                this.currentIndex = -1;
            }

            public T Current
            {
                get
                {
                    if (this.currentIndex < 0
                        || this.currentIndex >= this.stack.nativeArray.Length)
                    {
                        throw new Exception("Out of range");
                    }

                    return this.stack.nativeArray.GetFrom<T>(this.currentIndex);
                }
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                this.currentIndex++;
                return this.currentIndex < this.stack.nativeArray.Length;
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
