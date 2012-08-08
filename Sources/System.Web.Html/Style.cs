//-----------------------------------------------------------------------
// <copyright file="Style.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Style
    /// </summary>
    [IgnoreNamespace]
    [Imported]
    public sealed class Style
    {
        private Style() { }

        /// <summary>Whether the object contains an accelerator key.</summary>
        [IntrinsicField]
        public bool Accelerator;

        /// <summary>The background properties of an object.</summary>
        [IntrinsicField]
        public string Background;

        /// <summary>How the background image is attached to the object within the document.</summary>
        [IntrinsicField]
        public string BackgroundAttachment;

        /// <summary>The color behind the content of the object.</summary>
        [IntrinsicField]
        public string BackgroundColor;

        /// <summary>The background image of the object.</summary>
        [IntrinsicField]
        public string BackgroundImage;

        /// <summary>The position of the background of the object.</summary>
        [IntrinsicField]
        public string BackgroundPosition;

        /// <summary>The x-coordinate of the backgroundPosition property.</summary>
        [IntrinsicField]
        public string BackgroundPositionX;

        /// <summary>The y-coordinate of the backgroundPosition property.</summary>
        [IntrinsicField]
        public string BackgroundPositionY;

        /// <summary>How the background of the object is tiled.</summary>
        [IntrinsicField]
        public string BackgroundRepeat;

        /// <summary>The properties to draw a border around the object.</summary>
        [IntrinsicField]
        public string Border;

        /// <summary>The properties of the bottom border of the object.</summary>
        [IntrinsicField]
        public string BorderBottom;

        /// <summary>The color of the bottom border of the object.</summary>
        [IntrinsicField]
        public string BorderBottomColor;

        /// <summary>The style of the bottom border of the object.</summary>
        [IntrinsicField]
        public string BorderBottomStyle;

        /// <summary>The width of the bottom border of the object.</summary>
        [IntrinsicField]
        public string BorderBottomWidth;

        /// <summary>Whether the row and cell borders of a table are joined in a single border or detached.</summary>
        [IntrinsicField]
        public string BorderCollapse;

        /// <summary>The border color of the object.</summary>
        [IntrinsicField]
        public string BorderColor;

        /// <summary>The properties of the left border of the object.</summary>
        [IntrinsicField]
        public string BorderLeft;

        /// <summary>The color of the left border of the object.</summary>
        [IntrinsicField]
        public string BorderLeftColor;

        /// <summary>The style of the left border of the object.</summary>
        [IntrinsicField]
        public string BorderLeftStyle;

        /// <summary>The width of the left border of the object.</summary>
        [IntrinsicField]
        public string BorderLeftWidth;

        /// <summary>The properties of the right border of the object.</summary>
        [IntrinsicField]
        public string BorderRight;

        /// <summary>The color of the right border of the object.</summary>
        [IntrinsicField]
        public string BorderRightColor;

        /// <summary>The style of the right border of the object.</summary>
        [IntrinsicField]
        public string BorderRightStyle;

        /// <summary>The width of the right border of the object.</summary>
        [IntrinsicField]
        public string BorderRightWidth;

        /// <summary>The style of the borders of the object.</summary>
        [IntrinsicField]
        public string BorderStyle;

        /// <summary>The properties of the top border of the object.</summary>
        [IntrinsicField]
        public string BorderTop;

        /// <summary>The color of the top border of the object.</summary>
        [IntrinsicField]
        public string BorderTopColor;

        /// <summary>The style of the top border of the object.</summary>
        [IntrinsicField]
        public string BorderTopStyle;

        /// <summary>The width of the top border of the object.</summary>
        [IntrinsicField]
        public string BorderTopWidth;

        /// <summary>The width of the borders of the object.</summary>
        [IntrinsicField]
        public string BorderWidth;

        /// <summary>The bottom position of the object in relation to the bottom of the next positioned object.</summary>
        [IntrinsicField]
        public string Bottom;

        /// <summary>Whether the object allows floating objects on its left side, right side, or both, so that the next text displays past the floating objects.</summary>
        [IntrinsicField]
        public string Clear;

        /// <summary>Which part of a positioned object is visible.</summary>
        [IntrinsicField]
        public string Clip;

        /// <summary>The color of the text of the object.</summary>
        [IntrinsicField]
        public string Color;

        /// <summary>The side of the object the text will flow.</summary>
        [IntrinsicField]
        public string CssFloat;

        /// <summary>The persisted representation of this style.</summary>
        [IntrinsicField]
        public string CssText;

        /// <summary>The type of cursor to display as the mouse pointer moves over the object.</summary>
        [IntrinsicField]
        public string Cursor;

        /// <summary>The reading order of content within the object.</summary>
        [IntrinsicField]
        public string Direction;

        /// <summary>Whether the object is rendered.</summary>
        [IntrinsicField]
        public string Display;

        /// <summary>The collection of filters applied to an object. (Specific to Internet Explorer)</summary>
        [IntrinsicField]
        public string Filter;

        /// <summary>The font properties of the object or one or more of six user-preference fonts.</summary>
        [IntrinsicField]
        public string Font;

        /// <summary>The name of the font used for text in the object.</summary>
        [IntrinsicField]
        public string FontFamily;

        /// <summary>The font size used for text in the object.</summary>
        [IntrinsicField]
        public string FontSize;

        /// <summary>The font style of the object as italic, normal, or oblique.</summary>
        [IntrinsicField]
        public string FontStyle;

        /// <summary>Whether the text of the object is in small capital letters.</summary>
        [IntrinsicField]
        public string FontVariant;

        /// <summary>The weight of the font of the object.</summary>
        [IntrinsicField]
        public string FontWeight;

        /// <summary>The height of the object.</summary>
        [IntrinsicField]
        public string Height;

        /// <summary>The position of the object relative to the left edge of the next positioned object in the document hierarchy.</summary>
        [IntrinsicField]
        public string Left;

        /// <summary>The amount of additional space between letters in the object.</summary>
        [IntrinsicField]
        public string LetterSpacing;

        /// <summary>The distance between lines in the object.</summary>
        [IntrinsicField]
        public string LineHeight;

        /// <summary>The listStyle properties of the object.</summary>
        [IntrinsicField]
        public string ListStyle;

        /// <summary>The image to use as a list-item marker for the object.</summary>
        [IntrinsicField]
        public string ListStyleImage;

        /// <summary>SHow the list-item marker is drawn relative to the content of the object.</summary>
        [IntrinsicField]
        public string ListStylePosition;

        /// <summary>The type of the list-item marker for the object.</summary>
        [IntrinsicField]
        public string ListStyleType;

        /// <summary>The width of the top, right, bottom, and left margins of the object.</summary>
        [IntrinsicField]
        public string Margin;

        /// <summary>The height of the bottom margin of the object.</summary>
        [IntrinsicField]
        public string MarginBottom;

        /// <summary>The width of the left margin of the object.</summary>
        [IntrinsicField]
        public string MarginLeft;

        /// <summary>The width of the right margin of the object.</summary>
        [IntrinsicField]
        public string MarginRight;

        /// <summary>The height of the top margin of the object.</summary>
        [IntrinsicField]
        public string MarginTop;

        /// <summary>The maximum height for displayable block level elements.</summary>
        [IntrinsicField]
        public string MaxHeight;

        /// <summary>The maximum width for displayable block level elements.</summary>
        [IntrinsicField]
        public string MaxWidth;

        /// <summary>The minimum height for an element.</summary>
        [IntrinsicField]
        public string MinHeight;

        /// <summary>The minimum width for displayable block level element.</summary>
        [IntrinsicField]
        public string MinWidth;

        /// <summary>The interpolation (resampling) method used to stretch images. (Specific to Internet Explorer)</summary>
        [IntrinsicField]
        public string MsInterpolationMode;

        /// <summary>How to blend the object into the rendering.</summary>
        [IntrinsicField]
        public string Opacity;

        /// <summary>How to manage the content of the object when the content exceeds the height or width of the object.</summary>
        [IntrinsicField]
        public string Overflow;

        /// <summary>How to manage the content of the object when the content exceeds the width of the object.</summary>
        [IntrinsicField]
        public string OverflowX;

        /// <summary>How to manage the content of the object when the content exceeds the height of the object.</summary>
        [IntrinsicField]
        public string OverflowY;

        /// <summary>The amount of space to insert between the object and its margin or, if there is a border, between the object and its border.</summary>
        [IntrinsicField]
        public string Padding;

        /// <summary>The amount of space to insert between the bottom border of the object and the content.</summary>
        [IntrinsicField]
        public string PaddingBottom;

        /// <summary>The amount of space to insert between the left border of the object and the content.</summary>
        [IntrinsicField]
        public string PaddingLeft;

        /// <summary>The amount of space to insert between the right border of the object and the content.</summary>
        [IntrinsicField]
        public string PaddingRight;

        /// <summary>The amount of space to insert between the top border of the object and the content.</summary>
        [IntrinsicField]
        public string PaddingTop;

        /// <summary>Whether a page break occurs after the object.</summary>
        [IntrinsicField]
        public string PageBreakAfter;

        /// <summary>Whether a page break occurs before the object.</summary>
        [IntrinsicField]
        public string PageBreakBefore;

        /// <summary>The bottom position of the object.</summary>
        [IntrinsicField]
        public int PixelBottom;

        /// <summary>The height of the object.</summary>
        [IntrinsicField]
        public int PixelHeight;

        /// <summary>The left position of the object.</summary>
        [IntrinsicField]
        public int PixelLeft;

        /// <summary>The right position of the object.</summary>
        [IntrinsicField]
        public int PixelRight;

        /// <summary>The top position of the object.</summary>
        [IntrinsicField]
        public int PixelTop;

        /// <summary>The width of the object.</summary>
        [IntrinsicField]
        public int PixelWidth;

        /// <summary>The bottom position of the object in the units specified by the bottom attribute.</summary>
        [IntrinsicField]
        public int PosBottom;

        /// <summary>The height of the object in the units specified by the height attribute.</summary>
        [IntrinsicField]
        public int PosHeight;

        /// <summary>The type of positioning used for the object.</summary>
        [IntrinsicField]
        public string Position;

        /// <summary>The left position of the object in the units specified by the left attribute.</summary>
        [IntrinsicField]
        public int PosLeft;

        /// <summary>The right position of the object in the units specified by the right attribute.</summary>
        [IntrinsicField]
        public int PosRight;

        /// <summary>The top position of the object in the units specified by the top attribute.</summary>
        [IntrinsicField]
        public int PosTop;

        /// <summary>The width of the object in the units specified by the width attribute.</summary>
        [IntrinsicField]
        public int PosWidth;

        /// <summary>The position of the object relative to the right edge of the next positioned object in the document hierarchy.</summary>
        [IntrinsicField]
        public string Right;

        /// <summary>The side of the object the text will flow.</summary>
        [IntrinsicField]
        public string StyleFloat;

        /// <summary>Whether the table layout is fixed.</summary>
        [IntrinsicField]
        public string TableLayout;

        /// <summary>Whether The text in the object is left-aligned, right-aligned, centered, or justified.</summary>
        [IntrinsicField]
        public string TextAlign;

        /// <summary>Indicates whether the text in the object has blink, line-through, overline, or underline decorations.</summary>
        [IntrinsicField]
        public string TextDecoration;

        /// <summary>Whether the object's text "blinks."</summary>
        [IntrinsicField]
        public string TextDecorationBlink;

        /// <summary>Whether the text in the object has a line drawn through it.</summary>
        [IntrinsicField]
        public string TextDecorationLineThrough;

        /// <summary>Whether the textDecoration property for the object has been set to none.</summary>
        [IntrinsicField]
        public string TextDecorationNone;

        /// <summary>Whether the text in the object has a line drawn over it.</summary>
        [IntrinsicField]
        public string TextDecorationOverline;

        /// <summary>Whether the text in the object is underlined.</summary>
        [IntrinsicField]
        public string TextDecorationUnderline;

        /// <summary>The indentation of the first line of text in the object.</summary>
        [IntrinsicField]
        public string TextIndent;

        /// <summary>The type of alignment used to justify text in the object.</summary>
        [IntrinsicField]
        public string TextJustify;

        /// <summary>Indicates whether to render ellipses(...) to indicate text overflow.</summary>
        [IntrinsicField]
        public string TextOverflow;

        /// <summary>The rendering of the text in the object.</summary>
        [IntrinsicField]
        public string TextTransform;

        /// <summary>The position of the object relative to the top of the next positioned object in the document hierarchy.</summary>
        [IntrinsicField]
        public string Top;

        /// <summary>The vertical alignment of the object.</summary>
        [IntrinsicField]
        public string VerticalAlign;

        /// <summary>Whether the content of the object is displayed.</summary>
        [IntrinsicField]
        public string Visibility;

        /// <summary>Indicates whether lines are automatically broken inside the object.</summary>
        [IntrinsicField]
        public string WhiteSpace;

        /// <summary>The width of the object.</summary>
        [IntrinsicField]
        public string Width;

        /// <summary>The amount of additional space between words in the object.</summary>
        [IntrinsicField]
        public string WordSpacing;

        /// <summary>Whether to break words when the content exceeds the boundaries of its container.</summary>
        [IntrinsicField]
        public string WordWrap;

        /// <summary>The direction and flow of the content in the object.</summary>
        [IntrinsicField]
        public string WritingMode;

        /// <summary>The stacking order of positioned objects.</summary>
        [IntrinsicField]
        public short ZIndex;

        /// <summary>The magnification scale of the object.</summary>
        [IntrinsicField]
        public string Zoom;

        [IntrinsicProperty]
        public extern string this[string name]
        {
            get;
            set;
        }
    }
}