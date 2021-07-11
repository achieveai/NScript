//-----------------------------------------------------------------------
// <copyright file="Navigator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Web.Html.Geolocation;

    /// <summary>
    /// Definition for Navigator.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class Navigator
    {
        /// <summary>
        /// Gets the navigator.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern Navigator();

        /// <summary>
        /// Returns the name of the browser.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public extern string AppName
        { get; }

        /// <summary>
        /// Returns the version of the browser.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public extern string AppVersion
        { get; }

        /// <summary>
        /// Retrieves the current language (applies to IE and Opera).
        /// </summary>
        /// <value>
        /// The browser language.
        /// </value>
        public extern string BrowserLanguage
        { get; }

        /// <summary>
        /// Gets a value indicating whether the cookie is enabled.
        /// </summary>
        /// <value>
        /// true if cookie enabled, false if not.
        /// </value>
        public extern bool CookieEnabled
        { get; }

        /// <summary>
        /// Returns a string representing the language of the browser (applies to Gecko, Opera, and
        /// WebKit).
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public extern string Language
        { get; }

        /// <summary>
        /// Retrieves the default language used by the operating system (applies to IE).
        /// </summary>
        /// <value>
        /// The system language.
        /// </value>
        public extern string SystemLanguage
        { get; }

        /// <summary>
        /// Retrieves the operating system's natural language setting (applies to IE and Opera).
        /// </summary>
        /// <value>
        /// The user language.
        /// </value>
        public extern string UserLanguage
        { get; }

        /// <summary>
        /// Gets the geolocation.
        /// </summary>
        /// <value>
        /// The geolocation.
        /// </value>
        public extern GeolocationService Geolocation
        { get; }

        /// <summary>
        /// Gets a value indicating whether the on line.
        /// </summary>
        /// <value>
        /// true if on line, false if not.
        /// </value>
        public extern bool OnLine
        { get; }

        /// <summary>
        /// Returns the name of the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        public extern string Platform
        { get; }

        /// <summary>
        /// Gets a value indicating whether the standalone.
        /// </summary>
        /// <value>
        /// true if standalone, false if not.
        /// </value>
        public extern bool Standalone
        { get; }

        /// <summary>
        /// Returns the complete User-Agent header.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public extern string UserAgent
        { get; }

        /// <summary>
        /// Gets a value indicating whether this object has send beacon API.
        /// </summary>
        /// <value>
        /// true if this object has send beacon api, false if not.
        /// </value>
        public extern bool HasSendBeaconApi
        {
            [Script("return !!this.sendBeacon;")]
            get;
        }

        /// <summary>
        /// Sends a beacon.
        /// </summary>
        /// <param name="url">  URL of the document. </param>
        /// <param name="data"> The data. </param>
        public extern void SendBeacon(string url, string data);

        /// <summary>
        /// Sends a beacon.
        /// </summary>
        /// <param name="url">  URL of the document. </param>
        /// <param name="data"> The data. </param>
        public extern void SendBeacon(string url, FormData data);
    }
}