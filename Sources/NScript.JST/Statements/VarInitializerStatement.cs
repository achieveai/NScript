//-----------------------------------------------------------------------
// <copyright file="VarInitializerStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for VarInitializerStatement
    /// </summary>
    public class VarInitializerStatement : InitializerStatement
    {
        public VarInitializerStatement(
            Location location,
            IdentifierScope scope,
            IList<Expression> initializerExpressions)
            : base(location, scope, initializerExpressions)
        {
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(ICustomSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddValue("varInitializers", this.Initializers);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .EnterLocation(this.Location);

            writer.Write(Keyword.Var);
            this.WriteContent(writer);

            writer.Write(Symbols.SemiColon)
                .LeaveLocation();
        }

    }
}
