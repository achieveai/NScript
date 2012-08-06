//-----------------------------------------------------------------------
// <copyright file="Element.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Web.Html.Filters;

    /// <summary>
    /// Definition for Element
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    public class Element : Node
    {
        internal Element() { }

        [IntrinsicField]
        public string AccessKey;

        [IntrinsicField]
        public string ClassName;

        [IntrinsicField]
        public readonly int ClientHeight;

        [IntrinsicField]
        public readonly int ClientLeft;

        [IntrinsicField]
        public readonly int ClientTop;

        [IntrinsicField]
        public readonly int ClientWidth;

        [IntrinsicField]
        public readonly Style CurrentStyle;

        [IntrinsicField]
        public string Dir;

        // TODO: Is this on Element or just some types of elements?
        [IntrinsicField]
        public bool Disabled;

        [IntrinsicField]
        public string ID;

        [IntrinsicField]
        public string InnerHTML;

        [IntrinsicField]
        public string InnerText;

        [IntrinsicField]
        public readonly int OffsetHeight;

        [IntrinsicField]
        public readonly int OffsetLeft;

        [IntrinsicField]
        public readonly Element OffsetParent;

        [IntrinsicField]
        public readonly int OffsetTop;

        [IntrinsicField]
        public readonly int OffsetWidth;

        [IntrinsicField]
        public readonly Style RuntimeStyle;

        [IntrinsicField]
        public readonly int ScrollHeight;

        [IntrinsicField]
        public int ScrollLeft;

        [IntrinsicField]
        public int ScrollTop;

        [IntrinsicField]
        public readonly int ScrollWidth;

        [IntrinsicField]
        public readonly Style Style;

        [IntrinsicField]
        public int TabIndex;

        [IntrinsicField]
        public readonly string TagName;

        [IntrinsicField]
        public string Title;

        /// <summary>
        /// Need support for static instance methods for filters.
        /// the return type is List&lt;VisualFilter&gt;
        /// </summary>
        [IntrinsicField]
        private readonly NativeArray filters;

        public extern Element AppendChild(Element child);

        public extern Element AppendChild(DocumentFragment child);

        public extern void Blur();

        public extern void Click();

        public extern Element CloneNode();

        public extern Element CloneNode(bool deep);

        public extern bool Contains(Element element);

        public extern bool DispatchEvent(MutableEvent eventObject);

        public extern bool DragDrop();

        public extern void Focus();

        public extern Element QuerySelector(string selector);

        public extern void ScrollIntoView();

        public extern void ScrollIntoView(bool alignTop);

        public extern void SetActive();

        public DomList<VisualFilter> GetFilters()
        {
            return new DomList<VisualFilter>(this.filters);
        }

        public DomList<ClientRect> GetClientRectsList()
        {
            return new DomList<ClientRect>(this.GetClientRects());
        }

        public DomList<Element> GetElementsByClassName(string className)
        {
            return new DomList<Element>(this.GetElementsByClassNameInternal(className));
        }

        [ScriptName("getElementsByTagName")]
        public DomList<Element> GetElementsByTagName(string tagName)
        {
            return new DomList<Element>(this.GetElementsByTagNameInternal(tagName));
        }

        [ScriptName("querySelectorall")]
        public DomList<Element> QuerySelectorAll(string selector)
        {
            return new DomList<Element>(this.QuerySelectorAllInternal(selector));
        }

        public void Bind(string eventName, Action<ElementEvent> handler)
        {
            var dataCache = DomDataCache.GetDataCache(this);
            dataCache.AddEvent(eventName, handler);
        }

        public void UnBind(string eventName, Action<ElementEvent> handler)
        {
            var dataCache = DomDataCache.GetDataCache(this);
            dataCache.RemoveEvent(eventName, handler);
        }

        public void UnBind(string eventName)
        {
            var dataCache = DomDataCache.GetDataCache(this);
            dataCache.RemoveEvent(eventName);
        }

        private extern NativeArray GetClientRects();

        [ScriptName("getElementsByClassName")]
        private extern NativeArray GetElementsByClassNameInternal(string className);

        [ScriptName("getElementsByTagName")]
        private extern NativeArray GetElementsByTagNameInternal(string tagName);

        [ScriptName("querySelectorall")]
        private extern NativeArray QuerySelectorAllInternal(string selector);

        internal extern void AttachEvent(string eventName, Action handler);

        internal extern void DetachEvent(string eventName, Action handler);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        /// <param name="useCapture">Whether the listener wants to initiate capturing the event.</param>
        internal extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event such as 'load'.</param>
        /// <param name="listener">The listener to be invoked in response to the event.</param>
        /// <param name="useCapture">Whether the listener wants to initiate capturing the event.</param>
        internal extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);
    }
}