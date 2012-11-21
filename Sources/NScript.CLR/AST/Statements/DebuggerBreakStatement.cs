namespace NScript.CLR.AST
{
    using NScript.Utils;

    /// <summary>
    /// Debugger break statement.
    /// </summary>
    public class DebuggerBreakStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebuggerBreakStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public DebuggerBreakStatement(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }
    }
}
