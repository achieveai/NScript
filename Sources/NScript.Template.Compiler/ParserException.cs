using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NScript.Template.Compiler
{
    public class ParserException : ApplicationException
    {
        private readonly int lineNumber;
        private readonly int columnNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        public ParserException(
            string message,
            int lineNumber,
            int columnNumber)
            : this(message, lineNumber, columnNumber, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="innerException">The inner exception.</param>
        public ParserException(
            string message,
            int lineNumber,
            int columnNumber,
            Exception innerException)
            : base(message, innerException)
        {
            this.lineNumber = lineNumber;
            this.columnNumber = columnNumber;
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber
        {
            get { return this.lineNumber; }
        }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        /// <value>The column number.</value>
        public int ColumnNumber
        {
            get { return this.columnNumber; }
        }
    }
}