namespace System.Collections
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace, ScriptName("Array")]
    public sealed class ArrayList : IEnumerable
    {
        public ArrayList()
        {
        }

        public ArrayList(int capacity)
        {
        }

        public void Add(object item)
        {
        }

        public void AddRange(object[] items)
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

        public void Clear()
        {
        }

        public ArrayList Clone()
        {
            return null;
        }

        public ArrayList Concat(params object[] objects)
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

        public extern int IndexOf(object item, int startIndex);

        public extern void Insert(int index, object item);

        public extern void InsertRange(int index, object[] items);

        public extern string Join();

        public extern string Join(string delimiter);

        public extern Array Map(ArrayItemMapCallback mapItemCallback);

        public extern Array Map(ArrayMapCallback mapCallback);

        [IntrinsicOperator]
        public extern static explicit operator Array(ArrayList list);

        public static ArrayList Parse(string s)
        {
            return null;
        }

        public bool Remove(object item)
        {
            return false;
        }

        public void RemoveAt(int index)
        {
        }

        public Array RemoveRange(int index, int count)
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

        [IntrinsicProperty, ScriptName("length")]
        public int Count
        {
            get
            {
                return 0;
            }
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
    }
}

