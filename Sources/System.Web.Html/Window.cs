//-----------------------------------------------------------------------
// <copyright file="Window.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Web.Html.Data;

    /// <summary>
    /// Definition for Window.
    /// </summary>
    [IgnoreNamespace]
    public sealed class Window
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern Window();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        [ScriptAlias("window")]
        [IntrinsicProperty]
        public extern static Window Instance
        { get; }

        /// <summary>
        /// The application cache.
        /// </summary>
        /// <value>
        /// The application cache.
        /// </value>
        public extern ApplicationCache ApplicationCache { get; }

        /// <summary>
        /// IE only.
        /// </summary>
        /// <value>
        /// Information describing the clipboard.
        /// </value>
        public extern DataTransfer ClipboardData { get; }

        /// <summary>
        /// The closed.
        /// </summary>
        /// <value>
        /// true if closed, false if not.
        /// </value>
        public extern bool Closed { get; }

        /// <summary>
        /// The dialog arguments.
        /// </summary>
        /// <value>
        /// The dialog arguments.
        /// </value>
        public extern object DialogArguments { get; }

        /// <summary>
        /// The default status.
        /// </summary>
        /// <value>
        /// The default status.
        /// </value>
        public extern string DefaultStatus {get; set; }

        /// <summary>
        /// The document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public extern Document Document { get; }

        /// <summary>
        /// Provides information about the current event being handled.
        /// </summary>
        /// <value>
        /// The event.
        /// </value>
        public extern ElementEvent Event { get; }

        /// <summary>
        /// The frame element.
        /// </summary>
        /// <value>
        /// The frame element.
        /// </value>
        public extern IFrameElement FrameElement { get; }

        /// <summary>
        /// The history.
        /// </summary>
        /// <value>
        /// The history.
        /// </value>
        public extern History History { get; }

        /// <summary>
        /// Height of the inner.
        /// </summary>
        /// <value>
        /// The height of the inner.
        /// </value>
        public extern int InnerHeight { get; }

        /// <summary>
        /// Width of the inner.
        /// </summary>
        /// <value>
        /// The width of the inner.
        /// </value>
        public extern int InnerWidth { get; }

        /// <summary>
        /// The local storage.
        /// </summary>
        /// <value>
        /// The local storage.
        /// </value>
        public extern Storage LocalStorage { get; }

        /// <summary>
        /// The location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public extern Location Location { get; }

        /// <summary>
        /// The navigator.
        /// </summary>
        /// <value>
        /// The navigator.
        /// </value>
        public extern Navigator Navigator { get; }

        /// <summary>
        /// The parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public extern Window Parent { get; }

        /// <summary>
        /// The onerror.
        /// </summary>
        public extern event Action<Window, ErrorEvent> OnError;

        /// <summary>
        /// The onresize.
        /// </summary>
        public extern event Action<Window> OnResize;

        /// <summary>
        /// Gets the on load event handler.
        /// </summary>
        public extern event Action<Window> OnLoad;

        /// <summary>
        /// Event queue for all listeners interested in OnHashChanged events.
        /// </summary>
        public extern event Action<Window> OnHashChange;

        /// <summary>
        /// Event queue for all listeners interested in OnContextMenu events.
        /// </summary>
        public extern event Action<Window> OnContextMenu;

        /// <summary>
        /// Event queue for all listeners interested in OnDeviceMotion events.
        /// </summary>
        public extern event Action<Window, ElementEvent> OnDeviceMotion;

        /// <summary>
        /// Event queue for all listeners interested in OnBeforeUnload events.
        /// </summary>
        public extern event Action<Window> OnBeforeUnload;

        /// <summary>
        /// Event queue for all listeners interested in OnBeforePrint events.
        /// </summary>
        public extern event Action<Window> OnBeforePrint;

        /// <summary>
        /// Event queue for all listeners interested in OnAfterPrint events.
        /// </summary>
        public extern event Action<Window> OnAfterPrint;

        /// <summary>
        /// Event queue for all listeners interested in OnUnload events.
        /// </summary>
        public extern event Action<Window> OnUnload;

        /// <summary>
        /// Event queue for all listeners interested in OnOnline events.
        /// </summary>
        public extern event Action<Window> OnOnline;

        /// <summary>
        /// Event queue for all listeners interested in OnOffline events.
        /// </summary>
        public extern event Action<Window> OnOffline;

        /// <summary>
        /// The opener.
        /// </summary>
        /// <value>
        /// The opener.
        /// </value>
        public extern Window Opener { get; }

        /// <summary>
        /// The orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public extern Orientation Orientation {get; set; }

        /// <summary>
        /// Height of the outer.
        /// </summary>
        /// <value>
        /// The height of the outer.
        /// </value>
        public extern int OuterHeight { get; }

        /// <summary>
        /// Width of the outer.
        /// </summary>
        /// <value>
        /// The width of the outer.
        /// </value>
        public extern int OuterWidth { get; }

        /// <summary>
        /// The page x coordinate offset.
        /// </summary>
        /// <value>
        /// The page x coordinate offset.
        /// </value>
        public extern int PageXOffset { get; }

        /// <summary>
        /// The page y coordinate offset.
        /// </summary>
        /// <value>
        /// The page y coordinate offset.
        /// </value>
        public extern int PageYOffset { get; }

        /// <summary>
        /// Gets the screen x coordinate.
        /// </summary>
        /// <value>
        /// The screen x coordinate.
        /// </value>
        public extern int? ScreenX { get; }

        /// <summary>
        /// Gets the screen y coordinate.
        /// </summary>
        /// <value>
        /// The screen y coordinate.
        /// </value>
        public extern int? ScreenY { get; }

        /// <summary>
        /// Gets the screen left.
        /// </summary>
        /// <value>
        /// The screen left.
        /// </value>
        public extern int? ScreenLeft { get; }

        /// <summary>
        /// Gets the screen top.
        /// </summary>
        /// <value>
        /// The screen top.
        /// </value>
        public extern int? ScreenTop { get; }

        /// <summary>
        /// The screen.
        /// </summary>
        /// <value>
        /// The screen.
        /// </value>
        public extern Screen Screen { get; }

        /// <summary>
        /// The self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        public extern Window Self { get; }

        /// <summary>
        /// The session storage.
        /// </summary>
        /// <value>
        /// The session storage.
        /// </value>
        public extern Storage SessionStorage { get; }

        /// <summary>
        /// The status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public extern string Status { get; }

        /// <summary>
        /// The top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public extern Window Top { get; }

        /// <summary>
        /// The frames.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        public extern Window[] Frames { get; }

        /// <summary>
        /// Opens the given document.
        /// </summary>
        /// <param name="url">   URL of the document. </param>
        /// <param name="title"> title of the window. </param>
        /// <returns>
        /// Window object for the opened window.
        /// </returns>
        public extern Window Open(string url, string title);

        /// <summary>
        /// Opens the given document.
        /// </summary>
        /// <param name="url">             URL of the document. </param>
        /// <param name="title">           title of the window. </param>
        /// <param name="windowOpenParam"> The window open parameter. </param>
        /// <returns>
        /// Window object for opened window.
        /// </returns>
        public extern Window Open(string url, string title, string windowOpenParam);

        /// <summary>
        /// Closes this object.
        /// </summary>
        public extern void Close();

        /// <summary>
        /// Binds.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void Bind(string eventName, Action<Window, ElementEvent> handler, bool capture = false)
        {
            EventBinder.AddEvent(this, eventName, handler, capture);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        /// <param name="capture">   (optional) the capture. </param>
        public void UnBind(string eventName, Action<Window, ElementEvent> handler, bool capture = false)
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
    }
}