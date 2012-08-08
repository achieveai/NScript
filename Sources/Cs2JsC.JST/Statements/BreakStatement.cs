//-----------------------------------------------------------------------
// <copyright file="BreakStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for BreakStatement
    /// </summary>
    public class BreakStatement : Statement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreakStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        public BreakStatement(
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
                .Write(Keyword.Break)
                .Write(Symbols.SemiColon);
        }
    }
}
