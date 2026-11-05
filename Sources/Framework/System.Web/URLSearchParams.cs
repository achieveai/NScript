//-----------------------------------------------------------------------
// <copyright file="URLSearchParams.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Query string reader/writer (<c>URLSearchParams</c>). ToString() gives "a=1&amp;b=2"
    /// with no leading "?".
    /// </summary>
    [IgnoreNamespace, ScriptName("URLSearchParams")]
    public sealed class URLSearchParams
    {
        /// <summary>
        /// Creates empty parameters.
        /// </summary>
        public extern URLSearchParams();

        /// <summary>
        /// Parses a query string; a leading "?" is ignored.
        /// </summary>
        /// <param name="init"> The query string, such as "?a=1&amp;b=2". </param>
        public extern URLSearchParams(string init);

        /// <summary>
        /// Gets the first value for a name, or null when it is missing.
        /// </summary>
        public extern string Get(string name);

        /// <summary>
        /// Gets every value for a name; empty when it is missing.
        /// </summary>
        public extern NativeArray<string> GetAll(string name);

        /// <summary>
        /// Returns true when the name is present.
        /// </summary>
        public extern bool Has(string name);

        /// <summary>
        /// Sets the name to one value, replacing any others.
        /// </summary>
        public extern void Set(string name, string value);

        /// <summary>
        /// Adds a value for the name, keeping the others.
        /// </summary>
        public extern void Append(string name, string value);

        /// <summary>
        /// Removes every value for the name.
        /// </summary>
        public extern void Delete(string name);

        /// <summary>
        /// Sorts the pairs by name.
        /// </summary>
        public extern void Sort();
    }
}
