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
    /// Error object.
    /// </summary>
    [JsonType]
    public class ErrorObject
    {
        public extern string Stack { get; }

        public extern string Message { get; }
    }
}
