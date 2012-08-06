//-----------------------------------------------------------------------
// <copyright file="TouchEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TouchEvent
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class TouchEvent : ElementEvent
    {
        private TouchEvent() { }

        [IntrinsicField]
        public readonly TouchInfo[] ChangedTouches;

        [IntrinsicField]
        public readonly TouchInfo[] TargetTouches;

        [IntrinsicField]
        public readonly TouchInfo[] Touches;
    }
}