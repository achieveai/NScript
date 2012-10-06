//-----------------------------------------------------------------------
// <copyright file="TextElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TextElement
    /// </summary>
    [PsudoType]
    [IgnoreNamespace]
    public sealed class TextElement : InputElement
    {
        internal TextElement() { }

        [IntrinsicField]
        public int MaxLength;

        [IntrinsicField]
        public bool ReadOnly;
    }
}