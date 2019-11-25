namespace NScript.CLR.AST
{
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Base class for all type of variables.
    /// </summary>
    public abstract class Variable : ICustomSerializable
    {
        /// <summary>
        /// Backing field for name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Backing field for Type.
        /// </summary>
        private readonly TypeReference type;

        /// <summary>
        /// Backing field for Defining scope
        /// </summary>
        private ScopeBlock definingScope;

        private bool isHoisted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="definingScope">The defining scope.</param>
        protected Variable(
            string name,
            TypeReference type,
            ScopeBlock definingScope)
        {
            this.name = name;
            this.type = type;
            this.definingScope = definingScope;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public TypeReference Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Gets the defining scope.
        /// </summary>
        /// <value>The defining scope.</value>
        public ScopeBlock DefiningScope
        {
            get
            {
                return this.definingScope;
            }

            internal set
            {
                this.definingScope = value;
            }
        }

        /// <summary>
        /// Returns if this variable is really used in anonymous delegate.
        /// </summary>
        public bool IsHoisted
        {
            get { return this.isHoisted; }
        }

        /// <summary>
        /// Sets the hoisted flag.
        /// </summary>
        public void SetHoisted()
        {
            this.isHoisted = true;
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public virtual void Serialize(ICustomSerializer serializer)
        {
            if (this.isHoisted)
            {
                serializer.AddValue("isHoisted", true);
            }
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
            Variable right = obj as Variable;

            return right != null
                && this.Name == right.Name
                && this.Type.Equals(right.Type);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
