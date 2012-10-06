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
    [PsudoType]
    public class InputElement : Element
    {
        /// <summary>
        /// Name of the on change event.
        /// </summary>
        public const string OnChangeEvtName = "change";

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal InputElement() { }

        /// <summary>
        /// The default value.
        /// </summary>
        [IntrinsicField]
        public readonly string DefaultValue;

        /// <summary>
        /// The form.
        /// </summary>
        [IntrinsicField]
        public readonly FormElement Form;

        /// <summary>
        /// The name.
        /// </summary>
        [IntrinsicField]
        public string Name;

        /// <summary>
        /// The type.
        /// </summary>
        [IntrinsicField]
        public string Type;

        /// <summary>
        /// The value.
        /// </summary>
        [IntrinsicField]
        public string Value;

        /// <summary>
        /// Selects this object.
        /// </summary>
        public extern void Select();
    }
}