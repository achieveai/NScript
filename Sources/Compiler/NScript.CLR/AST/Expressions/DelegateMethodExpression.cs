//-----------------------------------------------------------------------
// <copyright file="DelegateMethodExpression.cs" company="">
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
    /// Definition for DelegateMethodExpression
    /// </summary>
    public class DelegateMethodExpression : Expression
    {
        /// <summary>
        /// Delegate type.
        /// </summary>
        private readonly TypeReference delegateType;

        /// <summary>
        /// Backing field for Method
        /// </summary>
        private MethodReferenceExpression methodExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateMethodExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="methodExpression">The method expression.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        public DelegateMethodExpression(
            ClrContext context,
            Location location,
            MethodReferenceExpression methodExpression,
            TypeReference delegateType)
            : base(context, location)
        {
            this.methodExpression = methodExpression;
            this.delegateType = delegateType;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public MethodReferenceExpression Method
        {
            get { return this.methodExpression; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.delegateType;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.methodExpression = (MethodReferenceExpression) processor.Process(this.Method);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("delegateType", this.ResultType.ToString());
            serializationInfo.AddValue("methodReference", this.Method);
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
            DelegateMethodExpression right = obj as DelegateMethodExpression;

            return right != null
                && this.Method.Equals(right.Method);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(DelegateMethodExpression).GetHashCode()
                ^ this.Method.GetHashCode();
        }
    }
}
