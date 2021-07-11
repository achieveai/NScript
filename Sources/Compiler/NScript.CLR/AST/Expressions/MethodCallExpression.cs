//-----------------------------------------------------------------------
// <copyright file="MethodCallExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for MethodCallExpression
    /// </summary>
    public class MethodCallExpression : Expression
    {
        /// <summary>
        /// Backing field for MethodReference.
        /// </summary>
        private readonly Expression methodReference;

        /// <summary>
        /// Backing collection for Parameters.
        /// </summary>
        private readonly List<Expression> parameters =
            new List<Expression>();

        /// <summary>
        /// Backing field for Parameters.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <param name="parameters">The parameters.</param>
        public MethodCallExpression(
            ClrContext context,
            Location location,
            Expression methodReference,
            params Expression[] parameters)
            : base(context, location)
        {
            this.methodReference = methodReference;
            this.parameters.AddRange(parameters);
            this.readonlyParameters = new ReadOnlyCollection<Expression>(this.parameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="methodReference">The method reference.</param>
        public MethodCallExpression(
            ClrContext context,
            Location location,
            Expression methodReference)
            : base(context, location)
        {
            this.methodReference = methodReference;
            this.readonlyParameters = new ReadOnlyCollection<Expression>(this.parameters);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public Expression MethodReference
        {
            get
            {
                return this.methodReference;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IList<Expression> Parameters
        {
            get
            {
                return this.readonlyParameters;
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
                MethodReferenceExpression methodReferenceExpression = this.methodReference as MethodReferenceExpression;

                if (methodReferenceExpression != null)
                {
                    return methodReferenceExpression.MethodReference.ReturnType;
                }

                ConstructorReferenceExpression constructorReferenceExpression = this.methodReference as ConstructorReferenceExpression;
                if (constructorReferenceExpression != null)
                {
                    return constructorReferenceExpression.Constructor.DeclaringType;
                }

                TypeReference leftExpressionType = this.MethodReference.ResultType as TypeReference;

                if (leftExpressionType != null
                    && this.KnownReferences.MulticastDelegate.IsSameDefinition(leftExpressionType.Resolve().BaseType))
                {
                    MethodDefinition invokeMethodDefinition = null;

                    foreach (var function in leftExpressionType.Resolve().Methods)
                    {
                        if (function.Name == "Invoke")
                        {
                            invokeMethodDefinition = function;
                            break;
                        }
                    }

                    return invokeMethodDefinition.ReturnType.FixGenericTypeArguments(leftExpressionType);
                }

                if (this.MethodReference is LocalFunctionReference localFunctionReference)
                {
                    return localFunctionReference.ReturnType;
                }

                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether left side is delegate.
        /// </summary>
        /// <value><c>true</c> if left side is delegate otherwise, <c>false</c>.</value>
        public bool LeftSideIsDelegate
        {
            get { return this.KnownReferences.MulticastDelegate.IsSameDefinition(this.MethodReference.ResultType.Resolve().BaseType); }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int parameterIndex = 0; parameterIndex < this.parameters.Count; parameterIndex++)
            {
                this.parameters[parameterIndex] = (Expression)processor.Process(this.parameters[parameterIndex]);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("methodReference", this.methodReference);
            serializationInfo.AddValue("parameters", this.parameters);
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
            MethodCallExpression right = obj as MethodCallExpression;

            if (right == null
                || !this.MethodReference.Equals(right.MethodReference)
                || this.Parameters.Count != right.Parameters.Count)
            {
                return false;
            }

            for (int parameterINdex = 0; parameterINdex < this.parameters.Count; parameterINdex++)
            {
                if (!this.parameters[parameterINdex].Equals(right.parameters[parameterINdex]))
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
            return typeof(MethodCallExpression).GetHashCode()
                ^ this.MethodReference.GetHashCode();
        }
    }
}
