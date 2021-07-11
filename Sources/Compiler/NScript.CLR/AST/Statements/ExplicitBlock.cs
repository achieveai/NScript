//-----------------------------------------------------------------------
// <copyright file="ExplicitBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    /// <summary>
    /// Definition for ExplicitBlock
    /// </summary>
    public class ExplicitBlock : Statement
    {
        List<Statement> statements = new List<Statement>();

        ReadOnlyCollection<Statement> readonlyStatements;

        public ExplicitBlock(
            ClrContext context,
            Location location,
            params Statement[] statements)
            : base(context, location)
        {
            this.statements.AddRange(statements);
            this.readonlyStatements = new ReadOnlyCollection<Statement>(this.statements);
        }

        public IList<Statement> Statements
        { get { return this.readonlyStatements; } }

        public virtual void AddStatement(Statement statement)
        {
            this.statements.Add(statement);
        }

        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int i = 0; i < this.statements.Count; i++)
            {
                this.statements[i] = (Statement)processor.Process(this.statements[i]);
            }
        }

        public override void Serialize(ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("statements", this.statements);
        }
    }
}