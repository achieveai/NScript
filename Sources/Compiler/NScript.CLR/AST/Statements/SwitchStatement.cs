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
    public class SwitchStatement : ScopeBlock
    {
        public SwitchStatement(
            ClrContext context,
            Location location,
            Expression switchValue,
            List<(List<Pattern>, Statement)> caseBlocks,
            List<(LocalVariable localVariable, bool isUsed)> variables,
            List<LocalFunctionVariable> localFunctionNames)
            : base(context, location, variables, localFunctionNames)
        {
            SwitchValue = switchValue;
            CaseBlocks = caseBlocks;
        }

        /// <summary>
        /// Gets the switch value.
        /// </summary>
        /// <value>The switch value.</value>
        public Expression SwitchValue { get;  }

        /// <summary>
        /// Gets the case blocks.
        /// </summary>
        /// <value>The case blocks.</value>
        public List<(List<Pattern> cases, Statement block)> CaseBlocks { get; }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("statement", SwitchValue);

            serializationInfo.AddValue(
                "caseBlocks",
                CaseBlocks,
                (processor, tupl) =>
                    {
                        serializationInfo.AddValue(
                            "caseIds",
                            tupl.Item1,
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

                        processor.AddValue("block", tupl.Item2);
                    });
        }
    }
}
