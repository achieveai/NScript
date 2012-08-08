//-----------------------------------------------------------------------
// <copyright file="CanvasElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Web.Html.Graphics;

    /// <summary>
    /// Definition for CanvasElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class CanvasElement
    {
        private CanvasElement() { }

        [IntrinsicField]
        public int Height;

        [IntrinsicField]
        public int Width;

        public extern CanvasContext GetContext(string contextID);

        public extern CanvasContext GetContext(Rendering renderingType);

        [ScriptName("toDataURL")]
        public extern string GetDataUrl();

        [ScriptName("toDataURL")]
        public extern string GetDataUrl(string type);

        [ScriptName("toDataURL")]
        public extern string GetDataUrl(string type, params object[] typeArguments);
    }
}