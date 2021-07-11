//-----------------------------------------------------------------------
// <copyright file="NewArrayExpression.cs" company="WebApps.Net">
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
    /// Definition for NewArrayExpression
    /// </summary>
    public class NewArrayExpression : Expression
    {
        /// <summary>
        /// Backing field for TypeReference.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Backing field for ResultType.
        /// </summary>
        private readonly TypeReference resultType;

        /// <summary>
        /// Backing field for ArraySize.
        /// </summary>
        private Expression arraySize;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewArrayExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="paramDef">The type reference.</param>
        /// <param name="arraySizeExpression">The array size expression.</param>
        public NewArrayExpression(
            ClrContext context,
            Location location,
            TypeReference typeReference,
            Expression arraySizeExpression)
            : base(context, location)
        {
            this.typeReference = typeReference;
            this.arraySize = arraySizeExpression;
            this.resultType = new ArrayType(typeReference);
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
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Expression Size
        {
            get
            {
                return this.arraySize;
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
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.arraySize = (Expression)processor.Process(this.arraySize);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("typeReference", this.Type.ToString());
            serializationInfo.AddValue("size", this.Size);
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
            NewArrayExpression right = obj as NewArrayExpression;

            return right != null
                && this.Type.Equals(right.Type)
                && this.Size.Equals(right.Size);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(NewArrayExpression).GetHashCode()
                ^ this.Size.GetHashCode()
                ^ this.Type.GetHashCode();
        }
    }
}
