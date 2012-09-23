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
    /// Definition for Window
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    [ScriptName("Window")]
    public sealed class Window
    {
        private Window() { }

        [ScriptAlias("window")]
        [IntrinsicProperty]
        public extern static Window Instance
        { get; }

        [IntrinsicField]
        public readonly ApplicationCache ApplicationCache;

        /// <summary>
        /// IE only.
        /// </summary>
        [IntrinsicField]
        public readonly DataTransfer ClipboardData;

        [IntrinsicField]
        public readonly bool Closed;

        [IntrinsicField]
        public readonly object DialogArguments;

        [IntrinsicField]
        public string DefaultStatus;

        [IntrinsicField]
        public readonly Document Document;

        /// <summary>
        /// Provides information about the current event being handled.
        /// </summary>
        [IntrinsicField]
        public readonly ElementEvent Event;

        [IntrinsicField]
        public readonly IFrameElement FrameElement;

        [IntrinsicField]
        public readonly History History;

        [IntrinsicField]
        public readonly int InnerHeight;

        [IntrinsicField]
        public readonly int InnerWidth;

        [IntrinsicField]
        public readonly Storage LocalStorage;

        [IntrinsicField]
        public readonly Location Location;

        [IntrinsicField]
        public readonly Navigator Navigator;

        [IntrinsicField]
        public readonly Window Parent;

        [IntrinsicField]
        public ErrorHandler Onerror;

        [IntrinsicField]
        public Action Onload;

        [IntrinsicField]
        public readonly Window Opener;

        [IntrinsicField]
        public Orientation Orientation;

        [IntrinsicField]
        public readonly int OuterHeight;

        [IntrinsicField]
        public readonly int OuterWidth;

        [IntrinsicField]
        public readonly int PageXOffset;

        [IntrinsicField]
        public readonly int PageYOffset;

        [IntrinsicField]
        public readonly Screen Screen;

        [IntrinsicField]
        public readonly Window Self;

        [IntrinsicField]
        public readonly Storage SessionStorage;

        [IntrinsicField]
        public readonly string Status;

        [IntrinsicField]
        public readonly Window Top;

        [IntrinsicField]
        public readonly Window[] Frames;
    }
}