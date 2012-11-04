//-----------------------------------------------------------------------
// <copyright file="CanvasContext2D.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Graphics
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for CanvasContext2D
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class CanvasContext2D : CanvasContext
    {
        private CanvasContext2D() { }

        [IntrinsicField]
        public double GlobalAlpha;

        [IntrinsicField]
        public object FillStyle;

        [IntrinsicField]
        public string Font;

        [IntrinsicField]
        public double LineWidth;

        [IntrinsicField]
        public int MiterLimit;

        [IntrinsicField]
        public double ShadowBlur;

        [IntrinsicField]
        public string ShadowColor;

        [IntrinsicField]
        public double ShadowOffsetX;

        [IntrinsicField]
        public double ShadowOffsetY;

        [IntrinsicField]
        public object StrokeStyle;

        [IntrinsicField]
        private string globalCompositeOperation;

        [IntrinsicField]
        private string textBaseline;

        [IntrinsicField]
        private string lineCap;

        [IntrinsicField]
        public string lineJoin;

        [IntrinsicField]
        private string textAlign;

        [MakeStaticUsage]
        public CompositeOperation CompositeOperation
        {
            get
            {
                switch (this.globalCompositeOperation)
                {
                    default:
                    case "copy":
                        return Graphics.CompositeOperation.Copy;
                    case "destination-atop":
                        return Graphics.CompositeOperation.DestinationAtop;
                    case "destination-in":
                        return Graphics.CompositeOperation.DestinationIn;
                    case "destination-out":
                        return Graphics.CompositeOperation.DestinationOut;
                    case "destination-over":
                        return Graphics.CompositeOperation.DestinationOver;
                    case "lighter":
                        return Graphics.CompositeOperation.Lighter;
                    case "source-atop":
                        return Graphics.CompositeOperation.SourceAtop;
                    case "source-in":
                        return Graphics.CompositeOperation.SourceIn;
                    case "source-out":
                        return Graphics.CompositeOperation.SourceOut;
                    case "source-over":
                        return Graphics.CompositeOperation.SourceOver;
                    case "xor":
                        return Graphics.CompositeOperation.Xor;
                }
            }

            set
            {
                switch (value)
                {
                    case CompositeOperation.Copy:
                        this.globalCompositeOperation = "copy";
                        break;
                    case CompositeOperation.DestinationAtop:
                        this.globalCompositeOperation = "destination-atop";
                        break;
                    case CompositeOperation.DestinationIn:
                        this.globalCompositeOperation = "destination-in";
                        break;
                    case CompositeOperation.DestinationOut:
                        this.globalCompositeOperation = "destination-out";
                        break;
                    case CompositeOperation.DestinationOver:
                        this.globalCompositeOperation = "destination-over";
                        break;
                    case CompositeOperation.Lighter:
                        this.globalCompositeOperation = "lighter";
                        break;
                    case CompositeOperation.SourceAtop:
                        this.globalCompositeOperation = "source-atop";
                        break;
                    case CompositeOperation.SourceIn:
                        this.globalCompositeOperation = "source-in";
                        break;
                    case CompositeOperation.SourceOut:
                        this.globalCompositeOperation = "source-out";
                        break;
                    case CompositeOperation.SourceOver:
                        this.globalCompositeOperation = "source-over";
                        break;
                    case CompositeOperation.Xor:
                        this.globalCompositeOperation = "xor";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the line cap.
        /// </summary>
        /// <value>
        /// The line cap.
        /// </value>
        [MakeStaticUsage]
        public LineCap LineCap
        {
            get
            {
                switch (this.lineCap)
                {
                    case "butt":
                        return LineCap.Butt;
                    case "round":
                        return LineCap.Round;
                    case "square":
                        return LineCap.Square;
                    default:
                        return LineCap.Butt;
                }
            }

            set
            {
                switch (value)
                {
                    case LineCap.Butt:
                        this.lineCap = "butt";
                        break;
                    case LineCap.Round:
                        this.lineCap = "round";
                        break;
                    case LineCap.Square:
                        this.lineCap = "square";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the line join.
        /// </summary>
        /// <value>
        /// The line join.
        /// </value>
        [MakeStaticUsage]
        public LineJoin LineJoin
        {
            get
            {
                switch (this.lineJoin)
                {
                    default:
                    case "miter":
                        return LineJoin.Miter;
                    case "round":
                        return LineJoin.Round;
                    case "bevel":
                        return LineJoin.Bevel;
                }
            }

            set
            {
                switch (value)
                {
                    case LineJoin.Miter:
                        this.lineCap = "miter";
                        break;
                    case LineJoin.Round:
                        this.lineCap = "round";
                        break;
                    case LineJoin.Bevel:
                        this.lineCap = "bevel";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text align.
        /// </summary>
        /// <value>
        /// The text align.
        /// </value>
        [MakeStaticUsage]
        public TextAlign TextAlign
        {
            get
            {
                switch (this.textAlign)
                {
                    default:
                    case "start":
                        return TextAlign.Start;
                    case "end":
                        return TextAlign.End;
                    case "left":
                        return TextAlign.Left;
                    case "right":
                        return TextAlign.Right;
                    case "center":
                        return TextAlign.Center;
                }
            }

            set
            {
                switch (value)
                {
                    case TextAlign.Start:
                        this.textAlign = "start";
                        break;
                    case TextAlign.End:
                        this.textAlign = "end";
                        break;
                    case TextAlign.Left:
                        this.textAlign = "left";
                        break;
                    case TextAlign.Right:
                        this.textAlign = "right";
                        break;
                    case TextAlign.Center:
                        this.textAlign = "center";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text baseline.
        /// </summary>
        /// <value>
        /// The text baseline.
        /// </value>
        [MakeStaticUsage]
        public TextBaseline TextBaseline
        {
            get
            {
                switch (this.textBaseline)
                {
                    default:
                    case "top":
                        return TextBaseline.Top;
                    case "hanging":
                        return TextBaseline.Hanging;
                    case "middle":
                        return TextBaseline.Middle;
                    case "bottom":
                        return TextBaseline.Bottom;
                    case "alphabetic":
                        return TextBaseline.Alphabetic;
                    case "ideographic":
                        return TextBaseline.Ideographic;
                }
            }

            set
            {
                switch (value)
                {
                    case TextBaseline.Top:
                        this.textAlign = "top";
                        break;
                    case TextBaseline.Hanging:
                        this.textAlign = "hanging";
                        break;
                    case TextBaseline.Middle:
                        this.textAlign = "middle";
                        break;
                    case TextBaseline.Bottom:
                        this.textAlign = "bottom";
                        break;
                    case TextBaseline.Alphabetic:
                        this.textAlign = "alphabetic";
                        break;
                    case TextBaseline.Ideographic:
                        this.textAlign = "ideographic";
                        break;
                }
            }
        }

        public extern void Arc(double x, double y, double radius, double startAngle, double endAngle, bool anticlockwise);

        public extern void ArcTo(double x1, double y1, double x2, double y2, double radius);

        public extern void BeginPath();

        public extern void BezierCurveTo(double cp1x, double cp1y, double cp2x, double cp2y, double x, double y);

        public extern void ClearRect(double x, double y, double w, double h);

        public extern void Clip();

        public extern void ClosePath();

        public extern Gradient CreateLinearGradient(double x0, double y0, double x1, double y1);

        public extern Gradient CreateRadialGradient(double x0, double y0, double r0, double x1, double y1, double r1);

        public extern ImageData CreateImageData(double sw, double sh);

        public extern ImageData CreateImageData(ImageData imagedata);

        public extern Pattern CreatePattern(CanvasElement canvas, string repetition);

        public extern Pattern CreatePattern(ImageElement image, string repetition);

        public extern void DrawImage(ImageElement image, double dx, double dy);

        public extern void DrawImage(ImageElement image, double dx, double dy, double dw, double dh);

        public extern void DrawImage(ImageElement image, double sx, double sy, double sw, double sh, double dx, double dy, double dw, double dh);

        public extern void DrawImage(CanvasElement image, double dx, double dy);

        public extern void DrawImage(CanvasElement image, double dx, double dy, double dw, double dh);

        public extern void DrawImage(CanvasElement image, double sx, double sy, double sw, double sh, double dx, double dy, double dw, double dh);

        public extern void Fill();

        public extern void FillRect(double x, double y, double w, double h);

        public extern void FillText(string text, double x, double y);

        public extern void FillText(string text, double x, double y, double maxWidth);

        public extern ImageData GetImageData(double sx, double sy, double sw, double sh);

        public extern bool IsPointInPath(double x, double y);

        public extern void LineTo(double x, double y);

        public extern TextMetrics MeasureText(string text);

        public extern void MoveTo(double x, double y);

        public extern void PutImageData(ImageData imagedata, double dx, double dy);

        public extern void PutImageData(ImageData imagedata, double dx, double dy, double dirtyX, double dirtyY, double dirtyWidth, double dirtyHeight);

        public extern void QuadraticCurveTo(double cpx, double cpy, double x, double y);

        public extern void Rect(double x, double y, double w, double h);

        public extern void Restore();

        public extern void Rotate(double angle);

        public extern void Save();

        public extern void Scale(double x, double y);

        public extern void SetTransform(double m11, double m12, double m21, double m22, double dx, double dy);

        public extern void Stroke();

        public extern void StrokeRect(double x, double y, double w, double h);

        public extern void StrokeText(string text, double x, double y);

        public extern void StrokeText(string text, double x, double y, double maxWidth);

        public extern void Transform(double m11, double m12, double m21, double m22, double dx, double dy);

        public extern void Translate(double x, double y);
    }
}