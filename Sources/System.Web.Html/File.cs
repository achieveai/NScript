//-----------------------------------------------------------------------
// <copyright file="File.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for File.
    /// </summary>
    [IgnoreNamespace]
    public class File
    {
        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern File();

        /// <summary>
        /// Gets the date of the last modified.
        /// </summary>
        /// <value>
        /// The date of the last modified.
        /// </value>
        public extern DateTime LastModifiedDate
        { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public extern long Size
        { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public extern string Type
        { get; }
    }
}
