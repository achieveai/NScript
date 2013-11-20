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
        public const string OnKeyPressEvtName = "keypress";

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
        /// Name of the on mouse move event.
        /// </summary>
        public const string OnMouseMoveEvtName = "mousemove";

        /// <summary>
        /// Name of the on mouse wheel event.
        /// </summary>
        public const string OnMouseWheelEvtName = "mousewheel";

        /// <summary>
        /// Name of the on click event.
        /// </summary>
        public const string OnClickEvtName = "click";

        /// <summary>
        /// Name of the on focus event.
        /// </summary>
        public const string OnFocusEvtName = "focus";

        /// <summary>
        /// Name of the on blur event.
        /// </summary>
        public const string OnBlurEvtName = "blur";

        /// <summary>
        /// Name of the on scroll event.
        /// </summary>
        public const string OnScrollEvtName = "scroll";

        /// <summary>
        /// Name of the on touch start event.
        /// </summary>
        public const string OnTouchStartEvtName = "touchstart";

        /// <summary>
        /// Name of the on touch end event.
        /// </summary>
        public const string OnTouchEndEvtName = "touchend";

        /// <summary>
        /// Name of the on touch move event.
        /// </summary>
        public const string OnTouchMoveEvtName = "touchmove";

        /// <summary>
        /// Name of the on animation start event.
        /// </summary>
        public const string OnAnimationStartEvtName = "animationstart";

        /// <summary>
        /// Name of the on animation end event.
        /// </summary>
        public const string OnAnimationEndEvtName = "animationend";

        /// <summary>
        /// Name of the on web kit animation start event.
        /// </summary>
        public const string OnWebKitAnimationStartEvtName = "webkitAnimationStart";

        /// <summary>
        /// Name of the on web kit animation end event.
        /// </summary>
        public const string OnWebKitAnimationEndEvtName = "webkitAnimationEnd";

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected extern Element();

        /// <summary>
        /// The previous element sibling.
        /// </summary>
        public extern Element PreviousElementSibling
        { get; }

        /// <summary>
        /// The next element sibling.
        /// </summary>
        public extern Element NextElementSibling
        { get; }

        /// <summary>
        /// The access key.
        /// </summary>
        public extern string AccessKey
        { get; set; }

        /// <summary>
        /// Name of the class.
        /// </summary>
        public extern string ClassName
        { get; set; }

        /// <summary>
        /// Height of the client.
        /// </summary>
        public extern double ClientHeight
        { get; }

        /// <summary>
        /// The client left.
        /// </summary>
        public extern double ClientLeft
        { get; }

        /// <summary>
        /// The client top.
        /// </summary>
        public extern double ClientTop
        { get; }

        /// <summary>
        /// Width of the client.
        /// </summary>
        public extern double ClientWidth
        { get; }

        /// <summary>
        /// The current style.
        /// </summary>
        public extern Style CurrentStyle
        { get; }

        /// <summary>
        /// Gets a list of class.
        /// </summary>
        /// <value>
        /// A List of class.
        /// </value>
        public extern TokenList ClassList
        { get; }

        /// <summary>
        /// The dir.
        /// </summary>
        public extern string Dir
        { get; set; }

        // TODO: Is this on Element or just some types of elements?

        /// <summary>
        /// true to disable, false to enable.
        /// </summary>
        public extern bool Disabled
        { get; set; }

        /// <summary>
        /// The identifier.
        /// </summary>
        [ScriptName("id")]
        public extern string ID
        { get; set; }

        /// <summary>
        /// The inner html.
        /// </summary>
        public extern string InnerHTML
        { get; set; }

        /// <summary>
        /// The inner text.
        /// </summary>
        public extern string InnerText
        { get; set; }

        /// <summary>
        /// Gets the outer HTML.
        /// </summary>
        /// <value>
        /// The outer HTML.
        /// </value>
        public extern string OuterHTML
        { get; }

        /// <summary>
        /// Height of the offset.
        /// </summary>
        public extern double OffsetHeight
        { get; }

        /// <summary>
        /// The offset left.
        /// </summary>
        public extern double OffsetLeft
        { get; }

        /// <summary>
        /// The offset parent.
        /// </summary>
        public extern Element OffsetParent
        { get; }

        /// <summary>
        /// The offset top.
        /// </summary>
        public extern double OffsetTop
        { get; }

        /// <summary>
        /// Width of the offset.
        /// </summary>
        public extern double OffsetWidth
        { get; }

        /// <summary>
        /// The runtime style.
        /// </summary>
        public extern Style RuntimeStyle
        { get; }

        /// <summary>
        /// Height of the scroll.
        /// </summary>
        public extern double ScrollHeight
        { get; }

        /// <summary>
        /// The scroll left.
        /// </summary>
        public extern double ScrollLeft
        { get; set; }

        /// <summary>
        /// The scroll top.
        /// </summary>
        public extern double ScrollTop
        { get; set; }

        /// <summary>
        /// Width of the scroll.
        /// </summary>
        public extern double ScrollWidth
        { get; }

        /// <summary>
        /// The style.
        /// </summary>
        public extern Style Style
        { get; }

        /// <summary>
        /// Zero-based index of the tab.
        /// </summary>
        public extern int TabIndex
        { get; set; }

        /// <summary>
        /// Name of the tag.
        /// </summary>
        public extern string TagName
        { get; }

        /// <summary>
        /// The title.
        /// </summary>
        public extern string Title
        { get; set; }

        /// <summary>
        /// Need support for static instance methods for filters. the return type is List&lt;
        /// VisualFilter&gt;
        /// </summary>
        private extern NativeArray filters
        { get; }

        /// <summary>
        /// Gets the on key down.
        /// </summary>
        /// <value>
        /// The on key down.
        /// </value>
        public extern event Action<Element, ElementEvent> OnKeyDown;

        /// <summary>
        /// Gets the on key up.
        /// </summary>
        /// <value>
        /// The on key up.
        /// </value>
        public extern event Action<Element, ElementEvent> OnKeyUp;

        /// <summary>
        /// Gets the on key press.
        /// </summary>
        /// <value>
        /// The on key press.
        /// </value>
        public extern event Action<Element, ElementEvent> OnKeyPress;

        /// <summary>
        /// Gets the on mouse up.
        /// </summary>
        /// <value>
        /// The on mouse up.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseUp;

        /// <summary>
        /// Gets the on mouse down.
        /// </summary>
        /// <value>
        /// The on mouse down.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseDown;

        /// <summary>
        /// Gets the on mouse enter.
        /// </summary>
        /// <value>
        /// The on mouse enter.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseEnter;

        /// <summary>
        /// Gets the on mouse out.
        /// </summary>
        /// <value>
        /// The on mouse out.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseOut;

        /// <summary>
        /// Gets the on mouse move event.
        /// </summary>
        /// <value>
        /// The on mouse move event.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseMove;

        /// <summary>
        /// Gets the on mouse wheel event handler.
        /// </summary>
        /// <value>
        /// The on mouse wheel event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnMouseWheel;

        /// <summary>
        /// Gets the on click.
        /// </summary>
        /// <value>
        /// The on click.
        /// </value>
        public extern event Action<Element, ElementEvent> OnClick;

        /// <summary>
        /// Gets the on focus event handler.
        /// </summary>
        /// <value>
        /// The on focus event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnFocus;

        /// <summary>
        /// Gets the on blur event handler.
        /// </summary>
        /// <value>
        /// The on blur event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnBlur;

        /// <summary>
        /// Gets the on touch start event handler.
        /// </summary>
        /// <value>
        /// The on touch start event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnTouchStart;

        /// <summary>
        /// Gets the on touch end event handler.
        /// </summary>
        /// <value>
        /// The on touch end event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnTouchEnd;

        /// <summary>
        /// Gets the on touch move event handler.
        /// </summary>
        /// <value>
        /// The on touch move event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnTouchMove;

        /// <summary>
        /// Gets the on scroll event handler.
        /// </summary>
        /// <value>
        /// The on scroll event handler.
        /// </value>
        public extern event Action<Element, ElementEvent> OnScroll;

        /// <summary>
        /// Gets the on animation start event handler.
        /// </summary>
        /// <value>
        /// The on animation start event handler.
        /// </value>
        public event Action<Element, ElementEvent> OnAnimationStart
        {
            add
            {
                this.Bind(OnAnimationStartEvtName, value);
                this.Bind(OnWebKitAnimationStartEvtName, value);
            }

            remove
            {
                this.UnBind(OnAnimationStartEvtName, value);
                this.UnBind(OnWebKitAnimationStartEvtName, value);
            }
        }

        /// <summary>
        /// Gets the on animation end event handler.
        /// </summary>
        /// <value>
        /// The on animation end event handler.
        /// </value>
        public event Action<Element, ElementEvent> OnAnimationEnd
        {
            add
            {
                this.Bind(OnAnimationEndEvtName, value);
                this.Bind(OnWebKitAnimationEndEvtName, value);
            }

            remove
            {
                this.UnBind(OnAnimationEndEvtName, value);
                this.UnBind(OnWebKitAnimationEndEvtName, value);
            }
        }

        /// <summary>
        /// Appends a child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element AppendChild(Node child);

        /// <summary>
        /// Appends a child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// The child element itself.
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
        /// Adds the class name.
        /// </summary>
        /// <param name="className"> The name of the event such as 'load'. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public void AddClassName(string className)
        {
            if (!object.IsNullOrUndefined(this.ClassList))
            {
                this.ClassList.Add(className);
                return;
            }

            if (className == null
                || (className = className.Trim()).Length == 0)
            {
                return;
            }

            if (this.ClassName == null
                || this.ClassName.Length == 0)
            {
                this.ClassName = className;
                return;
            }

            int index = 0;
            while ((index = this.ClassName.IndexOf(className, index)) != -1)
            {
                if ((index == 0 || this.ClassName[index - 1] == ' ')
                    && (index == this.ClassName.Length - className.Length || this.ClassName[index + className.Length] == ' '))
                {
                    return;
                }

                index++;
            }

            this.ClassName = this.ClassName + " " + className;
            return;
        }

        /// <summary>
        /// Query if 'className' has class name.
        /// </summary>
        /// <param name="className"> The name of the event such as 'load'. </param>
        /// <returns>
        /// true if class name, false if not.
        /// </returns>
        public bool HasClassName(string className)
        {
            if (!object.IsNullOrUndefined(this.ClassList))
            {
                return this.ClassList.Contains(className);
            }

            int index = 0;
            while ((index = this.ClassName.IndexOf(className, index + 1)) != -1)
            {
                if ((index == 0 || this.ClassName[index - 1] == ' ')
                    && (index == this.ClassName.Length - className.Length || this.ClassName[index + className.Length] == ' '))
                {
                    return true;
                }

                index++;
            }

            return false;
        }

        /// <summary>
        /// Removes the class name described by className.
        /// </summary>
        /// <param name="className"> The name of the event such as 'load'. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public void RemoveClassName(string className)
        {
            if (!object.IsNullOrUndefined(this.ClassList))
            {
                this.ClassList.Remove(className);
                return;
            }

            if (className == null
                || (className = className.Trim()).Length == 0
                || this.ClassName == null
                || this.ClassName.Length == 0)
            {
                return;
            }

            int index = 0;
            while ((index = this.ClassName.IndexOf(className, index)) != -1)
            {
                if ((index == 0 || this.ClassName[index - 1] == ' ')
                    && (index == this.ClassName.Length - className.Length || this.ClassName[index + className.Length] == ' '))
                {
                    this.ClassName = this.ClassName.Substring(0, index > 0 ? index - 1 : 0) + this.ClassName.Substring(index + className.Length);
                    return;
                }

                index++;
            }

            return;
        }

        /// <summary>
        /// Toggle class name.
        /// </summary>
        /// <param name="className"> Name of the class. </param>
        public void ToggleClassName(string className)
        {
            if (!object.IsNullOrUndefined(this.ClassList))
            {
                this.ClassList.Toggle(className);
            }

            if (this.HasClassName(className))
            {
                this.RemoveClassName(className);
            }
            else
            {
                this.AddClassName(className);
            }
        }

        /// <summary>
        /// Binds.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void Bind(string eventName, Action<Element, ElementEvent> handler, bool capture = false)
        {
            EventBinder.AddEvent(this, eventName, handler, capture);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void UnBind(string eventName, Action<Element, ElementEvent> handler, bool capture = false)
        {
            EventBinder.RemoveEvent(this, eventName, handler, capture);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        public void UnBind(string eventName)
        {
            EventBinder.RemoveEvent(this, eventName, true);
            EventBinder.RemoveEvent(this, eventName, false);
        }

        /// <summary>
        /// Unbinds all events registered through Bind.
        /// </summary>
        public void UnBindAll()
        {
            EventBinder.CleanUp(this);
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
            this.Style.Display = string.Empty;
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
        public extern NativeArray<Element> GetElementsByClassName(string className);

        /// <summary>
        /// Gets the elements by tag name internal.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name internal.
        /// </returns>
        [ScriptName("getElementsByTagName")]
        public extern NativeArray<Element> GetElementsByTagName(string tagName);

        /// <summary>
        /// Queries a selector all internal.
        /// </summary>
        /// <param name="selector"> The selector. </param>
        /// <returns>
        /// The selector all internal.
        /// </returns>
        [ScriptName("querySelectorAll")]
        public extern NativeArray<Element> QuerySelectorAll(string selector);

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
        public extern void AddEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        public extern void RemoveEventListener(string eventName, Action<ElementEvent> listener, bool useCapture);
    }
}