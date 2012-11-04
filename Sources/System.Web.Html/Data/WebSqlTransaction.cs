//-----------------------------------------------------------------------
// <copyright file="WebSqlTransaction.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WebSqlTransaction
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class WebSqlTransaction
    {
        private WebSqlTransaction() { }

        public extern void ExecuteSql(
            string sql,
            NativeArray arguments,
            Action<WebSqlTransaction, WebSqlResultSet> callback,
            Action<WebSqlTransaction, WebSqlError> errorCallback);
    }
}