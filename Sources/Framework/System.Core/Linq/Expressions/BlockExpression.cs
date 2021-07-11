//-----------------------------------------------------------------------
// <copyright file="BlockExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Threading;

	/// <summary>Represents a block that contains a sequence of expressions where variables can be defined.</summary>
	public class BlockExpression : Expression
	{
		/// <summary>Gets the expressions in this block.</summary>
		/// <returns>The read-only collection containing all the expressions in this block.</returns>
		public extern ReadOnlyCollection<Expression> Expressions
		{
			get;
		}

		/// <summary>Gets the variables defined in this block.</summary>
		/// <returns>The read-only collection containing all the variables defined in this block.</returns>
		public extern ReadOnlyCollection<ParameterExpression> Variables
		{
			get;
		}

		/// <summary>Gets the last expression in this block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the last expression in this block.</returns>
		public extern Expression Result
		{
			get;
		}

		/// <summary>Returns the node type of this expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.BlockExpression.Type" /> that represents the static type of the expression.</returns>
		public extern override Type Type
		{
			get;
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children changed, or an expression with the updated children.</returns>
		/// <param name="variables">The <see cref="P:System.Linq.Expressions.BlockExpression.Variables" /> property of the result. </param>
		/// <param name="expressions">The <see cref="P:System.Linq.Expressions.BlockExpression.Expressions" /> property of the result. </param>
		public extern BlockExpression Update( IEnumerable<ParameterExpression> variables, IEnumerable<Expression> expressions);
	}
}
