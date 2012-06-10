//-----------------------------------------------------------------------
// <copyright file="CheckBoxElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for CheckBoxElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public class CheckBoxElement : InputElement
    {
        internal CheckBoxElement() { }

        [IntrinsicField]
        public readonly bool DefaultChecked;

        [IntrinsicField]
        public bool Checked;
    }
}