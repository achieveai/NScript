namespace System.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an object whose members can be dynamically added and removed at run time.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ExpandoObject : IDynamicMetaObjectProvider, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
        private Object innerDict;
        private int count = 0;

        public ExpandoObject()
        {
            this.innerDict = new Object();
        }

        public ExpandoObject(Object innerDict)
        {
            this.innerDict = innerDict;
            this.count = this.ComputeCount();
        }

        public extern object this[string index]
        {
            [Script(@"
                if (!(index in this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict}))
                    throw new @{[mscorlib]System.Exception}(""Key not found"");
                return this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict}[index];")]
            get;

            [Script(@"this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict}[index] = value;")]
            set;
        }

        public IEnumerable<string> Keys
        {
            get { return this.GetKeys().GetArray(); }
        }

        public IEnumerable<object> Values
        {
            get { return this.GetValues().GetArray(); }
        }

        public int Count
        {
            get { return this.count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(string key, object value)
        {
            if (this.ContainsKey(key))
            {
                throw new Exception("Key already exists");
            }

            this.count++;
            this[key] = value;
        }

        [Script(@"return key in this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict};")]
        public extern bool ContainsKey(string key);

        [Script(@"
            var rv = delete this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict}[key];
            if (rv) this.@{[mscorlib]System.Dynamic.ExpandoObject::count}--;
            return rv;")]
        public extern bool Remove(string key);

        public bool TryGetValue(string key, out object value)
        {
            if (this.ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = null;
            return false;
        }

        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.innerDict = new object();
            this.count = 0;
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.ContainsKey(item.Key) && this[item.Key].AreEqual(item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] arr, int index)
        {
            var keys = this.GetKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                arr[i + index] = new KeyValuePair<string, object>(
                    keys[i],
                    this[keys[i]]);
            }
        }

        public void CopyTo(Array array, int index)
        {
            var keys = this.GetKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                array.SetValue(i + index, new KeyValuePair<string, object>(keys[i], this[keys[i]]));
            }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if (this.Contains(item))
            {
                this.Remove(item.Key);
                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [Script(@"
            var rv = [], key;
            for(key in this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict})
                rv.push(key);
            return rv;
            ")]
        private extern NativeArray<string> GetKeys();

        [Script(@"
            var rv = [], key;
            for(key in this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict})
                rv.push(this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict}[key]);
            return rv;
            ")]
        private extern NativeArray<object> GetValues();

        [Script(@"
            var rv = 0, key;
            for(key in this.@{[mscorlib]System.Dynamic.ExpandoObject::innerDict})
                rv++;
            return rv;
            ")]
        private extern int ComputeCount();

        private class Enumerator : IEnumerator<KeyValuePair<string, object>>
        {
            private ExpandoObject dict;
            private IEnumerator<string> keys;

            public Enumerator(ExpandoObject dict)
            {
                this.dict = dict;
                this.keys = this.dict.Keys.GetEnumerator();
            }

            public KeyValuePair<string, object> Current
            {
                get
                {
                    return new KeyValuePair<string, object>(this.keys.Current, this.dict[this.keys.Current]);
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

        public DynamicMetaObject GetMetaObject(Linq.Expressions.Expression parameter)
        {
            throw new NotImplementedException();
        }
    }
}
