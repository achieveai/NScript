//-----------------------------------------------------------------------
// <copyright file="InlineIteratorExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
using NScript.Utils;
using Mono.Cecil;

    /// <summary>
    /// Definition for InlineIteratorExpression
    /// </summary>
    public class InlineIteratorExpression : Expression
    {
        ParameterBlock iteratorBlock;
        TypeReference iteratorType;

        public InlineIteratorExpression(
            ClrContext context,
            Location location,
            ParameterBlock iteratorBlock,
            TypeReference iteratorType)
            :base(context, location)
        {
            this.iteratorBlock = iteratorBlock;
            this.iteratorType = iteratorType;
        }

        public override TypeReference ResultType
        {
            get
            {
                return this.iteratorType;
            }
        }

        public ParameterBlock IteratorBlock
        {
            get { return this.iteratorBlock; }
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
            this.iteratorBlock = (ParameterBlock)processor.Process(this.iteratorBlock);
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("iteratorType", this.iteratorType.ToString());
            serializationInfo.AddValue("block", this.iteratorBlock);
        }
    }
}
