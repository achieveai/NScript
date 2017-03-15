//-----------------------------------------------------------------------
// <copyright file="Event.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using Runtime.CompilerServices;
    using System;

    /// <summary>
    /// Definition for Event
    /// </summary>
    [ImportedType, IgnoreNamespace]
    public class Event
    {
        [ScriptName("AT_TARGET")]
        public static extern int? AT_TARGET
        { get; }

        [ScriptName("BUBBLING_PHASE")]
        public static extern int? BUBBLING_PHASE
        { get; }

        [ScriptName("CAPTURING_PHASE")]
        public static extern int? CAPTURING_PHASE
        { get; }

        public extern bool Bubbles
        { get; }

        public extern bool CancelBubble
        { get; set; }

        public extern bool Cancelable
        { get; }

        public extern EventTarget CurrentTarget
        { get; }

        public extern bool DefaultPrevented
        { get; }

        public extern int EventPhase
        { get; }

        public extern bool IsTrusted
        { get; }

        public extern bool ReturnValue
        { get; set; }

        public extern object SrcElement
        { get; }

        public extern EventTarget Target
        { get; }

        public extern Number Timestamp
        { get; }

        public extern string Type
        { get; }

        public extern bool Scoped
        { get; }

        [ScriptName("AT_TARGET")]
        public extern int? AtTarget
        { get; }

        [ScriptName("BUBBLING_PHASE")]
        public extern int? BubblingPhase
        { get; }

        [ScriptName("CAPTURING_PHASE")]
        public extern int? CapturingPhase
        { get; }

        public extern void InitEvent(
            string eventTypeArg,
            bool canBubbleArg,
            bool cancelableArg);

        public extern void PreventDefault();

        public extern void StopImmediatePropagation();

        public extern void StopPropagation();

        public extern NativeArray<EventTarget> DeepPath();
    }
}
