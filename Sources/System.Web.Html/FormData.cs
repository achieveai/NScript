//-----------------------------------------------------------------------
// <copyright file="FormData.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for FormData.
    /// </summary>
    [IgnoreNamespace]
    public class FormData
    {
        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern FormData();

        /// <summary>
        /// Gets the form data.
        /// </summary>
        /// <param name="formElement"> The form element. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern FormData(FormElement formElement);

        /// <summary>
        /// Appends a name.
        /// </summary>
        /// <param name="name">     The name. </param>
        /// <param name="data">     The data. </param>
        /// <param name="fileName"> (optional) filename of the file. </param>
        public extern void Append(string name, File data, string fileName = null);

        /// <summary>
        /// Appends a name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <param name="data"> The data. </param>
        public extern void Append(string name, string data);
    }
}
