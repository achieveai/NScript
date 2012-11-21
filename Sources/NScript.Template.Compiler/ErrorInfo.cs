using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Template.Compiler
{
    public enum ErrorType
    {
        NotFound,
        Compile
    }

    public enum FileType
    {
        Reference,
        Template
    }

    public class ErrorInfo
    {
        public string FileName      { get; private set; }
        public FileType FileType    { get; private set; }
        public ErrorType ErrorType  { get; private set; }

        public int LineNumber       { get; private set; }
        public int Column           { get; private set; }

        public string ErrorMessage  { get; private set; }

        /// <summary>
        /// Constructor used for errors coming from the argument parsing and file include validation.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <param name="errorType"></param>
        public ErrorInfo(string fileName, FileType fileType, ErrorType errorType)
        {
            this.FileName = fileName;
            this.FileType = FileType;
            this.ErrorType = errorType;
        }

        /// <summary>
        /// Constructor used for errors coming from the parsing.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        /// <param name="column"></param>
        /// <param name="errorMessage"></param>
        public ErrorInfo(string fileName, int lineNumber, int column, string errorMessage)
        {
            this.FileName = fileName;
            this.LineNumber = lineNumber;
            this.Column = column;
            this.ErrorMessage = errorMessage;

            // Not really used
            this.FileType = FileType.Template;
            this.ErrorType = ErrorType.Compile;
        }
    }
}
