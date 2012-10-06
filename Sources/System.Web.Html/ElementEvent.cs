//-----------------------------------------------------------------------
// <copyright file="ElementEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ElementEvent
    /// </summary>
    [PsudoType, IgnoreNamespace]
    public class ElementEvent
    {
        internal ElementEvent() { }

        [IntrinsicField]
        public readonly bool AltKey;

        [IntrinsicField]
        public readonly int Button;

        [IntrinsicField]
        public bool CancelBubble;

        [IntrinsicField]
        public readonly bool CtrlKey;

        [IntrinsicField]
        public readonly Element CurrentTarget;

        [IntrinsicField]
        public readonly DataTransfer DataTransfer;

        [IntrinsicField]
        public readonly string Detail;

        [IntrinsicField]
        public readonly Element FromElement;

        [IntrinsicField]
        public readonly int KeyCode;

        [IntrinsicField]
        public readonly bool MetaKey;

        [IntrinsicField]
        public readonly int OffsetX;

        [IntrinsicField]
        public readonly int OffsetY;

        [IntrinsicField]
        public bool ReturnValue;

        [IntrinsicField]
        public readonly bool ShiftKey;

        [IntrinsicField]
        public readonly Element SrcElement;

        [IntrinsicField]
        public readonly Element Target;

        [IntrinsicField]
        public readonly DateTime TimeStamp;

        [IntrinsicField]
        public readonly Element ToElement;

        [IntrinsicField]
        public readonly string Type;

        public extern void PreventDefault();

        public extern void StopImmediatePropagation();

        public extern void StopPropagation();
    }
}