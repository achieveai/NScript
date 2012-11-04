//-----------------------------------------------------------------------
// <copyright file="GestureEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for GestureEvent
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class GestureEvent : ElementEvent
    {
        private GestureEvent() { }

        [IntrinsicField]
        public readonly double Rotation;

        [IntrinsicField]
        public readonly double Scale;
    }
}