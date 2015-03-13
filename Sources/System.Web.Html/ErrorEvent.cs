//-----------------------------------------------------------------------
// <copyright file="ErrorEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ErrorEvent
    /// </summary>
    [IgnoreNamespace]
    [JsonType]
    public class ErrorEvent
    {
        public extern string Message{get;}

        [ScriptName("filename")]
        public extern string Filename { get; }

        [ScriptName("lineno")]
        public extern int? LineNumber { get; }

        [ScriptName("colno")]
        public extern int? ColNumber { get; }

        public extern ErrorObject Error { get; }
    }

    /// <summary>
    /// Error object.
    /// </summary>
    [JsonType]
    [IgnoreNamespace]
    public class ErrorObject
    {
        public extern string Stack { get; }

        public extern string Message { get; }
    }
}
