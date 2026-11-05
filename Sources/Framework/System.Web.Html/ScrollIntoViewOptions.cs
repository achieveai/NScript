//-----------------------------------------------------------------------
// <copyright file="ScrollIntoViewOptions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Options for <see cref="Element.ScrollIntoView(ScrollIntoViewOptions)"/>. Emitted as a
    /// plain object literal: <c>{ block: ..., inline: ..., behavior: ... }</c>.
    /// </summary>
    [JsonType]
    public sealed class ScrollIntoViewOptions
    {
        /// <summary>
        /// Gets or sets the vertical alignment. One of the <see cref="ScrollLogicalPosition"/> values.
        /// </summary>
        public extern string Block
        { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment. One of the <see cref="ScrollLogicalPosition"/> values.
        /// </summary>
        public extern string Inline
        { get; set; }

        /// <summary>
        /// Gets or sets the scroll behavior. One of the <see cref="ScrollIntoViewBehavior"/> values.
        /// </summary>
        public extern string Behavior
        { get; set; }
    }

    /// <summary>
    /// Values for <see cref="ScrollIntoViewOptions.Block"/> and <see cref="ScrollIntoViewOptions.Inline"/>.
    /// </summary>
    public static class ScrollLogicalPosition
    {
        public const string Start = "start";
        public const string Center = "center";
        public const string End = "end";
        public const string Nearest = "nearest";
    }

    /// <summary>
    /// Values for <see cref="ScrollIntoViewOptions.Behavior"/>.
    /// </summary>
    public static class ScrollIntoViewBehavior
    {
        public const string Auto = "auto";
        public const string Smooth = "smooth";
        public const string Instant = "instant";
    }
}
