namespace System.Collections
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, ScriptName("Array"), Imported]
    public sealed class ArrayGrouping : IEnumerable
    {
        private ArrayGrouping()
        {
        }

        public object Aggregate(object seedValue, ArrayAggregator aggregator)
        {
            return null;
        }

        public object Aggregate(object seedValue, ArrayItemAggregator aggregator)
        {
            return null;
        }

        public Array Clone()
        {
            return null;
        }

        public Array Concat(params object[] objects)
        {
            return null;
        }

        public bool Contains(object item)
        {
            return false;
        }

        public bool Every(ArrayFilterCallback filterCallback)
        {
            return false;
        }

        public bool Every(ArrayItemFilterCallback itemFilterCallback)
        {
            return false;
        }

        public Array Extract(int index)
        {
            return null;
        }

        public Array Extract(int index, int count)
        {
            return null;
        }

        public Array Filter(ArrayFilterCallback filterCallback)
        {
            return null;
        }

        public Array Filter(ArrayItemFilterCallback itemFilterCallback)
        {
            return null;
        }

        public void ForEach(ArrayCallback callback)
        {
        }

        public void ForEach(ArrayItemCallback itemCallback)
        {
        }

        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public ArrayGrouping[] GroupBy(ArrayItemKeyGenerator keyCallback)
        {
            return null;
        }

        public Dictionary Index(ArrayItemKeyGenerator keyCallback)
        {
            return null;
        }

        public int IndexOf(object item)
        {
            return 0;
        }

        public int IndexOf(object item, int startIndex)
        {
            return 0;
        }

        public string Join()
        {
            return null;
        }

        public string Join(string delimiter)
        {
            return null;
        }

        public Array Map(ArrayItemMapCallback mapItemCallback)
        {
            return null;
        }

        public Array Map(ArrayMapCallback mapCallback)
        {
            return null;
        }

        public void Reverse()
        {
        }

        public bool Some(ArrayFilterCallback filterCallback)
        {
            return false;
        }

        public bool Some(ArrayItemFilterCallback itemFilterCallback)
        {
            return false;
        }

        public void Sort()
        {
        }

        public void Sort(CompareCallback compareCallback)
        {
        }

        [IntrinsicProperty]
        public object this[int index]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        [IntrinsicProperty]
        public string Key
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public int Length
        {
            get
            {
                return 0;
            }
        }
    }
}

