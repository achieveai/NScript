using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Template.Compiler
{
    public class ConsoleLogger : ILog
    {
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogError(string message)
        {
            PrintError(message);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogWarning(string message)
        {
            PrintWarning(message);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        public void LogWarning(ErrorInfo errorInfo)
        {
            if (errorInfo.ErrorType == ErrorType.NotFound)
            {
                if (errorInfo.FileType == FileType.Reference)
                {
                    PrintWarning(String.Format("Template file {0} not found", errorInfo.FileName));
                }
                else
                {
                    PrintWarning(String.Format("Reference file {0} not found", errorInfo.FileName));
                }
            }
            else if (errorInfo.ErrorType == ErrorType.Compile)
            {
                PrintWarning(
                    String.Format(
                        "Parsing warning for {0} at line#{1}, col#{2}, message: {3}",
                        errorInfo.FileName,
                        errorInfo.LineNumber,
                        errorInfo.Column,
                        errorInfo.ErrorMessage));
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorInfo">The error info.</param>
        public void LogError(ErrorInfo errorInfo)
        {
            if (errorInfo.ErrorType == ErrorType.NotFound)
            {
                if (errorInfo.FileType == FileType.Reference)
                {
                    PrintError(String.Format("Template file {0} not found", errorInfo.FileName));
                }
                else
                {
                    PrintError(String.Format("Reference file {0} not found", errorInfo.FileName));
                }
            }
            else if (errorInfo.ErrorType == ErrorType.Compile)
            {
                PrintWarning(
                    String.Format(
                        "Parsing error for {0} at line#{1}, col#{2}, message: {3}",
                        errorInfo.FileName,
                        errorInfo.LineNumber,
                        errorInfo.Column,
                        errorInfo.ErrorMessage));
            }
        }

        /// <summary>
        /// Prints an error to the console.error stream in red.
        /// </summary>
        /// <param name="error">error to print</param>
        private void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(error);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints the warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        private void PrintWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine(warning);
            Console.ResetColor();
        }
    }
}
