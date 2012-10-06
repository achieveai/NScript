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
    /// Definition for Element.
    /// </summary>
    [IgnoreNamespace]
    [PsudoType]
    public class Element : Node
    {
        /// <summary>
        /// Name of the on key up event.
        /// </summary>
        public const string OnKeyUpEvtName = "keyup";

        /// <summary>
        /// Name of the on key down event.
        /// </summary>
        public const string OnKeyDownEvtName = "keydown";

        /// <summary>
        /// The on key press evtname.
        /// </summary>
        public const string OnKeyPressEvtname = "keypress";

        /// <summary>
        /// Name of the on mouse up event.
        /// </summary>
        public const string OnMouseUpEvtName = "mouseup";

        /// <summary>
        /// Name of the on mouse down event.
        /// </summary>
        public const string OnMouseDownEvtName = "mousedown";

        /// <summary>
        /// Name of the on mouse enter event.
        /// </summary>
        public const string OnMouseEnterEvtName = "mouseenter";

        /// <summary>
        /// Name of the on mouse out event.
        /// </summary>
        public const string OnMouseOutEvtName = "mouseout";

        /// <summary>
        /// Name of the on click event.
        /// </summary>
        public const string OnClickEvtName = "click";

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal Element() { }

        /// <summary>
        /// The previous element sibling.
        /// </summary>
        [IntrinsicField]
        public readonly Element PreviousElementSibling;

        /// <summary>
        /// The next element sibling.
        /// </summary>
        [IntrinsicField]
        public readonly Element NextElementSibling;

        /// <summary>
        /// The access key.
        /// </summary>
        [IntrinsicField]
        public string AccessKey;

        /// <summary>
        /// Name of the class.
        /// </summary>
        [IntrinsicField]
        public string ClassName;

        /// <summary>
        /// Height of the client.
        /// </summary>
        [IntrinsicField]
        public readonly int ClientHeight;

        /// <summary>
        /// The client left.
        /// </summary>
        [IntrinsicField]
        public readonly int ClientLeft;

        /// <summary>
        /// The client top.
        /// </summary>
        [IntrinsicField]
        public readonly int ClientTop;

        /// <summary>
        /// Width of the client.
        /// </summary>
        [IntrinsicField]
        public readonly int ClientWidth;

        /// <summary>
        /// The current style.
        /// </summary>
        [IntrinsicField]
        public readonly Style CurrentStyle;

        /// <summary>
        /// The dir.
        /// </summary>
        [IntrinsicField]
        public string Dir;

        // TODO: Is this on Element or just some types of elements?

        /// <summary>
        /// true to disable, false to enable.
        /// </summary>
        [IntrinsicField]
        public bool Disabled;

        /// <summary>
        /// The identifier.
        /// </summary>
        [IntrinsicField]
        public string ID;

        /// <summary>
        /// The inner html.
        /// </summary>
        [IntrinsicField]
        public string InnerHTML;

        /// <summary>
        /// The inner text.
        /// </summary>
        [IntrinsicField]
        public string InnerText;

        /// <summary>
        /// Height of the offset.
        /// </summary>
        [IntrinsicField]
        public readonly int OffsetHeight;

        /// <summary>
        /// The offset left.
        /// </summary>
        [IntrinsicField]
        public readonly int OffsetLeft;

        /// <summary>
        /// The offset parent.
        /// </summary>
        [IntrinsicField]
        public readonly Element OffsetParent;

        /// <summary>
        /// The offset top.
        /// </summary>
        [IntrinsicField]
        public readonly int OffsetTop;

        /// <summary>
        /// Width of the offset.
        /// </summary>
        [IntrinsicField]
        public readonly int OffsetWidth;

        /// <summary>
        /// The runtime style.
        /// </summary>
        [IntrinsicField]
        public readonly Style RuntimeStyle;

        /// <summary>
        /// Height of the scroll.
        /// </summary>
        [IntrinsicField]
        public readonly int ScrollHeight;

        /// <summary>
        /// The scroll left.
        /// </summary>
        [IntrinsicField]
        public int ScrollLeft;

        /// <summary>
        /// The scroll top.
        /// </summary>
        [IntrinsicField]
        public int ScrollTop;

        /// <summary>
        /// Width of the scroll.
        /// </summary>
        [IntrinsicField]
        public readonly int ScrollWidth;

        /// <summary>
        /// The style.
        /// </summary>
        [IntrinsicField]
        public readonly Style Style;

        /// <summary>
        /// Zero-based index of the tab.
        /// </summary>
        [IntrinsicField]
        public int TabIndex;

        /// <summary>
        /// Name of the tag.
        /// </summary>
        [IntrinsicField]
        public readonly string TagName;

        /// <summary>
        /// The title.
        /// </summary>
        [IntrinsicField]
        public string Title;

        /// <summary>
        /// Need support for static instance methods for filters. the return type is List&lt;
        /// VisualFilter&gt;
        /// </summary>
        [IntrinsicField]
        private readonly NativeArray filters;

        /// <summary>
        /// The element binder.
        /// </summary>
        private DomDataCache<ElementEvent> elementBinder;

        /// <summary>
        /// Appends a child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element AppendChild(Element child);

        /// <summary>
        /// Appends a child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element AppendChild(DocumentFragment child);

        /// <summary>
        /// Blurs this object.
        /// </summary>
        public extern void Blur();

        /// <summary>
        /// Clicks this object.
        /// </summary>
        public extern void Click();

        /// <summary>
        /// Gets the clone node.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern Element CloneNode();

        /// <summary>
        /// Clone node.
        /// </summary>
        /// <param name="deep"> true to deep. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element CloneNode(bool deep);

        /// <summary>
        /// Query if this object contains the given element.
        /// </summary>
        /// <param name="element"> The Element to test for containment. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        public extern bool Contains(Element element);

        /// <summary>
        /// Dispatch event.
        /// </summary>
        /// <param name="eventObject"> The event object. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool DispatchEvent(MutableEvent eventObject);

        /// <summary>
        /// Determines if we can drag drop.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool DragDrop();

        /// <summary>
        /// Focus this object.
        /// </summary>
        public extern void Focus();

        /// <summary>
        /// Queries a selector.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector.
        /// </returns>
        public extern Element QuerySelector(string selector);

        /// <summary>
        /// Scroll into view.
        /// </summary>
        public extern void ScrollIntoView();

        /// <summary>
        /// Scroll into view.
        /// </summary>
        /// <param name="alignTop"> true to align top. </param>
        public extern void ScrollIntoView(bool alignTop);

        /// <summary>
        /// Sets the active.
        /// </summary>
        public extern void SetActive();

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <returns>
        /// The filters.
        /// </returns>
        public DomList<VisualFilter> GetFilters()
        {
            return new DomList<VisualFilter>(this.filters);
        }

        /// <summary>
        /// Gets the client rects list.
        /// </summary>
        /// <returns>
        /// The client rects list.
        /// </returns>
        public DomList<ClientRect> GetClientRectsList()
        {
            return new DomList<ClientRect>(this.GetClientRects());
        }

        /// <summary>
        /// Gets the elements by class name.
        /// </summary>
        /// <param name="className"> Name of the class. </param>
        /// <returns>
        /// The elements by class name.
        /// </returns>
        public DomList<Element> GetElementsByClassName(string className)
        {
            return new DomList<Element>(this.GetElementsByClassNameInternal(className));
        }

        /// <summary>
        /// Gets the elements by tag name.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name.
        /// </returns>
        public DomList<Element> GetElementsByTagName(string tagName)
        {
            return new DomList<Element>(this.GetElementsByTagNameInternal(tagName));
        }

        /// <summary>
        /// Queries a selector all.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector all.
        /// </returns>
        public DomList<Element> QuerySelectorAll(string selector)
        {
            return new DomList<Element>(this.QuerySelectorAllInternal(selector));
        }

        /// <summary>
        /// Adds the class name.
        /// </summary>
        /// <param name="className"> The name of the event such as 'load'. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool AddClassName(string className)
        {
            if (className == null
                || (className = className.Trim()).Length == 0)
            {
                return false;
            }

            if (this.ClassName == null
                || this.ClassName.Length == 0)
            {
                this.ClassName = className;
                return true;
            }

            int index = 0;
            while ((index = this.ClassName.IndexOf(className, index)) != -1)
            {
                if ((index == 0 || this.ClassName[index - 1] == ' ')
                    && (index == this.ClassName.Length - className.Length || this.ClassName[index + className.Length] == ' '))
                {
                    return false;
                }
            }

            this.ClassName = this.ClassName + " " + className;
            return true;
        }

        /// <summary>
        /// Removes the class name described by className.
        /// </summary>
        /// <param name="className"> The name of the event such as 'load'. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool RemoveClassName(string className)
        {
            if (className == null
                || (className = className.Trim()).Length == 0
                || this.ClassName == null
                || this.ClassName.Length == 0)
            {
                return false;
            }

            int index = 0;
            while ((index = this.ClassName.IndexOf(className, index)) != -1)
            {
                if ((index == 0 || this.ClassName[index - 1] == ' ')
                    && (index == this.ClassName.Length - className.Length || this.ClassName[index + className.Length] == ' '))
                {
                    this.ClassName = this.ClassName.Substring(0, index > 0 ? index - 1 : 0) + this.ClassName.Substring(index + className.Length);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Binds.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void Bind(string eventName, Action<ElementEvent> handler, bool capture = false)
        {
            if (object.IsNullOrUndefined(this.elementBinder))
            {
                this.elementBinder = new DomDataCache<ElementEvent>(this);
            }

            this.elementBinder.AddEvent(eventName, handler, capture);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void UnBind(string eventName, Action<ElementEvent> handler, bool capture = false)
        {
            if (!object.IsNullOrUndefined(this.elementBinder))
            {
                this.elementBinder.RemoveEvent(eventName, handler, capture);
            }
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        public void UnBind(string eventName)
        {
            if (!object.IsNullOrUndefined(this.elementBinder))
            {
                this.elementBinder.RemoveEvent(eventName);
            }
        }

        /// <summary>
        /// Unbinds all events registered through Bind.
        /// </summary>
        public void UnBindAll()
        {
            if (!object.IsNullOrUndefined(this.elementBinder))
            {
                this.elementBinder.Dispose();
                this.elementBinder = null;
            }
        }

        /// <summary>
        /// Hides this element by setting display to none.
        /// </summary>
        public void Hide()
        {
            this.Style.Display = "none";
        }

        /// <summary>
        /// Shows this object by setting display to default.
        /// </summary>
        public void Show()
        {
            this.Style.Display = null;
        }

        /// <summary>
        /// Gets the client rects.
        /// </summary>
        /// <returns>
        /// The client rects.
        /// </returns>
        private extern NativeArray GetClientRects();

        /// <summary>
        /// Gets the elements by class name internal.
        /// </summary>
        /// <param name="className"> Name of the class. </param>
        /// <returns>
        /// The elements by class name internal.
        /// </returns>
        [ScriptName("getElementsByClassName")]
        private extern NativeArray GetElementsByClassNameInternal(string className);

        /// <summary>
        /// Gets the elements by tag name internal.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name internal.
        /// </returns>
        [ScriptName("getElementsByTagName")]
        private extern NativeArray GetElementsByTagNameInternal(string tagName);

        /// <summary>
        /// Queries a selector all internal.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector all internal.
        /// </returns>
        [ScriptName("querySelectorall")]
        private extern NativeArray QuerySelectorAllInternal(string selector);

        /// <summary>
        /// Attach event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="handler">   The handler. </param>
        internal extern void AttachEvent(string eventName, Action handler);

        /// <summary>
        /// Detach event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="handler">   The handler. </param>
        internal extern void DetachEvent(string eventName, Action handler);

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        internal extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        internal extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);
    }
}