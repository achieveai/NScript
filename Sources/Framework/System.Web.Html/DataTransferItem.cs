//-----------------------------------------------------------------------
// <copyright file="DataTransferItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// One item of <see cref="DataTransfer.Items"/>.
    /// </summary>
    [IgnoreNamespace]
    public sealed class DataTransferItem
    {
        private extern DataTransferItem();

        /// <summary>
        /// Gets the kind: "file" or "string".
        /// </summary>
        public extern string Kind
        { get; }

        /// <summary>
        /// Gets the MIME type.
        /// </summary>
        public extern string Type
        { get; }

        /// <summary>
        /// Gets a value indicating whether the browser has <c>webkitGetAsEntry</c>.
        /// </summary>
        public static extern bool SupportsGetAsEntry
        {
            [Script("return typeof @:DataTransferItem !== 'undefined' && typeof @:DataTransferItem.prototype.webkitGetAsEntry === 'function';")]
            get;
        }

        /// <summary>
        /// Gets the file, or null when the item is not a file.
        /// </summary>
        public extern File GetAsFile();

        /// <summary>
        /// Gets the file system entry (file or folder), or null when the item is not a file.
        /// Only valid during the drop event. Check <see cref="SupportsGetAsEntry"/> first.
        /// </summary>
        public extern FileSystemEntry WebkitGetAsEntry();
    }
}
