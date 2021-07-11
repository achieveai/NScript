//-----------------------------------------------------------------------
// <copyright file="TokenList.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;

    /// <summary>
    /// Definition for ClassList.
    /// </summary>
    public class TokenList
    {
        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public extern int Length
        { get; }

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="i"> Zero-based index of the entry to access. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public extern string this[int i]
        { get; }

        /// <summary>
        /// Query if this object contains the given className.
        /// </summary>
        /// <param name="className"> The string to test for containment. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        public extern bool Contains(string className);

        /// <summary>
        /// Adds className.
        /// </summary>
        /// <param name="className"> The string to test for containment. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool Add(string className);

        /// <summary>
        /// Removes the given className.
        /// </summary>
        /// <param name="className"> The string to test for containment. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool Remove(string className);

        /// <summary>
        /// Toggles.
        /// </summary>
        /// <param name="className"> The string to test for containment. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern void Toggle(string className);
    }
}
