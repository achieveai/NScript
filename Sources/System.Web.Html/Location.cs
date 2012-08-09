//-----------------------------------------------------------------------
// <copyright file="Location.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Location
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class Location
    {
        private Location() { }

        [IntrinsicField]
        public readonly string Hostname;

        [IntrinsicField]
        [ScriptName("host")]
        public readonly string HostnameAndPort;

        [IntrinsicField]
        public readonly string Pathname;

        [IntrinsicField]
        public readonly string Port;

        [IntrinsicField]
        public readonly string Protocol;

        [IntrinsicField]
        public readonly string Search;

        [IntrinsicField]
        public string Hash;

        [IntrinsicField]
        public string Href;

        /// <summary>
        /// Navigates the window to a new location and updates the browser's history.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public extern void Assign(string url);

        /// <summary>
        /// Reload the current document.
        /// </summary>
        public extern void Reload();

        /// <summary>
        /// Reload the current document.
        /// </summary>
        /// <param name="forceGet">If true, the document will be reloaded from the server, otherwise it may be loaded from the browser's cache.</param>
        public extern void Reload(bool forceGet);

        /// <summary>
        /// Navigates the window to a new location without updating the browser's history.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public extern void Replace(string url);
    }
}