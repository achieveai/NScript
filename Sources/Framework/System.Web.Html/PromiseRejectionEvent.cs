//-----------------------------------------------------------------------
// <copyright file="PromiseRejectionEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Event for <see cref="Window.OnUnhandledRejection"/>.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class PromiseRejectionEvent : Event
    {
        private extern PromiseRejectionEvent();

        /// <summary>
        /// Gets the rejected promise.
        /// </summary>
        public extern Promise Promise
        { get; }

        /// <summary>
        /// Gets the rejection reason (often an Error, but any value is allowed).
        /// </summary>
        public extern object Reason
        { get; }
    }
}
