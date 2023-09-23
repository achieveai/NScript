//-----------------------------------------------------------------------
// <copyright file="ThrowStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using NScript.Utils;

    public class ThrowStatement : Statement
    {
        public ThrowStatement(
            Location location,
            IdentifierScope scope,
            Expression innerExpression)
            : base(location, scope)
        {
            Expression = innerExpression;
        }

        public Expression Expression { get; }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer
                .EnterLocation(this.Location)
                .Write(Keyword.Throw)
                .Write(this.Expression)
                .Write(Symbols.SemiColon)
                .LeaveLocation();
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("innerExpression", this.Expression);
        }
    }
}
