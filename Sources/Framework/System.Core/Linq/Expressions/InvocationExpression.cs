//-----------------------------------------------------------------------
// <copyright file="InvocationExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an expression that applies a delegate or lambda expression to a list of argument expressions.</summary>
	public sealed class InvocationExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="P:System.Linq.Expressions.InvocationExpression.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.InvocationExpression.Type" /> that represents the static type of the expression.</returns>
		public extern sealed override Type Type
		{
			get;
		}

		/// <summary>Returns the node type of this expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the delegate or lambda expression to be applied.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the delegate to be applied.</returns>
		public extern Expression Expression
		{
			get;
		}

		/// <summary>Gets the arguments that the delegate or lambda expression is applied to.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.Expression" /> objects which represent the arguments that the delegate is applied to.</returns>
		public extern ReadOnlyCollection<Expression> Arguments
		{
			get;
		}
		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="expression">The <see cref="P:System.Linq.Expressions.InvocationExpression.Expression" /> property of the result.</param>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.InvocationExpression.Arguments" /> property of the result.</param>
		public extern InvocationExpression Update(Expression expression, IEnumerable<Expression> arguments);
		protected extern override Expression Accept(ExpressionVisitor visitor);
	}
}
