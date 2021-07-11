//-----------------------------------------------------------------------
// <copyright file="InitializerStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for InitializerStatement
    /// </summary>
    public class InitializerStatement : Statement
    {
        List<Expression> initializerAssignments = new List<Expression>();

        ReadOnlyCollection<Expression> readonlyAssignments = null;

        public InitializerStatement(
            ClrContext clrContext,
            Location loc,
            params Expression[] expressions)
            : base(clrContext, loc)
        {
            this.initializerAssignments.AddRange(expressions);
            this.readonlyAssignments = new ReadOnlyCollection<Expression>(this.initializerAssignments);
        }

        public IList<Expression> Initializers
        { get { return this.readonlyAssignments; } }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            base.ProcessThroughPipeline(processor);
            for (int i = this.initializerAssignments.Count - 1; i >= 0; i--)
            {
                this.initializerAssignments[i] = (Expression)processor.Process(this.initializerAssignments[i]);
                if (this.initializerAssignments[i] == null)
                {
                    this.initializerAssignments.RemoveAt(i);
                }
            }
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("initializers", this.initializerAssignments);
        }
    }
}
