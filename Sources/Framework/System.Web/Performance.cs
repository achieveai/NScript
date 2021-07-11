//-----------------------------------------------------------------------
// <copyright file="Performance.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Performance
    /// </summary>
    /// <remarks>
    /// Sample shape
    ///
    /// {
    ///      "onwebkitresourcetimingbufferfull":null,
    ///      "memory":{
    ///          "jsHeapSizeLimit":793000000,
    ///          "usedJSHeapSize":10000000,
    ///          "totalJSHeapSize":11900000
    ///      },
    ///      "timing":{
    ///          "loadEventEnd":1428266471688,
    ///          "loadEventStart":1428266471688,
    ///          "domComplete":1428266471688,
    ///          "domContentLoadedEventEnd":1428266471651,
    ///          "domContentLoadedEventStart":1428266471576,
    ///          "domInteractive":1428266471576,
    ///          "domLoading":1428266466276,
    ///          "responseEnd":1428266466275,
    ///          "responseStart":1428266466275,
    ///          "requestStart":1428266464318,
    ///          "secureConnectionStart":0,
    ///          "connectEnd":1428266464315,
    ///          "connectStart":1428266464315,
    ///          "domainLookupEnd":1428266464315,
    ///          "domainLookupStart":1428266464315,
    ///          "fetchStart":1428266464315,
    ///          "redirectEnd":0,
    ///          "redirectStart":0,
    ///          "unloadEventEnd":0,
    ///          "unloadEventStart":0,
    ///          "navigationStart":1428266464013,
    ///      },
    ///      "navigation":{
    ///          "redirectCount":0,
    ///          "type":0
    ///      }}

    /// Navigation.Type:
    ///
    /// Constant          Value   Description
    /// TYPE_NAVIGATENEXT    0   Navigation started by clicking on a link, or entering the URL in the user agent's address bar, or form submission, or initializing through a script operation other than the ones used by TYPE_RELOAD and TYPE_BACK_FORWARD as listed below.
    /// TYPE_RELOAD          1   Navigation through the reload operation or the location.reload() method.
    /// TYPE_BACK_FORWARD    2   Navigation through a history traversal operation.
    /// TYPE_UNDEFINED       255 Any navigation types not defined by values above.
    ///
    /// </remarks>
    [IgnoreNamespace, ScriptName("Performance")]
    public class Performance
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private extern Performance();

        /// <summary>
        /// Gets the timing.
        /// </summary>
        /// <value>
        /// The timing.
        /// </value>
        public extern PerformanceTiming Timing
        { get; }

        /// <summary>
        /// Gets the navigation.
        /// </summary>
        /// <value>
        /// The navigation.
        /// </value>
        public extern Navigation Navigation
        { get; }

        /// <summary>
        /// Gets the memory.
        /// </summary>
        /// <value>
        /// The memory.
        /// </value>
        public extern BrowserMemory Memory
        { get; }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <returns>
        /// The entries.
        /// </returns>
        public extern PerformanceResourceTiming[] GetEntries();

        /// <summary>
        /// Gets entries by name.
        /// </summary>
        /// <returns>
        /// The entries by name.
        /// </returns>
        public extern PerformanceResourceTiming[] GetEntriesByName();

        /// <summary>
        /// Gets entries by type.
        /// </summary>
        /// <returns>
        /// The entries by type.
        /// </returns>
        public extern PerformanceResourceTiming[] GetEntriesByType();
    }

    /// <summary>
    /// Performance timing.
    /// </summary>
    [JsonType]
    public class PerformanceTiming
    {
        /// <summary>
        /// Gets the navigation start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, right after the
        /// prompt for unload terminates on the previous document in the same browsing context. If there is no previous
        /// document, this value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The navigation start.
        /// </value>
        public extern long NavigationStart
        { get; }

        /// <summary>
        /// Gets the unload event start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the unload event has
        /// been thrown. If there is no previous document, or if the previous document, or one of the needed redirects,
        /// is not of the same origin, the value returned is 0.
        /// </remarks>
        /// <value>
        /// The unload event start.
        /// </value>
        public extern long UnloadEventStart
        { get; }

        /// <summary>
        /// Gets the unload event start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the unload event
        /// handler finishes. If there is no previous document, or if the previous document, or one of the needed
        /// redirects, is not of the same origin, the value returned is 0.
        /// </remarks>
        /// <value>
        /// The unload event start.
        /// </value>
        public extern long UnloadEventEnd
        { get; }

        /// <summary>
        /// Gets the redirect start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the first HTTP
        /// redirect starts. If there is no redirect, or if one of the redirect is not of the same origin, the value
        /// returned is 0.
        /// </remarks>
        /// <value>
        /// The redirect start.
        /// </value>
        public extern long RedirectStart
        { get; }

        /// <summary>
        /// Gets the redirect end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the last HTTP
        /// redirect is completed, that is when the last byte of the HTTP response has been received. If there is no
        /// redirect, or if one of the redirect is not of the same origin, the value returned is 0.
        /// </remarks>
        /// <value>
        /// The redirect end.
        /// </value>
        public extern long RedirectEnd
        { get; }

        /// <summary>
        /// Gets the fetch start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the browser is ready
        /// to fetch the document using an HTTP request. This moment is before the check to any application cache.
        /// </remarks>
        /// <value>
        /// The fetch start.
        /// </value>
        public extern long FetchStart
        { get; }

        /// <summary>
        /// Gets the domain lookup start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the domain
        /// lookup starts. If a persistent connection is used, or the information is stored in a cache or a local
        /// resource, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The domain lookup start.
        /// </value>
        public extern long DomainLookupStart
        { get; }

        /// <summary>
        /// Gets the domain lookup end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the domain
        /// lookup is finished. If a persistent connection is used, or the information is stored in a cache or a local
        /// resource, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The domain lookup end.
        /// </value>
        public extern long DomainLookupEnd
        { get; }

        /// <summary>
        /// Gets the connect start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the request
        /// to open a connection is sent to the network. If the transport layer reports an error and the connection
        /// establishment is started again, the last connection establisment start time is given. If a persistent
        /// connection is used, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The connect start.
        /// </value>
        public extern long ConnectStart
        { get; }

        /// <summary>
        /// Gets the connect end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the connection
        /// is opened network. If the transport layer reports an error and the connection establishment is started
        /// again, the last connection establisment end time is given. If a persistent connection is used, the value
        /// will be the same as PerformanceTiming.fetchStart. A connection is considered as opened when all secure
        /// connection handshake, or SOCKS authentication, is terminated.
        /// </remarks>
        /// <value>
        /// The connect end.
        /// </value>
        public extern long ConnectEnd
        { get; }

        /// <summary>
        /// Gets the secure connection start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the secure
        /// connection handshake starts. If no such connection is requested, it returns 0.
        /// </remarks>
        /// <value>
        /// The secure connection start.
        /// </value>
        public extern long SecureConnectionStart
        { get; }

        /// <summary>
        /// Gets the request start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// sent the request to obtain the actual document, from the server or from a cache. If the transport layer
        /// fails after the start of the request and the connection is reopened, this property will be set to the time
        /// corresponding to the new request.
        /// </remarks>
        /// <value>
        /// The request start.
        /// </value>
        public extern long RequestStart
        { get; }

        /// <summary>
        /// Gets the response start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// received the first byte of the response, from the server from a cache, of from a local resource.
        /// </remarks>
        /// <value>
        /// The response start.
        /// </value>
        public extern long ResponseStart
        { get; }

        /// <summary>
        /// Gets the response end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// received the last byte of the response, or when the connection is closed if this happened first, from the
        /// server from a cache, of from a local resource.
        /// </remarks>
        /// <value>
        /// The response end.
        /// </value>
        public extern long ResponseEnd
        { get; }

        /// <summary>
        /// Gets the dom loading.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the parser
        /// started its work, that is when its Document.readyState changes to 'loading' and the corresponding
        /// readystatechange event is thrown.
        /// </remarks>
        /// <value>
        /// The dom loading.
        /// </value>
        public extern long DomLoading
        { get; }

        /// <summary>
        /// Gets the dom interactive.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the parser
        /// finished its work on the main document, that is when its Document.readyState changes to 'interactive' and
        /// the corresponding readystatechange event is thrown.
        /// </remarks>
        /// <value>
        /// The dom interactive.
        /// </value>
        public extern long DomInteractive
        { get; }

        /// <summary>
        /// Gets the dom content loaded event start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, right before the
        /// parser sent the DOMContentLoaded event, that is right after all the scripts that need to be executed right
        /// after parsing has been executed.
        /// </remarks>
        /// <value>
        /// The dom content loaded event start.
        /// </value>
        public extern long DomContentLoadedEventStart
        { get; }

        /// <summary>
        /// Gets the dom content loaded event end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, right after all the
        /// scripts that need to be executed as soon as possible, in order or not, has been executed.
        /// </remarks>
        /// <value>
        /// The dom content loaded event end.
        /// </value>
        public extern long DomContentLoadedEventEnd
        { get; }

        /// <summary>
        /// Gets the dom complete.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the parser
        /// finished its work on the main document, that is when its Document.readyState changes to 'complete' and the
        /// corresponding readystatechange event is thrown.
        /// </remarks>
        /// <value>
        /// The dom complete.
        /// </value>
        public extern long DomComplete
        { get; }

        /// <summary>
        /// Gets the load event start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the load event
        /// was sent for the current document. If this event has not yet been sent, it returns 0.
        /// </remarks>
        /// <value>
        /// The load event start.
        /// </value>
        public extern long LoadEventStart
        { get; }

        /// <summary>
        /// Gets the load event end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the load event
        /// handler terminated, that is when the load event is completed. If this event has not yet been sent, or is
        /// not yet completed, it returns 0.
        /// </remarks>
        /// <value>
        /// The load event end.
        /// </value>
        public extern long LoadEventEnd
        { get; }
    }

    [JsonType]
    public class PerformanceResourceTiming
    {
        /// <summary>
        /// Gets the redirect start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the first HTTP
        /// redirect starts. If there is no redirect, or if one of the redirect is not of the same origin, the value
        /// returned is 0.
        /// </remarks>
        /// <value>
        /// The redirect start.
        /// </value>
        public extern long RedirectStart
        { get; }

        /// <summary>
        /// Gets the redirect end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the last HTTP
        /// redirect is completed, that is when the last byte of the HTTP response has been received. If there is no
        /// redirect, or if one of the redirect is not of the same origin, the value returned is 0.
        /// </remarks>
        /// <value>
        /// The redirect end.
        /// </value>
        public extern long RedirectEnd
        { get; }

        /// <summary>
        /// Gets the fetch start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, the browser is ready
        /// to fetch the document using an HTTP request. This moment is before the check to any application cache.
        /// </remarks>
        /// <value>
        /// The fetch start.
        /// </value>
        public extern long FetchStart
        { get; }

        /// <summary>
        /// Gets the domain lookup start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the domain
        /// lookup starts. If a persistent connection is used, or the information is stored in a cache or a local
        /// resource, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The domain lookup start.
        /// </value>
        public extern long DomainLookupStart
        { get; }

        /// <summary>
        /// Gets the domain lookup end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the domain
        /// lookup is finished. If a persistent connection is used, or the information is stored in a cache or a local
        /// resource, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The domain lookup end.
        /// </value>
        public extern long DomainLookupEnd
        { get; }

        /// <summary>
        /// Gets the connect start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the request
        /// to open a connection is sent to the network. If the transport layer reports an error and the connection
        /// establishment is started again, the last connection establisment start time is given. If a persistent
        /// connection is used, the value will be the same as PerformanceTiming.fetchStart.
        /// </remarks>
        /// <value>
        /// The connect start.
        /// </value>
        public extern long ConnectStart
        { get; }

        /// <summary>
        /// Gets the connect end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the connection
        /// is opened network. If the transport layer reports an error and the connection establishment is started
        /// again, the last connection establisment end time is given. If a persistent connection is used, the value
        /// will be the same as PerformanceTiming.fetchStart. A connection is considered as opened when all secure
        /// connection handshake, or SOCKS authentication, is terminated.
        /// </remarks>
        /// <value>
        /// The connect end.
        /// </value>
        public extern long ConnectEnd
        { get; }

        /// <summary>
        /// Gets the secure connection start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, where the secure
        /// connection handshake starts. If no such connection is requested, it returns 0.
        /// </remarks>
        /// <value>
        /// The secure connection start.
        /// </value>
        public extern long SecureConnectionStart
        { get; }

        /// <summary>
        /// Gets the request start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// sent the request to obtain the actual document, from the server or from a cache. If the transport layer
        /// fails after the start of the request and the connection is reopened, this property will be set to the time
        /// corresponding to the new request.
        /// </remarks>
        /// <value>
        /// The request start.
        /// </value>
        public extern long RequestStart
        { get; }

        /// <summary>
        /// Gets the response start.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// received the first byte of the response, from the server from a cache, of from a local resource.
        /// </remarks>
        /// <value>
        /// The response start.
        /// </value>
        public extern long ResponseStart
        { get; }

        /// <summary>
        /// Gets the response end.
        /// </summary>
        /// <remarks>
        /// Is an unsigned long long representing the moment, in miliseconds since the UNIX epoch, when the browser
        /// received the last byte of the response, or when the connection is closed if this happened first, from the
        /// server from a cache, of from a local resource.
        /// </remarks>
        /// <value>
        /// The response end.
        /// </value>
        public extern long ResponseEnd
        { get; }

        /// <summary>
        /// Gets the type of the entry.
        /// </summary>
        /// <value>
        /// The type of the entry.
        /// </value>
        public extern string EntryType
        { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public extern string Duration
        { get; }

        /// <summary>
        /// Gets the time of the start.
        /// </summary>
        /// <value>
        /// The time of the start.
        /// </value>
        public extern int StartTime
        { get; }

        /// <summary>
        /// Gets the type of the initiator.
        /// </summary>
        /// <value>
        /// The type of the initiator.
        /// </value>
        public extern string InitiatorType
        { get; }
    }

    public enum NavigationType
    {
        /// <summary>
        /// Navigation started by clicking on a link, or entering the URL in the user agent's address bar, or form
        /// submission, or initializing through a script operation other than the ones used by Reload and
        /// BackForward as listed below.
        /// </summary>
        NavigateNext = 0,

        /// <summary>
        /// Navigation through the reload operation or the location.reload() method.
        /// </summary>
        Reload = 1,

        /// <summary>
        /// Navigation through a history traversal operation.
        /// </summary>
        BackForward = 2,

        /// <summary>
        /// Any navigation types not defined by values above.
        /// </summary>
        Undefined = 255
    }

    [JsonType]
    public class Navigation
    {
        public extern int RedirectCount
        { get; }

        public extern NavigationType Type
        { get; }
    }

    /// <summary>
    /// Browser memory.
    /// Only available in chrome
    /// </summary>
    [JsonType]
    public class BrowserMemory
    {
        public extern int JsHeapSizeLimit
        { get; }

        public extern int UsedJSHeapSize
        { get; }

        public extern int TotalJSHeapSize
        { get; }
    }
}
