//-----------------------------------------------------------------------
// <copyright file="DynamicMemberAccessor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for DynamicMemberAccessor
    /// </summary>
    public class DynamicMemberAccessor : Expression
    {
        public DynamicMemberAccessor(
            ClrContext context,
            Location location,
            Expression instanceExpression,
            string memberName)
            :base (context, location)
        {
            this.InstanceExpression = instanceExpression;
            this.MemberName = memberName;
        }

        public Expression InstanceExpression
        { get; private set; }

        public string MemberName
        { get; private set; }

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
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("inst", this.InstanceExpression);
            serializationInfo.AddValue("name", this.MemberName);
        }
    }
}
