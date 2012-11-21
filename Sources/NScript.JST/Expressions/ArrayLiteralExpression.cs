//-----------------------------------------------------------------------
// <copyright file="ArrayLiteralExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NScript.Utils;

    /// <summary>
    /// Array literal expression.
    /// </summary>
    public class ArrayLiteralExpression : Expression
    {
        /// <summary>
        /// Backing field for readonly elements.
        /// </summary>
        private readonly IList<Expression> elements;

        /// <summary>
        /// Backing field for Elements.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayLiteralExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="elements">The elements.</param>
        public ArrayLiteralExpression(
            Location location,
            IdentifierScope scope,
            IList<Expression> elements)
            : base(location, scope)
        {
            this.elements = elements;
            this.readonlyElements = new ReadOnlyCollection<Expression>(this.elements);
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public IList<Expression> Elements
        {
            get
            {
                return this.readonlyElements;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("elements", this.elements);
        }
    }
}
