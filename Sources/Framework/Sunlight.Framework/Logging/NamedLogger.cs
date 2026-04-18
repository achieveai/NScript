//-----------------------------------------------------------------------
// <copyright file="NamedLogger.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework
{
    using System.Diagnostics;

    /// <summary>
    /// Category-scoped logger returned by <see cref="Logger.ForCategory"/>.
    /// Delegates every call to the <see cref="Logger"/> dispatch path, tagging
    /// events with this instance's <see cref="Category"/>.
    /// </summary>
    /// <remarks>
    /// Trace/Debug are declared directly on this concrete type (not on
    /// <see cref="ILogger"/>) so <c>[Conditional("DEBUG")]</c> strips call sites
    /// in Release builds. C# does not allow <c>[Conditional]</c> on interface
    /// members — see <see cref="ILogger"/> for the rationale.
    /// </remarks>
    public class NamedLogger : ILogger
    {
        private readonly string category;

        internal NamedLogger(string category)
        {
            this.category = category;
        }

        public string Category
        {
            get { return this.category; }
        }

        [Conditional("DEBUG")]
        public void Trace(string message)
        {
            Logger.DispatchInternal(LogLevel.Trace, this.category, message, null);
        }

        [Conditional("DEBUG")]
        public void Trace(string message, string[] properties)
        {
            Logger.DispatchInternal(LogLevel.Trace, this.category, message, properties);
        }

        [Conditional("DEBUG")]
        public void Debug(string message)
        {
            Logger.DispatchInternal(LogLevel.Debug, this.category, message, null);
        }

        [Conditional("DEBUG")]
        public void Debug(string message, string[] properties)
        {
            Logger.DispatchInternal(LogLevel.Debug, this.category, message, properties);
        }

        public void Info(string message)
        {
            Logger.DispatchInternal(LogLevel.Info, this.category, message, null);
        }

        public void Info(string message, string[] properties)
        {
            Logger.DispatchInternal(LogLevel.Info, this.category, message, properties);
        }

        public void Warn(string message)
        {
            Logger.DispatchInternal(LogLevel.Warn, this.category, message, null);
        }

        public void Warn(string message, string[] properties)
        {
            Logger.DispatchInternal(LogLevel.Warn, this.category, message, properties);
        }

        public void Error(string message)
        {
            Logger.DispatchInternal(LogLevel.Error, this.category, message, null);
        }

        public void Error(string message, string[] properties)
        {
            Logger.DispatchInternal(LogLevel.Error, this.category, message, properties);
        }
    }
}
