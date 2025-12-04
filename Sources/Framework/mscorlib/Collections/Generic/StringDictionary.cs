//-----------------------------------------------------------------------
// <copyright file="StringDictionary.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for StringDictionary
    /// </summary>
    public class StringDictionary<TValue> : IStringDictionary<TValue>
    {
        private Object innerDict;
        private int count = 0;

        public StringDictionary()
        {
            this.innerDict = new Object();
        }

        public StringDictionary(Object innerDict)
        {
            this.innerDict = innerDict;
            this.count = this.ComputeCount();
        }

        public extern TValue this[string index]
        {
            [Script(@"
                if (!(index in this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}))
                    throw new @{[mscorlib]System.Exception}(""Key not found"");
                return this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}[index];")]
            get;

            [Script(@"
                if (!(index in this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}))
                    this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::count}++;
                this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}[index] = value;
            ")]
            set;
        }

        public IEnumerable<string> Keys
        {
            get { return new ArrayG<string>(this.GetKeys()); }
        }

        public IEnumerable<TValue> Values
        {
            get { return new ArrayG<TValue>(this.GetValues()); }
        }

        public int Count
        {
            get { return this.count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        [Script(@"
            if (key in this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict})
                throw new @{[mscorlib]System.Exception}(""key already exists"");
            this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::count}++;
            this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}[key] = value;
        ")]
        public extern void Add(string key, TValue value);

        [Script(@"return key in this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict};")]
        public extern bool ContainsKey(string key);

        [Script(@"
            var rv = delete this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}[key];
            if (rv) this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::count}--;
            return rv;")]
        public extern bool Remove(string key);

        public bool TryGetValue(string key, out TValue value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Add(KeyValuePair<string, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.innerDict = new object();
            this.count = 0;
        }

        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return this.ContainsKey(item.Key) && this[item.Key].AreEqual(item.Value);
        }

        public void CopyTo(KeyValuePair<string, TValue>[] arr, int index)
        {
            ArrayG<string> keys = (ArrayG<string>)this.Keys;
            for (int i = 0; i < keys.Length; i++)
            {
                arr[i + index] = new KeyValuePair<string, TValue>(keys[i], this[keys[i]]);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ArrayG<string> keys = (ArrayG<string>)this.Keys;
            for (int i = 0; i < keys.Length; i++)
            {
                array.SetValue(i + index, new KeyValuePair<string, TValue>(keys[i], this[keys[i]]));
            }
        }

        public bool Remove(KeyValuePair<string, TValue> item)
        {
            if (this.Contains(item))
            {
                this.Remove(item.Key);
                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Script(@"
            return ({}).constructor.keys(this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict});
            ")]
        private extern NativeArray<string> GetKeys();

        [Script(@"
            var dict = this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict};
            var keys = ({}).constructor.keys(dict);
            var rv = [];
            var i;
            for(i = 0; i < keys.length; i++)
                rv.push(dict[keys[i]]);
            return rv;
            ")]
        private extern NativeArray<TValue> GetValues();

        [Script(@"
            return ({}).constructor.keys(this.@{[mscorlib]System.Collections.Generic.StringDictionary`1::innerDict}).length;
            ")]
        private extern int ComputeCount();

        private class Enumerator : IEnumerator<KeyValuePair<string, TValue>>
        {
            private StringDictionary<TValue> dict;
            private IEnumerator<string> keys;

            public Enumerator(StringDictionary<TValue> dict)
            {
                this.dict = dict;
                this.keys = this.dict.Keys.GetEnumerator();
            }

            public KeyValuePair<string, TValue> Current
            {
                get
                {
                    return new KeyValuePair<string, TValue>(this.keys.Current, this.dict[this.keys.Current]);
                }
            }

            public bool MoveNext()
            {
                return this.keys.MoveNext();
            }

            public void Reset()
            {
                this.keys.Reset();
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public void Dispose()
            { }
        }
    }
}