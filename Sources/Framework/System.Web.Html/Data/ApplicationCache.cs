//-----------------------------------------------------------------------
// <copyright file="ApplicationCache.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace]
    public sealed class ApplicationCache
    {
        private extern ApplicationCache();

        /// <summary>
        /// Gets the current status of the application cache.
        /// </summary>
        public extern ApplicationCacheStatus Status { get; }

        public extern event Action<ApplicationCache, ElementEvent> OnCached;
        public extern event Action<ApplicationCache, ElementEvent> OnChecking;
        public extern event Action<ApplicationCache, ElementEvent> OnDownloading;
        public extern event Action<ApplicationCache, ElementEvent> OnError;
        public extern event Action<ApplicationCache, ElementEvent> OnNoUpdate;
        public extern event Action<ApplicationCache, ElementEvent> OnProgress;
        public extern event Action<ApplicationCache, ElementEvent> OnUpdateReady;
        public extern event Action<ApplicationCache, ElementEvent> OnObsolete;

        /// <summary>
        /// Adds the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public extern void Add(string uri);

        /// <summary>
        /// Replaces the active cache with the latest version.
        /// </summary>
        public extern void SwapCache();

        /// <summary>
        /// Manually triggers the update process.
        /// </summary>
        public extern void Update();
    }
}