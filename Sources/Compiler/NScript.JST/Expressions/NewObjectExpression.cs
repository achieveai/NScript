//-----------------------------------------------------------------------
// <copyright file="NewObjectExpression.cs" company="">
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
    /// Definition for NewObjectExpression
    /// </summary>
    public class NewObjectExpression : Expression
    {
        /// <summary>
        /// Backing field for ObjectAccessExpression.
        /// </summary>
        private readonly Expression objectAccessExpression;

        /// <summary>
        /// Backing collection for arguments.
        /// </summary>
        private readonly IList<Expression> arguments;

        /// <summary>
        /// Backing field for Arguments.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewObjectExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="objectAccessExpression">The object access expression.</param>
        public NewObjectExpression(
            Location location,
            IdentifierScope scope,
            Expression objectAccessExpression,
            params Expression[] arguments)
            : base(location, scope)
        {
            this.objectAccessExpression = objectAccessExpression;
            this.arguments = arguments;
            this.readonlyArguments = new ReadOnlyCollection<Expression>(this.arguments);
        }

        /// <summary>
        /// Gets the object access expression.
        /// </summary>
        /// <value>The object access expression.</value>
        public Expression ObjectAccessExpression
        {
            get
            {
                return this.objectAccessExpression;
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<Expression> Arguments
        {
            get { return this.readonlyArguments; }
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precedence.</value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.Primary;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("objectExpression", this.arguments);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer
                .Write(Keyword.New)
                .Write(
                    this.objectAccessExpression,
                    this.objectAccessExpression.OperatorPlacement != JST.OperatorPlacement.Prefix
                        && this.objectAccessExpression.Precedence < this.Precedence)
                .WriteArguments(this.arguments);
        }
    }
}
