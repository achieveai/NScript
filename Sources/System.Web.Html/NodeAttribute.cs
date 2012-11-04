//-----------------------------------------------------------------------
// <copyright file="NodeAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for NodeAttribute
    /// </summary>
    [Extended]
    [ScriptName("Attr")]
    public sealed class NodeAttribute
    {
        [IntrinsicField]
        public readonly string Name;

        [IntrinsicField]
        public readonly bool Specified;

        [IntrinsicField]
        public string Value;
    }
}