//-----------------------------------------------------------------------
// <copyright file="SourceElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for SourceElement
    /// </summary>
    [IgnoreNamespace, PsudoType]
    public class SourceElement : Element
    {
        protected SourceElement() { }

        [IntrinsicField]
        public string Src;

        [IntrinsicField]
        public string Type;
    }
}
