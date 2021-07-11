//-----------------------------------------------------------------------
// <copyright file="ElementCollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ElementCollection
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class NodeCollection
    {
        private NodeCollection() { }

        [IntrinsicField]
        public readonly int Length;

        [IntrinsicProperty]
        public extern Element this[int index]
        {
            get;
            set;
        }
    }
}