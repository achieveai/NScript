//-----------------------------------------------------------------------
// <copyright file="WebSqlError.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WebSqlError
    /// </summary>
    [IgnoreNamespace]
    public sealed class WebSqlError
    {
        private extern WebSqlError();

        public extern int Code { get; }

        public extern string Message { get; }
    }
}