//-----------------------------------------------------------------------
// <copyright file="OptionElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for OptionElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public class OptionElement
    {
        private OptionElement() { }

        [IntrinsicField]
        public readonly FormElement Form;

        [IntrinsicField]
        public bool Selected;

        [IntrinsicField]
        public string Text;

        [IntrinsicField]
        public string Value;
    }
}