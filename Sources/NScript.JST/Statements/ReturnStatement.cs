//-----------------------------------------------------------------------
// <copyright file="ReturnStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.Utils;
namespace NScript.JST
{
    /// <summary>
    /// Definition for ReturnStatement
    /// </summary>
    public class ReturnStatement : Statement
    {
        /// <summary>
        /// Backing field for returnExpression.
        /// </summary>
        private readonly Expression returnExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="returnExpression">The return expression.</param>
        public ReturnStatement(
            Location location,
            IdentifierScope scope,
            Expression returnExpression)
            : base(location, scope)
        {
            this.returnExpression = returnExpression;
        }

        /// <summary>
        /// Gets the return expression.
        /// </summary>
        /// <value>The return expression.</value>
        public Expression ReturnExpression
        {
            get
            {
                return this.returnExpression;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("expression", this.ReturnExpression);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .EnterLocation(this.Location)
                .Write(Keyword.Return);

            if (this.ReturnExpression != null)
            {
                writer.Write(this.returnExpression);
            }

            writer.Write(Symbols.SemiColon)
                .LeaveLocation();
        }
    }
}
