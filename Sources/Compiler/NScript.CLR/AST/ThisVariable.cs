namespace NScript.CLR.AST
{
    using Mono.Cecil;

    /// <summary>
    /// Variable representing this class.
    /// </summary>
    public class ThisVariable : ParameterVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThisVariable"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="definingScope">The defining scope.</param>
        public ThisVariable(
            ParameterDefinition paramDef,
            ParameterBlock definingScope)
            : base(paramDef, definingScope)
        {}
    }
}
