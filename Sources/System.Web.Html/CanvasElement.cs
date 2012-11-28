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
    /// Definition for CanvasElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class CanvasElement : Element
    {
        /// <summary>
        /// Gets the canvas element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern CanvasElement();

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public extern int Height
        { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public extern int Width
        { get; set; }

        /// <summary>
        /// Gets a context.
        /// </summary>
        /// <param name="contextID"> Identifier for the context. </param>
        /// <returns>
        /// The context.
        /// </returns>
        public extern CanvasContext GetContext(string contextID);

        /// <summary>
        /// Gets a context.
        /// </summary>
        /// <param name="renderingType"> Type of the rendering. </param>
        /// <returns>
        /// The context.
        /// </returns>
        public extern CanvasContext GetContext(Rendering renderingType);

        /// <summary>
        /// Gets the data url.
        /// </summary>
        /// <returns>
        /// The data url.
        /// </returns>
        [ScriptName("toDataURL")]
        public extern string GetDataUrl();

        /// <summary>
        /// Gets a data url.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        /// The data url.
        /// </returns>
        [ScriptName("toDataURL")]
        public extern string GetDataUrl(string type);

        /// <summary>
        /// Gets a data url.
        /// </summary>
        /// <param name="type">          The type. </param>
        /// <param name="typeArguments"> A variable-length parameters list containing type arguments. </param>
        /// <returns>
        /// The data url.
        /// </returns>
        [ScriptName("toDataURL")]
        public extern string GetDataUrl(string type, params object[] typeArguments);
    }
}