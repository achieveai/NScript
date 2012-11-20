//-----------------------------------------------------------------------
// <copyright file="AnonymousMethodBodyExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for AnonymousMethodBodyExpression
    /// </summary>
    public class AnonymousMethodBodyExpression : Expression
    {
        private ParameterBlock parameterBlock;
        private TypeReference methodType;

        public AnonymousMethodBodyExpression(
            ClrContext context,
            Location location,
            ParameterBlock parameterBlock,
            TypeReference methodType)
            : base(context, location)
        {
            this.parameterBlock = parameterBlock;
            this.methodType = methodType;
        }

        public ParameterBlock ParameterBlock
        {
            get { return this.parameterBlock; }
        }

        public override TypeReference ResultType
        {
            get
            {
                return this.methodType;
            }
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
            this.parameterBlock = (ParameterBlock)processor.Process(this.parameterBlock);
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("methodType", this.methodType.ToString());
            serializationInfo.AddValue("paramBlock", this.parameterBlock);
        }
    }
}