//-----------------------------------------------------------------------
// <copyright file="IDictionary.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    /// <summary>
    /// Definition for IDictionary
    /// </summary>
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey index]
        { get; set; }

        IEnumerable<TKey> Keys
        { get; }

        IEnumerable<TValue> Values
        { get; }

        void Add(TKey key, TValue value);

        bool ContainsKey(TKey key);

        bool Remove(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }
}