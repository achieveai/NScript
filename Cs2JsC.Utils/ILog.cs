//-----------------------------------------------------------------------
// <copyright file="ILog.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    /// <summary>
    /// Definition for ILog
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="error">The error.</param>
        void LogError(string error);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        void LogError(ErrorInfo errorInfo);

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        void LogWarning(string warning);

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warningInfo">The warning info.</param>
        void LogWarning(ErrorInfo warningInfo);

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors
        { get; }
    }
}