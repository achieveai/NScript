//-----------------------------------------------------------------------
// <copyright file="LayoutBatcher.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Batches layout-sensitive DOM reads (clientWidth/Height, offsetWidth/Height,
    /// scrollWidth/Height, getBoundingClientRect) so that the browser only has to
    /// recompute layout once per animation frame regardless of how many callers
    /// requested measurements.
    /// </summary>
    /// <remarks>
    /// Three phases per batch:
    /// <list type="number">
    /// <item><description>Collect — <see cref="ReadAsync{T}"/> appends a pending
    /// entry and schedules a single <c>requestAnimationFrame</c> if none is
    /// already pending. The caller receives a <see cref="Promise{T}"/>.</description></item>
    /// <item><description>Measure — inside the rAF callback, every queued
    /// measurer is invoked against the live DOM. Results (or exceptions) are
    /// stashed on each entry. A follow-up <c>setImmediate</c> is scheduled for
    /// dispatch.</description></item>
    /// <item><description>Dispatch — runs on the next tick, outside the rAF
    /// frame. Each entry's <c>resolve</c>/<c>reject</c> is called under the
    /// originating <see cref="CallContext"/> so downstream <c>.then</c>
    /// continuations keep their correlation ids. A failed entry does not abort
    /// remaining entries.</description></item>
    /// </list>
    /// Follows the batching idiom established by
    /// <see cref="UI.Helpers.BindingGraph.GraphFlushCoordinator"/> and the
    /// save/restore pattern established by <see cref="TaskScheduler"/>.
    /// See ADR 0015.
    /// </remarks>
    public static class LayoutBatcher
    {
        private sealed class PendingRead
        {
            public Action Measure;
            public Action Dispatch;
            public Action<object> Reject;
            public CallContext CapturedContext;
        }

        private sealed class ReadResult<T>
        {
            public T Value;
            public object Error;
            public bool Ok;
        }

        private static IWindowTimer windowTimer;
        private static List<PendingRead> pending;
        private static bool rafScheduled;

        static LayoutBatcher()
        {
            pending = new List<PendingRead>();
            rafScheduled = false;

            // Wire the async-read hooks on Element so every Element.ClientHeight /
            // GetBoundingClientRect() routes through LayoutBatcher automatically.
            // System.Web.Html lives below Sunlight.Framework in the dependency
            // graph so the hooks are delegates installed at startup.
            Element.AsyncReadDouble = ReadDoubleAsync;
            Element.AsyncReadClientRect = ReadClientRectAsync;
        }

        /// <summary>
        /// Triggers the static constructor (idempotent). Call from any code path
        /// that must guarantee the Element hooks are installed before the first
        /// async measurement access — e.g. framework startup.
        /// </summary>
        public static void Init()
        {
            // no-op — touching the type triggers the static ctor.
        }

        /// <summary>
        /// The timer used to schedule rAF and setImmediate. Defaults to
        /// <see cref="WindowTimer"/>; tests inject <see cref="TestWindowTimer"/>
        /// in deferred mode to drive flush phases manually.
        /// </summary>
        public static IWindowTimer Timer
        {
            get
            {
                if (windowTimer == null)
                {
                    windowTimer = new WindowTimer();
                }
                return windowTimer;
            }

            set
            {
                windowTimer = value;
            }
        }

        /// <summary>
        /// Drop all pending reads and clear the injected timer so the next
        /// test (or production caller) starts from a clean slate. The timer
        /// getter re-lazy-initializes to <see cref="WindowTimer"/> on next
        /// access if no test replacement has been installed.
        /// </summary>
        public static void Reset()
        {
            pending = new List<PendingRead>();
            rafScheduled = false;
            windowTimer = null;
        }

        /// <summary>
        /// Enqueue a DOM read. The <paramref name="measurer"/> runs inside the
        /// next <c>requestAnimationFrame</c>; the returned promise resolves on
        /// the subsequent <c>setImmediate</c> under the <see cref="CallContext"/>
        /// active at the time of this call.
        /// </summary>
        public static Promise<T> ReadAsync<T>(Element el, Func<Element, T> measurer)
        {
            var entry = new PendingRead();
            entry.CapturedContext = CallContext.Current;

            // Result state lives on a dedicated carrier object. Using an
            // instance rather than bare closure locals isolates each enqueued
            // read from NScript's transpiler-specific closure hoisting and
            // keeps measured value + error explicit. The carrier also lets
            // DispatchPhase's catch settle the victim promise via Reject.
            var result = new ReadResult<T>();

            var promise = new Promise<T>((resolve, reject) =>
            {
                entry.Reject = reject;

                entry.Measure = () =>
                {
                    try
                    {
                        result.Value = measurer(el);
                        result.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex;
                        result.Ok = false;
                    }
                };

                entry.Dispatch = () =>
                {
                    if (result.Ok)
                    {
                        resolve(result.Value);
                    }
                    else
                    {
                        reject(result.Error);
                    }
                };
            });

            // Per JS spec, new Promise(executor) runs the executor
            // synchronously before returning, so entry.Measure and
            // entry.Dispatch are guaranteed installed here. Enqueueing
            // AFTER the Promise constructor returns keeps the "entry is
            // complete before it enters the batch" invariant explicit.
            pending.Add(entry);
            ScheduleFlushIfNeeded();

            // Wrap so .then/.catch callbacks attached by downstream code
            // observe the call-site CallContext. DispatchPhase's
            // save/restore bracket protects only the synchronous
            // resolve/reject closure — actual .then microtasks fire
            // afterward and need WrapPromise to re-set Current.
            return CallContext.WrapPromise<T>(promise);
        }

        /// <summary>
        /// Specialized entry point for <c>double</c> measurements so the
        /// <see cref="Element"/> hook delegate type stays monomorphic.
        /// </summary>
        public static Promise<double> ReadDoubleAsync(Element el, Func<Element, double> measurer)
        {
            return ReadAsync<double>(el, measurer);
        }

        /// <summary>
        /// Specialized entry point for <see cref="ClientRect"/> measurements.
        /// </summary>
        public static Promise<ClientRect> ReadClientRectAsync(Element el, Func<Element, ClientRect> measurer)
        {
            return ReadAsync<ClientRect>(el, measurer);
        }

        private static string FormatException(Exception ex)
        {
            if (ex == null)
            {
                return "unknown error";
            }
            string message = ex.Message ?? string.Empty;
            string stack = ex.Stack ?? string.Empty;
            if (stack.Length == 0)
            {
                return message;
            }
            if (message.Length == 0)
            {
                return stack;
            }
            return message + "\n" + stack;
        }

        private static void ScheduleFlushIfNeeded()
        {
            if (rafScheduled)
            {
                return;
            }
            rafScheduled = true;
            Timer.RequestAnimationFrame(MeasurePhase);
        }

        private static void MeasurePhase()
        {
            // Snapshot and reset pending list FIRST so that reads enqueued by
            // resolvers (or during the subsequent dispatch tick) go into the
            // next batch rather than this one.
            var batch = pending;
            pending = new List<PendingRead>();
            rafScheduled = false;

            for (int i = 0; i < batch.Count; i++)
            {
                // Per-entry try/catch lives inside entry.Measure itself so one
                // failed measurer does not poison the remaining batch. The
                // null guard is defensive — the enqueue path installs Measure
                // before pending.Add, so null here indicates a framework bug.
                var m = batch[i].Measure;
                if (m != null)
                {
                    m();
                }
            }

            // Dispatch on the next tick so that writes triggered by user
            // continuations do not pollute this rAF frame. Mirrors ADR 0015.
            Timer.SetImmediate(() => DispatchPhase(batch));
        }

        private static void DispatchPhase(List<PendingRead> batch)
        {
            for (int i = 0; i < batch.Count; i++)
            {
                var entry = batch[i];
                var previous = CallContext.Current;
                try
                {
                    CallContext.Current = entry.CapturedContext;
                    var d = entry.Dispatch;
                    if (d != null)
                    {
                        d();
                    }
                }
                catch (Exception ex)
                {
                    // Reaching this catch means the Dispatch closure itself
                    // threw (e.g. CallContext.Current setter tripped). Settle
                    // the victim promise via Reject so any awaiter sees a
                    // failure rather than hanging forever, then continue so
                    // one corrupted entry does not poison the rest of the
                    // batch. Log Message + Stack for post-mortem; a bare
                    // Message loses the whole call chain.
                    try
                    {
                        if (entry.Reject != null)
                        {
                            entry.Reject(ex);
                        }
                    }
                    catch
                    {
                        // Reject itself throwing would be a platform bug;
                        // swallow so remaining entries still dispatch.
                    }

                    Logger.Error("LayoutBatcher dispatch failed: " + FormatException(ex));
                }
                finally
                {
                    CallContext.Current = previous;
                }
            }
        }
    }
}
