//-----------------------------------------------------------------------
// <copyright file="OptionElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for OptionElement.
    /// </summary>
    [IgnoreNamespace]
    public class OptionElement : Node
    {
        /// <summary>
        /// Gets the option element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern OptionElement();

        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <value>
        /// The form.
        /// </value>
        public extern FormElement Form
        { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the selected.
        /// </summary>
        /// <value>
        /// true if selected, false if not.
        /// </value>
        public extern bool Selected
        { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public extern string Text
        { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public extern string Value
        { get; set; }
    }
}