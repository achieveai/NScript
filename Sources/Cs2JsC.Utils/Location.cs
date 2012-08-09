//-----------------------------------------------------------------------
// <copyright file="Location.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    /// <summary>
    /// Definition for Location
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Backing field for FileName.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// Backing field for startLine.
        /// </summary>
        private readonly int startLine;

        /// <summary>
        /// Backing field for StartColumn.
        /// </summary>
        private readonly int startColumn;

        /// <summary>
        /// Backing field for EncLine.
        /// </summary>
        private readonly int endLine;

        /// <summary>
        /// Backing field for EncColumn
        /// </summary>
        private readonly int endColumn;

        public Location(string fileName, int startLine, int startColumn, int endLine, int endColumn)
        {
            this.fileName = fileName;
            this.startLine = startLine;
            this.startColumn = startColumn;
            this.endLine = endLine;
            this.endColumn = endColumn;
        }

        public Location(string fileName, int startLine, int startColumn)
        {
            this.fileName = fileName;
            this.startLine = startLine;
            this.startColumn = startColumn;
            this.endLine = int.MaxValue;
            this.endColumn = int.MaxValue;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        /// <summary>
        /// Gets the start line.
        /// </summary>
        /// <value>The start line.</value>
        public int StartLine
        {
            get
            {
                return this.startLine;
            }
        }

        /// <summary>
        /// Gets the start column.
        /// </summary>
        /// <value>The start column.</value>
        public int StartColumn
        {
            get
            {
                return this.startColumn;
            }
        }

        /// <summary>
        /// Gets the end line.
        /// </summary>
        /// <value>The end line.</value>
        public int EndLine
        {
            get
            {
                return this.endLine;
            }
        }

        /// <summary>
        /// Gets the end column.
        /// </summary>
        /// <value>The end column.</value>
        public int EndColumn
        {
            get
            {
                return this.endColumn;
            }
        }

        /// <summary>
        /// Determines whether the specified other is before.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// <c>true</c> if the specified other is before; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBefore(Location other)
        {
            int rv = this.fileName.CompareTo(other.fileName);

            if (rv != 0)
            {
                return rv < 0;
            }

            rv = this.startLine - other.startLine;

            if (rv != 0)
            {
                return rv < 0;
            }

            return this.startColumn < other.startColumn;
        }

        /// <summary>
        /// Determines whether the specified other is after.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        /// <c>true</c> if the specified other is after; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAfter(Location other)
        {
            int rv = this.fileName.CompareTo(other.fileName);

            if (rv != 0)
            {
                return rv > 0;
            }

            rv = this.endLine - other.endLine;

            if (rv != 0)
            {
                return rv > 0;
            }

            return this.endColumn > other.endColumn;
        }
    }
}