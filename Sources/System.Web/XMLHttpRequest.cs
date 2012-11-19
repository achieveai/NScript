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
    [IgnoreNamespace]
    public class ProgressEvent
    {
        /// <summary>
        /// Gets the progress event.
        /// </summary>
        private extern ProgressEvent();

        /// <summary>
        /// The length computable.
        /// </summary>
        public extern bool LengthComputable { get; }

        /// <summary>
        /// The loaded.
        /// </summary>
        public extern long Loaded { get; }

        /// <summary>
        /// Number of.
        /// </summary>
        public extern long Total { get; }
    }

    /// <summary>
    /// Definition for Class.
    /// </summary>
    [IgnoreNamespace, ScriptName("XMLHttpRequest")]
    public class XMLHttpRequest
    {
        /// <summary>
        /// State of the ready.
        /// </summary>
        public extern ReadyState ReadyState { get; }

        /// <summary>
        /// The response text.
        /// </summary>
        public extern string ResponseText { get; }

        /// <summary>
        /// The status.
        /// </summary>
        public extern short Status { get; }

        /// <summary>
        /// The status text.
        /// </summary>
        public extern string StatusText { get; }

        /// <summary>
        /// The timeout.
        /// </summary>
        public extern int Timeout { get; set; }

        /// <summary>
        /// The on read sate change.
        /// </summary>
        public extern event Action<XMLHttpRequest, ProgressEvent> OnReadSateChange;

        public extern event Action<XMLHttpRequest, ProgressEvent> OnLoad;

        public extern event Action<XMLHttpRequest, ProgressEvent> OnError;

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

            if (cb != null)
            {
                request.OnLoad +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(request.ResponseText, request.Status, false);
                    };

                request.OnError +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(null, request.Status, true);
                    };
            }

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

            if (cb != null)
            {
                request.OnLoad +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(request.ResponseText, request.Status, false);
                    };

                request.OnError +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        cb(null, request.Status, true);
                    };
            }

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
        public void Bind(string eventName, Action<XMLHttpRequest, ProgressEvent> handler)
        {
            EventBinder.AddEvent(this, eventName, handler);
        }

        /// <summary>
        /// Un bind.
        /// </summary>
        /// <param name="eventName"> The name of the event such as 'load'. </param>
        /// <param name="handler">   The handler. </param>
        public void UnBind(string eventName, Action<XMLHttpRequest, ProgressEvent> handler)
        {
            EventBinder.RemoveEvent(this, eventName, handler);
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
