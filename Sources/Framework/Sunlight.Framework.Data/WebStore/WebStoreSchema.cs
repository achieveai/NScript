namespace Sunlight.Framework.Data.WebStore
{
    using System.Collections.Generic;

    /// <summary>
    /// Top-level schema handed to <see cref="WebStoreFactory.Create(WebStoreSchema)"/>.
    /// Bundles the database name, version, and every <see cref="TableSchema"/>
    /// that should live in it.
    /// </summary>
    public class WebStoreSchema
    {
        /// <summary>Name of the IDB database backing this schema.</summary>
        public readonly string DatabaseName;

        /// <summary>All tables (object stores) declared by this schema.</summary>
        public readonly TableSchema[] Tables;

        /// <summary>
        /// Monotonically increasing version. Bump whenever <see cref="Tables"/>
        /// or any <see cref="TableSchema"/> changes to trigger an upgrade path.
        /// </summary>
        public readonly int VersionId;

        private readonly StringDictionary<TableSchema> tableSchemas;

        public WebStoreSchema(
            string databaseName,
            int versionId,
            params TableSchema[] tables)
        {
            DatabaseName = databaseName;
            VersionId = versionId;
            Tables = tables;
            tableSchemas = new StringDictionary<TableSchema>();
            foreach (var table in tables)
            { tableSchemas.Add(table.Name, table); }
        }

        /// <summary>Lookup — throws if <paramref name="tableName"/> is not declared.</summary>
        public TableSchema GetTableSchema(string tableName)
        { return tableSchemas[tableName]; }

        /// <summary>Returns true when <paramref name="tableName"/> is declared.</summary>
        public bool HasTableSchema(string tableName)
        { return tableSchemas.ContainsKey(tableName); }
    }
}
