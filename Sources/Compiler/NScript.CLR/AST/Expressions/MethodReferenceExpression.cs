//-----------------------------------------------------------------------
// <copyright file="MethodReferenceExpression.cs" company="">
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
    /// Definition for MethodReferenceExpression
    /// </summary>
    public class MethodReferenceExpression : MemberReferenceExpression
    {
        /// <summary>
        /// Backing field for MethodReference
        /// </summary>
        private readonly MethodReference methodReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        public MethodReferenceExpression(
            ClrContext context,
            Location location,
            MethodReference methodReference,
            Expression leftExpression)
            : base(
                context,
                location,
                methodReference,
                leftExpression)
        {
            if (methodReference.Resolve().IsStatic)
            {
                throw new InvalidOperationException("Wrong constructor called for static method.");
            }

            this.methodReference = methodReference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        public MethodReferenceExpression(
            ClrContext context,
            Location location,
            MethodReference methodReference)
            : base(context, location, methodReference)
        {
            if (!methodReference.Resolve().IsStatic)
            {
                throw new InvalidOperationException("Wrong constructor called for instance method.");
            }

            this.methodReference = methodReference;
        }

        /// <summary>
        /// Gets the method reference.
        /// </summary>
        /// <value>The method reference.</value>
        public MethodReference MethodReference
        {
            get
            {
                return this.methodReference;
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("methodName", this.methodReference.Name);
        }
    }
}
