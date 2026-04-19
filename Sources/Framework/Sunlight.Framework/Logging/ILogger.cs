//-----------------------------------------------------------------------
// <copyright file="ILogger.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    /// <summary>
    /// Category-scoped logger surface returned by <see cref="Logger.ForCategory"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the "always-available" levels (Info / Warn / Error) are on the
    /// interface. Trace and Debug live on <see cref="NamedLogger"/> directly —
    /// those methods carry <see cref="System.Diagnostics.ConditionalAttribute"/>
    /// which C# does not allow on interface members. Exposing Trace/Debug here
    /// would silently bypass compile-time stripping when a caller accessed
    /// the logger through this interface, so we keep them off to fail fast.
    /// </para>
    /// <para>
    /// Callers that want compile-time stripping in Release builds should hold a
    /// <see cref="NamedLogger"/> reference (the concrete return type of
    /// <see cref="Logger.ForCategory"/>) rather than this interface.
    /// </para>
    /// </remarks>
    public interface ILogger
    {
        string Category { get; }

        void Info(string message);

        void Info(string message, string[] properties);

        void Warn(string message);

        void Warn(string message, string[] properties);

        void Error(string message);

        void Error(string message, string[] properties);
    }
}
