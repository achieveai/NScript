//-----------------------------------------------------------------------
// <copyright file="ParseException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ParseException
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException(
            string message,
            int lineNumber,
            int linePosition)
            :base(message)
        {
            this.Line = lineNumber;
            this.Position = linePosition;
        }
        public ParseException(
            string message,
            int lineNumber,
            int linePosition,
            Exception baseException)
            :base(message, baseException)
        {
            this.Line = lineNumber;
            this.Position = linePosition;
        }

        public ParseException(
            Antlr.Runtime.RecognitionException ex)
            : this(ex.Message, ex.Line, ex.CharPositionInLine, ex)
        { }

        public int Position { get; set; }

        public int Line { get; set; }
    }
}
