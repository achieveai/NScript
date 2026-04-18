//-----------------------------------------------------------------------
// <copyright file="ILogSink.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    /// <summary>
    /// Destination for <see cref="LogEvent"/>s fanned out by <see cref="Logger"/>.
    /// Sinks own their internal async buffering / batching — the dispatch call
    /// from <see cref="Logger"/> is synchronous.
    /// </summary>
    public interface ILogSink
    {
        /// <summary>
        /// Called for every emitted event that passes <see cref="Logger.MinLevel"/>.
        /// Implementations must not throw — <see cref="Logger"/> does wrap calls in a
        /// try/catch, but a throwing sink is still a bug because it hides useful
        /// stack context from the developer.
        /// </summary>
        void Handle(LogEvent evt);

        /// <summary>
        /// Force any pending buffered events to flush to their destination.
        /// Console / no-buffer sinks implement this as a no-op.
        /// </summary>
        void Flush();

        /// <summary>
        /// Tear down any long-lived resources (timers, <c>beforeunload</c>
        /// listeners, etc.). Called by <see cref="Logger.RemoveSink"/> and
        /// <see cref="Logger.ClearSinks"/> so sinks can participate in clean
        /// lifecycle management rather than linger as leaked handles.
        /// </summary>
        void Detach();
    }
}
