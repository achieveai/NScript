namespace NScript.CLR.AST
{
    using NScript.Utils;

    /// <summary>
    /// Statement representing continue within a loop.
    /// </summary>
    public class ContinueStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public ContinueStatement(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
        }
    }
}
