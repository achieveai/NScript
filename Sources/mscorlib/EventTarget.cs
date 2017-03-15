//-----------------------------------------------------------------------
// <copyright file="EventTarget.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using Runtime.CompilerServices;
    using System;

    [IgnoreNamespace, ImportedType]
    public class EventTarget
    {
        public extern void DispatchEvent(Event evt);

        public extern void AddEventListener(string eventName, Action<Event> listener);

        public extern void AddEventListener(string eventName, Action<Event> listener, bool useCapture);

        public void Bind(string eventName, Action<EventTarget, Event> handler, bool capture = false)
        {
            EventBinder.AddEvent(this, eventName, handler, capture);
        }

        public void UnBind(string eventName, Action<EventTarget, Event> handler, bool capture = false)
        {
            EventBinder.RemoveEvent(this, eventName, handler, capture);
        }

        public void UnBind(string eventName)
        {
            EventBinder.RemoveEvent(this, eventName, true);
            EventBinder.RemoveEvent(this, eventName, false);
        }

        public void UnBindAll()
        {
            EventBinder.CleanUp(this);
        }
    }
}
