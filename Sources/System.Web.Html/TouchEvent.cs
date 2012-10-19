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
    [PsudoType, IgnoreNamespace]
    public sealed class TouchEvent : ElementEvent
    {
        private TouchEvent() { }

        public TouchInfo[] ChangedTouches
        {
            get
            {
                if (object.IsNullOrUndefined(this.changedTouchesInternal))
                {
                    this.changedTouchesInternal = this.changedTouches.GetArray<TouchInfo>();
                }

                return this.changedTouchesInternal;
            }
        }

        public TouchInfo[] TargetTouches
        {
            get
            {
                if (object.IsNullOrUndefined(this.targetTouchesInternal))
                {
                    this.targetTouchesInternal = this.targetTouches.GetArray<TouchInfo>();
                }

                return this.targetTouchesInternal;
            }
        }

        public TouchInfo[] Touches
        {
            get
            {
                if (object.IsNullOrUndefined(this.touchesInternal))
                {
                    this.touchesInternal = this.touches.GetArray<TouchInfo>();
                }

                return this.touchesInternal;
            }
        }

        [IntrinsicField]
        private readonly NativeArray changedTouches;

        [IntrinsicField]
        private readonly NativeArray targetTouches;

        [IntrinsicField]
        private readonly NativeArray touches;

        private TouchInfo[] touchesInternal;
        private TouchInfo[] targetTouchesInternal;
        private TouchInfo[] changedTouchesInternal;
    }
}