//-----------------------------------------------------------------------
// <copyright file="KeyValuePair.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    /// <summary>
    /// Definition for KeyValuePair
    /// </summary>
    public struct KeyValuePair<K, V>
    {
        private readonly K key;
        private readonly V val;

        public KeyValuePair(K key, V value)
        {
            this.key = key;
            this.val = value;
        }

        public K Key
        { get { return this.key; } }

        public V Value
        { get { return this.val; } }
    }
}