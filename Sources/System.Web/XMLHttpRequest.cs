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
        /// Type of the BLOB.
        /// </summary>
        public const string BlobType = "blob";

        /// <summary>
        /// Type of the array buffer.
        /// </summary>
        public const string ArrayBufferType = "arraybuffer";

        /// <summary>
        /// Type of the document.
        /// </summary>
        public const string DocumentType = "document";

        /// <summary>
        /// Type of the JSON.
        /// </summary>
        public const string JsonType = "json";

        /// <summary>
        /// Type of the text.
        /// </summary>
        public const string TextType = "text";

        /// <summary>
        /// State of the ready.
        /// </summary>
        public extern ReadyState ReadyState { get; }

        /// <summary>
        /// The response text.
        /// </summary>
        public extern string ResponseText { get; }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        /// <value>
        /// The type of the response.
        /// </value>
        public extern string ResponseType { get; set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public extern object Response { get; }

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

        /// <summary>
        /// Event queue for all listeners interested in OnLoad events.
        /// </summary>
        public extern event Action<XMLHttpRequest, ProgressEvent> OnLoad;

        /// <summary>
        /// Event queue for all listeners interested in OnError events.
        /// </summary>
        public extern event Action<XMLHttpRequest, ProgressEvent> OnError;

        /// <summary>
        /// Event queue for all listeners interested in OnTimeout events.
        /// </summary>
        public extern event Action<XMLHttpRequest, ProgressEvent> OnTimeout;

        /// <summary>
        /// Gets the given document.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <param name="cb">  The cb. </param>
        [IgnoreGenericArguments]
        public static void GetArrayBuffer<T>(
            string url,
            Action<NativeArray<T>, short, bool> cb,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout = -1)
        {
            XMLHttpRequest.GetRaw(
                url,
                (request, code, error) =>
                { if (cb != null) cb(error ? null : (NativeArray<T>)request.Response, code, error); },
                XMLHttpRequest.ArrayBufferType,
                acceptType,
                headerPair,
                timeout);
        }

        /// <summary>
        /// Gets array buffer promise.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="url">        URL of the document. </param>
        /// <param name="acceptType"> (optional) type of the accept. </param>
        /// <param name="headerPair"> (optional) the header pair. </param>
        /// <param name="timeout">    The timeout. </param>
        /// <returns>
        /// Promise object for NativeArray of T;
        /// </returns>
        public static Promise<NativeArray<T>> GetArrayBufferPromise<T>(
            string url,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout=-1)
        {
            return new Promise<NativeArray<T>>(
                delegate(Action<NativeArray<T>> resolve, Action<object> reject)
                {
                    XMLHttpRequest.GetArrayBuffer<T>(
                        url,
                        (arr, code, isError) =>
                        {
                            if (isError)
                            {
                                reject((int?)code);
                            }
                            else
                            {
                                resolve(arr);
                            }
                        },
                        acceptType,
                        headerPair,
                        timeout);
                });
        }

        /// <summary>
        /// Gets the given document.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <param name="cb">  The cb. </param>
        public static void GetBlob(
            string url,
            Action<Blob, short, bool> cb,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout = -1)
        {
            XMLHttpRequest.GetRaw(
                url,
                (request, code, error) =>
                { if (cb != null) cb(error ? null : (Blob)request.Response, code, error); },
                XMLHttpRequest.BlobType,
                acceptType,
                headerPair,
                timeout);
        }

        /// <summary>
        /// Gets BLOB promise.
        /// </summary>
        /// <param name="url">        URL of the document. </param>
        /// <param name="acceptType"> (optional) type of the accept. </param>
        /// <param name="headerPair"> (optional) the header pair. </param>
        /// <param name="timeout">    The timeout. </param>
        /// <returns>
        /// The BLOB promise.
        /// </returns>
        public static Promise<Blob> GetBlob(
            string url,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout=-1)
        {
            return new Promise<Blob>(
                delegate(Action<Blob> resolve, Action<object> reject)
                {
                    XMLHttpRequest.GetBlob(
                        url,
                        (arr, code, isError) =>
                        {
                            if (isError)
                            {
                                reject((int?)code);
                            }
                            else
                            {
                                resolve(arr);
                            }
                        },
                        acceptType,
                        headerPair,
                        timeout);
                });
        }

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
            XMLHttpRequest.GetRaw(
                url,
                (request, code, error) => { if (cb != null) cb(request.ResponseText, code, error); },
                null,
                acceptType,
                headerPair,
                timeout);
        }

        /// <summary>
        /// Submists a Get request.
        /// </summary>
        /// <param name="url">        URL of the document. </param>
        /// <param name="acceptType"> (optional) type of the accept. </param>
        /// <param name="headerPair"> (optional) the header pair. </param>
        /// <param name="timeout">    The timeout. </param>
        /// <returns>
        /// The promise.
        /// </returns>
        public static Promise<string> Get(
            string url,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout=-1)
        {
            return new Promise<string>(
                delegate(Action<string> resolve, Action<object> reject)
                {
                    XMLHttpRequest.Get(
                        url,
                        (arr, code, isError) =>
                        {
                            if (isError)
                            {
                                reject((int?)code);
                            }
                            else
                            {
                                resolve(arr);
                            }
                        },
                        acceptType,
                        headerPair,
                        timeout);
                });
        }

        /// <summary>
        /// Gets the given document.
        /// </summary>
        /// <param name="url"> URL of the document. </param>
        /// <param name="cb">  The cb. </param>
        public static void GetRaw(
            string url,
            Action<XMLHttpRequest, short, bool> cb,
            string responseType,
            string acceptType = "text/*",
            string[] headerPair = null,
            int timeout = -1)
        {
            var request = new XMLHttpRequest();
            request.Open("GET", url, true);
            request.SetRequestHeader("Accept", acceptType);
            if (timeout > 0)
            { request.Timeout = timeout; }

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
                        if (cb != null)
                        { cb(request, request.Status, request.Status >= 400); }
                    };

                Action<XMLHttpRequest, ProgressEvent> errorCb = 
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        if (cb != null)
                        { cb(request, request.Status, true); }
                    };

                request.OnError += errorCb;
                request.OnTimeout += errorCb;
            }

            if (!string.IsNullOrEmpty(responseType))
            { request.ResponseType = responseType; }

            request.Send();
        }

        /// <summary>
        /// Make a GET request to server.
        /// </summary>
        /// <param name="url">          URL of the document. </param>
        /// <param name="responseType"> The type of the response. </param>
        /// <param name="acceptType">   (optional) type of the accept. </param>
        /// <param name="headerPair">   (optional) the header pair. </param>
        /// <param name="timeout">      The timeout. </param>
        /// <returns>
        /// The Promise object.
        /// </returns>
        public static Promise<XMLHttpRequest> GetRaw(
            string url,
            string responseType,
            string acceptType = "*",
            string[] headerPair = null,
            int timeout=-1)
        {
            return new Promise<XMLHttpRequest>(
                delegate(Action<XMLHttpRequest> resolve, Action<object> reject)
                {
                    XMLHttpRequest.GetRaw(
                        url,
                        (arr, code, isError) =>
                        {
                            if (isError)
                            {
                                reject((int?)code);
                            }
                            else
                            {
                                resolve(arr);
                            }
                        },
                        responseType,
                        acceptType,
                        headerPair,
                        timeout);
                });
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
            object data,
            string acceptType = "text/*",
            string[] headerPair = null)
        {
            XMLHttpRequest.PostRaw(
                url,
                (req, status, isError) => cb(req.ResponseText, status, isError),
                contentType,
                data,
                acceptType,
                headerPair);
        }

        /// <summary>
        /// Post this message.
        /// </summary>
        /// <param name="url">         URL of the document. </param>
        /// <param name="cb">          The cb. </param>
        /// <param name="contentType"> Type of the content. </param>
        /// <param name="data">        The name of the event such as 'load'. </param>
        public static void PostRaw(
            string url,
            Action<XMLHttpRequest, short, bool> cb,
            string contentType,
            object data,
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
                        if (cb != null)
                        { cb(request, request.Status, request.Status >= 400); }
                    };

                request.OnError +=
                    (s, e) =>
                    {
                        EventBinder.CleanUp(request);
                        if (cb != null)
                        { cb(request, request.Status, true); }
                    };
            }

            request.Send(data);
        }

        /// <summary>
        /// Post data to server, return a promise.
        /// </summary>
        /// <param name="url">         URL of the document. </param>
        /// <param name="contentType"> Type of the content. </param>
        /// <param name="data">        The name of the event such as 'load'. </param>
        /// <param name="acceptType">  (optional) type of the accept. </param>
        /// <param name="headerPair">  (optional) the header pair. </param>
        /// <returns>
        /// Promise object
        /// </returns>
        public static Promise<string> Post(
            string url,
            string contentType,
            object data,
            string acceptType = "*",
            string[] headerPair = null)
        {
            return new Promise<string>(
                delegate(Action<string> resolve, Action<object> reject)
                {
                    XMLHttpRequest.Post(
                        url,
                        (arr, code, isError) =>
                        {
                            if (isError)
                            {
                                reject((int?)code);
                            }
                            else
                            {
                                resolve(arr);
                            }
                        },
                        contentType,
                        data,
                        acceptType,
                        headerPair);
                });
        }

        public static Promise<XMLHttpRequest> PostRaw(
            string url,
            string contentType,
            object data,
            string acceptType = "*",
            string[] headerPair = null)
        {
            return new Promise<XMLHttpRequest>(
                delegate(Action<XMLHttpRequest> resolve, Action<object> reject)
                {
                    XMLHttpRequest.PostRaw(
                        url,
                        (xmlHttpRequest, code, isError) =>
                        {
                            if (isError)
                            { reject((int?)code); }
                            else
                            { resolve(xmlHttpRequest); }
                        },
                        contentType,
                        data,
                        acceptType,
                        headerPair);
                });
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
        /// Send this message.
        /// </summary>
        /// <param name="data"> The name of the event such as 'load'. </param>
        public extern void Send(object data);

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
