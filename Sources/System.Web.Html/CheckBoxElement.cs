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
    [IgnoreNamespace]
    public class CheckBoxElement : InputElement
    {
        internal extern CheckBoxElement();

        public extern bool DefaultChecked
        { get; }

        public extern bool Checked
        { get; set; }
    }
}