//-----------------------------------------------------------------------
// <copyright file="PropertyReferenceExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for PropertyReferenceExpression
    /// </summary>
    public class PropertyReferenceExpression : MemberReferenceExpression
    {
        /// <summary>
        /// Backing collection for arguments.
        /// </summary>
        private readonly List<Expression> arguments =
            new List<Expression>();

        /// <summary>
        /// Backing field for FieldReference.
        /// </summary>
        private readonly PropertyReference propertyReference;

        /// <summary>
        /// Backing field for Arguments
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="propertyReference">The property reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="arguments">The arguments.</param>
        public PropertyReferenceExpression(
            ClrContext context,
            Location location,
            PropertyReference propertyReference,
            Expression leftExpression,
            IEnumerable<Expression> arguments = null)
            : base(context, location, propertyReference, leftExpression)
        {
            if (propertyReference.Resolve().IsStatic())
            {
                throw new ArgumentException("static member can't have LeftExpression");
            }

            this.propertyReference = propertyReference;

            if (arguments != null)
            {
                this.arguments.AddRange(arguments);
            }

            this.readonlyArguments = new ReadOnlyCollection<Expression>(this.arguments);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="propertyReference">The property reference.</param>
        public PropertyReferenceExpression(
            ClrContext context,
            Location location,
            PropertyReference propertyReference)
            : base(context, location, propertyReference, null)
        {
            if (!propertyReference.Resolve().IsStatic())
            {
                throw new ArgumentException("leftExpression not passed for instance member");
            }

            this.propertyReference = propertyReference;
            this.readonlyArguments = new ReadOnlyCollection<Expression>(this.arguments);
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <value>The property reference.</value>
        public PropertyReference PropertyReference
        {
            get
            {
                return this.propertyReference;
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<Expression> Arguments
        {
            get { return this.readonlyArguments; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.propertyReference.PropertyType.FixGenericTypeArguments(
                    this.propertyReference.DeclaringType);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("propertyName", this.PropertyReference.Name);
            serializationInfo.AddValue("arguments", this.Arguments);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int argumentIndex = 0; argumentIndex < this.arguments.Count; argumentIndex++)
            {
                this.arguments[argumentIndex] = (Expression)processor.Process(this.arguments[argumentIndex]);
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
            PropertyReferenceExpression right = obj as PropertyReferenceExpression;

            if (right == null
                || !this.PropertyReference.Equals(right.PropertyReference)
                || this.Arguments.Count != right.Arguments.Count)
            {
                return false;
            }

            for (int argumentIndex = 0; argumentIndex < this.Arguments.Count; argumentIndex++)
            {
                if (!this.Arguments[argumentIndex].Equals(right.Arguments[argumentIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(PropertyReferenceExpression).GetHashCode()
                ^ this.PropertyReference.GetHashCode();
        }
    }
}