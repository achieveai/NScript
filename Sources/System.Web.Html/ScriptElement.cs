//-----------------------------------------------------------------------
// <copyright file="ScriptElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ScriptElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class ScriptElement : Element
    {
        private ScriptElement() { }

        [IntrinsicField]
        public string Src;

        [IntrinsicField]
        public string Type;

        [IntrinsicField]
        public string ReadyState;
    }
}