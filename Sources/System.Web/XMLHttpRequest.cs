//-----------------------------------------------------------------------
// <copyright file="Class.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Values that represent ReadyState.
    /// </summary>
    public enum ReadyState
    {
        /// <summary>
        /// .
        /// </summary>
        Unsent = 0,
        /// <summary>
        /// .
        /// </summary>
        Opened = 1,
        /// <summary>
        /// .
        /// </summary>
        HeadersReceived = 2,
        /// <summary>
        /// .
        /// </summary>
        Loading = 3,
        /// <summary>
        /// .
        /// </summary>
        Done = 4
    }

    /// <summary>
    /// Progress event.
    /// </summary>
    [PsudoType, IgnoreNamespace]
    public class ProgressEvent
    {
        /// <summary>
        /// The length computable.
        /// </summary>
        [IntrinsicField]
        public readonly bool LengthComputable;

        /// <summary>
        /// The loaded.
        /// </summary>
        [IntrinsicField]
        public readonly long Loaded;

        /// <summary>
        /// Number of.
        /// </summary>
        [IntrinsicField]
        public readonly long Total;
    }

    /// <summary>
    /// Definition for Class.
    /// </summary>
    [Extended, IgnoreNamespace, ScriptName("XMLHttpRequest")]
    public class XMLHttpRequest
    {
        /// <summary>
        /// State of the ready.
        /// </summary>
        [IntrinsicField]
        public readonly ReadyState ReadyState;

        /// <summary>
        /// The response text.
        /// </summary>
        [IntrinsicField]
        public readonly string ResponseText;

        /// <summary>
        /// The status.
        /// </summary>
        [IntrinsicField]
        public readonly short Status;

        /// <summary>
        /// The status text.
        /// </summary>
        [IntrinsicField]
        public readonly string StatusText;

        /// <summary>
        /// The timeout.
        /// </summary>
        [IntrinsicField]
        public int Timeout;

        /// <summary>
        /// The on read sate change.
        /// </summary>
        [IntrinsicField]
        public Action OnReadSateChange;

        /// <summary>
        /// The data cache.
        /// </summary>
        private DomDataCache<ProgressEvent> dataCache;

        /// <summary>
        /// Gets the given document.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <param name="cb">  The cb. </param>
        public static void Get(
            string url,
            Action<string, short, bool> cb,
            string acceptType = "text/*",
            string[] headerPair = null,
            int timeout = -1)
        {
            var request = new XMLHttpRequest();
            request.Open("GET", url, true);
            request.SetRequestHeader("Accept", acceptType);
            if (headerPair != null)
            {
                for (int iHeader = 0; iHeader < headerPair.Length - 1; iHeader+=2)
                {
                    request.SetRequestHeader(headerPair[iHeader], headerPair[iHeader + 1]);
                }
            }

            request.Bind(
                "load",
                (e) =>
            {
                if (cb != null)
                {
                    cb(request.ResponseText, request.Status, false);
                }
            });

            request.Bind(
                "error",
                (e) =>
                {
                    if (cb != null)
                    {
                        cb(null, request.Status, true);
                    }
                });

            request.Send();
        }

        /// <summary>
        /// Post this message.
        /// </summary>
        /// <param name="url">         URL of the document. </param>
        /// <param name="cb">          The cb. </param>
        /// <param name="contentType"> Type of the content. </param>
        /// <param name="data">        The name of the event such as 'load'. </param>
        public static void Post(
            string url,
            Action<string, short, bool> cb,
            string contentType,
            string data,
            string acceptType = "text/*",
            string[] headerPair = null)
        {
            var request = new XMLHttpRequest();
            request.Open("POST", url, true);
            request.SetRequestHeader("Content-Type", contentType);
            request.SetRequestHeader("Accept", acceptType);
            if (headerPair != null)
            {
                for (int iHeader = 0; iHeader < headerPair.Length - 1; iHeader+=2)
                {
                    request.SetRequestHeader(headerPair[iHeader], headerPair[iHeader + 1]);
                }
            }

            request.Bind(
                "load",
                (e) =>
            {
                if (cb != null)
                {
                    cb(request.ResponseText, request.Status, false);
                }
            });

            request.Bind(
                "error",
                (e) =>
                {
                    if (cb != null)
                    {
                        cb(null, request.Status, true);
                    }
                });

            request.Send(data);
        }

        /// <summary>
        /// Aborts this object.
        /// </summary>
        /// ### <param name="eventName"> The name of the event such as 'load'. </param>
        public extern void Abort();

        /// <summary>
        /// Gets all response headers.
        /// </summary>
        /// <returns>
        /// all response headers.
        /// </returns>
        /// ### <param name="eventName"> The name of the event such as 'load'. </param>
        public extern string GetAllResponseHeaders();

        /// <summary>
        /// Gets a response header.
        /// </summary>
        /// <param name="header"> The name of the event such as 'load'. </param>
        /// <returns>
        /// The response header.
        /// </returns>
        public extern string GetResponseHeader(string header);

        /// <summary>
        /// Opens.
        /// </summary>
        /// <param name="method"> The name of the event such as 'load'. </param>
        /// <param name="url">    URL of the document. </param>
        public extern void Open(
            string method,
            string url);

        /// <summary>
        /// Opens.
        /// </summary>
        /// <param name="method"> The name of the event such as 'load'. </param>
        /// <param name="url">    URL of the document. </param>
        /// <param name="async">  true to asynchronous. </param>
        public extern void Open(
            string method,
            string url,
            bool async);

        /// <summary>
        /// Opens.
        /// </summary>
        /// <param name="method">   The name of the event such as 'load'. </param>
        /// <param name="url">      URL of the document. </param>
        /// <param name="async">    true to asynchronous. </param>
        /// <param name="userName"> Name of the user. </param>
        /// <param name="password"> The password. </param>
        public extern void Open(
            string method,
            string url,
            bool async,
            string userName,
            string password);

        /// <summary>
        /// Send this message.
        /// </summary>
        /// ### <param name="eventName"> The name of the event such as 'load'. </param>
        public extern void Send();

        /// <summary>
        /// Send this message.
        /// </summary>
        /// <param name="data"> The name of the event such as 'load'. </param>
        public extern void Send(string data);

        /// <summary>
        /// Sets a request header.
        /// </summary>
        /// <param name="header"> The name of the event such as 'load'. </param>
        /// <param name="value">  The value. </param>
        public extern void SetRequestHeader(string header, string value);

        /// <summary>
        /// Binds.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        public void Bind(string eventName, Action<ProgressEvent> handler)
        {
            if (object.IsNullOrUndefined(this.dataCache))
            {
                this.dataCache = new DomDataCache<ProgressEvent>(this);
            }

            this.dataCache.AddEvent(eventName, handler);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        public void UnBind(string eventName, Action<ProgressEvent> handler)
        {
            if (!object.IsNullOrUndefined(this.dataCache))
            {
                dataCache.RemoveEvent(eventName, handler);
            }
        }

        /// <summary>
        /// Adds a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        internal extern void AddEventListener(string eventName, Action<ProgressEvent> listener, bool useCapture);

        /// <summary>
        /// Removes a listener for the specified event.
        /// </summary>
        /// <param name="eventName">  The name of the event such as 'load'. </param>
        /// <param name="listener">   The listener to be invoked in response to the event. </param>
        /// <param name="useCapture"> Whether the listener wants to initiate capturing the event. </param>
        internal extern void RemoveEventListener(string eventName, Action<ProgressEvent> listener, bool useCapture);
    }
}
