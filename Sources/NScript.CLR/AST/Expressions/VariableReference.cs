namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Variable references.
    /// </summary>
    public class VariableReference : Expression
    {
        /// <summary>
        /// Backing field for variable.
        /// </summary>
        private readonly Variable variable;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReference"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="variable">The variable.</param>
        public VariableReference(
            ClrContext context,
            Location location,
            Variable variable)
            : base(context, location)
        {
            this.variable = variable;
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <value>The variable.</value>
        public Variable Variable
        {
            get
            {
                return this.variable;
            }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.Variable.Type;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("variable", this.variable);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            VariableReference right = obj as VariableReference;

            return right != null
                && this.Variable.Equals(right.Variable);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Variable.GetHashCode();
        }
    }
}
