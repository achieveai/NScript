namespace System.Collections
{
    using System;
    using System.Runtime.CompilerServices;

    [ScriptName("Array"), IgnoreNamespace, Imported]
    public sealed class Queue
    {
        public void Clear()
        {
        }

        public bool Contains(object item)
        {
            return false;
        }

        public object Dequeue()
        {
            return null;
        }

        public void Enqueue(object item)
        {
        }

        public object Peek()
        {
            return null;
        }

        [ScriptName("length"), IntrinsicProperty]
        public int Count
        {
            get
            {
                return 0;
            }
        }
    }
}

