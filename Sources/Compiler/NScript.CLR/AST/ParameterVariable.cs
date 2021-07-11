namespace NScript.CLR.AST
{
    using Mono.Cecil;

    /// <summary>
    /// Parameter variables.
    /// </summary>
    public class ParameterVariable : Variable
    {
        /// <summary>
        /// Backing field for ParameterType.
        /// </summary>
        private readonly ParameterDefinition paramReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterVariable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="paramType">Type of the parameter.</param>
        /// <param name="type">The type.</param>
        /// <param name="definingScope">The defining scope.</param>
        public ParameterVariable(
            ParameterDefinition paramReference,
            ParameterBlock definingScope)
            : base(
                paramReference.Name,
                paramReference.ParameterType,
                definingScope)
        {
            this.paramReference = paramReference;
        }

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public ParameterAttributes ParameterType
        { get { return this.paramReference.Attributes; } }

        /// <summary>
        /// Gets the parameter reference.
        /// </summary>
        public ParameterReference ParameterReference
        { get { return this.paramReference; } }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Utils.ICustomSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddValue("name", this.Name);
            serializer.AddValue("parameterType", this.ParameterType);
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
            ParameterVariable right = obj as ParameterVariable;

            return right != null
                && this.Name == right.Name;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + (int)this.ParameterType;
        }
    }
}
