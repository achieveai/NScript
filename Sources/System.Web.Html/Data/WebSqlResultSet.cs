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
    [Imported]
    [IgnoreNamespace]
    public sealed class WebSqlResultSet
    {
        private WebSqlResultSet() { }

        [IntrinsicField]
        public readonly int InsertId;

        [IntrinsicField]
        public readonly int RowsAffected;

        [IntrinsicField]
        public readonly NativeArray /*WebSqlResultSetRow[]*/ Rows;
    }
}