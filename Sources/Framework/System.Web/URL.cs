//-----------------------------------------------------------------------
// <copyright file="URL.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The browser's URL parser. The constructor throws on an invalid URL.
    /// </summary>
    [IgnoreNamespace, ScriptName("URL")]
    public sealed class URL
    {
        /// <summary>
        /// Parses an absolute URL.
        /// </summary>
        public extern URL(string href);

        /// <summary>
        /// Parses a URL, resolving a relative one against a base.
        /// </summary>
        public extern URL(string href, string baseUrl);

        public extern string Href { get; }

        public extern string Origin { get; }

        public extern string Protocol { get; }

        public extern string Host { get; }

        public extern string Hostname { get; }

        public extern string Port { get; }

        public extern string Pathname { get; }

        public extern string Search { get; }

        public extern string Hash { get; }

        public extern URLSearchParams SearchParams { get; }

        [ScriptName("createObjectURL")]
        public static extern bool HasCreateObjectURL { get; }

        [ScriptName("createObjectURL")]
        public static extern string CreateObjectURL(Blob blob);

        [ScriptName("createObjectURL"), IgnoreGenericArguments]
        public static extern string CreateObjectURL<T>(NativeArray<T> blob);

        [ScriptName("createObjectURL")]
        public static extern string CreateObjectURL(Object blob);
    }
}
