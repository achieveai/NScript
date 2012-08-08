//-----------------------------------------------------------------------
// <copyright file="NumberDictionary.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for NumberDictionary
    /// </summary>
    public class NumberDictionary<TValue> : INumberDictionary<TValue>
    {
        private Object innerDict;
        private int count = 0;

        public NumberDictionary()
        {
            this.innerDict = new Object();
        }

        public NumberDictionary(Object innerDict)
        {
            this.innerDict = innerDict;
            this.count = this.ComputeCount();
        }

        public extern TValue this[Number index]
        {
            [Script(@"
                if (!(index in this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict}))
                    throw new @{[mscorlib]System.Exception}(""Key not found"");
                return this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict}[index];")]
            get;

            [Script(@"this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict}[index] = value;")]
            set;
        }

        public IEnumerable<Number> Keys
        {
            get { return new ArrayG<Number>(this.GetKeys()); }
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

        public void Add(Number key, TValue value)
        {
            if (this.ContainsKey(key))
            {
                throw new Exception("Key already exists");
            }

            this.count++;
            this[key] = value;
        }

        [Script(@"return key in this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict};")]
        public extern bool ContainsKey(Number key);

        [Script(@"
            var rv = delete this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict};
            if (rv) this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::count}--;
            return rv;")]
        public extern bool Remove(Number key);

        public bool TryGetValue(Number key, out TValue value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Add(KeyValuePair<Number, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.innerDict = new object();
            this.count = 0;
        }

        public bool Contains(KeyValuePair<Number, TValue> item)
        {
            return this.ContainsKey(item.Key) && this[item.Key].AreEqual(item.Value);
        }

        public void CopyTo(KeyValuePair<Number, TValue>[] arr, int index)
        {
            ArrayG<Number> keys = (ArrayG<Number>)this.Keys;
            for (int i = 0; i < keys.Length; i++)
            {
                arr[i + index] = new KeyValuePair<Number, TValue>(keys[i], this[keys[i]]);
            }
        }

        public bool Remove(KeyValuePair<Number, TValue> item)
        {
            if (this.Contains(item))
            {
                this.Remove(item.Key);
                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<Number, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Script(@"
            var rv = [], key;
            for(key in this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict})
                rv.push(key);
            return rv;
            ")]
        private extern NativeArray GetKeys();

        [Script(@"
            var rv = [], key;
            for(key in this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict})
                rv.push(this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict}[key]);
            return rv;
            ")]
        private extern NativeArray GetValues();

        [Script(@"
            var rv = 0, key;
            for(key in this.@{[mscorlib]System.Collections.Generic.NumberDictionary`1::innerDict})
                rv++;
            return rv;
            ")]
        private extern int ComputeCount();

        private class Enumerator : IEnumerator<KeyValuePair<Number, TValue>>
        {
            private NumberDictionary<TValue> dict;
            private IEnumerator<Number> keys;

            public Enumerator(NumberDictionary<TValue> dict)
            {
                this.dict = dict;
                this.keys = this.dict.Keys.GetEnumerator();
            }

            public KeyValuePair<Number, TValue> Current
            {
                get
                {
                    return new KeyValuePair<Number, TValue>(this.keys.Current, this.dict[this.keys.Current]);
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