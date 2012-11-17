//-----------------------------------------------------------------------
// <copyright file="FormElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for FormElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class FormElement : Element
    {
        /// <summary>
        /// Gets the form element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern FormElement();

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public extern int Length
        { get; }

        /// <summary>
        /// Gets or sets the accept charset.
        /// </summary>
        /// <value>
        /// The accept charset.
        /// </value>
        public extern string AcceptCharset
        { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public extern string Action
        { get; set; }

        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        /// <value>
        /// The elements.
        /// </value>
        public extern Element[] Elements
        { get; set; }

        /// <summary>
        /// Gets or sets the type of the encode.
        /// </summary>
        /// <value>
        /// The type of the encode.
        /// </value>
        public extern string EncType
        { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public extern string Encoding
        { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public extern string Method
        { get; set; }

        /// <summary>
        /// Gets or sets target for the.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public extern string Target
        { get; set; }

        /// <summary>
        /// Resets this object.
        /// </summary>
        public extern void Reset();

        /// <summary>
        /// Submits this object.
        /// </summary>
        public extern void Submit();
    }
}