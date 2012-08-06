//-----------------------------------------------------------------------
// <copyright file="MethodCallExpression.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Cs2JsC.Utils;

    /// <summary>
    /// Call expression.
    /// </summary>
    public class MethodCallExpression : Expression
    {
        /// <summary>
        /// Backing field for applicant.
        /// </summary>
        private readonly Expression methodExpression;

        /// <summary>
        /// Backing field for read-only arguments.
        /// </summary>
        private readonly IList<Expression> arguments;

        /// <summary>
        /// Backing field for Arguments.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="methodExpression">The expression pointing to method.</param>
        /// <param name="arguments">The arguments.</param>
        public MethodCallExpression(
            Location location,
            IdentifierScope scope,
            Expression methodExpression,
            IList<Expression> arguments)
            : base(location, scope)
        {
            this.methodExpression = methodExpression;
            this.arguments = arguments;
            this.readonlyArguments = new ReadOnlyCollection<Expression>(this.arguments);
            if (arguments.Contains(null))
            {
                throw new System.ArgumentNullException("arguments");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="methodExpression">The method expression.</param>
        /// <param name="arguments">The arguments.</param>
        public MethodCallExpression(
            Location location,
            IdentifierScope scope,
            Expression methodExpression,
            params Expression[] arguments)
            : this(location, scope, methodExpression, (IList<Expression>)arguments)
        { }

        /// <summary>
        /// Gets the method expression.
        /// </summary>
        /// <value>The method expression.</value>
        public Expression MethodExpression
        {
            get
            {
                return this.methodExpression;
            }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IList<Expression> Arguments
        {
            get
            {
                return this.readonlyArguments;
            }
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precedence.</value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.LHS;
            }
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
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("method", this.MethodExpression);
            serializer.AddValue("arguments", this.Arguments);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.EnterLocation(this.Location)
                .Write(this.MethodExpression, this.MethodExpression.Precedence < this.Precedence)
                .WriteArguments(this.arguments)
                .LeaveLocation();
        }
    }
}