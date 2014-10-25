//-----------------------------------------------------------------------
// <copyright file="Globals.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Globals
    /// </summary>
    [Extended, IgnoreNamespace, GlobalMethods]
    public static class Globals
    {
        /// <summary>
        /// Encode URI component.
        /// Normally called for Query Parameter Name or Value.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// Encoded URI components.
        /// Will Encode "@#$&=:/,;?+"
        /// Will NOT encode "~!*()"
        /// </returns>
        [ScriptAlias("encodeURIComponent")]
        public extern static string EncodeURIComponent(string str);

        /// <summary>
        /// Decode URI component.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// .
        /// </returns>
        [ScriptAlias("decodeURIComponent")]
        public extern static string DecodeURIComponent(string str);

        /// <summary>
        /// Encode URI.
        /// Normall called for whole URL.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// Will Encode: All special characters except below
        /// Will NOT encode "~!@#$&*()=:/,;?+'"
        /// </returns>
        [ScriptAlias("encodeURIComponent")]
        public extern static string EncodeURI(string str);

        /// <summary>
        /// Decode URI.
        /// </summary>
        /// <param name="str"> The. </param>
        /// <returns>
        /// .
        /// </returns>
        [ScriptAlias("decodeURI")]
        public extern static string DecodeURI(string str);

        [ScriptAlias("clearImmediate")]
        public extern static void ClearImmediate(int timeoutHandle);

        [ScriptAlias("clearTimeout")]
        public extern static void ClearTimeout(int timeoutHandle);

        [ScriptAlias("setImmediate")]
        public extern static void SetImmediate(Action callback);

        [ScriptAlias("setTimeout")]
        public extern static int SetTimeout(Action callback, int interval);

        [ScriptAlias("clearInterval")]
        public extern static void ClearInterval(int timeoutHandle);

        [ScriptAlias("setInterval")]
        public extern static int SetInterval(Action callback, int interval);
    }
}
