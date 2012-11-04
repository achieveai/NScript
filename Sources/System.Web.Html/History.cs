//-----------------------------------------------------------------------
// <copyright file="History.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for History
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class History
    {
        private History() { }

        /// <summary>
        /// Retrieves the number of elements in the history list.
        /// </summary>
        [IntrinsicField]
        public readonly int Length;

        /// <summary>
        /// Retrieves the current state object.
        /// </summary>
        [IntrinsicField]
        public readonly object State;

        /// <summary>
        /// Navigates to the previous page in history.
        /// </summary>
        public extern void Back();

        /// <summary>
        /// Navigates to the next page in history.
        /// </summary>
        public extern void Forward();

        /// <summary>
        /// Navigates to a page in history relative to the current page.
        /// </summary>
        /// <param name="delta">The number of pages in history to go back (negative) or forward (positive).</param>
        public extern void Go(int delta);

        /// <summary>
        /// Pushes the given data onto the session history stack.
        /// </summary>
        /// <param name="data">The data to serialize into JSON format.</param>
        /// <param name="title">The title to place into history.</param>
        public extern void PushState(object data, string title);

        /// <summary>
        /// Pushes the given data onto the session history stack.
        /// </summary>
        /// <param name="data">The data to serialize into JSON format.</param>
        /// <param name="title">The title to place into history.</param>
        /// <param name="url">The URL to place into history.</param>
        public extern void PushState(object data, string title, string url);

        /// <summary>
        /// Updates the most recent entry on the history stack.
        /// </summary>
        /// <param name="data">The data to serialize into JSON format.</param>
        /// <param name="title">The title to place into history.</param>
        public extern void ReplaceState(object data, string title);

        /// <summary>
        /// Updates the most recent entry on the history stack.
        /// </summary>
        /// <param name="data">The data to serialize into JSON format.</param>
        /// <param name="title">The title to place into history.</param>
        /// <param name="url">The URL to place into history.</param>
        public extern void ReplaceState(object data, string title, string url);
    }
}