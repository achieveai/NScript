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
    [Imported]
    [IgnoreNamespace]
    public sealed class WebSqlError
    {
        private WebSqlError() { }

        [IntrinsicField]
        public readonly int Code;

        [IntrinsicField]
        public readonly string Message;
    }
}