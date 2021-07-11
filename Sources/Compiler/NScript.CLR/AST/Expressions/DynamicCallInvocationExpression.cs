//-----------------------------------------------------------------------
// <copyright file="DynamicCallInvocationExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for DynamicCallInvocationExpression
    /// </summary>
    public class DynamicCallInvocationExpression : Expression
    {
        /// <summary>
        /// Backing collection for Parameters.
        /// </summary>
        private readonly List<Expression> parameters =
            new List<Expression>();

        /// <summary>
        /// Backing field for Parameters.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyParameters;

        public DynamicCallInvocationExpression(
            ClrContext context,
            Location location,
            Expression instanceExpression,
            params Expression[] parameters)
            :base (context, location)
        {
            this.InstanceExpression = instanceExpression;
            this.parameters.AddRange(parameters);
            this.readonlyParameters = new ReadOnlyCollection<Expression>(this.parameters);
        }

        public Expression InstanceExpression
        { get; private set; }

        public IList<Expression> Parameters
            => this.readonlyParameters;

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override Mono.Cecil.TypeReference ResultType
        {
            get
            {
                return this.Context.KnownReferences.Object;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.InstanceExpression = (Expression)processor.Process(this.InstanceExpression);
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
            serializationInfo.AddValue("inst", this.InstanceExpression);
            serializationInfo.AddValue("parameters", this.parameters);
        }

        public override bool Equals(object obj)
        {
            var right = obj as DynamicCallInvocationExpression;

            if (right == null
                || !this.InstanceExpression.Equals(right.InstanceExpression)
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

        public override int GetHashCode()
        {
            return typeof(DynamicCallInvocationExpression).GetHashCode()
                ^ this.InstanceExpression.GetHashCode();
        }
    }
}
