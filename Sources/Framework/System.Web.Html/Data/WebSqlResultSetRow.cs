//-----------------------------------------------------------------------
// <copyright file="WebSqlResultSetRow.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WebSqlResultSetRow
    /// </summary>
    [IgnoreNamespace]
    public class WebSqlResultSetRow
    {
        private extern WebSqlResultSetRow();

        public extern object this[string name]
        { get; }
    }
}