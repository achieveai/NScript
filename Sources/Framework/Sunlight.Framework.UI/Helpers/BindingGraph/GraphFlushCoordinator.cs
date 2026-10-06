namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

    /// <summary>
    /// How a batched flush is scheduled relative to the current JS task.
    /// </summary>
    public enum GraphFlushMode
    {
        /// <summary>
        /// Flush on the microtask queue: after the current task, before any
        /// timer, event, or paint. Default.
        /// </summary>
        Microtask = 0,

        /// <summary>
        /// Flush as a high-priority TaskScheduler task (setImmediate /
        /// setTimeout(0)). A paint may happen before the flush.
        /// </summary>
        Macrotask = 1
    }

    /// <summary>
    /// Global flush coordinator. Collects dirty graphs and flushes them
    /// in depth order (parents before nested children) on one scheduled
    /// boundary. Only used when <see cref="BatchingEnabled"/> is true;
    /// otherwise property changes flush synchronously.
    /// </summary>
    public static class GraphFlushCoordinator
    {
        /// <summary>
        /// Upper bound on outer passes in one FlushAll. A pass re-runs when a
        /// flush dirties a graph at a depth already drained (child writing to
        /// its parent). Mirrors the 100-pass guard inside GraphEngine.Flush.
        /// </summary>
        private const int MaxPasses = 100;

        /// <summary>
        /// When true, PropertyChanged notifications only mark nodes dirty and
        /// schedule one flush; the DOM is updated on the next boundary.
        /// When false (default), each notification flushes synchronously.
        /// </summary>
        public static bool BatchingEnabled;

        /// <summary>
        /// Scheduling boundary used when batching is enabled.
        /// </summary>
        public static GraphFlushMode Mode;

        // Pending dirty graphs organized by depth.
        // Index = depth, value = array of GraphState at that depth.
        private static NativeArray<NativeArray<GraphState>> pendingByDepth;

        // Maximum depth seen so far.
        private static int maxDepth;

        // Whether a flush is already scheduled (or running).
        private static bool flushScheduled;

        // Reentrancy guard for FlushAll / FlushNow.
        private static bool flushing;

        // Total number of pending graphs.
        private static int pendingCount;

        // Static initializer
        static GraphFlushCoordinator()
        {
            BatchingEnabled = false;
            Mode = GraphFlushMode.Microtask;
            pendingByDepth = new NativeArray<NativeArray<GraphState>>(8);
            for (int i = 0; i < 8; i++)
            {
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            flushScheduled = false;
            flushing = false;
            pendingCount = 0;
        }

        /// <summary>
        /// Number of graphs waiting for the next flush. Exposed for tests.
        /// </summary>
        public static int PendingCount
        {
            get { return pendingCount; }
        }

        // Register a dirty graph for the next flush cycle.
        // Called from GraphEngine.MarkDirty().
        public static void ScheduleDirty(GraphState state)
        {
            int depth = state.Depth;

            if (depth >= pendingByDepth.Length)
            {
                int newLength = depth + 4;
                NativeArray<NativeArray<GraphState>> grown = new NativeArray<NativeArray<GraphState>>(newLength);
                for (int i = 0; i < pendingByDepth.Length; i++)
                {
                    grown[i] = pendingByDepth[i];
                }
                for (int i = pendingByDepth.Length; i < newLength; i++)
                {
                    grown[i] = new NativeArray<GraphState>(0);
                }
                pendingByDepth = grown;
            }

            // Deduplicate: skip if this graph is already pending at this depth.
            NativeArray<GraphState> bucket = pendingByDepth[depth];
            int oldLen = bucket.Length;
            for (int i = 0; i < oldLen; i++)
            {
                if (bucket[i] == state) return;
            }

            // Append in-place using Push (JS array is dynamic).
            bucket.Push(state);

            if (depth > maxDepth)
            {
                maxDepth = depth;
            }

            pendingCount++;

            // While FlushAll is draining, flushScheduled stays true and the
            // outer pass loop picks up anything added during the drain.
            if (!flushScheduled)
            {
                flushScheduled = true;
                if (Mode == GraphFlushMode.Macrotask)
                {
                    TaskScheduler.Instance.EnqueHighPriTask(FlushAll, "GraphFlushCoordinator.FlushAll");
                }
                else
                {
                    TaskScheduler.Instance.EnqueueMicrotask(FlushAll, "GraphFlushCoordinator.FlushAll");
                }
            }
        }

        /// <summary>
        /// Flushes everything pending right now, synchronously. Escape hatch
        /// for code that writes a model and must read the DOM before the
        /// scheduled boundary (for example a layout measurement). No-op when
        /// nothing is pending or a flush is already running.
        /// </summary>
        public static void FlushNow()
        {
            if (pendingCount == 0 || flushing) return;
            // The scheduled callback still fires later and finds nothing to do.
            FlushAll();
        }

        // Flush all pending dirty graphs in depth order. Repeats while a flush
        // re-dirties a graph at a depth that was already drained in this pass.
        private static void FlushAll()
        {
            if (flushing) return;
            flushing = true;
            flushScheduled = true;

            try
            {
                int passCount = 0;
                while (pendingCount > 0)
                {
                    if (++passCount > MaxPasses)
                    {
                        Reset();
                        throw new Exception("Binding graphs did not settle after " + MaxPasses + " flush passes.");
                    }

                    // maxDepth is re-read every iteration: a parent flush can
                    // dirty a deeper child, which must still run in this pass.
                    for (int depth = 0; depth <= maxDepth; depth++)
                    {
                        NativeArray<GraphState> bucket = pendingByDepth[depth];
                        if (bucket.Length == 0) continue;

                        // Swap the bucket out first so graphs dirtied during
                        // this drain land in a fresh bucket for the next pass.
                        pendingByDepth[depth] = new NativeArray<GraphState>(0);
                        pendingCount -= bucket.Length;

                        for (int i = 0; i < bucket.Length; i++)
                        {
                            FlushOne(bucket[i]);
                        }
                    }
                }
            }
            finally
            {
                maxDepth = 0;
                flushScheduled = false;
                flushing = false;
            }
        }

        // Flush one graph. A graph that throws must not strand the others
        // (same contract as LayoutBatcher): log and continue.
        private static void FlushOne(GraphState state)
        {
            // Clear before flushing so a change raised during this flush can
            // schedule the graph again (Flush() also clears it, but returns
            // early for suspended graphs without doing so).
            state.FlushScheduled = false;
            if (state.Disposed || state.Suspended) return;

            try
            {
                GraphEngine.Flush(state.Descriptor, state);
            }
            catch (Exception ex)
            {
                Logger.Error("GraphFlushCoordinator: graph flush failed at depth "
                    + state.Depth + ": " + ex.Message);
            }
        }

        // Reset coordinator state. Useful for tests.
        public static void Reset()
        {
            for (int i = 0; i < pendingByDepth.Length; i++)
            {
                NativeArray<GraphState> bucket = pendingByDepth[i];
                for (int j = 0; j < bucket.Length; j++)
                {
                    bucket[j].FlushScheduled = false;
                }
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            pendingCount = 0;
            flushScheduled = false;
            flushing = false;
        }
    }
}
