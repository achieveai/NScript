namespace System
{
    using System.Collections;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public abstract class Array : IList, IEnumerable
    {
        public extern int Length
        {
            [ScriptName("gl")]
            get;
        }

        internal extern Array();

        [ScriptName("srt")]
        public static void Sort<T>(T[] array, Func<T, T, int> comparator)
        {
            ((ArrayG<T>)(object)array).Sort(comparator);
        }

        [ScriptName("cl")]
        public extern Array Clone();

        [ScriptName("cons")]
        public extern bool Contains(object item);

        [ScriptName("genum")]
        public extern IEnumerator GetEnumerator();

        [ScriptName("iof")]
        public extern int IndexOf(object item);

        [ScriptName("iof1")]
        public extern int IndexOf(object item, int startIndex);

        [ScriptName("rev")]
        public extern void Reverse();

        [ScriptName("gv")]
        public extern object GetValue(int index);

        [ScriptName("sv")]
        public extern void SetValue(int index, object value);

        extern IEnumerator IEnumerable.GetEnumerator();

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
            [ScriptName("gl")]
            get;
        }

        internal ArrayImpl() { }

        [ScriptName("srt")]
        public static void Sort<T>(T[] array, Func<T, T, int> comparator)
        {
            ((ArrayG<T>)(object)array).Sort(comparator);
        }

        [ScriptName("cl")]
        public abstract ArrayImpl Clone();

        [ScriptName("cons")]
        public abstract bool Contains(object item);

        [ScriptName("genum")]
        public abstract IEnumerator GetEnumerator();

        [ScriptName("iof")]
        public abstract int IndexOf(object item);

        [ScriptName("iof1")]
        public abstract int IndexOf(object item, int startIndex);

        [ScriptName("rev")]
        public abstract void Reverse();

        [ScriptName("gv")]
        public abstract object GetValue(int index);

        [ScriptName("sv")]
        public abstract void SetValue(int index, object value);

        [ScriptName("cpt")]
        public abstract void CopyTo(Array array, int index);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

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