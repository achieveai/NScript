namespace Sunlight.Framework.Data.WebStore
{
    using System;
    using System.Collections;
    using System.Web;

    /// <summary>
    /// Opaque continuation token issued by
    /// <see cref="WebStoreTable{TKey,TValue}.QueryPage"/> /
    /// <see cref="WebStoreTable{TKey,TValue}.QueryKeysPage"/>. Round-trips to a
    /// string via <see cref="ToToken"/> / <see cref="FromToken(string)"/> so
    /// callers can persist it across sessions, network hops, or storage tiers
    /// without reaching into the cursor's internal state.
    /// <para>
    /// The wire format is a JSON envelope with literal short keys (<c>v</c>,
    /// <c>d</c>, <c>p</c>, <c>i</c>, <c>t</c>) — NScript minification renames
    /// C# field names, so reflecting on field names would break the round-trip
    /// across builds.
    /// </para>
    /// </summary>
    public sealed class Cursor
    {
        private const string TokenVersion = "1";

        internal Cursor(
            object indexKey,
            object primaryKey,
            string direction,
            string tableSig)
        {
            IndexKey = indexKey;
            PrimaryKey = primaryKey;
            Direction = direction;
            TableSig = tableSig;
        }

        /// <summary>
        /// Index key of the last accepted record, or <c>null</c> when the cursor
        /// was issued by a primary-key scan (no secondary index involved).
        /// </summary>
        internal object IndexKey { get; }

        /// <summary>Primary key of the last accepted record. Always populated.</summary>
        internal object PrimaryKey { get; }

        /// <summary>IDB cursor direction string ("next" or "prev") at issue time.</summary>
        internal string Direction { get; }

        /// <summary>Table-signature (object-store name) at issue time.</summary>
        internal string TableSig { get; }

        /// <summary>
        /// Encode the cursor into a stable, persistable string. The encoding is
        /// stable across NScript minified builds because the JSON keys are
        /// literal — never C# field names.
        /// </summary>
        public string ToToken()
        {
            var env = new Dictionary();
            env.Set("v", TokenVersion);
            env.Set("d", Direction);
            env.Set("t", TableSig);
            env.Set("p", PrimaryKey);
            if (IndexKey != null)
            { env.Set("i", IndexKey); }
            return JSON.Stringify(env);
        }

        /// <summary>
        /// Decode a cursor from a token previously produced by
        /// <see cref="ToToken"/>. Throws a descriptive <see cref="Exception"/>
        /// when the token is malformed or carries an unsupported version.
        /// </summary>
        public static Cursor FromToken(string token)
        {
            if (token == null || token.Length == 0)
            { throw new Exception("Cursor token is null or empty"); }

            Dictionary env;
            try { env = JSON.Parse(token); }
            catch (Exception ex)
            { throw new Exception("Cursor token is not valid JSON: " + ex.Message); }

            if (env == null)
            { throw new Exception("Cursor token decoded to null"); }

            if (!env.ContainsKey("v") || env.Get<string>("v") != TokenVersion)
            { throw new Exception("Cursor token version mismatch (expected v=1)"); }

            if (!env.ContainsKey("d"))
            { throw new Exception("Cursor token missing direction"); }

            var direction = env.Get<string>("d");
            if (direction != "next" && direction != "prev")
            { throw new Exception("Cursor token has invalid direction"); }

            if (!env.ContainsKey("t"))
            { throw new Exception("Cursor token missing table signature"); }

            var tableSig = env.Get<string>("t");
            if (tableSig == null || tableSig.Length == 0)
            { throw new Exception("Cursor token missing table signature"); }

            if (!env.ContainsKey("p"))
            { throw new Exception("Cursor token missing primary key"); }

            var primaryKey = env.Get<object>("p");
            var indexKey = env.ContainsKey("i") ? env.Get<object>("i") : null;
            return new Cursor(indexKey, primaryKey, direction, tableSig);
        }
    }
}
