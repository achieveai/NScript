//-----------------------------------------------------------------------
// <copyright file="InputElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for InputElement.
    /// </summary>
    [IgnoreNamespace]
    public class InputElement : Element
    {
        /// <summary>
        /// Name of the on change event.
        /// </summary>
        public const string OnChangeEvtName = "change";

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal extern InputElement();

        /// <summary>
        /// Gets the on change event handler.
        /// </summary>
        /// <value>
        /// The on change event handler.
        /// </value>
        public event Action<InputElement, ElementEvent> OnChange;

        /// <summary>
        /// The default value.
        /// </summary>
        public extern string DefaultValue
        { get; }

        /// <summary>
        /// The form.
        /// </summary>
        public extern FormElement Form
        { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public extern string Name
        { get; set; }

        /// <summary>
        /// The type.
        /// </summary>
        public extern string Type
        { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public extern string Value
        { get; set; }

        /// <summary>
        /// Selects this object.
        /// </summary>
        public extern void Select();
    }
}