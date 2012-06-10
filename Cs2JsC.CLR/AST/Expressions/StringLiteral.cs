//-----------------------------------------------------------------------
// <copyright file="StringLiteral.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for StringLiteral
    /// </summary>
    public class StringLiteral : LiteralExpression
    {
        /// <summary>
        /// Backing field for String.
        /// </summary>
        private readonly string str;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="str">The string.</param>
        public StringLiteral(
            ClrContext context,
            Location location,
            string str)
            : base(context, location)
        {
            this.str = str;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <value>The string.</value>
        public string String
        {
            get
            {
                return this.str;
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
                return this.KnownReferences.String;
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
            serializationInfo.AddValue("value", this.String);
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
            StringLiteral right = obj as StringLiteral;

            return right != null
                && this.String == right.String;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(StringLiteral).GetHashCode() ^ this.String.GetHashCode();
        }
    }
}
