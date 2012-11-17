//-----------------------------------------------------------------------
// <copyright file="Style.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Style.
    /// </summary>
    [IgnoreNamespace]
    [ScriptName("CSSStyleDeclaration")]
    public sealed class Style
    {
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern Style();

        /// <summary>
        /// Whether the object contains an accelerator key.
        /// </summary>
        /// <value>
        /// true if accelerator, false if not.
        /// </value>
        public extern bool Accelerator {get; set; }

        /// <summary>
        /// The background properties of an object.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public extern string Background {get; set; }

        /// <summary>
        /// How the background image is attached to the object within the document.
        /// </summary>
        /// <value>
        /// The background attachment.
        /// </value>
        public extern string BackgroundAttachment {get; set; }

        /// <summary>
        /// The color behind the content of the object.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public extern string BackgroundColor {get; set; }

        /// <summary>
        /// The background image of the object.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        public extern string BackgroundImage {get; set; }

        /// <summary>
        /// The position of the background of the object.
        /// </summary>
        /// <value>
        /// The background position.
        /// </value>
        public extern string BackgroundPosition {get; set; }

        /// <summary>
        /// The x-coordinate of the backgroundPosition property.
        /// </summary>
        /// <value>
        /// The background position x coordinate.
        /// </value>
        public extern string BackgroundPositionX {get; set; }

        /// <summary>
        /// The y-coordinate of the backgroundPosition property.
        /// </summary>
        /// <value>
        /// The background position y coordinate.
        /// </value>
        public extern string BackgroundPositionY {get; set; }

        /// <summary>
        /// How the background of the object is tiled.
        /// </summary>
        /// <value>
        /// The background repeat.
        /// </value>
        public extern string BackgroundRepeat {get; set; }

        /// <summary>
        /// The properties to draw a border around the object.
        /// </summary>
        /// <value>
        /// The border.
        /// </value>
        public extern string Border {get; set; }

        /// <summary>
        /// The properties of the bottom border of the object.
        /// </summary>
        /// <value>
        /// The border bottom.
        /// </value>
        public extern string BorderBottom {get; set; }

        /// <summary>
        /// The color of the bottom border of the object.
        /// </summary>
        /// <value>
        /// The color of the border bottom.
        /// </value>
        public extern string BorderBottomColor {get; set; }

        /// <summary>
        /// The style of the bottom border of the object.
        /// </summary>
        /// <value>
        /// The border bottom style.
        /// </value>
        public extern string BorderBottomStyle {get; set; }

        /// <summary>
        /// The width of the bottom border of the object.
        /// </summary>
        /// <value>
        /// The width of the border bottom.
        /// </value>
        public extern string BorderBottomWidth {get; set; }

        /// <summary>
        /// Whether the row and cell borders of a table are joined in a single border or detached.
        /// </summary>
        /// <value>
        /// The border collapse.
        /// </value>
        public extern string BorderCollapse {get; set; }

        /// <summary>
        /// The border color of the object.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        public extern string BorderColor {get; set; }

        /// <summary>
        /// The properties of the left border of the object.
        /// </summary>
        /// <value>
        /// The border left.
        /// </value>
        public extern string BorderLeft {get; set; }

        /// <summary>
        /// The color of the left border of the object.
        /// </summary>
        /// <value>
        /// The color of the border left.
        /// </value>
        public extern string BorderLeftColor {get; set; }

        /// <summary>
        /// The style of the left border of the object.
        /// </summary>
        /// <value>
        /// The border left style.
        /// </value>
        public extern string BorderLeftStyle {get; set; }

        /// <summary>
        /// The width of the left border of the object.
        /// </summary>
        /// <value>
        /// The width of the border left.
        /// </value>
        public extern string BorderLeftWidth {get; set; }

        /// <summary>
        /// The properties of the right border of the object.
        /// </summary>
        /// <value>
        /// The border right.
        /// </value>
        public extern string BorderRight {get; set; }

        /// <summary>
        /// The color of the right border of the object.
        /// </summary>
        /// <value>
        /// The color of the border right.
        /// </value>
        public extern string BorderRightColor {get; set; }

        /// <summary>
        /// The style of the right border of the object.
        /// </summary>
        /// <value>
        /// The border right style.
        /// </value>
        public extern string BorderRightStyle {get; set; }

        /// <summary>
        /// The width of the right border of the object.
        /// </summary>
        /// <value>
        /// The width of the border right.
        /// </value>
        public extern string BorderRightWidth {get; set; }

        /// <summary>
        /// The style of the borders of the object.
        /// </summary>
        /// <value>
        /// The border style.
        /// </value>
        public extern string BorderStyle {get; set; }

        /// <summary>
        /// The properties of the top border of the object.
        /// </summary>
        /// <value>
        /// The border top.
        /// </value>
        public extern string BorderTop {get; set; }

        /// <summary>
        /// The color of the top border of the object.
        /// </summary>
        /// <value>
        /// The color of the border top.
        /// </value>
        public extern string BorderTopColor {get; set; }

        /// <summary>
        /// The style of the top border of the object.
        /// </summary>
        /// <value>
        /// The border top style.
        /// </value>
        public extern string BorderTopStyle {get; set; }

        /// <summary>
        /// The width of the top border of the object.
        /// </summary>
        /// <value>
        /// The width of the border top.
        /// </value>
        public extern string BorderTopWidth {get; set; }

        /// <summary>
        /// The width of the borders of the object.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        public extern string BorderWidth {get; set; }

        /// <summary>
        /// The bottom position of the object in relation to the bottom of the next positioned object.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public extern string Bottom {get; set; }

        /// <summary>
        /// Whether the object allows floating objects on its left side, right side, or both, so that the
        /// next text displays past the floating objects.
        /// </summary>
        /// <value>
        /// The clear.
        /// </value>
        public extern string Clear {get; set; }

        /// <summary>
        /// Which part of a positioned object is visible.
        /// </summary>
        /// <value>
        /// The clip.
        /// </value>
        public extern string Clip {get; set; }

        /// <summary>
        /// The color of the text of the object.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public extern string Color {get; set; }

        /// <summary>
        /// The side of the object the text will flow.
        /// </summary>
        /// <value>
        /// The css float.
        /// </value>
        public extern string CssFloat {get; set; }

        /// <summary>
        /// The persisted representation of this style.
        /// </summary>
        /// <value>
        /// The css text.
        /// </value>
        public extern string CssText {get; set; }

        /// <summary>
        /// The type of cursor to display as the mouse pointer moves over the object.
        /// </summary>
        /// <value>
        /// The cursor.
        /// </value>
        public extern string Cursor {get; set; }

        /// <summary>
        /// The reading order of content within the object.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public extern string Direction {get; set; }

        /// <summary>
        /// Whether the object is rendered.
        /// </summary>
        /// <value>
        /// The display.
        /// </value>
        public extern string Display {get; set; }

        /// <summary>
        /// The collection of filters applied to an object. (Specific to Internet Explorer)
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public extern string Filter {get; set; }

        /// <summary>
        /// The font properties of the object or one or more of six user-preference fonts.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public extern string Font {get; set; }

        /// <summary>
        /// The name of the font used for text in the object.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public extern string FontFamily {get; set; }

        /// <summary>
        /// The font size used for text in the object.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public extern string FontSize {get; set; }

        /// <summary>
        /// The font style of the object as italic, normal, or oblique.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public extern string FontStyle {get; set; }

        /// <summary>
        /// Whether the text of the object is in small capital letters.
        /// </summary>
        /// <value>
        /// The font variant.
        /// </value>
        public extern string FontVariant {get; set; }

        /// <summary>
        /// The weight of the font of the object.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public extern string FontWeight {get; set; }

        /// <summary>
        /// The height of the object.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public extern string Height {get; set; }

        /// <summary>
        /// The position of the object relative to the left edge of the next positioned object in the
        /// document hierarchy.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public extern string Left {get; set; }

        /// <summary>
        /// The amount of additional space between letters in the object.
        /// </summary>
        /// <value>
        /// The letter spacing.
        /// </value>
        public extern string LetterSpacing {get; set; }

        /// <summary>
        /// The distance between lines in the object.
        /// </summary>
        /// <value>
        /// The height of the line.
        /// </value>
        public extern string LineHeight {get; set; }

        /// <summary>
        /// The listStyle properties of the object.
        /// </summary>
        /// <value>
        /// The list style.
        /// </value>
        public extern string ListStyle {get; set; }

        /// <summary>
        /// The image to use as a list-item marker for the object.
        /// </summary>
        /// <value>
        /// The list style image.
        /// </value>
        public extern string ListStyleImage {get; set; }

        /// <summary>
        /// SHow the list-item marker is drawn relative to the content of the object.
        /// </summary>
        /// <value>
        /// The list style position.
        /// </value>
        public extern string ListStylePosition {get; set; }

        /// <summary>
        /// The type of the list-item marker for the object.
        /// </summary>
        /// <value>
        /// The type of the list style.
        /// </value>
        public extern string ListStyleType {get; set; }

        /// <summary>
        /// The width of the top, right, bottom, and left margins of the object.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        public extern string Margin {get; set; }

        /// <summary>
        /// The height of the bottom margin of the object.
        /// </summary>
        /// <value>
        /// The margin bottom.
        /// </value>
        public extern string MarginBottom {get; set; }

        /// <summary>
        /// The width of the left margin of the object.
        /// </summary>
        /// <value>
        /// The margin left.
        /// </value>
        public extern string MarginLeft {get; set; }

        /// <summary>
        /// The width of the right margin of the object.
        /// </summary>
        /// <value>
        /// The margin right.
        /// </value>
        public extern string MarginRight {get; set; }

        /// <summary>
        /// The height of the top margin of the object.
        /// </summary>
        /// <value>
        /// The margin top.
        /// </value>
        public extern string MarginTop {get; set; }

        /// <summary>
        /// The maximum height for displayable block level elements.
        /// </summary>
        /// <value>
        /// The height of the maximum.
        /// </value>
        public extern string MaxHeight {get; set; }

        /// <summary>
        /// The maximum width for displayable block level elements.
        /// </summary>
        /// <value>
        /// The width of the maximum.
        /// </value>
        public extern string MaxWidth {get; set; }

        /// <summary>
        /// The minimum height for an element.
        /// </summary>
        /// <value>
        /// The height of the minimum.
        /// </value>
        public extern string MinHeight {get; set; }

        /// <summary>
        /// The minimum width for displayable block level element.
        /// </summary>
        /// <value>
        /// The width of the minimum.
        /// </value>
        public extern string MinWidth {get; set; }

        /// <summary>
        /// The interpolation (resampling) method used to stretch images. (Specific to Internet Explorer)
        /// </summary>
        /// <value>
        /// The milliseconds interpolation mode.
        /// </value>
        public extern string MsInterpolationMode {get; set; }

        /// <summary>
        /// How to blend the object into the rendering.
        /// </summary>
        /// <value>
        /// The opacity.
        /// </value>
        public extern string Opacity {get; set; }

        /// <summary>
        /// How to manage the content of the object when the content exceeds the height or width of the
        /// object.
        /// </summary>
        /// <value>
        /// The overflow.
        /// </value>
        public extern string Overflow {get; set; }

        /// <summary>
        /// How to manage the content of the object when the content exceeds the width of the object.
        /// </summary>
        /// <value>
        /// The overflow x coordinate.
        /// </value>
        public extern string OverflowX {get; set; }

        /// <summary>
        /// How to manage the content of the object when the content exceeds the height of the object.
        /// </summary>
        /// <value>
        /// The overflow y coordinate.
        /// </value>
        public extern string OverflowY {get; set; }

        /// <summary>
        /// The amount of space to insert between the object and its margin or, if there is a border,
        /// between the object and its border.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        public extern string Padding {get; set; }

        /// <summary>
        /// The amount of space to insert between the bottom border of the object and the content.
        /// </summary>
        /// <value>
        /// The padding bottom.
        /// </value>
        public extern string PaddingBottom {get; set; }

        /// <summary>
        /// The amount of space to insert between the left border of the object and the content.
        /// </summary>
        /// <value>
        /// The padding left.
        /// </value>
        public extern string PaddingLeft {get; set; }

        /// <summary>
        /// The amount of space to insert between the right border of the object and the content.
        /// </summary>
        /// <value>
        /// The padding right.
        /// </value>
        public extern string PaddingRight {get; set; }

        /// <summary>
        /// The amount of space to insert between the top border of the object and the content.
        /// </summary>
        /// <value>
        /// The padding top.
        /// </value>
        public extern string PaddingTop {get; set; }

        /// <summary>
        /// Whether a page break occurs after the object.
        /// </summary>
        /// <value>
        /// The page break after.
        /// </value>
        public extern string PageBreakAfter {get; set; }

        /// <summary>
        /// Whether a page break occurs before the object.
        /// </summary>
        /// <value>
        /// The page break before.
        /// </value>
        public extern string PageBreakBefore {get; set; }

        /// <summary>
        /// The bottom position of the object.
        /// </summary>
        /// <value>
        /// The pixel bottom.
        /// </value>
        public extern int PixelBottom {get; set; }

        /// <summary>
        /// The height of the object.
        /// </summary>
        /// <value>
        /// The height of the pixel.
        /// </value>
        public extern int PixelHeight {get; set; }

        /// <summary>
        /// The left position of the object.
        /// </summary>
        /// <value>
        /// The pixel left.
        /// </value>
        public extern int PixelLeft {get; set; }

        /// <summary>
        /// The right position of the object.
        /// </summary>
        /// <value>
        /// The pixel right.
        /// </value>
        public extern int PixelRight {get; set; }

        /// <summary>
        /// The top position of the object.
        /// </summary>
        /// <value>
        /// The pixel top.
        /// </value>
        public extern int PixelTop {get; set; }

        /// <summary>
        /// The width of the object.
        /// </summary>
        /// <value>
        /// The width of the pixel.
        /// </value>
        public extern int PixelWidth {get; set; }

        /// <summary>
        /// The bottom position of the object in the units specified by the bottom attribute.
        /// </summary>
        /// <value>
        /// The position bottom.
        /// </value>
        public extern int PosBottom {get; set; }

        /// <summary>
        /// The height of the object in the units specified by the height attribute.
        /// </summary>
        /// <value>
        /// The height of the position.
        /// </value>
        public extern int PosHeight {get; set; }

        /// <summary>
        /// The type of positioning used for the object.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public extern string Position {get; set; }

        /// <summary>
        /// The left position of the object in the units specified by the left attribute.
        /// </summary>
        /// <value>
        /// The position left.
        /// </value>
        public extern int PosLeft {get; set; }

        /// <summary>
        /// The right position of the object in the units specified by the right attribute.
        /// </summary>
        /// <value>
        /// The position right.
        /// </value>
        public extern int PosRight {get; set; }

        /// <summary>
        /// The top position of the object in the units specified by the top attribute.
        /// </summary>
        /// <value>
        /// The position top.
        /// </value>
        public extern int PosTop {get; set; }

        /// <summary>
        /// The width of the object in the units specified by the width attribute.
        /// </summary>
        /// <value>
        /// The width of the position.
        /// </value>
        public extern int PosWidth {get; set; }

        /// <summary>
        /// The position of the object relative to the right edge of the next positioned object in the
        /// document hierarchy.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public extern string Right {get; set; }

        /// <summary>
        /// The side of the object the text will flow.
        /// </summary>
        /// <value>
        /// The style float.
        /// </value>
        public extern string StyleFloat {get; set; }

        /// <summary>
        /// Whether the table layout is fixed.
        /// </summary>
        /// <value>
        /// The table layout.
        /// </value>
        public extern string TableLayout {get; set; }

        /// <summary>
        /// Whether The text in the object is left-aligned, right-aligned, centered, or justified.
        /// </summary>
        /// <value>
        /// The text align.
        /// </value>
        public extern string TextAlign {get; set; }

        /// <summary>
        /// Indicates whether the text in the object has blink, line-through, overline, or underline
        /// decorations.
        /// </summary>
        /// <value>
        /// The text decoration.
        /// </value>
        public extern string TextDecoration {get; set; }

        /// <summary>
        /// Whether the object's text "blinks.".
        /// </summary>
        /// <value>
        /// The text decoration blink.
        /// </value>
        public extern string TextDecorationBlink {get; set; }

        /// <summary>
        /// Whether the text in the object has a line drawn through it.
        /// </summary>
        /// <value>
        /// The text decoration line through.
        /// </value>
        public extern string TextDecorationLineThrough {get; set; }

        /// <summary>
        /// Whether the textDecoration property for the object has been set to none.
        /// </summary>
        /// <value>
        /// The text decoration none.
        /// </value>
        public extern string TextDecorationNone {get; set; }

        /// <summary>
        /// Whether the text in the object has a line drawn over it.
        /// </summary>
        /// <value>
        /// The text decoration overline.
        /// </value>
        public extern string TextDecorationOverline {get; set; }

        /// <summary>
        /// Whether the text in the object is underlined.
        /// </summary>
        /// <value>
        /// The text decoration underline.
        /// </value>
        public extern string TextDecorationUnderline {get; set; }

        /// <summary>
        /// The indentation of the first line of text in the object.
        /// </summary>
        /// <value>
        /// The text indent.
        /// </value>
        public extern string TextIndent {get; set; }

        /// <summary>
        /// The type of alignment used to justify text in the object.
        /// </summary>
        /// <value>
        /// The text justify.
        /// </value>
        public extern string TextJustify {get; set; }

        /// <summary>
        /// Indicates whether to render ellipses(...) to indicate text overflow.
        /// </summary>
        /// <value>
        /// The text overflow.
        /// </value>
        public extern string TextOverflow {get; set; }

        /// <summary>
        /// The rendering of the text in the object.
        /// </summary>
        /// <value>
        /// The text transform.
        /// </value>
        public extern string TextTransform {get; set; }

        /// <summary>
        /// The position of the object relative to the top of the next positioned object in the document
        /// hierarchy.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public extern string Top {get; set; }

        /// <summary>
        /// The vertical alignment of the object.
        /// </summary>
        /// <value>
        /// The vertical align.
        /// </value>
        public extern string VerticalAlign {get; set; }

        /// <summary>
        /// Whether the content of the object is displayed.
        /// </summary>
        /// <value>
        /// The visibility.
        /// </value>
        public extern string Visibility {get; set; }

        /// <summary>
        /// Indicates whether lines are automatically broken inside the object.
        /// </summary>
        /// <value>
        /// The white space.
        /// </value>
        public extern string WhiteSpace {get; set; }

        /// <summary>
        /// The width of the object.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public extern string Width {get; set; }

        /// <summary>
        /// The amount of additional space between words in the object.
        /// </summary>
        /// <value>
        /// The word spacing.
        /// </value>
        public extern string WordSpacing {get; set; }

        /// <summary>
        /// Whether to break words when the content exceeds the boundaries of its container.
        /// </summary>
        /// <value>
        /// The word wrap.
        /// </value>
        public extern string WordWrap {get; set; }

        /// <summary>
        /// The direction and flow of the content in the object.
        /// </summary>
        /// <value>
        /// The writing mode.
        /// </value>
        public extern string WritingMode {get; set; }

        /// <summary>
        /// The stacking order of positioned objects.
        /// </summary>
        /// <value>
        /// The z coordinate index.
        /// </value>
        public extern short ZIndex {get; set; }

        /// <summary>
        /// The magnification scale of the object.
        /// </summary>
        /// <value>
        /// The zoom.
        /// </value>
        public extern string Zoom {get; set; }

        /// <summary>
        /// Indexer to get or set items within this collection using array index syntax.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        public extern string this[string name]
        {
            get;
            set;
        }
    }
}