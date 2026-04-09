//-----------------------------------------------------------------------
// <copyright file="CallContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Ambient context that flows across async boundaries.
    /// Analogous to .NET CallContext / AsyncLocal.
    /// JS is single-threaded so a static Current is safe for synchronous code;
    /// external async boundaries are handled by compiler-level await wrapping.
    /// </summary>
    public class CallContext
    {
        private static int nextId;
        private static CallContext current;

        public readonly int ActionId;
        public readonly string TraceId;
        public readonly string SpanId;
        public readonly string ParentSpanId;
        public readonly int Depth;

        static CallContext()
        {
            EventBinder.OnEventDispatch = () =>
            {
                var prev = CallContext.current;
                CallContext.StartRoot();
                return prev;
            };
            EventBinder.OnEventDispatchEnd = (prev) =>
            {
                CallContext.current = (CallContext)prev;
            };
            System.Web.XMLHttpRequest.OnBeforeSend = (request) =>
            {
                var ctx = CallContext.current;
                if (ctx != null)
                {
                    request.SetRequestHeader("traceparent", ctx.ToTraceparent());
                }
            };
        }

        private CallContext(int actionId, string traceId, string spanId,
                            string parentSpanId, int depth)
        {
            this.ActionId = actionId;
            this.TraceId = traceId;
            this.SpanId = spanId;
            this.ParentSpanId = parentSpanId;
            this.Depth = depth;
        }

        /// <summary>
        /// Delegate invoked when a new root action starts.
        /// TaskScheduler subscribes to this to trigger priority demotion.
        /// </summary>
        public static Action<int> OnNewRootAction;

        /// <summary>
        /// The currently active context. Null if no action is in progress.
        /// </summary>
        public static CallContext Current
        {
            get { return CallContext.current; }
            internal set { CallContext.current = value; }
        }

        /// <summary>
        /// Create a new root context (call at user-gesture entry points).
        /// Notifies subscribers (e.g. TaskScheduler) to demote older queued tasks.
        /// </summary>
        public static CallContext StartRoot()
        {
            var ctx = new CallContext(
                CallContext.nextId++,
                CallContext.GenerateTraceId(),
                CallContext.GenerateSpanId(),
                null,
                0);
            CallContext.current = ctx;
            if (CallContext.OnNewRootAction != null) CallContext.OnNewRootAction(ctx.ActionId);
            return ctx;
        }

        /// <summary>
        /// Create a child context within the current action.
        /// Inherits the same ActionId and TraceId; gets a new SpanId.
        /// </summary>
        public CallContext StartChild()
        {
            var child = new CallContext(
                this.ActionId,
                this.TraceId,
                CallContext.GenerateSpanId(),
                this.SpanId,
                this.Depth + 1);
            CallContext.current = child;
            return child;
        }

        /// <summary>
        /// Format as W3C traceparent header value:
        /// "00-{traceId}-{spanId}-01"
        /// </summary>
        public string ToTraceparent()
        {
            return "00-" + this.TraceId + "-" + this.SpanId + "-01";
        }

        /// <summary>
        /// Compiler-inserted wrapper for external async calls: captures context
        /// before await, restores it when the promise resolves or rejects.
        /// </summary>
        /// <remarks>
        /// [Script] is required here because:
        /// 1. Promise.then() is a native JS API — NScript's Promise (which is the
        ///    same as Task in NScript; both compile to JS Promise) is an [ImportedType]
        ///    facade with no .Then() C# method exposed.
        /// 2. The callback functions passed to .then() must be raw JS functions, not
        ///    NScript-compiled lambdas. If written in C#, the lambdas would go through
        ///    NScript's async state machine / TaskScheduler, which is exactly what we
        ///    are trying to work around — we need direct JS callbacks that restore the
        ///    ambient context without any framework interception.
        /// </remarks>
        [Script(@"
            var ctx = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
            return p.then(
                function(v) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; return v; },
                function(e) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; throw e; }
            );
        ")]
        public static extern Promise<T> WrapPromise<T>(Promise<T> p);

        /// <summary>
        /// Non-generic overload for void-returning promises (Task in C# = Promise in JS).
        /// </summary>
        /// <remarks>
        /// See generic overload remarks for why [Script] is required.
        /// </remarks>
        [Script(@"
            var ctx = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
            return p.then(
                function(v) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; return v; },
                function(e) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; throw e; }
            );
        ")]
        public static extern Promise WrapPromise(Promise p);

        private static string GenerateHexSegment()
        {
            return Math.Random().ToString(16).Substring(2, 8);
        }

        private static string GenerateTraceId()
        {
            return GenerateHexSegment() + GenerateHexSegment()
                 + GenerateHexSegment() + GenerateHexSegment();
        }

        private static string GenerateSpanId()
        {
            return GenerateHexSegment() + GenerateHexSegment();
        }
    }
}
