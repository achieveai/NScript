//-----------------------------------------------------------------------
// <copyright file="InputElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for InputElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    [PsudoType]
    public class InputElement : Element
    {
        internal InputElement() { }

        [IntrinsicField]
        public readonly string DefaultValue;

        [IntrinsicField]
        public readonly FormElement Form;

        [IntrinsicField]
        public string Name;

        [IntrinsicField]
        public string Type;

        [IntrinsicField]
        public string Value;

        public extern void Select();
    }
}