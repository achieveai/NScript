//-----------------------------------------------------------------------
// <copyright file="DoubleLiteral.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for DoubleLiteral
    /// </summary>
    public class DoubleLiteral : LiteralExpression
    {
        /// <summary>
        /// Backing field for isSingle.
        /// </summary>
        private readonly bool isSingle;

        /// <summary>
        /// Backing field for Value.
        /// </summary>
        private readonly double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleLiteral"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public DoubleLiteral(
            ClrContext context,
            Location location,
            float value)
            : base(context, location)
        {
            this.isSingle = true;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleLiteral"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public DoubleLiteral(
            ClrContext context,
            Location location,
            double value)
            : base(context, location)
        {
            this.isSingle = false;
            this.value = value;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is single.
        /// </summary>
        /// <value><c>true</c> if this instance is single; otherwise, <c>false</c>.</value>
        public bool IsSingle
        {
            get
            {
                return this.isSingle;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
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
                return this.isSingle
                    ? this.KnownReferences.Single
                    : this.KnownReferences.Double;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("isSingle", this.isSingle);
            serializationInfo.AddValue("value", this.value);
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
            DoubleLiteral right = obj as DoubleLiteral;

            return right != null
                && this.IsSingle == right.IsSingle
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
            return typeof(DoubleLiteral).GetHashCode()
                ^ this.Value.GetHashCode();
        }
    }
}
