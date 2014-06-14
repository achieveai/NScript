//-----------------------------------------------------------------------
// <copyright file="RuntimeBinderInternalCompilerException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.CSharp.RuntimeBinder
{
    using System;

    /// <summary>
    /// Definition for RuntimeBinderInternalCompilerException
    /// </summary>
    public class RuntimeBinderInternalCompilerException : Exception
    {
        public RuntimeBinderInternalCompilerException()
            :this (string.Empty,null)
        { }

        public RuntimeBinderInternalCompilerException(string message)
            : this(message, null) { }

        public RuntimeBinderInternalCompilerException(string message, Exception innerEx)
            : base(message)
        { }
    }
}
