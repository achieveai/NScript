namespace System.Collections
{
    using System;
    using System.Runtime.CompilerServices;

    [ScriptName("Array"), Extended, IgnoreNamespace]
    public sealed class Stack
    {
        public void Clear()
        {
        }

        public bool Contains(object item)
        {
            return false;
        }

        public object Peek()
        {
            return null;
        }

        public object Pop()
        {
            return null;
        }

        public void Push(object item)
        {
        }

        [IntrinsicProperty, ScriptName("length")]
        public int Count
        {
            get
            {
                return 0;
            }
        }
    }
}

