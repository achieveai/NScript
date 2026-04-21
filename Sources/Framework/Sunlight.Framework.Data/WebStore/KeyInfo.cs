namespace Sunlight.Framework.Data.WebStore
{
    using System;
    using System.Collections;

    /// <summary>
    /// Declares how a primary key is derived for records in a
    /// <see cref="TableSchema"/>. Supports dotted key paths that walk nested
    /// properties at read time.
    /// </summary>
    public class KeyInfo
    {
        /// <summary>Dotted path to the key property on stored records.</summary>
        public readonly string KeyPath;

        /// <summary>
        /// When true, IDB assigns ascending numeric keys automatically for
        /// records that do not carry a key at <see cref="KeyPath"/>.
        /// </summary>
        public readonly bool? AutoIncrement;

        public KeyInfo(string keyPath, bool? autoIncrement)
        {
            KeyPath = keyPath;
            AutoIncrement = autoIncrement;
        }

        /// <summary>
        /// Extract the key value from a record by walking <paramref name="keyInfo"/>'s
        /// dotted path. Returns null when any step of the walk encounters a
        /// missing property.
        /// </summary>
        public static TKey GetKeyValue<TKey, TValue>(
            KeyInfo keyInfo,
            TValue value)
        {
            var dict = Dictionary.GetDictionary(value);
            var keyParts = keyInfo.KeyPath.Split('.');

            int step = 0;
            while (step < keyParts.Length && dict != null)
            {
                dict = dict.Get<Dictionary>(keyParts[step++]);
            }

            return Type.AS<Dictionary, TKey>(dict);
        }
    }
}
