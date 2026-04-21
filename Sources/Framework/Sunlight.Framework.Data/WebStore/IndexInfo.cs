namespace Sunlight.Framework.Data.WebStore
{
    using System.Collections;
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Base class for secondary index declarations on a
    /// <see cref="TableSchema"/>. Subclassed by <see cref="SingleIndexInfo"/>
    /// and <see cref="MultiColumnIndexInfo"/>.
    /// </summary>
    public abstract class IndexInfoBase
    {
        /// <summary>True for multi-entry indexes (each array element indexed individually).</summary>
        public readonly bool IsMultiEntry;

        /// <summary>True when the index enforces uniqueness of indexed values.</summary>
        public readonly bool IsUnique;

        /// <summary>Name of the index as registered in IDB.</summary>
        public readonly string Name;

        protected IndexInfoBase(
            string name,
            bool isUnique = false,
            bool isMultiEntry = false)
        {
            Name = name;
            IsMultiEntry = isMultiEntry;
            IsUnique = isUnique;
        }

        /// <summary>
        /// Returns a non-negative "fitness" score for serving <paramref name="query"/>
        /// through this index, or -1 if the index cannot serve it. Lower scores
        /// indicate a tighter fit.
        /// </summary>
        public abstract int ServeQueryScore(Query query);

        /// <summary>Registers the index on <paramref name="objectStore"/> during schema upgrade.</summary>
        public abstract void InitIndex(
            IDBObjectStore<string, Dictionary> objectStore);
    }

    /// <summary>
    /// Composite index over two or more key paths. Prefers queries whose
    /// <see cref="Query.KeyPaths"/> prefix-match the declared column order.
    /// </summary>
    public class MultiColumnIndexInfo : IndexInfoBase
    {
        /// <summary>Ordered list of key paths making up the composite index.</summary>
        public readonly string[] KeysPath;

        public MultiColumnIndexInfo(
            string name,
            string[] keysPath,
            bool isUnique = false,
            bool isMultiEntry = false)
            : base(
                  name,
                  isUnique,
                  isMultiEntry)
        {
            KeysPath = keysPath;
        }

        public override int ServeQueryScore(Query query)
        {
            if (query.KeyPaths.Length > KeysPath.Length)
            { return -1; }

            for (int iKeyPath = 0; iKeyPath < query.KeyPaths.Length; iKeyPath++)
            {
                if (KeysPath[iKeyPath] != query.KeyPaths[iKeyPath])
                { return -1; }
            }

            return KeysPath.Length - query.KeyPaths.Length;
        }

        public override void InitIndex(
            IDBObjectStore<string, Dictionary> objectStore)
        {
            _ = objectStore.CreateIndex(
                Name,
                KeysPath,
                new IDBIndexParameters {
                    MultiEntry = IsMultiEntry,
                    Unique = IsUnique,
                });
        }
    }

    /// <summary>
    /// Single-column index. Scores 0 (perfect fit) for queries whose sole
    /// <see cref="Query.KeyPaths"/> entry matches <see cref="KeyPath"/>.
    /// </summary>
    public class SingleIndexInfo : IndexInfoBase
    {
        public readonly string KeyPath;

        public SingleIndexInfo(
                    string name,
                    string keyPath,
                    bool isUnique = false,
                    bool isMultiEntry = false)
                    : base(
                          name,
                          isUnique,
                          isMultiEntry)
        { KeyPath = keyPath; }

        public override int ServeQueryScore(Query query)
        {
            if (query.KeyPaths.Length == 1 && query.KeyPaths[0] == KeyPath)
            { return 0; }

            return -1;
        }

        public override void InitIndex(
            IDBObjectStore<string, Dictionary> objectStore)
        {
            _ = objectStore.CreateIndex(
                Name,
                KeyPath,
                new IDBIndexParameters {
                    MultiEntry = IsMultiEntry,
                    Unique = IsUnique,
                });
        }
    }
}
