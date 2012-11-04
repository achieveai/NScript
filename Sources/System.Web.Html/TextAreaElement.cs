//-----------------------------------------------------------------------
// <copyright file="TextAreaElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TextAreaElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class TextAreaElement : InputElement
    {
        private TextAreaElement() { }

        [IntrinsicField]
        public int Cols;

        [IntrinsicField]
        public bool ReadOnly;

        [IntrinsicField]
        public int Rows;
    }
}