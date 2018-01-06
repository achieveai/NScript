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

        public extern string AlignContent { get; set; }

        public extern string AlignItems { get; set; }

        public extern string AlignSelf { get; set; }

        public extern string AlignmentBaseline { get; set; }

        public extern string All { get; set; }

        public extern string Animation { get; set; }

        public extern string AnimationDelay { get; set; }

        public extern string AnimationDirection { get; set; }

        public extern string AnimationDuration { get; set; }

        public extern string AnimationFillMode { get; set; }

        public extern string AnimationIterationCount { get; set; }

        public extern string AnimationName { get; set; }

        public extern string AnimationPlayState { get; set; }

        public extern string AnimationTimingFunction { get; set; }

        public extern string BackfaceVisibility { get; set; }
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

        public extern string BackgroundClip {get; set; }

        /// <summary>
        /// The background image of the object.
        /// </summary>
        /// <value>
        /// The background image.
        /// </value>
        public extern string BackgroundImage {get; set; }

        public extern string BackgroundOrigin { get; set; }

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

        public extern string BackgroundRepeatX { get; set; }

        public extern string BackgroundRepeatY { get; set; }

        public extern string BackgroundSize { get; set; }

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

        public extern string BorderBottomLeftRadius { get; set; }

        public extern string BorderBottomRightRadius { get; set; }

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

        public extern string BorderImage {get; set; }

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

        public extern string BorderRadius { get; set; }

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

        public extern string BorderSpacing { get; set; }

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

        public extern string BorderTopLeftRadius { get; set; }

        public extern string BorderTopRightRadius { get; set; }

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

        public extern string BoxShadow { get; set; }

        public extern string BoxSizing { get; set; }

        public extern string BreakAfter { get; set; }

        public extern string BreakBefore { get; set; }

        public extern string BreakInside { get; set; }

        public extern string BufferedRendering { get; set; }

        public extern string CaptionSide { get; set; }

        public extern string CaretColor { get; set; }

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

        public extern string ClipPath { get; set; }

        public extern string ClipRule { get; set; }

        /// <summary>
        /// The color of the text of the object.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public extern string Color {get; set; }

        public extern string ColorInterpolation { get; set; }

        public extern string ColorInterpolationFilters { get; set; }

        public extern string ColorRendering { get; set; }

        public extern string ColumnCount { get; set; }

        public extern string ColumnFill { get; set; }

        public extern string ColumnGap { get; set; }

        public extern string ColumnRule { get; set; }

        public extern string ColumnRuleColor { get; set; }

        public extern string ColumnRuleStyle { get; set; }

        public extern string ColumnRuleWidth { get; set; }

        public extern string ColumnSpan { get; set; }

        public extern string ColumnWidth { get; set; }

        public extern string Columns { get; set; }

        public extern string Contain { get; set; }

        public extern string Content { get; set; }

        public extern string CounterIncrement { get; set; }

        public extern string CounterReset { get; set; }

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

        [ScriptName("cx")]
        public extern string SvgCenterX { get; set; }

        [ScriptName("cy")]
        public extern string SvgCenterY { get; set; }

        [ScriptName("d")]
        public extern string SvgPath { get; set; }

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

        public extern string DomainantBaseline { get; set; }

        public extern string EmptyCells { get; set; }

        public extern string Fill { get; set; }

        public extern string FillOpacity { get; set; }

        public extern string FillRule { get; set; }

        /// <summary>
        /// The collection of filters applied to an object. (Specific to Internet Explorer)
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public extern string Filter {get; set; }

        public extern string Flex { get; set; }

        public extern string FlexBasis { get; set; }

        public extern string FlexDirection { get; set; }

        public extern string FlexFlow { get; set; }

        public extern string FlexGrow { get; set; }

        public extern string FlexShrink { get; set; }

        public extern string FlexWrap { get; set; }

        public extern string Float { get; set; }

        public extern string FloodColor { get; set; }

        public extern string FloodOpacity { get; set; }

        /// <summary>
        /// The font properties of the object or one or more of six user-preference fonts.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public extern string Font {get; set; }

        public extern string FontDisplay { get; set; }

        /// <summary>
        /// The name of the font used for text in the object.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
        public extern string FontFamily {get; set; }

        public extern string FontFeatureSettings { get; set; }

        public extern string FontKerning { get; set; }

        /// <summary>
        /// The font size used for text in the object.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        public extern string FontSize {get; set; }

        public extern string FontStretch { get; set; }

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

        public extern string FontVariantCaps { get; set; }

        public extern string FontVariantEastAsian { get; set; }

        public extern string FontVariantLigatures { get; set; }

        public extern string FontVariantNumeric { get; set; }

        public extern string FontVariationSettings { get; set; }

        /// <summary>
        /// The weight of the font of the object.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public extern string FontWeight {get; set; }

        public extern string Grid { get; set; }

        public extern string GridArea { get; set; }

        public extern string GridAutoColumns { get; set; }

        public extern string GridAutoFlow { get; set; }

        public extern string GridAutoRows { get; set; }

        public extern string GridColumn { get; set; }

        public extern string GridColumnEnd { get; set; }

        public extern string GridColumnGap { get; set; }

        public extern string GridColumnStart { get; set; }

        public extern string GridGap { get; set; }

        public extern string GridRow { get; set; }

        public extern string GridRowEnd { get; set; }

        public extern string GridRowGap { get; set; }

        public extern string GridRowStart { get; set; }

        public extern string GridTemplate { get; set; }

        public extern string GridTemplateAreas { get; set; }

        public extern string GridTemplateColumns { get; set; }

        public extern string GridTemplateRows { get; set; }

        /// <summary>
        /// The height of the object.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public extern string Height {get; set; }

        public extern string Hyphens { get; set; }

        public extern string ImageRendering { get; set; }

        public extern string InlineSize { get; set; }

        public extern string Isolation { get; set; }

        public extern string JustifyContent { get; set; }

        public extern string JustifyItems { get; set; }

        public extern string JustifySelf { get; set; }

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

        public extern string LightingColor { get; set; }

        public extern string LineBreak { get; set; }

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

        public extern string Marker { get; set; }

        public extern string MarkerEnd { get; set; }

        public extern string MarkerMid { get; set; }

        public extern string MarkerStart { get; set; }

        public extern string Mask { get; set; }

        public extern string MaskType { get; set; }

        public extern string MaxBlockSize { get; set; }

        /// <summary>
        /// The maximum height for displayable block level elements.
        /// </summary>
        /// <value>
        /// The height of the maximum.
        /// </value>
        public extern string MaxHeight {get; set; }

        public extern string MaxInlineSize { get; set; }

        /// <summary>
        /// The maximum width for displayable block level elements.
        /// </summary>
        /// <value>
        /// The width of the maximum.
        /// </value>
        public extern string MaxWidth {get; set; }

        public extern string MaxZoom { get; set; }

        public extern string MinBlockSize { get; set; }

        /// <summary>
        /// The minimum height for an element.
        /// </summary>
        /// <value>
        /// The height of the minimum.
        /// </value>
        public extern string MinHeight {get; set; }

        public extern string MinInlineSize { get; set; }

        /// <summary>
        /// The minimum width for displayable block level element.
        /// </summary>
        /// <value>
        /// The width of the minimum.
        /// </value>
        public extern string MinWidth {get; set; }

        public extern string MinZoom { get; set; }

        public extern string MixBlendMode { get; set; }

        public extern string ObjectFit { get; set; }

        public extern string ObjectPosition { get; set; }

        public extern string Offset { get; set; }

        public extern string OffsetDistance { get; set; }

        public extern string OffsetPath { get; set; }

        public extern string OffsetRotate { get; set; }

        /// <summary>
        /// How to blend the object into the rendering.
        /// </summary>
        /// <value>
        /// The opacity.
        /// </value>
        public extern string Opacity {get; set; }

        public extern string Order { get; set; }

        public extern string Orientation { get; set; }

        public extern string Orphans { get; set; }

        public extern string Outline { get; set; }

        public extern string OutlineColor { get; set; }

        public extern string OutlineOffset { get; set; }

        public extern string OutlineStyle { get; set; }

        public extern string OutlineWidth { get; set; }

        /// <summary>
        /// How to manage the content of the object when the content exceeds the height or width of the
        /// object.
        /// </summary>
        /// <value>
        /// The overflow.
        /// </value>
        public extern string Overflow {get; set; }

        public extern string OverflowAnchor { get; set; }

        public extern string OverflowWrap { get; set; }

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

        public extern string OverflowBehavior { get; set; }

        public extern string OverflowBehaviorX { get; set; }

        public extern string OverflowBehaviorY { get; set; }

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

        public extern string Page { get; set; }

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

        public extern string PageBreakInside { get; set; }

        public extern string PaintOrder { get; set; }

        public extern string ParentRule { get; set; }

        public extern string Prespective { get; set; }

        public extern string PrespectiveOrigin { get; set; }

        public extern string PlaceContent { get; set; }

        public extern string PlaceItems { get; set; }

        public extern string PlaceSelf { get; set; }

        public extern string PointerEvents { get; set; }

        public extern string Position { get; set; }

        public extern string Quotes { get; set; }

        [ScriptName("r")]
        public extern string SvgRadius { get; set; }

        public extern string Resize { get; set; }

        /// <summary>
        /// The position of the object relative to the right edge of the next positioned object in the
        /// document hierarchy.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public extern string Right {get; set; }

        [ScriptName("rx")]
        public extern string SvgRadiusX { get; set; }

        [ScriptName("rx")]
        public extern string SvgRadiusY { get; set; }

        public extern string ScrollBehavior { get; set; }

        public extern string ShapeImageThreashold { get; set; }

        public extern string ShapeMargin { get; set; }

        public extern string ShapeOutside { get; set; }

        public extern string ShapeRendering { get; set; }

        public extern string Size { get; set; }

        public extern string Speak { get; set; }

        public extern string Src { get; set; }

        public extern string StopColor { get; set; }

        public extern string StopOpacity { get; set; }

        public extern string Stroke { get; set; }

        public extern string StrokeDasharray { get; set; }

        public extern string StrokeDashoffset { get; set; }

        public extern string StrokeLinecap { get; set; }

        public extern string StrokeLineJoin { get; set; }

        public extern string StrokeMiterLimit { get; set; }

        public extern string StrokeOpacity { get; set; }

        public extern string StorkeWidth { get; set; }

        public extern string TabSize { get; set; }

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

        public extern string TextAlignLast { get; set; }

        public extern string TextAnchor { get; set; }

        public extern string TextCombineUpright { get; set; }

        /// <summary>
        /// Indicates whether the text in the object has blink, line-through, overline, or underline
        /// decorations.
        /// </summary>
        /// <value>
        /// The text decoration.
        /// </value>
        public extern string TextDecoration {get; set; }

        public extern string TextDecorationColor {get; set; }

        public extern string TextDecorationLine {get; set; }

        public extern string TextDecorationNone {get; set; }

        public extern string TextDecorationSkipInk { get; set; }

        public extern string TextDecorationStyle { get; set; }

        public extern string TextDecorationOverline {get; set; }

        public extern string TextDecorationUnderline {get; set; }

        public extern string TextIndent {get; set; }

        public extern string TextOrientation { get; set; }

        public extern string TextOverflow {get; set; }

        public extern string TextRendering { get; set; }

        public extern string TextShadow { get; set; }

        public extern string TextSizeAdjust { get; set; }

        public extern string TextTransform {get; set; }

        public extern string TextUnderlinePosition { get; set; }

        public extern string Top {get; set; }

        public extern string TouchAction
        { get; set; }

        public extern string Transform { get; set; }

        public extern string TransformBox { get; set; }

        public extern string TransformOrigin { get; set; }

        public extern string TransformStyle { get; set; }

        public extern string Transition { get; set; }

        public extern string TransitionDelay { get; set; }

        public extern string TransitionDuration { get; set; }

        public extern string TransitionProperty { get; set; }

        public extern string TransitionTimingFunction { get; set; }

        public extern string UnicodeBidi { get; set; }

        public extern string UnicodeRange { get; set; }

        public extern string UserSelect { get; set; }

        public extern string UserZoom { get; set; }

        public extern string VectorEffect { get; set; }

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

        public extern string WillChange { get; set; }

        public extern string WordBreak { get; set; }

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

        public extern string X { get; set; }

        public extern string Y { get; set; }

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

        public extern string this[int idx]
        { get; set; }

        public string X_Animation
        {
            get { return this.Animation; }
            set
            {
                this["webkitAnimation"] = value;
                this["mozAnimation"] = value;
                this.Animation = value;
            }
        }
        
        public string X_AnimationDelay
        {
            get { return this.AnimationDelay; }
            set
            {
                this["webkitAnimationDelay"] = value;
                this["mozAnimationDelay"] = value;
                this.AnimationDelay = value;
            }
        }
        
        public string X_AnimationDirection
        {
            get { return this.AnimationDirection; }
            set
            {
                this["webkitAnimationDirection"] = value;
                this["mozAnimationDirection"] = value;
                this.AnimationDirection = value;
            }
        }
        
        public string X_AnimationDuration
        {
            get { return this.AnimationDuration; }
            set
            {
                this["webkitAnimationDuration"] = value;
                this["mozAnimationDuration"] = value;
                this.AnimationDuration = value;
            }
        }
        
        public string X_AnimationFillMode
        {
            get { return this.AnimationFillMode; }
            set
            {
                this["webkitAnimationFillMode"] = value;
                this["mozAnimationFillMode"] = value;
                this.AnimationFillMode = value;
            }
        }
        
        public string X_AnimationIterationCount
        {
            get { return this.AnimationIterationCount; }
            set
            {
                this["webkitAnimationIterationCount"] = value;
                this["mozAnimationIterationCount"] = value;
                this.AnimationIterationCount = value;
            }
        }
        
        public string X_AnimationName
        {
            get { return this.AnimationName; }
            set
            {
                this["webkitAnimationName"] = value;
                this["mozAnimationName"] = value;
                this.AnimationName = value;
            }
        }
        
        public string X_AnimationPlayState
        {
            get { return this.AnimationPlayState; }
            set
            {
                this["webkitAnimationPlayState"] = value;
                this["mozAnimationPlayState"] = value;
                this.AnimationPlayState = value;
            }
        }
        
        public string X_AnimationTimingFunction
        {
            get { return this.AnimationTimingFunction; }
            set
            {
                this["webkitAnimationTimingFunction"] = value;
                this["mozAnimationTimingFunction"] = value;
                this.AnimationTimingFunction = value;
            }
        }
        
        public string X_BackfaceVisibility
        {
            get { return this.BackfaceVisibility; }
            set
            {
                this["webkitBackfaceVisibility"] = value;
                this["mozBackfaceVisibility"] = value;
                this.BackfaceVisibility = value;
            }
        }
        
        public string X_BackgroundClip
        {
            get { return this.BackgroundClip; }
            set
            {
                this["webkitBackgroundClip"] = value;
                this["mozBackgroundClip"] = value;
                this.BackgroundClip = value;
            }
        }
        
        public string X_BackgroundOrigin
        {
            get { return this.BackgroundOrigin; }
            set
            {
                this["webkitBackgroundOrigin"] = value;
                this["mozBackgroundOrigin"] = value;
                this.BackgroundOrigin = value;
            }
        }
        
        public string X_BackgroundSize
        {
            get { return this.BackgroundSize; }
            set
            {
                this["webkitBackgroundSize"] = value;
                this["mozBackgroundSize"] = value;
                this.BackgroundSize = value;
            }
        }
        
        public string X_BorderBottomLeftRadius
        {
            get { return this.BorderBottomLeftRadius; }
            set
            {
                this["webkitBorderBottomLeftRadius"] = value;
                this["mozBorderBottomLeftRadius"] = value;
                this.BorderBottomLeftRadius = value;
            }
        }
        
        public string X_BorderBottomRightRadius
        {
            get { return this.BorderBottomRightRadius; }
            set
            {
                this["webkitBorderBottomRightRadius"] = value;
                this["mozBorderBottomRightRadius"] = value;
                this.BorderBottomRightRadius = value;
            }
        }
        
        public string X_BorderImage
        {
            get { return this.BorderImage; }
            set
            {
                this["webkitBorderImage"] = value;
                this["mozBorderImage"] = value;
                this.BorderImage = value;
            }
        }
        
        public string X_BorderRadius
        {
            get { return this.BorderRadius; }
            set
            {
                this["webkitBorderRadius"] = value;
                this["mozBorderRadius"] = value;
                this.BorderRadius = value;
            }
        }
        
        public string X_BorderTopLeftRadius
        {
            get { return this.BorderTopLeftRadius; }
            set
            {
                this["webkitBorderTopLeftRadius"] = value;
                this["mozBorderTopLeftRadius"] = value;
                this.BorderTopLeftRadius = value;
            }
        }
        
        public string X_BorderTopRightRadius
        {
            get { return this.BorderTopRightRadius; }
            set
            {
                this["webkitBorderTopRightRadius"] = value;
                this["mozBorderTopRightRadius"] = value;
                this.BorderTopRightRadius = value;
            }
        }
        
        public string X_BoxSizing
        {
            get { return this.BoxSizing; }
            set
            {
                this["webkitBoxSizing"] = value;
                this["mozBoxSizing"] = value;
                this.BoxSizing = value;
            }
        }
        
        public string X_ColumnCount
        {
            get { return this.ColumnCount; }
            set
            {
                this["webkitColumnCount"] = value;
                this["mozColumnCount"] = value;
                this.ColumnCount = value;
            }
        }
        
        public string X_ColumnGap
        {
            get { return this.ColumnGap; }
            set
            {
                this["webkitColumnGap"] = value;
                this["mozColumnGap"] = value;
                this.ColumnGap = value;
            }
        }
        
        public string X_ColumnWidth
        {
            get { return this.ColumnWidth; }
            set
            {
                this["webkitColumnWidth"] = value;
                this["mozColumnWidth"] = value;
                this.ColumnWidth = value;
            }
        }
        
        public string X_ColumnRule
        {
            get { return this.ColumnRule; }
            set
            {
                this["webkitColumnRule"] = value;
                this["mozColumnRule"] = value;
                this.ColumnRule = value;
            }
        }
        
        public string X_ColumnRuleWidth
        {
            get { return this.ColumnRuleWidth; }
            set
            {
                this["webkitColumnRuleWidth"] = value;
                this["mozColumnRuleWidth"] = value;
                this.ColumnRuleWidth = value;
            }
        }
        
        public string X_ColumnRuleStyle
        {
            get { return this.ColumnRuleStyle; }
            set
            {
                this["webkitColumnRuleStyle"] = value;
                this["mozColumnRuleStyle"] = value;
                this.ColumnRuleStyle = value;
            }
        }
        
        public string X_ColumnRuleColor
        {
            get { return this.ColumnRuleColor; }
            set
            {
                this["webkitColumnRuleColor"] = value;
                this["mozColumnRuleColor"] = value;
                this.ColumnRuleColor = value;
            }
        }
        
        public string X_Columns
        {
            get { return this.Columns; }
            set
            {
                this["webkitColumns"] = value;
                this["mozColumns"] = value;
                this.Columns = value;
            }
        }
        
        public string X_ColumnSpan
        {
            get { return this.ColumnSpan; }
            set
            {
                this["webkitColumnSpan"] = value;
                this["mozColumnSpan"] = value;
                this.ColumnSpan = value;
            }
        }
        
        public string X_Filter
        {
            get { return this.Filter; }
            set
            {
                this["webkitFilter"] = value;
                this["mozFilter"] = value;
                this.Filter = value;
            }
        }
        
        public string X_GridColumn
        {
            get { return this.GridColumn; }
            set
            {
                this["webkitGridColumn"] = value;
                this["mozGridColumn"] = value;
                this.GridColumn = value;
            }
        }
        
        public string X_GridRow
        {
            get { return this.GridRow; }
            set
            {
                this["webkitGridRow"] = value;
                this["mozGridRow"] = value;
                this.GridRow = value;
            }
        }
        
        public string X_Hyphens
        {
            get { return this.Hyphens; }
            set
            {
                this["webkitHyphens"] = value;
                this["mozHyphens"] = value;
                this.Hyphens = value;
            }
        }
        
        public string X_Opacity
        {
            get { return this.Opacity; }
            set
            {
                this["webkitOpacity"] = value;
                this["mozOpacity"] = value;
                this.Opacity = value;
            }
        }
        
        public string X_ShapeOutside
        {
            get { return this.ShapeOutside; }
            set
            {
                this["webkitShapeOutside"] = value;
                this["mozShapeOutside"] = value;
                this.ShapeOutside = value;
            }
        }
        
        public string X_Transform
        {
            get { return this.Transform; }
            set
            {
                this["webkitTransform"] = value;
                this["mozTransform"] = value;
                this.Transform = value;
            }
        }
        
        public string X_TransformOrigin
        {
            get { return this.TransformOrigin; }
            set
            {
                this["webkitTransformOrigin"] = value;
                this["mozTransformOrigin"] = value;
                this.TransformOrigin = value;
            }
        }
        
        public string X_TransformStyle
        {
            get { return this.TransformStyle; }
            set
            {
                this["webkitTransformStyle"] = value;
                this["mozTransformStyle"] = value;
                this.TransformStyle = value;
            }
        }
        
        public string X_Transition
        {
            get { return this.Transition; }
            set
            {
                this["webkitTransition"] = value;
                this["mozTransition"] = value;
                this.Transition = value;
            }
        }
        
        public string X_TransitionDelay
        {
            get { return this.TransitionDelay; }
            set
            {
                this["webkitTransitionDelay"] = value;
                this["mozTransitionDelay"] = value;
                this.TransitionDelay = value;
            }
        }
        
        public string X_TransitionDuration
        {
            get { return this.TransitionDuration; }
            set
            {
                this["webkitTransitionDuration"] = value;
                this["mozTransitionDuration"] = value;
                this.TransitionDuration = value;
            }
        }
        
        public string X_TransitionProperty
        {
            get { return this.TransitionProperty; }
            set
            {
                this["webkitTransitionProperty"] = value;
                this["mozTransitionProperty"] = value;
                this.TransitionProperty = value;
            }
        }
        
        public string X_TransitionTimingFunction
        {
            get { return this.TransitionTimingFunction; }
            set
            {
                this["webkitTransitionTimingFunction"] = value;
                this["mozTransitionTimingFunction"] = value;
                this.TransitionTimingFunction = value;
            }
        }
        
    }
}