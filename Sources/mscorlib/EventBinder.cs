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

        private EventBinder(object element)
        {
            this.target = element;
        }

        public static EventBinder GetBinder(object importedElement)
        {
            if (Object.IsNullOrUndefined(importedElement.importedExtension))
            {
                importedElement.importedExtension = new Dictionary();
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

        [Script(@"this.target.addEventListener(evtName, cb, isCapture);")]
        private extern void AddEventListener(string evtName, Action<object> cb, bool isCapture);

        [Script(@"this.target.atachEvent('on' + evtName, cb);")]
        private extern void AttachEvent(string evtName, Action cb);

        [Script(@"this.target.removeEventListener(evtName, cb, isCapture);")]
        private extern void RemoveEventListener(string evtName, Action<object> cb, bool isCapture);

        [Script(@"this.target.detachEvent('on' + evtName, cb);")]
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
            ((Action<object,object>)this.capturePhaseEvents[GetEventType(evt)])(this.target, evt);
        }

        private void EventHandlerBubble(object evt)
        {
            if (this.disposed) return;
            ((Action<object,object>)this.bubblePhaseEvents[GetEventType(evt)])(this.target, evt);
        }
    }
}
