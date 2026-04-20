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
            bool resolved = false;

            LayoutBatcher.ReadDoubleAsync(null, e => 7.0).Then(v =>
            {
                resolved = true;
                done();
            });

            timer.FlushAnimationFrames();
            assert.IsFalse(resolved, "resolver did not fire during rAF phase");
            timer.FlushImmediates();
            assert.IsTrue(resolved, "resolver fired in setImmediate phase");
        }

        /// <summary>
        /// Dispatch order matches enqueue order for reads in the same batch.
        /// </summary>
        [Test]
        public static void TestMultipleReadsOrdered(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(3);
            var order = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                double captured = i;
                LayoutBatcher.ReadDoubleAsync(null, e => captured).Then(v =>
                {
                    order.Add(v);
                    // Microtasks fire in FIFO enqueue order, so when the list has
                    // three entries this is the last callback — a safe place to
                    // assert the full collected ordering.
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

            timer.FlushAnimationFrames();
            timer.FlushImmediates();
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
        /// time. During dispatch the captured context is restored, so two
        /// reads originating from different contexts observe their own
        /// context inside their resolver.
        /// </summary>
        [Test]
        public static void TestCallContextPerReadIsolation(Assert assert)
        {
            var timer = Setup();
            var done = assert.Async(2);

            var ctxA = CallContext.StartRoot();
            int observedActionA = -1;
            LayoutBatcher.ReadDoubleAsync(null, e => 1.0).Then(v =>
            {
                var cur = CallContext.Current;
                observedActionA = cur != null ? cur.ActionId : -1;
                assert.Equal(observedActionA, ctxA.ActionId, "read A sees its captured ActionId");
                done();
            });

            var ctxB = CallContext.StartRoot();
            int observedActionB = -1;
            LayoutBatcher.ReadDoubleAsync(null, e => 2.0).Then(v =>
            {
                var cur = CallContext.Current;
                observedActionB = cur != null ? cur.ActionId : -1;
                assert.Equal(observedActionB, ctxB.ActionId, "read B sees its captured ActionId");
                done();
            });

            // NOTE: The .Then callbacks we register above are raw NScript
            // lambdas, not compiler-wrapped await continuations. Two
            // mechanisms cooperate to make them see the captured context:
            // (1) LayoutBatcher.DispatchPhase sets CallContext.Current to
            // CapturedContext before calling resolve(); (2) CallContext.WrapPromise
            // captures Current at ReadAsync-call time and re-installs it in
            // .then/.catch microtasks. For reads enqueued without an ambient
            // swap between enqueue and dispatch, both mechanisms agree.
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
    }
}
