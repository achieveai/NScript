//-----------------------------------------------------------------------
// <copyright file="ConvertTypeExpression.cs" company="WebApps.Net">
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
    /// Definition for ConvertTypeExpression
    /// </summary>
    public class ConvertTypeExpression : Expression
    {
        /// <summary>
        /// Backing field for ResultType.
        /// </summary>
        private readonly TypeReference resultType;

        /// <summary>
        /// Backing field for nested expression.
        /// </summary>
        private readonly Expression nestedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertTypeExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="nestedExpression">The nested expression.</param>
        /// <param name="resultType">Type of the result.</param>
        public ConvertTypeExpression(
            ClrContext context,
            Location location,
            Expression nestedExpression,
            TypeReference resultType)
            : base(context, location)
        {
            this.resultType = resultType;
            this.nestedExpression = nestedExpression;
        }

        /// <summary>
        /// Gets the nested expression.
        /// </summary>
        /// <value>The nested expression.</value>
        public Expression NestedExpression
        {
            get
            {
                return this.nestedExpression;
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
                return this.resultType;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("paramDef", this.resultType.ToString());
            serializationInfo.AddValue("nestedExpression", this.nestedExpression);
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
            ConvertTypeExpression right = obj as ConvertTypeExpression;

            return right != null
                && this.ResultType.Equals(right.ResultType)
                && this.NestedExpression.Equals(right.NestedExpression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(ConvertTypeExpression).GetHashCode()
                ^ this.ResultType.GetHashCode()
                ^ this.NestedExpression.GetHashCode();
        }
    }
}
