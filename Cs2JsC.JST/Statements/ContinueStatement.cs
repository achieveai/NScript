//-----------------------------------------------------------------------
// <copyright file="ContinueStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for ContinueStatement
    /// </summary>
    public class ContinueStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinueStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        public ContinueStatement(
            Location location,
            IdentifierScope scope)
            :base(location, scope)
        {
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
        {
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .Write(Keyword.Continue)
                .Write(Symbols.SemiColon);
        }
    }
}
