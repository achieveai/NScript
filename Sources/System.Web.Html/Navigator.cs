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
    /// Definition for Navigator
    /// </summary>
    [PsudoType]
    [IgnoreNamespace]
    public sealed class Navigator
    {
        private Navigator() { }

        /// <summary>
        /// Returns the name of the browser.
        /// </summary>
        [IntrinsicField]
        public readonly string AppName;

        /// <summary>
        /// Returns the version of the browser.
        /// </summary>
        [IntrinsicField]
        public readonly string AppVersion;

        /// <summary>
        /// Retrieves the current language (applies to IE and Opera).
        /// </summary>
        [IntrinsicField]
        public readonly string BrowserLanguage;

        [IntrinsicField]
        public readonly bool CookieEnabled;

        /// <summary>
        /// Returns a string representing the language of the browser (applies to Gecko, Opera, and WebKit).
        /// </summary>
        [IntrinsicField]
        public readonly string Language;

        /// <summary>
        /// Retrieves the default language used by the operating system (applies to IE).
        /// </summary>
        [IntrinsicField]
        public readonly string SystemLanguage;

        /// <summary>
        /// Retrieves the operating system's natural language setting (applies to IE and Opera).
        /// </summary>
        [IntrinsicField]
        public readonly string UserLanguage;

        [IntrinsicField]
        public readonly GeolocationService Geolocation;

        [IntrinsicField]
        public readonly bool OnLine;

        /// <summary>
        /// Returns the name of the platform.
        /// </summary>
        [IntrinsicField]
        public readonly string Platform;

        [IntrinsicField]
        public readonly bool Standalone;

        /// <summary>
        /// Returns the complete User-Agent header.
        /// </summary>
        [IntrinsicField]
        public readonly string UserAgent;
    }
}