//-----------------------------------------------------------------------
// <copyright file="NScriptTask.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript
{
    using NScript.Utils;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Definition for NScriptTask
    /// </summary>
    public class NScriptTask : Task, ILog
    {
        /// <summary>
        ///  Were any errors found or not.
        /// </summary>
        private bool foundErrors = false;

        /// <summary>
        /// Gets or sets the name of the js file.
        /// </summary>
        /// <value>
        /// The name of the js file.
        /// </value>
        [Required]
        public string JsFileName { get; set; }

        /// <summary>
        /// Gets or sets the reference files.
        /// </summary>
        /// <value>
        /// The reference files.
        /// </value>
        [Required]
        public string ReferenceFiles { get; set; }

        /// <summary>
        /// Gets or sets the main assembly.
        /// </summary>
        /// <value>
        /// The main assembly.
        /// </value>
        [Required]
        public string MainAssembly { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return this.foundErrors; }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            Utils.Logger.Instance = this;
            return !this.foundErrors;
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void LogError(string error)
        {
            foundErrors = true;
            Log.LogError(error);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        public void LogError(ErrorInfo errorInfo)
        {
            foundErrors = true;
            Log.LogError(
                "",
                errorInfo.ErrorCode,
                "",
                errorInfo.FileName,
                errorInfo.LineNumber,
                errorInfo.Column,
                errorInfo.LineNumber,
                errorInfo.Column,
                errorInfo.ErrorMessage);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        public void LogWarning(string warning)
        {
            Log.LogWarning(warning);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="warningInfo">The warning info.</param>
        public void LogWarning(ErrorInfo warningInfo)
        {
            Log.LogWarning(
                "",
                warningInfo.ErrorCode,
                "",
                warningInfo.FileName,
                warningInfo.LineNumber,
                warningInfo.Column,
                warningInfo.LineNumber,
                warningInfo.Column,
                warningInfo.ErrorMessage);
        }
    }
}