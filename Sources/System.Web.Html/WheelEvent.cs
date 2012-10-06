//-----------------------------------------------------------------------
// <copyright file="WheelEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WheelEvent
    /// </summary>
    [PsudoType, IgnoreNamespace]
    public class WheelEvent : ElementEvent
    {
        [IntrinsicField]
        public readonly int WheelDelta;

        [IntrinsicField]
        public readonly int WheelDeltaX;

        [IntrinsicField]
        public readonly int WheelDeltaY;
    }
}
