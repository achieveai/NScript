//-----------------------------------------------------------------------
// <copyright file="TryExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a try/catch/finally/fault block.</summary>
	public sealed class TryExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.TryExpression.Type" /> that represents the static type of the expression.</returns>
		public extern sealed override Type Type
		{
			get;
		}

		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the body of the try block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the body of the try block.</returns>
		public extern Expression Body
		{
			get;
		}

		/// <summary>Gets the collection of <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions associated with the try block.</summary>
		/// <returns>The collection of <see cref="T:System.Linq.Expressions.CatchBlock" /> expressions associated with the try block.</returns>
		public extern ReadOnlyCollection<CatchBlock> Handlers
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the finally block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the finally block.</returns>
		public extern Expression Finally
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> representing the fault block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the fault block.</returns>
		public extern Expression Fault
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.TryExpression.Body" /> property of the result.</param>
		/// <param name="handlers">The <see cref="P:System.Linq.Expressions.TryExpression.Handlers" /> property of the result.</param>
		/// <param name="finally">The <see cref="P:System.Linq.Expressions.TryExpression.Finally" /> property of the result.</param>
		/// <param name="fault">The <see cref="P:System.Linq.Expressions.TryExpression.Fault" /> property of the result.</param>
		public extern TryExpression Update(Expression body, IEnumerable<CatchBlock> handlers, Expression @finally, Expression fault);
	}
}
