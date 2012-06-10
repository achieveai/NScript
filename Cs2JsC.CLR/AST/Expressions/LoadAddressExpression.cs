//-----------------------------------------------------------------------
// <copyright file="LoadAddressExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for LoadAddressExpression
    /// </summary>
    public class LoadAddressExpression : Expression
    {
        /// <summary>
        /// Backing field for NestedExpression
        /// </summary>
        private Expression nestedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadAddressExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="nestedExpression">The nested expression.</param>
        public LoadAddressExpression(
            ClrContext context,
            Location location,
            Expression nestedExpression)
            : base(context, location)
        {
            this.nestedExpression = nestedExpression;
        }

        /// <summary>
        /// Gets the nested expression.
        /// </summary>
        /// <value>The nested expression.</value>
        public Expression NestedExpression
        {
            get { return this.nestedExpression; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.nestedExpression.ResultType;
            }
        }

        /// <summary>
        /// Gets the load address expression.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>LoadAddressExpression for node.</returns>
        internal static LoadAddressExpression GetLoadAddressExpression(Node node)
        {
            if (node is VariableReference)
            {
                VariableReference variableReference = (VariableReference)node;
                return new VariableAddressReference(
                    node.Context,
                    node.Location,
                    ((VariableReference)node).Variable);
            }
            else if (node is LoadAddressExpression)
            {
                return (LoadAddressExpression)node;
            }

            return new LoadAddressExpression(node.Context, node.Location, (Expression)node);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.nestedExpression = (Expression) processor.Process(this.nestedExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
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
            LoadAddressExpression right = obj as LoadAddressExpression;

            return right != null
                && right.NestedExpression.Equals(right.NestedExpression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(LoadAddressExpression).GetHashCode()
                ^ this.NestedExpression.GetHashCode();
        }
    }
}
