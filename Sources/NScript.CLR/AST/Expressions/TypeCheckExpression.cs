//-----------------------------------------------------------------------
// <copyright file="TypeCheckExpression.cs" company="WebApps.Net">
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
    /// Definition for TypeCheckExpression
    /// </summary>
    public class TypeCheckExpression : Expression
    {
        /// <summary>
        /// Backing field for Type.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Backing field for CheckType.
        /// </summary>
        private readonly TypeCheckType checkType;

        /// <summary>
        /// Backing field for Expression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCheckExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="innerExpression">The inner expression.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="checkType">Type of the check.</param>
        public TypeCheckExpression(
            ClrContext context,
            Location location,
            Expression innerExpression,
            TypeReference typeReference,
            TypeCheckType checkType)
            : base(context, location)
        {
            this.innerExpression = innerExpression;
            this.typeReference = typeReference;
            this.checkType = checkType;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get
            {
                return this.innerExpression;
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
                return this.typeReference;
            }
        }

        /// <summary>
        /// Gets the type of the check.
        /// </summary>
        /// <value>The type of the check.</value>
        public TypeCheckType CheckType
        {
            get
            {
                return this.checkType;
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
                return this.checkType == TypeCheckType.IsType
                    ? this.KnownReferences.Boolean
                    : this.typeReference;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.innerExpression = (Expression)processor.Process(this.innerExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("checkType", this.CheckType);
            serializationInfo.AddValue("typeReference", this.Type.ToString());
            serializationInfo.AddValue("expression", this.Expression);
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
            TypeCheckExpression right = obj as TypeCheckExpression;

            return right != null
                && this.CheckType == right.CheckType
                && this.Expression.Equals(right.Expression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(TypeCheckExpression).GetHashCode() + this.Expression.GetHashCode() + (int)this.CheckType;
        }
    }
}
