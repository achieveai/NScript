//-----------------------------------------------------------------------
// <copyright file="ClientRect.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ClientRect
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class ClientRect
    {
        private ClientRect() { }

        [IntrinsicField]
        public readonly double Bottom; 

        [IntrinsicField]
        public readonly double Left; 

        [IntrinsicField]
        public readonly double Right; 

        [IntrinsicField]
        public readonly double Top; 
    }
}
