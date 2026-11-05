//-----------------------------------------------------------------------
// <copyright file="MediaQueryList.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Live result of <see cref="Window.MatchMedia(string)"/>.
    /// </summary>
    [IgnoreNamespace]
    public sealed class MediaQueryList : EventTarget
    {
        private extern MediaQueryList();

        /// <summary>
        /// Gets a value indicating whether the document currently matches the query.
        /// </summary>
        public extern bool Matches
        { get; }

        /// <summary>
        /// Gets the serialized media query.
        /// </summary>
        public extern string Media
        { get; }

        /// <summary>
        /// Raised when <see cref="Matches"/> changes.
        /// </summary>
        public extern event Action<MediaQueryList, MediaQueryListEvent> OnChange;
    }

    /// <summary>
    /// Event raised by <see cref="MediaQueryList.OnChange"/>.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class MediaQueryListEvent : Event
    {
        private extern MediaQueryListEvent();

        /// <summary>
        /// Gets a value indicating whether the document now matches the query.
        /// </summary>
        public extern bool Matches
        { get; }

        /// <summary>
        /// Gets the serialized media query.
        /// </summary>
        public extern string Media
        { get; }
    }
}
