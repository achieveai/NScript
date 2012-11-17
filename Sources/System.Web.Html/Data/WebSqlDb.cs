//-----------------------------------------------------------------------
// <copyright file="WebSqlDb.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WebSqlDb
    /// </summary>
    [IgnoreNamespace]
    public sealed class WebSqlDb
    {
        private extern WebSqlDb();

        public extern string Version { get; set; }

        public extern void ChangeVersion(
            string oldVersion,
            string newVersion,
            Action<WebSqlTransaction> callback,
            Action<WebSqlError> errorCallback,
            Action successCallback);

        public extern void ReadTransaction(
            Action<WebSqlTransaction> callback,
            Action<WebSqlError> errorCallback,
            Action successCallback);

        public extern void Transaction(
            Action<WebSqlTransaction> callback,
            Action<WebSqlError> errorCallback,
            Action successCallback);
    }
}