//-----------------------------------------------------------------------
// <copyright file="DomDataCache.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DomDataCache
    /// </summary>
    public class DomDataCache<T>
    {
        private StringDictionary<Action<T>> capturePhaseEvents = new StringDictionary<Action<T>>();
        private StringDictionary<Action<T>> bubblePhaseEvents = new StringDictionary<Action<T>>();
        private Dictionary dataDictionary = new Dictionary();
        private object target;
        private bool disposed = false;

        public DomDataCache(object element)
        {
            this.target = element;
        }

        public void SetDataItem(
            string dataId,
            object dataItem)
        {
            this.dataDictionary[dataId] = dataItem;
        }

        public U GetDataItem<U>(
            string dataId)
        {
            return (U)this.dataDictionary[dataId];
        }

        public bool HasDataItem(string dataId)
        {
            return this.dataDictionary.ContainsKey(dataId);
        }

        public void AddEvent(
            string name,
            Action<T> action,
            bool onCapture = false)
        {
            Action<T> elementEvent;
            bool isW3wc = DomDataCache<T>.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<T>> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (!evts.TryGetValue(name, out elementEvent))
            {
                elementEvent = action;

                if (onCapture && DomDataCache<T>.IsW3wc(this.target))
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
                elementEvent += action;
            }

            evts[name] = elementEvent;
        }

        public void RemoveEvent(
            string name,
            Action<T> handler,
            bool onCapture = false)
        {
            Action<T> elementEvent;
            bool isW3wc = DomDataCache<T>.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<T>> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (evts.TryGetValue(name, out elementEvent))
            {
                elementEvent -= handler;

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
            bool isW3wc = DomDataCache<T>.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<T>> evts = onCapture
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
            bool isW3wc = DomDataCache<T>.IsW3wc(this.target);
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
        private extern void AddEventListener(string evtName, Action<T> cb, bool isCapture);

        [Script(@"this.target.atachEvent('on' + evtName, cb);")]
        private extern void AttachEvent(string evtName, Action cb);

        [Script(@"this.target.removeEventListener(evtName, cb, isCapture);")]
        private extern void RemoveEventListener(string evtName, Action<T> cb, bool isCapture);

        [Script(@"this.target.detachEvent('on' + evtName, cb);")]
        private extern void DetachEvent(string evtName, Action cb);

        [Script("return !(!element.addEventListener);")]
        private extern static bool IsW3wc(object element);

        [Script("this.@{[System.Web]System.Web.DomDataCache`1::EventHandlerBubble(!0)}(event);")]
        private extern void EventHandlerIE();

        [Script("return evt.type;")]
        private extern static string GetEventType(T evt);

        [Script("return obj.getAttribute(attr);")]
        private extern static string GetAttribute(object obj, string attr);

        [Script("obj.setAttribute(attr, value);")]
        private extern static void SetAttribute(object obj, string attr, string value);

        [Script("obj.removeAttribute(attr);")]
        private extern static void RemoveAttribute(object obj, string attr);

        private void EventHandlerCapture(T evt)
        {
            if (this.disposed) return;
            this.capturePhaseEvents[GetEventType(evt)](evt);
        }

        private void EventHandlerBubble(T evt)
        {
            if (this.disposed) return;
            this.bubblePhaseEvents[GetEventType(evt)](evt);
        }
    }
}