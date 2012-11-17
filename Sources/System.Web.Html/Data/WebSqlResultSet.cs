//-----------------------------------------------------------------------
// <copyright file="WebSqlResult.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WebSqlResult
    /// </summary>
    [IgnoreNamespace]
    public sealed class WebSqlResultSet
    {
        private extern WebSqlResultSet();

        public extern int InsertId { get; }

        public extern int RowsAffected { get; }

        public extern WebSqlResultSetRow[] Rows { get; }
    }
}