//-----------------------------------------------------------------------
// <copyright file="JSON.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for JSON
    /// </summary>
    [Imported, IgnoreNamespace]
    public static class JSON
    {
        public extern static Dictionary Parse(string str);

        [IgnoreGenericArguments]
        public extern static T Parse<T>(string str);

        [IgnoreGenericArguments]
        public extern static string Stringify<T>(T obj);
    }
}
