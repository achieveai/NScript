//-----------------------------------------------------------------------
// <copyright file="Location.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Location.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class Location
    {
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern Location();

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public extern string Hostname
        { get; }

        /// <summary>
        /// Gets the hostname and port.
        /// </summary>
        /// <value>
        /// The hostname and port.
        /// </value>
        [ScriptName("host")]
        public extern string HostnameAndPort
        { get; }

        /// <summary>
        /// Gets the full pathname of the file.
        /// </summary>
        /// <value>
        /// The full pathname of the file.
        /// </value>
        public extern string Pathname
        { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public extern string Port
        { get; }

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public extern string Protocol
        { get; }

        /// <summary>
        /// Gets the search.
        /// </summary>
        /// <value>
        /// The search.
        /// </value>
        public extern string Search
        { get; }

        /// <summary>
        /// Gets the origin for c.
        /// </summary>
        /// <value>
        /// The origin.
        /// </value>
        public extern string Origin
        { get; }

        /// <summary>
        /// Gets or sets the hash.
        /// </summary>
        /// <value>
        /// The hash.
        /// </value>
        public extern string Hash
        { get; set; }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        public extern string Href
        { get; set; }

        /// <summary>
        /// Navigates the window to a new location and updates the browser's history.
        /// </summary>
        /// <param name="url"> The URL to navigate to. </param>
        public extern void Assign(string url);

        /// <summary>
        /// Reload the current document.
        /// </summary>
        public extern void Reload();

        /// <summary>
        /// Reload the current document.
        /// </summary>
        /// <param name="forceGet"> If true, the document will be reloaded from the server, otherwise it
        ///     may be loaded from the browser's cache. </param>
        public extern void Reload(bool forceGet);

        /// <summary>
        /// Navigates the window to a new location without updating the browser's history.
        /// </summary>
        /// <param name="url"> The URL to navigate to. </param>
        public extern void Replace(string url);
    }
}