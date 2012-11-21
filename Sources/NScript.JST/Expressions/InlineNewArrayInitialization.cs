//-----------------------------------------------------------------------
// <copyright file="InlineNewArrayInitialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    /// <summary>
    /// Definition for InlineNewArrayInitialization
    /// </summary>
    public class InlineNewArrayInitialization : Expression
    {
        /// <summary>
        /// Backing collection for values;
        /// </summary>
        private readonly List<Expression> values;

        /// <summary>
        /// Backing field for Values;
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineNewArrayInitialization"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="values">The values.</param>
        public InlineNewArrayInitialization(
            Location location,
            IdentifierScope scope,
            List<Expression> values)
            :base(location, scope)
        {
            this.values = values;
            this.readonlyValues = new ReadOnlyCollection<Expression>(this.values);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public IList<Expression> Values
        {
            get { return this.readonlyValues; }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get { return JST.Precedence.Primary; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is left to right.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left to right; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeftToRight
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("values", this.Values);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(Symbols.BrackedOpenSquare);

            for (int valueIndex = 0; valueIndex < this.Values.Count; valueIndex++)
            {
                if (valueIndex > 0)
                {
                    writer.Write(Symbols.Comma);
                }

                writer.Write(this.Values[valueIndex]);
            }

            writer.Write(Symbols.BracketCloseSquare);
        }
    }
}
