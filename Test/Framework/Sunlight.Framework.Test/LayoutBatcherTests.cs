//-----------------------------------------------------------------------
// <copyright file="LayoutBatcherTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;
    using SunlightUnit;
    using Sunlight.Framework;

    /// <summary>
    /// QUnit tests for <see cref="LayoutBatcher"/>.
    /// <para>
    /// Each test installs a <see cref="TestWindowTimer"/> in deferred mode so
    /// rAF and setImmediate callbacks can be driven explicitly. The measurers
    /// passed to <c>ReadAsync</c> ignore the <see cref="Element"/> argument
    /// — these tests exercise the batcher's orchestration, not real DOM I/O.
    /// </para>
    /// </summary>
    [TestFixture]
    public class LayoutBatcherTests
    {
        /// <summary>
        /// Reset shared <see cref="LayoutBatcher"/> state, clear any
        /// ambient <see cref="CallContext"/> carried over from a previous
        /// test, and install a deferred-mode timer. Every test starts from
        /// a clean slate because both the batcher and <see cref="CallContext.Current"/>
        /// are process-wide statics.
        /// </summary>
        private static TestWindowTimer Setup()
        {
            LayoutBatcher.Reset();
            CallContext.Current = null;
            var timer = new TestWindowTimer(true);
            LayoutBatcher.Timer = timer;
            return timer;
        }

        /// <summary>
        /// A single enqueued read resolves with the measurer's return value
        /// after the rAF measurement phase followed by the setImmediate
        /// dispatch phase.
        /// </summary>
        [Test]
        public static void TestSingleReadResolves(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);

            double observed = -1;
            LayoutBatcher.ReadDoubleAsync(null, e => 42.0).Then(v =>
            {
                observed = v;
                assert.Equal(observed, 42.0, "resolved value matches measurer output");
                done();
            });

            assert.Equal(timer.PendingAnimationFrameCount, 1, "exactly one rAF scheduled");
            timer.FlushAnimationFrames();
            assert.Equal(timer.PendingImmediateCount, 1, "dispatch queued after measure");
            timer.FlushImmediates();
        }

        /// <summary>
        /// N concurrent reads in a single tick share a single
        /// <c>requestAnimationFrame</c>. All resolve after one rAF + one
        /// setImmediate.
        /// </summary>
        [Test]
        public static void TestMultipleReadsShareFrame(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(3);
            int resolvedCount = 0;

            for (int i = 0; i < 3; i++)
            {
                int captured = i;
                LayoutBatcher.ReadDoubleAsync(null, e => captured * 10.0).Then(v =>
                {
                    resolvedCount++;
                    // The third microtask fires last (FIFO); assert inside its callback
                    // so the count check runs after all three have incremented.
                    if (resolvedCount == 3)
                    {
                        assert.Equal(resolvedCount, 3, "all three promises resolved");
                    }
                    done();
                });
            }

            assert.Equal(timer.PendingAnimationFrameCount, 1, "three reads share one rAF");
            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }

        /// <summary>
        /// The measurer function runs inside the rAF phase — before the
        /// promise resolves. Verified by asserting a side-effect before
        /// running the dispatch phase.
        /// </summary>
        [Test]
        public static void TestMeasureRunsInRAF(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);
            bool measured = false;

            LayoutBatcher.ReadDoubleAsync(null, e =>
            {
                measured = true;
                return 1.0;
            }).Then(v => done());

            assert.IsFalse(measured, "measurer has not run before rAF flush");
            timer.FlushAnimationFrames();
            assert.IsTrue(measured, "measurer ran inside rAF phase");
            timer.FlushImmediates();
        }

        /// <summary>
        /// Resolver continuations run in the setImmediate phase, not the rAF
        /// phase — so writes triggered by .then callbacks do not land in the
        /// measurement frame.
        /// </summary>
        [Test]
        public static void TestDispatchOutsideRAF(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);
            bool resolvedDuringRaf = false;
            bool rafFlushCompleted = false;

            LayoutBatcher.ReadDoubleAsync(null, e => 7.0).Then(v =>
            {
                // If this .Then microtask fires before FlushAnimationFrames
                // returns, rafFlushCompleted would still be false — that
                // would indicate the resolver ran synchronously inside the
                // rAF phase, which is exactly what the batcher must avoid.
                resolvedDuringRaf = !rafFlushCompleted;
                assert.IsFalse(resolvedDuringRaf,
                    "resolver did not fire during rAF phase");
                done();
            });

            timer.FlushAnimationFrames();
            rafFlushCompleted = true;
            timer.FlushImmediates();
        }

        /// <summary>
        /// Dispatch order matches enqueue order for reads in the same batch.
        /// Reads are enqueued from separate helper methods rather than a
        /// loop so each measurer captures its constant from its own lexical
        /// scope — NScript's transpiler handles for-loop variable capture
        /// differently from C#, and we want the test to cover batcher
        /// ordering rather than transpiler closure behavior.
        /// </summary>
        [Test]
        public static void TestMultipleReadsOrdered(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(3);
            var order = new List<double>();

            EnqueueOrderedRead(order, 0.0, done, assert);
            EnqueueOrderedRead(order, 1.0, done, assert);
            EnqueueOrderedRead(order, 2.0, done, assert);

            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }

        private static void EnqueueOrderedRead(List<double> order, double expected,
                                               Action done, Assert assert)
        {
            LayoutBatcher.ReadDoubleAsync(null, e => expected).Then(v =>
            {
                order.Add(v);
                // Microtasks fire in FIFO enqueue order; once the list has
                // three entries this is the last callback — the right place
                // to assert the full ordering.
                if (order.Count == 3)
                {
                    assert.Equal(order.Count, 3, "three values collected");
                    assert.Equal(order[0], 0.0, "first-enqueued resolves first");
                    assert.Equal(order[1], 1.0, "second-enqueued resolves second");
                    assert.Equal(order[2], 2.0, "third-enqueued resolves third");
                }
                done();
            });
        }

        /// <summary>
        /// A throw inside one measurer rejects only that read's promise —
        /// the remaining reads in the batch resolve normally.
        /// </summary>
        [Test]
        public static void TestFailedMeasurerIsolated(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(3);

            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(
                v =>
                {
                    assert.Equal(v, 1.0, "first read resolved despite sibling failure");
                    done();
                },
                (object err) => { });

            LayoutBatcher.ReadDoubleAsync(null, e => { throw new Exception("measure failure"); }).Then(
                v => { },
                (object err) =>
                {
                    assert.IsTrue(err != null, "failing read rejected with error");
                    done();
                });

            LayoutBatcher.ReadDoubleAsync(null, e => 3.0).Then(
                v =>
                {
                    assert.Equal(v, 3.0, "third read resolved despite sibling failure");
                    done();
                },
                (object err) => { });

            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }

        /// <summary>
        /// Each read captures <see cref="CallContext.Current"/> at enqueue
        /// time. Two reads originating from different contexts, flushed in
        /// separate batches, each observe their own context inside their
        /// resolver.
        /// </summary>
        /// <remarks>
        /// Batch B is created and flushed from inside batch A's .Then
        /// continuation. If we instead laid the two batches out as sibling
        /// synchronous calls, microtasks from batch A would not drain between
        /// <c>FlushImmediates</c> and the next <c>StartRoot</c> — both
        /// WrapPromise handlers would run back-to-back, and the last-written
        /// <c>CallContext.current</c> would win for every user .Then. Nesting
        /// the B setup inside A's resolver guarantees A's user handler has
        /// already run (and its assertion has already captured
        /// <c>CallContext.Current</c>) before <c>ctxB</c> is installed.
        /// </remarks>
        [Test]
        public static void TestCallContextPerReadIsolation(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(2);

            var ctxA = CallContext.StartRoot();
            int expectedA = ctxA.ActionId;
            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(v =>
            {
                var curA = CallContext.Current;
                var observedA = curA != null ? curA.ActionId : -1;
                assert.Equal(observedA, expectedA, "read A sees its captured ActionId");
                done();

                var ctxB = CallContext.StartRoot();
                int expectedB = ctxB.ActionId;
                LayoutBatcher.ReadDoubleAsync(null, e => 2.0).Then(v2 =>
                {
                    var curB = CallContext.Current;
                    var observedB = curB != null ? curB.ActionId : -1;
                    assert.Equal(observedB, expectedB, "read B sees its captured ActionId");
                    done();
                });
                timer.FlushAnimationFrames();
                timer.FlushImmediates();
            });

            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }

        /// <summary>
        /// After the dispatch phase completes, the ambient
        /// <see cref="CallContext.Current"/> must be whatever it was before
        /// DispatchPhase ran — never left pointing at a per-entry context.
        /// </summary>
        [Test]
        public static void TestCallContextRestoredAfterDispatch(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);

            // Install an "outer" ambient context and enqueue one read whose
            // captured context differs from it.
            var outer = CallContext.StartRoot();
            var inner = CallContext.StartRoot();
            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(v => done());

            // Before dispatch, reset the ambient context to outer so that
            // DispatchPhase will switch to inner and must restore to outer.
            CallContext.Current = outer;

            timer.FlushAnimationFrames();
            timer.FlushImmediates();

            var afterActionId = CallContext.Current != null ? CallContext.Current.ActionId : -1;
            assert.Equal(afterActionId, outer.ActionId,
                "ambient CallContext restored to pre-dispatch value");
        }

        /// <summary>
        /// If a resolver closure throws, DispatchPhase's finally block still
        /// restores the previous ambient context. One bad resolver must not
        /// leak its captured context into sibling reads or post-dispatch
        /// continuations.
        /// </summary>
        [Test]
        public static void TestCallContextPreservedOnResolverError(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);

            var outer = CallContext.StartRoot();
            var poison = CallContext.StartRoot();

            // Read whose captured ctx is "poison". Its .Then will throw —
            // DispatchPhase catches the exception but must still restore.
            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(v =>
            {
                throw new Exception("resolver boom");
            });

            // Second read — scheduled while ambient ctx = poison (inherits).
            // We'll assert the ambient is back to 'outer' after flush.
            LayoutBatcher.ReadDoubleAsync(null, e => 2.0).Then(v => done());

            // Reset ambient to outer so post-dispatch restore target is outer.
            CallContext.Current = outer;

            timer.FlushAnimationFrames();
            timer.FlushImmediates();

            var afterActionId = CallContext.Current != null ? CallContext.Current.ActionId : -1;
            assert.Equal(afterActionId, outer.ActionId,
                "ambient CallContext restored even after resolver threw");
        }

        /// <summary>
        /// Exercises <see cref="LayoutBatcher.ReadClientRectAsync"/> — the
        /// reference-type generic instantiation of <see cref="LayoutBatcher.ReadAsync{T}"/>.
        /// NScript handles reference-type generics differently from value types
        /// (no boxing, different default(T) semantics), so this path is worth
        /// exercising independently from the numeric read tests.
        /// </summary>
        [Test]
        public static void TestClientRectReadResolves(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);

            var rect = new ClientRect();
            LayoutBatcher.ReadClientRectAsync(null, e => rect).Then(v =>
            {
                assert.IsTrue(v != null, "ClientRect resolver received non-null value");
                assert.StrictEqual(v, rect, "resolved value is the exact instance measured");
                done();
            });

            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }

        /// <summary>
        /// Reads enqueued from within a resolver must land in the NEXT batch,
        /// not the currently-dispatching one. Guards the snapshot-and-reset
        /// invariant in <see cref="LayoutBatcher.MeasurePhase"/> — if a refactor
        /// ever moves the <c>pending</c> reset after the measurement loop, this
        /// test catches the regression.
        /// </summary>
        [Test]
        public static void TestReadDuringDispatchGoesToNextBatch(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(1);

            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(v =>
            {
                // Enqueue a new read from inside the first read's resolver.
                // This must schedule a brand-new rAF rather than piggyback on
                // the already-dispatched batch. Assertions happen inside this
                // user-handler microtask so the "second read has been enqueued
                // but not yet measured" window is observable — from outside the
                // microtask the check would race the microtask queue.
                bool secondResolved = false;

                LayoutBatcher.ReadDoubleAsync(null, e => 2.0).Then(v2 =>
                {
                    secondResolved = true;
                    assert.Equal(v2, 2.0, "second-batch read resolved with its own measurer value");
                    done();
                });

                assert.IsFalse(secondResolved,
                    "second read has not resolved yet — it is in the next batch");
                assert.Equal(timer.PendingAnimationFrameCount, 1,
                    "re-enqueued read scheduled a fresh rAF");

                // Flush batch 2 from within batch 1's user handler.
                timer.FlushAnimationFrames();
                timer.FlushImmediates();
            });

            // Flush batch 1: rAF measure + setImmediate dispatch. The first
            // read resolves; its .Then microtask enqueues the second read.
            timer.FlushAnimationFrames();
            timer.FlushImmediates();
        }
    }
}
