//-----------------------------------------------------------------------
// <copyright file="Gradient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Graphics
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Gradient
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class Gradient
    {
        private Gradient() { }

        public extern void AddColorStop(double offset, string color);
    }
}