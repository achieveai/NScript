//-----------------------------------------------------------------------
// <copyright file="DomDataCache.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DomDataCache
    /// </summary>
    public class DomDataCache
    {
        private static readonly NumberDictionary<DomDataCache> instance = new NumberDictionary<DomDataCache>();
        private static int cacheId = 0;
        private static string cacheIdString;

        private StringDictionary<Action<ElementEvent>> capturePhaseEvents = new StringDictionary<Action<ElementEvent>>();
        private StringDictionary<Action<ElementEvent>> bubblePhaseEvents = new StringDictionary<Action<ElementEvent>>();
        private Dictionary dataDictionary = new Dictionary();
        private Element target;

        private DomDataCache(Node element)
        {
            this.target = element.As<Element>();
        }

        public static DomDataCache GetDataCache(Node element)
        {
            if (cacheIdString == null)
            {
                cacheIdString = "__Id" + Math.Random(10000) + "_" + DateTime.Now.GetTime();
            }

            string attr = element.GetAttribute(cacheIdString);
            DomDataCache rv;
            if (attr != null)
            {
                Number number = Number.ParseInt(attr);
                rv = DomDataCache.instance[number];
            }
            else
            {
                rv = new DomDataCache(element);
                Number number = cacheId++;
                element.SetAttribute(cacheIdString, number.ToString());
                instance[number] = rv;
            }

            return rv;
        }

        public static void RemoveDataCache(Node element)
        {
            object attr = element.GetAttribute(cacheIdString);
            if (attr != null)
            {
                Number number = (Number)attr;
                DomDataCache dataCache = DomDataCache.instance[number];
                dataCache.Dispose();
                DomDataCache.instance.Remove(number);
                element.RemoveAttribute(cacheIdString);
            }
        }

        public void SetDataItem(
            string dataId,
            object dataItem)
        {
            this.dataDictionary[dataId] = dataItem;
        }

        public T GetDataItem<T>(
            string dataId)
        {
            return (T)this.dataDictionary[dataId];
        }

        public bool HasDataItem(string dataId)
        {
            return this.dataDictionary.ContainsKey(dataId);
        }

        public void AddEvent(
            string name,
            Action<ElementEvent> action,
            bool onCapture = false)
        {
            Action<ElementEvent> elementEvent;
            bool isW3wc = DomDataCache.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<ElementEvent>> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (!evts.TryGetValue(name, out elementEvent))
            {
                elementEvent = action;

                if (onCapture && DomDataCache.IsW3wc(this.target))
                {
                    this.target.AddEventListener(name, this.EventHandlerCapture, true);
                }
                else if (isW3wc)
                {
                    this.target.AddEventListener(name, this.EventHandlerBubble, false);
                }
                else
                {
                    this.target.AttachEvent("on" + name, this.EventHandlerIE);
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
            Action<ElementEvent> handler,
            bool onCapture = false)
        {
            Action<ElementEvent> elementEvent;
            bool isW3wc = DomDataCache.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<ElementEvent>> evts = onCapture
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
                        this.target.RemoveEventListener(name, this.EventHandlerCapture, true);
                    }
                    else if (isW3wc)
                    {
                        this.target.RemoveEventListener(name, this.EventHandlerBubble, false);
                    }
                    else
                    {
                        this.target.DetachEvent("on" + name, this.EventHandlerIE);
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
            bool isW3wc = DomDataCache.IsW3wc(this.target);
            onCapture = onCapture && isW3wc;
            StringDictionary<Action<ElementEvent>> evts = onCapture
                ? this.capturePhaseEvents
                : this.bubblePhaseEvents;

            if (evts.Remove(name))
            {
                if (onCapture)
                {
                    this.target.RemoveEventListener(name, this.EventHandlerCapture, true);
                }
                else if (isW3wc)
                {
                    this.target.RemoveEventListener(name, this.EventHandlerBubble, false);
                }
                else
                {
                    this.target.DetachEvent("on" + name, this.EventHandlerIE);
                }
            }
        }

        private void Dispose()
        {
            bool isW3wc = DomDataCache.IsW3wc(this.target);
            if (isW3wc)
            {
                foreach (var item in this.capturePhaseEvents)
                {
                    this.target.RemoveEventListener(item.Key, this.EventHandlerCapture, true);
                }
            }

            foreach (var item in this.bubblePhaseEvents)
            {
                if (isW3wc)
                {
                    this.target.RemoveEventListener(item.Key, this.EventHandlerBubble, true);
                }
                else
                {
                    this.target.DetachEvent("on" + item.Key, this.EventHandlerIE);
                }
            }

            this.capturePhaseEvents = null;
            this.bubblePhaseEvents = null;
            this.dataDictionary = null;
            this.target = null;
        }

        [Script("return !(!element.addEventListener);")]
        private extern static bool IsW3wc(Element element);

        [Script("this.@{[System.Web.Html]System.Web.Html.DomDataCache::EventHandlerBubble([System.Web.Html]System.Web.Html.ElementEvent)}(event);")]
        private extern void EventHandlerIE();

        private void EventHandlerCapture(ElementEvent evt)
        {
            this.capturePhaseEvents[evt.Type](evt);
        }

        private void EventHandlerBubble(ElementEvent evt)
        {
            this.bubblePhaseEvents[evt.Type](evt);
        }
    }
}