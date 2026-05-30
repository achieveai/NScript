// -----------------------------------------------------------------------
// <copyright file="TestLoggerProvider.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// In-memory <see cref="ILoggerProvider"/> that captures every MEL
    /// emission and its scope state. Used to assert what
    /// <see cref="Services.LogIngestionService"/> forwards to the
    /// configured logging pipeline.
    /// </summary>
    public sealed class TestLoggerProvider : ILoggerProvider
    {
        public readonly ConcurrentQueue<CapturedLog> Logs = new ConcurrentQueue<CapturedLog>();
        private readonly ConcurrentDictionary<string, TestLogger> _loggers = new ConcurrentDictionary<string, TestLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            return this._loggers.GetOrAdd(categoryName, name => new TestLogger(name, this));
        }

        public void Dispose() { }

        /// <summary> One captured log line + its scope state. </summary>
        public sealed class CapturedLog
        {
            public string Category { get; set; } = string.Empty;
            public LogLevel Level { get; set; }
            public string Message { get; set; } = string.Empty;
            public Exception? Exception { get; set; }
            public IReadOnlyDictionary<string, object?> Scope { get; set; } = new Dictionary<string, object?>();
        }

        private sealed class TestLogger : ILogger
        {
            private readonly string _category;
            private readonly TestLoggerProvider _owner;

            // Per-thread scope stack so concurrent test ingests don't
            // cross-contaminate. AsyncLocal would be more correct, but
            // the synchronous test paths don't await across scope
            // boundaries.
            [ThreadStatic]
            private static Stack<IReadOnlyDictionary<string, object?>>? _scopeStack;

            public TestLogger(string category, TestLoggerProvider owner)
            {
                this._category = category;
                this._owner = owner;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                var dict = state as IReadOnlyDictionary<string, object?>;
                if (dict == null)
                {
                    var newDict = new Dictionary<string, object?>();
                    if (state is IEnumerable<KeyValuePair<string, object?>> enumerable)
                    {
                        foreach (var pair in enumerable)
                        {
                            newDict[pair.Key] = pair.Value;
                        }
                    }
                    dict = newDict;
                }
                if (_scopeStack == null) { _scopeStack = new Stack<IReadOnlyDictionary<string, object?>>(); }
                _scopeStack.Push(dict);
                return new PopScope();
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                var captured = new TestLoggerProvider.CapturedLog
                {
                    Category = this._category,
                    Level = logLevel,
                    Message = formatter(state, exception),
                    Exception = exception,
                };

                // Merge top-of-stack scope so the test sees what scope
                // the producer pushed without having to mock the inner
                // BeginScope contract.
                if (_scopeStack != null && _scopeStack.Count > 0)
                {
                    captured.Scope = _scopeStack.Peek();
                }
                this._owner.Logs.Enqueue(captured);
            }

            private sealed class PopScope : IDisposable
            {
                public void Dispose()
                {
                    if (_scopeStack != null && _scopeStack.Count > 0)
                    {
                        _scopeStack.Pop();
                    }
                }
            }
        }
    }
}
