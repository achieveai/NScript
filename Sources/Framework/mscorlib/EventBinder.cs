//-----------------------------------------------------------------------
// <copyright file="EventBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for EventBinder
    /// </summary>
    public class EventBinder
    {
        private StringDictionary<Delegate> capturePhaseEvents = new StringDictionary<Delegate>();
        private StringDictionary<Delegate> bubblePhaseEvents = new StringDictionary<Delegate>();
        private Dictionary dataDictionary = null;
        private object target;
        private bool disposed = false;

        /// <summary>
        /// Hook called before dispatching a DOM event. Set by the framework
        /// (e.g., CallContext.StartRoot) to create a new action context.
        /// Returns the previous context so OnEventDispatchEnd can restore it.
        /// The event object is passed so subscribers can distinguish
        /// user-gesture DOM events (click, input, focus, etc.) from async I/O
        /// completion events that also flow through EventBinder
        /// (e.g. IndexedDB <c>success</c>/<c>error</c>/<c>upgradeneeded</c>,
        /// <c>IDBTransaction</c> <c>complete</c>, etc.). I/O completions must
        /// not start a new action root — they run on behalf of whichever
        /// action issued the underlying request.
        /// </summary>
        public static Func<object, object> OnEventDispatch;

        /// <summary>
        /// Hook called after a DOM event handler returns. Restores the
        /// previous context to avoid stale context leaking into background work.
        /// </summary>
        public static Action<object> OnEventDispatchEnd;

        private EventBinder(object element)
        {
            this.target = element;
        }

        public static EventBinder GetBinder(object importedElement)
        {
            if (Object.IsNullOrUndefined(importedElement.importedExtension))
            {
                importedElement.importedExtension = new Dictionary();
            }

            if (Object.IsNullOrUndefined(importedElement.importedExtension.importedExtension))
            {
                importedElement.importedExtension.importedExtension = new EventBinder(importedElement);
            }

            return (EventBinder)importedElement.importedExtension.importedExtension;
        }

        [IgnoreGenericArguments]
        public static void AddEvent<T, U>(object importedElement, string name, Action<T,U> action, bool onCapture = false)
        {
            EventBinder binder = EventBinder.GetBinder(importedElement);
            binder.AddEvent(name, action, onCapture);
        }

        [IgnoreGenericArguments]
        public static void RemoveEvent<T, U>(object importedElement, string name, Action<T, U> action, bool onCapture = false)
        {
            if (importedElement.importedExtension == null || importedElement.importedExtension.importedExtension == null)
            {
                return;
            }

            EventBinder binder = EventBinder.GetBinder(importedElement);
            binder.RemoveEvent(name, action, onCapture);
        }

        public static void RemoveEvent(object importedElement, string name, bool onCapture = false)
        {
            if (importedElement.importedExtension == null || importedElement.importedExtension.importedExtension == null)
            {
                return;
            }

            EventBinder binder = EventBinder.GetBinder(importedElement);
            binder.RemoveEvent(name, onCapture);
        }

        public static void CleanUp(object importedElement)
        {
            if (importedElement.importedExtension == null || importedElement.importedExtension.importedExtension == null)
            {
                return;
            }

            EventBinder binder = EventBinder.GetBinder(importedElement);
            binder.Dispose();

            importedElement.importedExtension.importedExtension = null;
        }

        public void SetDataItem(
            string dataId,
            object dataItem)
        {
            if (this.dataDictionary == null)
            {
                this.dataDictionary = new Dictionary();
            }

            this.dataDictionary[dataId] = dataItem;
        }

        public U GetDataItem<U>(string dataId)
        {
            return (U)this.dataDictionary[dataId];
        }

        public bool HasDataItem(string dataId)
        {
            if (this.dataDictionary == null)
            {
                return false;
            }

            return this.dataDictionary.ContainsKey(dataId);
        }

        [IgnoreGenericArguments]
        public void AddEvent<T,U>(
            string name,
            Action<T,U> action,
            bool onCapture = false)
        {
            Delegate elementEvent;
            bool isW3wc = EventBinder.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Delegate> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (!evts.TryGetValue(name, out elementEvent))
            {
                elementEvent = action;

                if (onCapture && EventBinder.IsW3wc(this.target))
                {
                    this.AddEventListener(name, this.EventHandlerCapture, true);
                }
                else if (isW3wc)
                {
                    this.AddEventListener(name, this.EventHandlerBubble, false);
                }
                else
                {
                    this.AttachEvent(name, this.EventHandlerIE);
                }
            }
            else
            {
                elementEvent = Delegate.Combine(elementEvent, action);
            }

            evts[name] = elementEvent;
        }

        [IgnoreGenericArguments]
        public void RemoveEvent<T,U>(
            string name,
            Action<T,U> handler,
            bool onCapture = false)
        {
            Delegate elementEvent;
            bool isW3wc = EventBinder.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Delegate> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (evts.TryGetValue(name, out elementEvent))
            {
                elementEvent = Delegate.Remove(elementEvent, handler);

                if (elementEvent == null)
                {
                    evts.Remove(name);
                    if (onCapture)
                    {
                        this.RemoveEventListener(name, this.EventHandlerCapture, true);
                    }
                    else if (isW3wc)
                    {
                        this.RemoveEventListener(name, this.EventHandlerBubble, false);
                    }
                    else
                    {
                        this.DetachEvent(name, this.EventHandlerIE);
                    }
                }
                else
                {
                    evts[name] = elementEvent;
                }
            }
        }

        public void RemoveEvent(
            string name,
            bool onCapture = false)
        {
            bool isW3wc = EventBinder.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Delegate> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (evts.Remove(name))
            {
                if (onCapture)
                {
                    this.RemoveEventListener(name, this.EventHandlerCapture, true);
                }
                else if (isW3wc)
                {
                    this.RemoveEventListener(name, this.EventHandlerBubble, true);
                }
                else
                {
                    this.DetachEvent(name, this.EventHandlerIE);
                }
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            bool isW3wc = EventBinder.IsW3wc(this.target);
            if (isW3wc)
            {
                foreach (var item in this.capturePhaseEvents)
                {
                    this.RemoveEventListener(item.Key, this.EventHandlerCapture, true);
                }
            }

            foreach (var item in this.bubblePhaseEvents)
            {
                if (isW3wc)
                {
                    this.RemoveEventListener(item.Key, this.EventHandlerBubble, true);
                }
                else
                {
                    this.DetachEvent(item.Key, this.EventHandlerIE);
                }
            }

            this.capturePhaseEvents = null;
            this.bubblePhaseEvents = null;
            this.dataDictionary = null;
            this.target = null;
        }

        [Script(@"this.@{[mscorlib]System.EventBinder::target}.addEventListener(evtName, cb, isCapture);")]
        private extern void AddEventListener(string evtName, Action<object> cb, bool isCapture);

        [Script(@"this.@{[mscorlib]System.EventBinder::target}.atachEvent('on' + evtName, cb);")]
        private extern void AttachEvent(string evtName, Action cb);

        [Script(@"this.@{[mscorlib]System.EventBinder::target}.removeEventListener(evtName, cb, isCapture);")]
        private extern void RemoveEventListener(string evtName, Action<object> cb, bool isCapture);

        [Script(@"this.@{[mscorlib]System.EventBinder::target}.detachEvent('on' + evtName, cb);")]
        private extern void DetachEvent(string evtName, Action cb);

        [Script("return !(!element.addEventListener);")]
        private extern static bool IsW3wc(object element);

        [Script("this.@{[mscorlib]System.EventBinder::EventHandlerBubble([mscorlib]System.Object)}(event);")]
        private extern void EventHandlerIE();

        [Script("return evt.type;")]
        private extern static string GetEventType(object evt);

        [Script("return obj.getAttribute(attr);")]
        private extern static string GetAttribute(object obj, string attr);

        [Script("obj.setAttribute(attr, value);")]
        private extern static void SetAttribute(object obj, string attr, string value);

        [Script("obj.removeAttribute(attr);")]
        private extern static void RemoveAttribute(object obj, string attr);

        private void EventHandlerCapture(object evt)
        {
            if (this.disposed) return;
            object prev = null;
            try
            {
                if (EventBinder.OnEventDispatch != null) prev = EventBinder.OnEventDispatch(evt);
                ((Action<object,object>)this.capturePhaseEvents[GetEventType(evt)])(this.target, evt);
            }
            finally
            {
                if (EventBinder.OnEventDispatchEnd != null) EventBinder.OnEventDispatchEnd(prev);
            }
        }

        private void EventHandlerBubble(object evt)
        {
            if (this.disposed) return;
            Delegate del;
            if (this.bubblePhaseEvents.TryGetValue(GetEventType(evt), out del))
            {
                object prev = null;
                try
                {
                    if (EventBinder.OnEventDispatch != null) prev = EventBinder.OnEventDispatch(evt);
                    ((Action<object, object>)del)(this.target, evt);
                }
                finally
                {
                    if (EventBinder.OnEventDispatchEnd != null) EventBinder.OnEventDispatchEnd(prev);
                }
            }
        }
    }
}
