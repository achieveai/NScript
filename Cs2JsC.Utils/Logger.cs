//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    using System;

    /// <summary>
    /// Definition for Logger
    /// </summary>
    public class Logger : ILog
    {
        /// <summary>
        /// Instance for active logger.
        /// </summary>
        private static ILog instance;

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ILog Instance
        {
            get
            {
                if (Logger.instance == null)
                {
                    Logger.instance = new Logger();
                }

                return Logger.instance;
            }

            set
            {
                Logger.instance = value;
            }
        }

        /// <summary>
        /// Backing field for HasErrors.
        /// </summary>
        private bool hasErrors = false;

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void LogError(string error)
        {
            this.hasErrors = true;
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            try
            {
                Console.Error.WriteLine(error);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        public void LogError(ErrorInfo errorInfo)
        {
            this.LogError(
                string.Format(
                    "{0}({1},{2}): error {3}: {4}",
                    errorInfo.FileName,
                    errorInfo.LineNumber,
                    errorInfo.Column,
                    errorInfo.ErrorCode,
                    errorInfo.ErrorMessage));
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void LogWarning(string warning)
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            try
            {
                Console.Error.WriteLine(warning);
            }
            finally
            {
                Console.ForegroundColor = currentColor;
            }
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warningInfo">The warning info.</param>
        public void LogWarning(ErrorInfo warningInfo)
        {
            this.LogWarning(
                string.Format(
                    "{0}({1},{2}): warning {3}: {4}",
                    warningInfo.FileName,
                    warningInfo.LineNumber,
                    warningInfo.Column,
                    warningInfo.ErrorCode,
                    warningInfo.ErrorMessage));
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return this.hasErrors; }
        }
    }
}