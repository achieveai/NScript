// file:	FileInput.cs
//
// summary:	Implements the file input class
namespace System.Web.Html
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// File input element.
    /// </summary>
    [IgnoreNamespace, ScriptName("HTMLInputElement")]
    public class FileInputElement : InputElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern FileInputElement();

        /// <summary>
        /// Gets or sets the accept.
        /// </summary>
        /// <value>
        /// The accept.
        /// </value>
        public extern string Accept
        { get; set; }

        /// <summary>
        /// Gets a list of files.
        /// </summary>
        /// <value>
        /// A List of files.
        /// </value>
        private extern NativeArray Files
        { get; }

        /// <summary>
        /// Gets file list.
        /// </summary>
        /// <returns>
        /// The file list.
        /// </returns>
        public DomList<File> GetFileList()
        {
            return new DomList<File>(this.Files);
        }
    }
}
