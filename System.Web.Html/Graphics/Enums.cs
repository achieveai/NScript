//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Graphics
{
    /// <summary>
    /// Definition for Enums
    /// </summary>
    public enum LineCap
    {
        Butt = 0,
        Round,
        Square
    }

    public enum LineJoin
    {
        Miter,
        Round,
        Bevel
    }

    public enum RenderingMode
    {
        Render2D,
        Render3D
    }

    public enum TextAlign
    {
        Start = 0,
        End = 1,
        Left = 2,
        Right = 3,
        Center = 4
    }

    public enum TextBaseline
    {
        Top = 0,
        Hanging = 1,
        Middle = 2,
        Bottom = 3,
        Alphabetic = 4,
        Ideographic = 5
    }

    public enum CompositeOperation
    {
        /// <summary>
        /// A (B is ignored). Display the source image instead of the destination image.
        /// </summary>
        Copy = 0,

        /// <summary>
        /// B atop A. Display the destination image wherever both images are opaque.
        /// Display the source image wherever the source image is opaque but the
        /// destination image is transparent. Display transparency elsewhere.
        /// </summary>
        DestinationAtop = 1,

        /// <summary>
        /// B in A. Display the destination image wherever both the destination image and
        /// source image are opaque. Display transparency elsewhere.
        /// </summary>
        DestinationIn = 2,

        /// <summary>
        /// B out A. Display the destination image wherever the destination image is opaque
        /// and the source image is transparent. Display transparency elsewhere.
        /// </summary>
        DestinationOut = 3,

        /// <summary>
        /// B over A. Display the destination image wherever the destination image is opaque.
        /// Display the source image elsewhere.
        /// </summary>
        DestinationOver = 4,

        /// <summary>
        /// A plus B. Display the sum of the source image and destination image, with color
        /// values approaching 1 as a limit.
        /// </summary>
        Lighter = 5,

        /// <summary>
        /// A atop B. Display the source image wherever both images are opaque. Display the
        /// destination image wherever the destination image is opaque but the source image
        /// is transparent. Display transparency elsewhere.
        /// </summary>
        SourceAtop = 6,

        /// <summary>
        /// A in B. Display the source image wherever both the source image and destination
        /// image are opaque. Display transparency elsewhere.
        /// </summary>
        SourceIn = 7,

        /// <summary>
        /// A out B. Display the source image wherever the source image is opaque and the
        /// destination image is transparent. Display transparency elsewhere.
        /// </summary>
        SourceOut = 8,

        /// <summary>
        /// A over B. Display the source image wherever the source image is opaque. Display the
        /// destination image elsewhere. This is the default option.
        /// </summary>
        SourceOver = 9,

        /// <summary>
        /// A xor B. Exclusive OR of the source image and destination image.
        /// </summary>
        Xor = 10
    }
}