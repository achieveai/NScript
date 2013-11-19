namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public abstract class Array : IList, IEnumerable
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

        extern bool IList.IsFixedSize
        {
            get;
        }

        extern bool IList.IsReadOnly
        {
            get;
        }

        extern object IList.this[int index]
        {
            get;
            set;
        }

        extern void IList.Add(object value);

        extern void IList.Clear();

        extern void IList.Insert(int index, object value);

        extern void IList.Remove(object value);

        extern void IList.RemoveAt(int index);

        extern int ICollection.Count
        { get; }

        public extern void CopyTo(Array array, int index);
    }

    public abstract class ArrayImpl : IList, IEnumerable
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

        public abstract void CopyTo(Array array, int index);

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get
            {
                return this.GetValue(index);
            }
            set
            {
                this.SetValue(index, value);
            }
        }

        void IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        int ICollection.Count
        {
            get { return this.Length; }
        }
    }
}