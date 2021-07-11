//-----------------------------------------------------------------------
// <copyright file="Dictionary.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;

    /// <summary>
    /// Definition for Dictionary
    /// </summary>
    [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
    public class Dictionary<K, V> : IDictionary<K, V>
    {
        public Dictionary(int count) { }

        #region IDictionary<K,V> Members

        public V this[K index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<K> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<V> Values
        {
            get { throw new NotImplementedException(); }
        }

        public void Add(K key, V value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(K key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(K key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(K key, out V value)
        {
            throw new NotImplementedException();
        }

        #endregion IDictionary<K,V> Members

        #region ICollection<KeyValuePair<K,V>> Members

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<K, V>[] arr, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotImplementedException();
        }

        #endregion ICollection<KeyValuePair<K,V>> Members

        #region IEnumerable<KeyValuePair<K,V>> Members

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion IEnumerable<KeyValuePair<K,V>> Members

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion IEnumerable Members

    }
}