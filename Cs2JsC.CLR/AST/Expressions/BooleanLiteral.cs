//-----------------------------------------------------------------------
// <copyright file="BooleanLiteral.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for BooleanLiteral
    /// </summary>
    public class BooleanLiteral : LiteralExpression
    {
        /// <summary>
        /// Backing field for Value.
        /// </summary>
        private readonly bool value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public BooleanLiteral(
            ClrContext context,
            Location location,
            bool value)
            : base(context, location)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BooleanLiteral"/> is value.
        /// </summary>
        /// <value><c>true</c> if value; otherwise, <c>false</c>.</value>
        public bool Value
        {
            get
            {
                return this.value;
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
                return this.KnownReferences.Boolean;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("value", this.Value);
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
            BooleanLiteral right = obj as BooleanLiteral;

            return right != null
                && this.Value == right.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(BooleanLiteral).GetHashCode()
                ^ this.Value.GetHashCode();
        }
    }
}
