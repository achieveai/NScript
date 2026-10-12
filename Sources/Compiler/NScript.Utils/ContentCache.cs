namespace NScript.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Process-wide, least-recently-used cache keyed by the content of its inputs (slice 2,
    /// Inc 4a-1). Holds only build-independent data: no ClrContext, RuntimeScopeManager or
    /// ConverterContext, so a warm build service can keep it across builds while plugins are
    /// still created per build. Razor and XWML parse caches use it. <c>NSCRIPT_RAZOR_CACHE=off</c>
    /// disables every instance (the name predates XWML's use).
    /// </summary>
    public sealed class ContentCache<T>
        where T : class
    {
        public static readonly bool Off =
            string.Equals(Environment.GetEnvironmentVariable("NSCRIPT_RAZOR_CACHE"), "off", StringComparison.OrdinalIgnoreCase);

        private readonly int capacity;
        private readonly object gate = new object();
        private readonly Dictionary<string, LinkedListNode<KeyValuePair<string, T>>> map =
            new Dictionary<string, LinkedListNode<KeyValuePair<string, T>>>(StringComparer.Ordinal);
        private readonly LinkedList<KeyValuePair<string, T>> order = new LinkedList<KeyValuePair<string, T>>();

        public ContentCache(int capacity)
        {
            this.capacity = capacity;
        }

        public int Hits { get; private set; }

        public int Misses { get; private set; }

        public int Count
        {
            get
            {
                lock (this.gate)
                {
                    return this.map.Count;
                }
            }
        }

        /// <summary>
        /// SHA-256 over the length-prefixed parts, so ("ab", "c") and ("a", "bc") differ and
        /// a null part differs from an empty one.
        /// </summary>
        public static string Key(params string[] parts)
        {
            using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            foreach (var part in parts)
            {
                var bytes = part == null ? null : Encoding.UTF8.GetBytes(part);
                hash.AppendData(BitConverter.GetBytes(bytes?.Length ?? -1));
                if (bytes != null)
                {
                    hash.AppendData(bytes);
                }
            }

            return Convert.ToHexString(hash.GetHashAndReset());
        }

        /// <summary>
        /// Returns the cached value for <paramref name="key"/>, or creates and stores it. A
        /// <paramref name="create"/> that throws stores nothing.
        /// </summary>
        public T GetOrCreate(string key, Func<T> create)
            => this.GetOrCreate(key, create, out _);

        /// <summary>
        /// As <see cref="GetOrCreate(string, Func{T})"/>; <paramref name="hit"/> says whether
        /// this call was served from the cache, so a caller can count its own hits even when
        /// builds run concurrently.
        /// </summary>
        public T GetOrCreate(string key, Func<T> create, out bool hit)
        {
            hit = false;
            if (Off)
            {
                return create();
            }

            lock (this.gate)
            {
                if (this.map.TryGetValue(key, out var node))
                {
                    this.order.Remove(node);
                    this.order.AddFirst(node);
                    this.Hits++;
                    hit = true;
                    return node.Value.Value;
                }
            }

            var value = create() ?? throw new InvalidOperationException("ContentCache: create returned null for key " + key + ".");
            lock (this.gate)
            {
                this.Misses++;
                if (!this.map.ContainsKey(key))
                {
                    this.map[key] = this.order.AddFirst(new KeyValuePair<string, T>(key, value));
                    while (this.map.Count > this.capacity)
                    {
                        this.map.Remove(this.order.Last.Value.Key);
                        this.order.RemoveLast();
                    }
                }
            }

            return value;
        }

        /// <summary>For tests.</summary>
        public void Clear()
        {
            lock (this.gate)
            {
                this.map.Clear();
                this.order.Clear();
                this.Hits = 0;
                this.Misses = 0;
            }
        }
    }
}
