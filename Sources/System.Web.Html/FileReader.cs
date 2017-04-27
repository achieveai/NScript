//-----------------------------------------------------------------------
// <copyright file="FileReader.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Values that represent FileReaderReadyState.
    /// </summary>
    public enum FileReaderReadyState
    {
        /// <summary>
        /// File reader is empty.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// File reader is loading.
        /// </summary>
        Loading = 1,

        /// <summary>
        /// File reader is done loading.
        /// </summary>
        Done = 2
    }

    /// <summary>
    /// Definition for FileReader.
    /// </summary>
    [IgnoreNamespace]
    public class FileReader
    {
        /// <summary>
        /// Gets the file reader.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern FileReader();

        [ScriptName("onabort")]
        /// <summary>
        /// Event queue for all listeners interested in OnAbort events.
        /// </summary>
        public extern Action<FileReader> OnAbort { get; set; }

        [ScriptName("onerror")]
        /// <summary>
        /// Event queue for all listeners interested in OnError events.
        /// </summary>
        public extern Action<FileReader, ErrorEvent> OnError { get; set; }

        [ScriptName("onload")]
        /// <summary>
        /// Event queue for all listeners interested in OnLoad events.
        /// </summary>
        public extern Action<FileReader> OnLoad { get; set; }

        [ScriptName("onloadend")]
        /// <summary>
        /// Event queue for all listeners interested in OnLoadEnd events.
        /// </summary>
        public extern Action<FileReader> OnLoadEnd { get; set; }

        [ScriptName("onloadstart")]
        /// <summary>
        /// Event queue for all listeners interested in OnLoadStart events.
        /// </summary>
        public extern Action<FileReader> OnLoadStart { get; set; }

        [ScriptName("onprogress")]
        /// <summary>
        /// Event queue for all listeners interested in OnProgress events.
        /// </summary>
        public extern Action<FileReader, ProgressEvent> OnProgress { get; set; }

        /// <summary>
        /// Aborts this object.
        /// </summary>
        public extern void Abort();

        /// <summary>
        /// Reads as array buffer.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsArrayBuffer(Blob file);

        /// <summary>
        /// Reads as binary string.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsBinaryString(Blob file);

        /// <summary>
        /// Reads as data URL.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsDataURL(Blob file);

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public extern object Result
        { get; }

        /// <summary>
        /// Gets the state of the ready.
        /// </summary>
        /// <value>
        /// The ready state.
        /// </value>
        public extern FileReaderReadyState ReadyState
        { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public extern DomError Error
        { get; }
    }
}
