//-----------------------------------------------------------------------
// <copyright file="YieldStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
using Cs2JsC.Utils;

    /// <summary>
    /// Definition for YieldStatement
    /// </summary>
    public class YieldStatement : Statement
    {
        Expression yieldValue;

        public YieldStatement(
            ClrContext context,
            Location location,
            Expression yieldValue)
            : base (context, location)
        {
            this.yieldValue = yieldValue;
        }

        public Expression YieldValue
        {
            get { return this.yieldValue; }
        }

        public bool IsBreak
        {
            get { return this.yieldValue == null; }
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
            this.yieldValue = (Expression)processor.Process(this.yieldValue);
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("yieldValue", this.yieldValue);
        }
    }
}
