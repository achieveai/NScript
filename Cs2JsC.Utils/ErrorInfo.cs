//-----------------------------------------------------------------------
// <copyright file="ErrorInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ErrorInfo
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Backing field for FileName.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// Backing field for errorCode.
        /// </summary>
        private readonly string errorCode;

        /// <summary>
        /// Backing field for Message.
        /// </summary>
        private readonly string message;

        /// <summary>
        /// Backing field for LineNumber.
        /// </summary>
        private readonly int lineNumber = -1;

        /// <summary>
        /// Backing field for Column.
        /// </summary>
        private readonly int column = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="column">The column.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ErrorInfo(
            string fileName,
            int lineNumber,
            int column,
            string errorCode,
            string errorMessage)
        {
            this.fileName = fileName;
            this.lineNumber = lineNumber;
            this.column = column;
            this.errorCode = errorCode;
            this.message = errorMessage;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName
        {
            get { return this.fileName; }
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber
        {
            get { return this.lineNumber; }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string ErrorCode
        {
            get { return this.errorCode; }
        }
    }
}
