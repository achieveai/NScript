namespace NScript.Template.Compiler
{
    using System;

    /// <summary>
    /// If template parser crashes due to any bug - the build fails with the message from the exception and the
    /// person doing the build cannot easily say what happened. So we handle System.Exception in
    /// <see cref="TemplateParserTask.Execute"/>, wrap the original exception in a well known TemplateParserException
    /// with a to-the-point error message stating that the tool crashed, the stacktrace, inner exception and so on.
    /// </summary>
    public class TemplateParserException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerException">The crash that actually happened in template parser</param>
        public TemplateParserException(Exception innerException)
            : base("Unexpected exception in template parser tool: " + innerException.Message, innerException)
        {
        }
    }
}