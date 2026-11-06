//-----------------------------------------------------------------------
// <copyright file="Clipboard.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// Async clipboard (<c>navigator.clipboard</c>). Calls need a secure context and,
    /// for reads, the user's permission; the task rejects when the browser refuses.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class Clipboard
    {
        private extern Clipboard();

        /// <summary>
        /// Writes text to the clipboard.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns>
        /// A task that completes when the text is written.
        /// </returns>
        public extern Task WriteText(string text);

        /// <summary>
        /// Reads text from the clipboard.
        /// </summary>
        /// <returns>
        /// A task with the clipboard text.
        /// </returns>
        public extern Task<string> ReadText();
    }
}
