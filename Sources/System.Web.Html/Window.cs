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
        public extern ApplicationCache ApplicationCache { get; }

        /// <summary>
        /// IE only.
        /// </summary>
        public extern DataTransfer ClipboardData { get; }

        /// <summary>
        /// The closed.
        /// </summary>
        public extern bool Closed { get; }

        /// <summary>
        /// The dialog arguments.
        /// </summary>
        public extern object DialogArguments { get; }

        /// <summary>
        /// The default status.
        /// </summary>
        public extern string DefaultStatus {get; set; }

        /// <summary>
        /// The document.
        /// </summary>
        public extern Document Document { get; }

        /// <summary>
        /// Provides information about the current event being handled.
        /// </summary>
        public extern ElementEvent Event { get; }

        /// <summary>
        /// The frame element.
        /// </summary>
        public extern IFrameElement FrameElement { get; }

        /// <summary>
        /// The history.
        /// </summary>
        public extern History History { get; }

        /// <summary>
        /// Height of the inner.
        /// </summary>
        public extern int InnerHeight { get; }

        /// <summary>
        /// Width of the inner.
        /// </summary>
        public extern int InnerWidth { get; }

        /// <summary>
        /// The local storage.
        /// </summary>
        public extern Storage LocalStorage { get; }

        /// <summary>
        /// The location.
        /// </summary>
        public extern Location Location { get; }

        /// <summary>
        /// The navigator.
        /// </summary>
        public extern Navigator Navigator { get; }

        /// <summary>
        /// The parent.
        /// </summary>
        public extern Window Parent { get; }

        /// <summary>
        /// The onerror.
        /// </summary>
        public extern event Action<Window, string, string, int> OnError;

        /// <summary>
        /// The onresize.
        /// </summary>
        public extern event Action<Window> OnResize;

        /// <summary>
        /// Gets the on load event handler.
        /// </summary>
        /// <value>
        /// The on load event handler.
        /// </value>
        public extern event Action<Window> OnLoad;

        /// <summary>
        /// The opener.
        /// </summary>
        public extern Window Opener { get; }

        /// <summary>
        /// The orientation.
        /// </summary>
        public extern Orientation Orientation {get; set; }

        /// <summary>
        /// Height of the outer.
        /// </summary>
        public extern int OuterHeight { get; }

        /// <summary>
        /// Width of the outer.
        /// </summary>
        public extern int OuterWidth { get; }

        /// <summary>
        /// The page x coordinate offset.
        /// </summary>
        public extern int PageXOffset { get; }

        /// <summary>
        /// The page y coordinate offset.
        /// </summary>
        public extern int PageYOffset { get; }

        /// <summary>
        /// The screen.
        /// </summary>
        public extern Screen Screen { get; }

        /// <summary>
        /// The self.
        /// </summary>
        public extern Window Self { get; }

        /// <summary>
        /// The session storage.
        /// </summary>
        public extern Storage SessionStorage { get; }

        /// <summary>
        /// The status.
        /// </summary>
        public extern string Status { get; }

        /// <summary>
        /// The top.
        /// </summary>
        public extern Window Top { get; }

        /// <summary>
        /// The frames.
        /// </summary>
        public extern Window[] Frames { get; }

        /// <summary>
        /// Opens the given document.
        /// </summary>
        /// <param name="url">    URL of the document. </param>
        /// <param name="target"> Target for the. </param>
        public extern void Open(string url, string target);
    }
}