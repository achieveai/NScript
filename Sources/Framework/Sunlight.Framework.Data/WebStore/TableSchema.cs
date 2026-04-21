namespace Sunlight.Framework.Data.WebStore
{
    using System.Collections;
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Declares the shape of a single object store: primary key, secondary
    /// indexes, and the upgrade logic that registers both with IDB.
    /// </summary>
    public class TableSchema
    {
        public readonly IndexInfoBase[] Indexes;
        public readonly KeyInfo Key;
        public readonly string Name;

        public TableSchema(
            string name,
            KeyInfo key,
            params IndexInfoBase[] indexes)
        {
            Indexes = indexes;
            Name = name;
            Key = key;
        }

        /// <summary>
        /// Registers this table (object store + its indexes) on
        /// <paramref name="idbDatabase"/>. Only valid from within a
        /// <c>versionchange</c> transaction.
        /// </summary>
        public void InitializeTable(IDBDatabase idbDatabase)
        {
            var objectStore = idbDatabase.CreateObjectStore<string, Dictionary>(
                Name,
                new IDBObjectStoreParameters
                {
                    KeyPath = Key.KeyPath,
                    AutoIncrement = Key.AutoIncrement,
                });

            for (int iIdx = Indexes.Length - 1; iIdx >= 0; iIdx--)
            { Indexes[iIdx].InitIndex(objectStore); }
        }

        /// <summary>
        /// True when <paramref name="query"/> can be served by walking the
        /// primary key alone (no secondary-index lookup needed).
        /// </summary>
        public bool CanUsePrimaryIndex(Query query)
        {
            return query.KeyPaths.Length == 0
                || (query.KeyPaths.Length == 1 && query.KeyPaths[0] == Key.KeyPath);
        }

        /// <summary>
        /// Picks the best secondary index for <paramref name="query"/>, or null
        /// when no index can serve it. Lower <see cref="IndexInfoBase.ServeQueryScore(Query)"/>
        /// wins.
        /// </summary>
        public IndexInfoBase GetIndex(Query query)
        {
            int bestScore = 1 << 20;
            IndexInfoBase selectedIndex = null;
            for (int iIdx = 0; iIdx < Indexes.Length; iIdx++)
            {
                var idx = Indexes[iIdx];
                var score = idx.ServeQueryScore(query);
                if (score == -1)
                { continue; }

                if (score < bestScore)
                {
                    selectedIndex = idx;
                    bestScore = score;
                }
            }

            return selectedIndex;
        }
    }
}
