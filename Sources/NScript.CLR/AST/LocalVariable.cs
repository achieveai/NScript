namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Mono.Cecil;
    using VariableDefinition = Mono.Cecil.Cil.VariableDefinition;

    /// <summary>
    /// Local variables.
    /// </summary>
    public class LocalVariable : Variable
    {
        /// <summary>
        /// Backing field for local.
        /// </summary>
        VariableDefinition local;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalVariable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="definingScope">The defining scope.</param>
        public LocalVariable(
            VariableDefinition local,
            ScopeBlock definingScope)
            : base(
                LocalVariable.GetName(local),
                local.VariableType,
                definingScope)
        {
            this.local = local;
        }

        /// <summary>
        /// Gets the local.
        /// </summary>
        public VariableDefinition Local
        { get { return this.local; } }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Utils.ICustomSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddValue("name", this.Name);
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
            LocalVariable right = obj as LocalVariable;

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
            return this.Name.GetHashCode() + this.Type.GetHashCode();
        }

        private static string GetName(VariableDefinition variableDefinition)
        {
            return variableDefinition.ToString();
        }
    }
}
