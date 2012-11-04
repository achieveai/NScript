//-----------------------------------------------------------------------
// <copyright file="ApplicationCache.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    [Extended]
    [IgnoreNamespace]
    public sealed class ApplicationCache
    {
        private ApplicationCache() { }

        /// <summary>
        /// Gets the current status of the application cache.
        /// </summary>
        [IntrinsicField]
        public readonly ApplicationCacheStatus Status;

        /// <summary>
        /// Adds the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public extern void Add(string uri);

        /// <summary>
        /// Adds the event listener.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="useCapture">if set to <c>true</c> [use capture].</param>
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Removes the event listener.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="useCapture">if set to <c>true</c> [use capture].</param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Replaces the active cache with the latest version.
        /// </summary>
        public extern void SwapCache();

        /// <summary>
        /// Manually triggers the update process.
        /// </summary>
        public extern void Update();

        /// <summary>
        /// Adds the event listener.
        /// </summary>
        /// <param name="evtName">Name of the evt.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="useCapture">if set to <c>true</c> [use capture].</param>
        public void AddEventListener(
            ApplicationCacheEvent evtName,
            Action<ElementEvent> listener,
            bool useCapture)
        {
            this.AddEventListener(evtName.ToString(), listener, useCapture);
        }

        /// <summary>
        /// Removes the event listener.
        /// </summary>
        /// <param name="evtName">Name of the evt.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="useCapture">if set to <c>true</c> [use capture].</param>
        public void RemoveEventListener(
            ApplicationCacheEvent evtName,
            Action<ElementEvent> listener,
            bool useCapture)
        {
            this.RemoveEventListener(evtName.ToString(), listener, useCapture);
        }
    }
}