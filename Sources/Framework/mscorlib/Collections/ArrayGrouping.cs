namespace System.Collections
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, ScriptName("Array"), Extended]
    public sealed class ArrayGrouping : IEnumerable
    {
        private extern ArrayGrouping();

        public extern object Aggregate(object seedValue, ArrayAggregator aggregator);

        public extern object Aggregate(object seedValue, ArrayItemAggregator aggregator);

        public extern Array Clone();

        public extern Array Concat(params object[] objects);

        public extern bool Contains(object item);

        public extern bool Every(ArrayFilterCallback filterCallback);

        public extern bool Every(ArrayItemFilterCallback itemFilterCallback);

        public extern Array Extract(int index);

        public extern Array Extract(int index, int count);

        public extern Array Filter(ArrayFilterCallback filterCallback);

        public extern Array Filter(ArrayItemFilterCallback itemFilterCallback);

        public extern void ForEach(ArrayCallback callback);

        public extern void ForEach(ArrayItemCallback itemCallback);

        public extern IEnumerator GetEnumerator();

        public extern ArrayGrouping[] GroupBy(ArrayItemKeyGenerator keyCallback);

        public extern Dictionary Index(ArrayItemKeyGenerator keyCallback);

        public extern int IndexOf(object item);

        public extern int IndexOf(object item, int startIndex);

        public extern string Join();

        public extern string Join(string delimiter);

        public extern Array Map(ArrayItemMapCallback mapItemCallback);

        public extern Array Map(ArrayMapCallback mapCallback);

        public extern void Reverse();

        public extern bool Some(ArrayFilterCallback filterCallback);

        public extern bool Some(ArrayItemFilterCallback itemFilterCallback);

        public extern void Sort();

        public extern void Sort(CompareCallback compareCallback);

        public extern object this[int index] { get; set; }

        public extern string Key { get; }

        public extern int Length { get; }
    }
}

