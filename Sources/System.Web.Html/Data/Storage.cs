//-----------------------------------------------------------------------
// <copyright file="Storage.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Storage.
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class Storage
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private extern Storage();

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public extern int Length { get; set; }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public extern string this[string key]
        {
            get;
            set;
        }

        /// <summary>
        /// Clears this object to its blank/initial state.
        /// </summary>
        public extern void Clear();

        /// <summary>
        /// Gets an item.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <returns>
        /// The item.
        /// </returns>
        public extern string GetItem(string key);

        /// <summary>
        /// Gets a key.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        /// The key.
        /// </returns>
        [ScriptName("key")]
        public extern string GetKey(int index);

        /// <summary>
        /// Removes the item described by key.
        /// </summary>
        /// <param name="key"> The key. </param>
        public extern void RemoveItem(string key);

        /// <summary>
        /// Sets an item.
        /// </summary>
        /// <param name="key">   The key. </param>
        /// <param name="value"> The value. </param>
        public extern void SetItem(string key, string value);
    }
}