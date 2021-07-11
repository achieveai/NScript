//-----------------------------------------------------------------------
// <copyright file="SwitchStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for SwitchStatement
    /// </summary>
    public class SwitchStatement : Statement
    {
        /// <summary>
        /// Backing field for StatementValue.
        /// </summary>
        private Expression statementValue;

        /// <summary>
        /// Backing field for CaseBlocks.
        /// </summary>
        private List<KeyValuePair<List<LiteralExpression>, Statement>> caseBlocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchStatement"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="statementValue">The statement value.</param>
        /// <param name="caseBlocks">The case blocks.</param>
        public SwitchStatement(
            ClrContext context,
            Location location,
            Expression statementValue,
            List<KeyValuePair<List<LiteralExpression>, Statement>> caseBlocks)
            : base(context, location)
        {
            this.statementValue = statementValue;
            this.caseBlocks = caseBlocks;
        }

        /// <summary>
        /// Gets the switch value.
        /// </summary>
        /// <value>The switch value.</value>
        public Expression SwitchValue
        {
            get { return this.statementValue; }
        }

        /// <summary>
        /// Gets the case blocks.
        /// </summary>
        /// <value>The case blocks.</value>
        public List<KeyValuePair<List<LiteralExpression>, Statement>> CaseBlocks
        {
            get { return this.caseBlocks; }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.statementValue = (Expression) processor.Process(this.statementValue);

            for (int caseBlockIndex = 0; caseBlockIndex < caseBlocks.Count; caseBlockIndex++)
            {
                this.caseBlocks[caseBlockIndex] =
                    new KeyValuePair<List<LiteralExpression>, Statement>(
                        this.caseBlocks[caseBlockIndex].Key,
                        (Statement) processor.Process(this.caseBlocks[caseBlockIndex].Value));
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("statement", this.statementValue);

            serializationInfo.AddValue(
                "caseBlocks",
                this.caseBlocks,
                (processor, kvPair) =>
                    {
                        serializationInfo.AddValue(
                            "caseIds",
                            kvPair.Key,
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

                        processor.AddValue("block", kvPair.Value);
                    });
        }
    }
}
