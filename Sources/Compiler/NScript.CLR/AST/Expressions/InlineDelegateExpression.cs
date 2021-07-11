//-----------------------------------------------------------------------
// <copyright file="InlineDelegateExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InlineDelegateExpression
    /// </summary>
    public class InlineDelegateExpression : Expression
    {
        /// <summary>
        /// Backing field for Delegate
        /// </summary>
        DelegateMethodExpression delegateExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineDelegateExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="delegateExpression">The delegate expression.</param>
        public InlineDelegateExpression(
            ClrContext context,
            DelegateMethodExpression delegateExpression)
            : base(context, delegateExpression.Location)
        {
            this.delegateExpression = delegateExpression;
        }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        public DelegateMethodExpression Delegate
        {
            get { return this.delegateExpression; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override TypeReference ResultType
        {
            get
            {
                return this.Delegate.ResultType;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("delegate", this.Delegate);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.delegateExpression = (DelegateMethodExpression)processor.Process(this.delegateExpression);
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
            InlineDelegateExpression right = obj as InlineDelegateExpression;

            return right != null
                && this.Delegate.Equals(right.Delegate);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(InlineDelegateExpression).GetHashCode()
                ^ this.Delegate.GetHashCode();
        }
    }
}
