//-----------------------------------------------------------------------
// <copyright file="JavaScriptStaticHelpers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for JavaScriptStaticHelpers
    /// </summary>
    public static class JavaScriptStaticHelpers
    {
        [IgnoreGenericArguments]
        [Script(@"return left === right;")]
        public extern static bool AreEqual<T>(this T left, T right);
    }
}