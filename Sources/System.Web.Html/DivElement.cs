//-----------------------------------------------------------------------
// <copyright file="DivElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DivElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    [PsudoType]
    public sealed class DivElement : Element
    {
        private DivElement() { }

        [IntrinsicField]
        public string Align;

        [IntrinsicField]
        public bool NoWrap;
    }
}