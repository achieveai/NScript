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
        [ScriptAlias("encodeURIComponent")]
        public extern static string EncodeURIComponent(string str);

        [ScriptAlias("decodeURIComponent")]
        public extern static string DecodeURIComponent(string str);

        [ScriptAlias("clearTimeout")]
        public extern static void ClearTimeout(int timeoutHandle);

        [ScriptAlias("setTimeout")]
        public extern static int SetTimeout(Action callback, int interval);

        [ScriptAlias("clearInterval")]
        public extern static void ClearInterval(int timeoutHandle);

        [ScriptAlias("setInterval")]
        public extern static int SetInterval(Action callback, int interval);
    }
}
