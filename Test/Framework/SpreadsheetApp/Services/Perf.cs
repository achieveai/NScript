namespace SpreadsheetApp.Services
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Html;

    /// <summary>
    /// Facades over the small measurement helper defined in SpreadsheetApp.htm
    /// (window.__perf). The helper owns the MutationObserver and timing; the
    /// app only hands it an operation and a completion callback.
    /// </summary>
    public static class Perf
    {
        [Script("return window.performance.now();")]
        public static extern double Now();

        /// <summary>
        /// Runs the operation and reports elapsed milliseconds once the microtask
        /// queue has drained (including a batched flush) and a forced layout has
        /// run. With countWrites the DOM mutation records under root are counted
        /// by a MutationObserver instead; that observer skews timing, so timed
        /// passes run without it and report -1 writes.
        /// </summary>
        [Script("window.__perf.measure(root, countWrites, op, done);")]
        public static extern void Measure(Element root, bool countWrites, Action op, Action<double, int> done);

        /// <summary>Publishes the benchmark entry points as window.__sheet.</summary>
        [Script("window.__perf.expose(runAll, isRunning, resultsJson, setBatching, op);")]
        public static extern void Expose(Action runAll, Func<bool> isRunning, Func<string> resultsJson,
            Action<bool> setBatching, Action<string> op);
    }
}
