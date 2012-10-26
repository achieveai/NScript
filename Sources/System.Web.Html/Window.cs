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
    [PsudoType]
    [IgnoreNamespace]
    public sealed class Window
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private Window() { }

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
        [IntrinsicField]
        public readonly ApplicationCache ApplicationCache;

        /// <summary>
        /// IE only.
        /// </summary>
        [IntrinsicField]
        public readonly DataTransfer ClipboardData;

        /// <summary>
        /// The closed.
        /// </summary>
        [IntrinsicField]
        public readonly bool Closed;

        /// <summary>
        /// The dialog arguments.
        /// </summary>
        [IntrinsicField]
        public readonly object DialogArguments;

        /// <summary>
        /// The default status.
        /// </summary>
        [IntrinsicField]
        public string DefaultStatus;

        /// <summary>
        /// The document.
        /// </summary>
        [IntrinsicField]
        public readonly Document Document;

        /// <summary>
        /// Provides information about the current event being handled.
        /// </summary>
        [IntrinsicField]
        public readonly ElementEvent Event;

        /// <summary>
        /// The frame element.
        /// </summary>
        [IntrinsicField]
        public readonly IFrameElement FrameElement;

        /// <summary>
        /// The history.
        /// </summary>
        [IntrinsicField]
        public readonly History History;

        /// <summary>
        /// Height of the inner.
        /// </summary>
        [IntrinsicField]
        public readonly int InnerHeight;

        /// <summary>
        /// Width of the inner.
        /// </summary>
        [IntrinsicField]
        public readonly int InnerWidth;

        /// <summary>
        /// The local storage.
        /// </summary>
        [IntrinsicField]
        public readonly Storage LocalStorage;

        /// <summary>
        /// The location.
        /// </summary>
        [IntrinsicField]
        public readonly Location Location;

        /// <summary>
        /// The navigator.
        /// </summary>
        [IntrinsicField]
        public readonly Navigator Navigator;

        /// <summary>
        /// The parent.
        /// </summary>
        [IntrinsicField]
        public readonly Window Parent;

        /// <summary>
        /// The onerror.
        /// </summary>
        [IntrinsicField]
        private Action<string, string, int> onerror;

        /// <summary>
        /// The onload.
        /// </summary>
        [IntrinsicField]
        private Action Onload;

        /// <summary>
        /// The onresize.
        /// </summary>
        [IntrinsicField]
        private Action Onresize;

        /// <summary>
        /// Gets the on load event handler.
        /// </summary>
        /// <value>
        /// The on load event handler.
        /// </value>
        public event Action OnLoad
        {
            add
            {
                if (this.Onload != null)
                {
                    this.Onload = this.Onload + value;
                }
                else
                {
                    this.Onload = value;
                }
            }

            remove
            {
                if (this.Onload != null)
                {
                    this.Onload = this.Onload - value;
                }
            }
        }

        /// <summary>
        /// Gets the on resize event handler.
        /// </summary>
        /// <value>
        /// The on resize event handler.
        /// </value>
        public event Action OnResize
        {
            add
            {
                if (this.Onresize != null)
                {
                    this.Onresize = this.Onresize + value;
                }
                else
                {
                    this.Onresize = value;
                }
            }

            remove
            {
                if (this.Onresize != null)
                {
                    this.Onresize = this.Onresize - value;
                }
            }
        }

        public event Action<string, string, int> OnError
        {
            add
            {
                if (this.onerror != null)
                {
                    this.onerror = this.onerror + value;
                }
                else
                {
                    this.onerror = value;
                }
            }

            remove
            {
                if (this.onerror != null)
                {
                    this.onerror = this.onerror - value;
                }
            }
        }

        /// <summary>
        /// The opener.
        /// </summary>
        [IntrinsicField]
        public readonly Window Opener;

        /// <summary>
        /// The orientation.
        /// </summary>
        [IntrinsicField]
        public Orientation Orientation;

        /// <summary>
        /// Height of the outer.
        /// </summary>
        [IntrinsicField]
        public readonly int OuterHeight;

        /// <summary>
        /// Width of the outer.
        /// </summary>
        [IntrinsicField]
        public readonly int OuterWidth;

        /// <summary>
        /// The page x coordinate offset.
        /// </summary>
        [IntrinsicField]
        public readonly int PageXOffset;

        /// <summary>
        /// The page y coordinate offset.
        /// </summary>
        [IntrinsicField]
        public readonly int PageYOffset;

        /// <summary>
        /// The screen.
        /// </summary>
        [IntrinsicField]
        public readonly Screen Screen;

        /// <summary>
        /// The self.
        /// </summary>
        [IntrinsicField]
        public readonly Window Self;

        /// <summary>
        /// The session storage.
        /// </summary>
        [IntrinsicField]
        public readonly Storage SessionStorage;

        /// <summary>
        /// The status.
        /// </summary>
        [IntrinsicField]
        public readonly string Status;

        /// <summary>
        /// The top.
        /// </summary>
        [IntrinsicField]
        public readonly Window Top;

        /// <summary>
        /// The frames.
        /// </summary>
        [IntrinsicField]
        public readonly Window[] Frames;

        /// <summary>
        /// Opens the given document.
        /// </summary>
        /// <param name="url">    URL of the document. </param>
        /// <param name="target"> Target for the. </param>
        public extern void Open(string url, string target);
    }
}