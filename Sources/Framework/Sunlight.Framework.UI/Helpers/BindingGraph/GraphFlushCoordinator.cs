namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using System;

    /// <summary>
    /// Global flush coordinator. Collects dirty graphs and flushes them
    /// in depth order on a single microtask boundary.
    /// </summary>
    public static class GraphFlushCoordinator
    {
        // Pending dirty graphs organized by depth.
        // Index = depth, value = array of GraphState at that depth.
        private static NativeArray<NativeArray<GraphState>> pendingByDepth;

        // Maximum depth seen so far.
        private static int maxDepth;

        // Whether a flush microtask is already scheduled.
        private static bool flushScheduled;

        // Total number of pending graphs.
        private static int pendingCount;

        // Static initializer
        static GraphFlushCoordinator()
        {
            pendingByDepth = new NativeArray<NativeArray<GraphState>>(8);
            for (int i = 0; i < 8; i++)
            {
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            flushScheduled = false;
            pendingCount = 0;
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

            if (!flushScheduled)
            {
                flushScheduled = true;
                TaskScheduler.Instance.EnqueHighPriTask(FlushAll, "GraphFlushCoordinator.FlushAll");
            }
        }

        // Flush all pending dirty graphs in depth order.
        // Runs on microtask boundary via TaskScheduler.
        private static void FlushAll()
        {
            for (int depth = 0; depth <= maxDepth; depth++)
            {
                NativeArray<GraphState> bucket = pendingByDepth[depth];
                for (int i = 0; i < bucket.Length; i++)
                {
                    GraphState state = bucket[i];
                    GraphEngine.Flush(state.Descriptor, state);
                }
                pendingByDepth[depth] = new NativeArray<GraphState>(0);
            }

            pendingCount = 0;
            flushScheduled = false;
        }

        // Reset coordinator state. Useful for tests.
        public static void Reset()
        {
            for (int i = 0; i < pendingByDepth.Length; i++)
            {
                pendingByDepth[i] = new NativeArray<GraphState>(0);
            }
            maxDepth = 0;
            pendingCount = 0;
            flushScheduled = false;
        }
    }
}
