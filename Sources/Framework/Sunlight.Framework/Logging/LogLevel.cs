//-----------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    /// <summary>
    /// Severity classification for <see cref="Logger"/> and <see cref="Sunlight.Framework.ILogger"/> entries.
    /// </summary>
    /// <remarks>
    /// Values are stable and must not change — the numeric level is emitted to sinks
    /// (ConsoleSink / HttpLogSink) as part of the structured payload.
    /// Trace is -1 so Debug/Info/Warn/Error remain at their original 0..3 values.
    /// </remarks>
    public enum LogLevel
    {
        Trace = -1,
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }
}
