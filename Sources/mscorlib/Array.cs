namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public abstract class Array : IEnumerable
    {
        public extern int Length
        { get; }

        internal extern Array();

        public static void Sort<T>(T[] array, Func<T, int> comparator)
        {
            ((ArrayG<T>)(object)array).Sort(comparator);
        }

        public extern Array Clone();

        public extern bool Contains(object item);

        public extern IEnumerator GetEnumerator();

        public extern int IndexOf(object item);

        public extern int IndexOf(object item, int startIndex);

        public extern void Reverse();

        public extern object GetValue(int index);

        public extern void SetValue(int index, object value);
    }

    public abstract class ArrayImpl : IEnumerable
    {
        public abstract int Length
        {
            get;
        }

        internal ArrayImpl() { }

        public static void Sort<T>(T[] array, Func<T, int> comparator)
        {
            ((ArrayG<T>)(object)array).Sort(comparator);
        }

        public abstract ArrayImpl Clone();

        public abstract bool Contains(object item);

        public abstract IEnumerator GetEnumerator();

        public abstract int IndexOf(object item);

        public abstract int IndexOf(object item, int startIndex);

        public abstract void Reverse();

        public abstract object GetValue(int index);

        public abstract void SetValue(int index, object value);
    }
}