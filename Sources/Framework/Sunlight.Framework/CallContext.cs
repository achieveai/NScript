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
        private static int eventDispatchDepth;

        public readonly int ActionId;
        public readonly string TraceId;
        public readonly string SpanId;
        public readonly string ParentSpanId;
        public readonly int Depth;

        // Sentinel returned by OnEventDispatch when the event is not a user
        // gesture (async I/O completion such as IndexedDB success/error).
        // OnEventDispatchEnd recognises it and skips the depth/current unwind
        // that only applies to the user-gesture path.
        private static readonly object NonUserGestureSentinel = new object();

        static CallContext()
        {
            EventBinder.OnEventDispatch = (evt) =>
            {
                // Async I/O completion events (IndexedDB request success/error,
                // IDBTransaction complete, etc.) flow through EventBinder but
                // must NOT start a new action root — they run on behalf of
                // whichever action issued the request. User-gesture DOM events
                // (click, input, focus, keydown, …) fire on DOM Element targets
                // with an associated tagName; async I/O events fire on
                // EventTarget subclasses whose target is not a DOM Element
                // (IDBRequest, IDBTransaction, IDBDatabase). Skip the root
                // here for non-Element targets so boot-time IDB traffic does
                // not create orphan root contexts that pollute the ambient
                // context the W3C traceparent propagation relies on.
                if (!CallContext.IsUserGestureEvent(evt)) return CallContext.NonUserGestureSentinel;
                CallContext.eventDispatchDepth++;
                var prev = CallContext.current;
                CallContext.StartRoot();
                return prev;
            };
            EventBinder.OnEventDispatchEnd = (prev) =>
            {
                if (prev == CallContext.NonUserGestureSentinel) return;
                CallContext.eventDispatchDepth--;
                // Only restore for nested event dispatches (depth > 0).
                // Top-level handlers keep their context active for async
                // continuations and TaskScheduler work after the handler returns.
                if (CallContext.eventDispatchDepth > 0)
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
            CallContext.ExposeDebugAccessors();

            // Force LayoutBatcher's static ctor to run so the Element async-read
            // hooks are installed before any application code accesses
            // element.ClientHeightAsync / GetBoundingClientRectAsync. CallContext
            // itself is reliably touched at startup (EventBinder's first dispatch
            // uses CallContext.OnEventDispatch, and observable bindings set up
            // during template initialization also touch it), so piggy-backing on
            // this ctor guarantees LayoutBatcher is wired before the first DOM
            // read regardless of whether the app explicitly calls Init().
            LayoutBatcher.Init();
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
        /// Clear the ambient context and reset the event-dispatch depth to 0.
        /// Intended for application boot sequences that run inside an async
        /// continuation (e.g. after awaiting IndexedDB initialization): DOM
        /// events that fire during the init window trigger
        /// <see cref="EventBinder.OnEventDispatch"/> → <see cref="StartRoot"/>,
        /// and the depth=0 "keep context active for async continuations" policy
        /// in <c>OnEventDispatchEnd</c> keeps those root contexts alive past
        /// boot, polluting the first true user-gesture context. Call this after
        /// the UI is activated to restore the clean "no action in progress"
        /// invariant. Does NOT disturb the WI-20 async-continuation preservation
        /// semantics for user-gesture handlers.
        /// </summary>
        /// <remarks>
        /// Also zeroes <c>eventDispatchDepth</c>. If this is invoked mid-dispatch
        /// (depth &gt; 0), the subsequent <c>OnEventDispatchEnd</c> would otherwise
        /// overwrite the cleared <c>current</c> with a stale <c>prev</c> captured
        /// at dispatch entry, silently undoing the cleanup. Resetting depth turns
        /// the depth&gt;0 unwind guard into a no-op so the cleared state sticks
        /// regardless of where the method is called from.
        /// </remarks>
        public static void ClearAmbient()
        {
            CallContext.eventDispatchDepth = 0;
            CallContext.current = null;
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
        /// Append this context's correlation fields — <c>actionId</c>,
        /// <c>traceId</c>, <c>spanId</c>, <c>parentSpanId</c> (when non-null),
        /// <c>depth</c> — to a JSON object currently being assembled in
        /// <paramref name="sb"/>. The builder assumes a preceding key/value
        /// has already been written and prefixes each field with a comma.
        /// </summary>
        /// <remarks>
        /// Owning the correlation serialization contract here keeps
        /// <see cref="LogJsonBuilder"/> and future sinks (e.g. WebSocket)
        /// decoupled from the set of fields — adding a new correlation field
        /// is a one-place change instead of a multi-sink sweep.
        /// <paramref name="escape"/> is passed in because this type lives in
        /// <c>Sunlight.Framework</c> and must not take a dependency on the
        /// internal <see cref="LogJsonBuilder"/> JS-escape bridge.
        /// </remarks>
        public void AppendCorrelationJson(StringBuilder sb, Func<string, string> escape)
        {
            sb.Append(",\"actionId\":");
            sb.Append(this.ActionId.ToString());
            sb.Append(",\"traceId\":");
            sb.Append(escape(this.TraceId));
            sb.Append(",\"spanId\":");
            sb.Append(escape(this.SpanId));
            if (this.ParentSpanId != null)
            {
                sb.Append(",\"parentSpanId\":");
                sb.Append(escape(this.ParentSpanId));
            }
            sb.Append(",\"depth\":");
            sb.Append(this.Depth.ToString());
        }

        /// <summary>
        /// Compiler-inserted wrapper for external async calls: captures context
        /// before await, restores it when the promise resolves or rejects.
        /// Handles both Promise and Promise&lt;T&gt; (same type at JS runtime).
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
        public static extern Promise WrapPromise(Promise p);

        /// <summary>
        /// Generic overload of <see cref="WrapPromise"/> for typed promises.
        /// Ensures <c>.Then</c> / <c>.Catch</c> callbacks attached downstream
        /// see the call-site context even when they run as microtasks after
        /// the resolving frame has torn its ambient context down.
        /// </summary>
        [Script(@"
            var ctx = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
            return p.then(
                function(v) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; return v; },
                function(e) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; throw e; }
            );
        ")]
        public static extern Promise<T> WrapPromise<T>(Promise<T> p);

        /// <summary>
        /// Expose diagnostic accessors on window.__callContext so that Playwright
        /// tests (and browser DevTools) can inspect the current CallContext even
        /// though the generated JS runs inside an IIFE.
        /// Always-on by design: NScript lacks conditional compilation, and all JS
        /// state is already client-visible. The underscore-prefixed global follows
        /// the same convention as React DevTools / Angular ng.probe.
        /// </summary>
        [Script(@"
            var ctx = {};
            ctx.getCurrent = function() {
                var c = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
                if (!c) return null;
                var r = {};
                r.actionId = c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::ActionId};
                r.traceId = c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::TraceId};
                r.spanId = c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::SpanId};
                r.parentSpanId = c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::ParentSpanId};
                r.depth = c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::Depth};
                r.traceparent = '00-' + c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::TraceId} + '-' + c.@{[Sunlight.Framework]Sunlight.Framework.CallContext::SpanId} + '-01';
                return r;
            };
            ctx.testXhrHook = function() {
                var headers = {};
                var mockReq = {};
                mockReq.setRequestHeader = function(k, v) { headers[k] = v; };
                var hook = @{[System.Web]System.Web.XMLHttpRequest::OnBeforeSend};
                if (hook) { hook(mockReq); }
                return headers;
            };
            if (typeof window !== 'undefined') { window.__callContext = ctx; }
        ")]
        private static extern void ExposeDebugAccessors();

        /// <summary>
        /// True if the event represents a user gesture that should start a
        /// new action root. User-gesture DOM events fire on DOM Elements
        /// (which expose <c>tagName</c>); async I/O completion events
        /// (IndexedDB <c>success</c>/<c>error</c>/<c>upgradeneeded</c>,
        /// <c>IDBTransaction</c> <c>complete</c>, etc.) fire on EventTarget
        /// subclasses whose target has no <c>tagName</c>.
        /// </summary>
        /// <remarks>
        /// Using <c>evt.target.tagName</c> (string truthiness) is the narrowest
        /// test that holds across all user-gesture DOM event types (click,
        /// input, focus, keydown, mouseover, drag, etc.) without depending on
        /// <c>instanceof Element</c>, which is brittle across iframe / Shadow
        /// DOM boundaries. <c>evt.isTrusted</c> cannot be used because IDB
        /// completion events are also trusted.
        /// </remarks>
        [Script(@"return !!(evt && evt.target && evt.target.tagName);")]
        private static extern bool IsUserGestureEvent(object evt);

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
