//-----------------------------------------------------------------------
// <copyright file="MapElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for MapElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class MapElement : Element
    {
        private MapElement() { }

        [IntrinsicField]
        public string Name;
    }
}