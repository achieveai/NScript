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
    /// Definition for URL
    /// </summary>
    [IgnoreNamespace]
    public static class URL
    {
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
