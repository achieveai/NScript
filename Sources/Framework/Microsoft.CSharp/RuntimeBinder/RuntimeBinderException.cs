//-----------------------------------------------------------------------
// <copyright file="RuntimeBinderException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.CSharp.RuntimeBinder
{
    using System;

    /// <summary>
    /// Definition for RuntimeBinderException
    /// </summary>
    public class RuntimeBinderException : Exception
    {
        public RuntimeBinderException()
            :this (string.Empty,null)
        { }

        public RuntimeBinderException(string message)
            : this(message, null) { }

        public RuntimeBinderException(string message, Exception innerEx)
            : base(message)
        { }
    }
}
