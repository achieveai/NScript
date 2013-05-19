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

        /// <summary>
        /// Event queue for all listeners interested in OnAbort events.
        /// </summary>
        public event Action<FileReader> OnAbort;

        /// <summary>
        /// Event queue for all listeners interested in OnError events.
        /// </summary>
        public event Action<FileReader> OnError;

        /// <summary>
        /// Event queue for all listeners interested in OnLoad events.
        /// </summary>
        public event Action<FileReader> OnLoad;

        /// <summary>
        /// Event queue for all listeners interested in OnLoadEnd events.
        /// </summary>
        public event Action<FileReader> OnLoadEnd;

        /// <summary>
        /// Event queue for all listeners interested in OnLoadStart events.
        /// </summary>
        public event Action<FileReader> OnLoadStart;

        /// <summary>
        /// Event queue for all listeners interested in OnProgress events.
        /// </summary>
        public event Action<FileReader> OnProgress;

        /// <summary>
        /// Aborts this object.
        /// </summary>
        public extern void Abort();

        /// <summary>
        /// Reads as array buffer.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsArrayBuffer(File file);

        /// <summary>
        /// Reads as binary string.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsBinaryString(File file);

        /// <summary>
        /// Reads as data URL.
        /// </summary>
        /// <param name="file"> The file. </param>
        public extern void ReadAsDataURL(File file);

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
