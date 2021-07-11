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
    [IgnoreNamespace]
    public sealed class Gradient
    {
        private extern Gradient();

        public extern void AddColorStop(double offset, string color);
    }
}