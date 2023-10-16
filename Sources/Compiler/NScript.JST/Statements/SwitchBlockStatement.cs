//-----------------------------------------------------------------------
// <copyright file="SwitchBlockStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NScript.Utils;

    /// <summary>
    /// Definition for SwitchBlockStatement
    /// </summary>
    public class SwitchBlockStatement : Statement
    {
        /// <summary>
        /// Backing field for StatementValue.
        /// </summary>
        private Expression statementValue;

        /// <summary>
        /// Backing field for caseBlocks.
        /// </summary>
        private List<(List<Expression> cases, Statement block)> caseBlocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchBlockStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="statementValue">The statement value.</param>
        /// <param name="caseBlocks">The case blocks.</param>
        public SwitchBlockStatement(
            Location location,
            IdentifierScope scope,
            Expression statementValue,
            List<(List<Expression>, Statement)> caseBlocks)
            : base(location, scope)
        {
            this.statementValue = statementValue;
            this.caseBlocks = caseBlocks;
            CaseBlocks = new ReadOnlyCollection<(List<Expression>, Statement)>(caseBlocks);
        }

        public ReadOnlyCollection<(List<Expression> cases, Statement block)> CaseBlocks
        { get; }

        public Expression Key => statementValue;

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("statement", this.statementValue);

            serializer.AddValue(
                "caseBlocks",
                this.caseBlocks,
                (processor, kvPair) =>
                    {
                        processor.AddValue(
                            "caseIds",
                            kvPair.cases,
                            (p, id) =>
                                {
                                    if (id == null)
                                    {
                                        p.AddValue("case", "default");
                                    }
                                    else
                                    {
                                        p.AddValue("case", id);
                                    }
                                });

                        processor.AddValue("block", kvPair.block);
                    });
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .Write(Keyword.Switch)
                .Write(Symbols.BracketOpenRound)
                .Write(this.statementValue)
                .Write(Symbols.BracketCloseRound)
                .EnterScope()
                .Write(Symbols.BracketOpenCurly);

            foreach (var keyValuePair in this.caseBlocks)
            {
                if (keyValuePair.cases == null
                    || keyValuePair.cases.Count == 0)
                {
                    writer.WriteNewLine()
                        .Write(Keyword.Default)
                        .Write(Symbols.Colon);
                }
                else
                {
                    foreach (var expression in keyValuePair.cases)
                    {
                        if (expression == null)
                        {
                            writer.WriteNewLine()
                                .Write(Keyword.Default)
                                .Write(Symbols.Colon);
                        }
                        else
                        {
                            writer.WriteNewLine()
                                .Write(Keyword.Case)
                                .Write(expression)
                                .Write(Symbols.Colon);
                        }
                    }
                }

                writer.Write(keyValuePair.block);
            }

            writer.ExitScope()
                .Write(Symbols.BracketCloseCurly);
        }
    }
}
