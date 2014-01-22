//-----------------------------------------------------------------------
// <copyright file="UnaryExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an expression that has a unary operator.</summary>
	public sealed class UnaryExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.UnaryExpression.Type" /> that represents the static type of the expression.</returns>
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

		/// <summary>Gets the operand of the unary operation.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the operand of the unary operation.</returns>
		public extern Expression Operand
		{
			get;
		}

		/// <summary>Gets the implementing method for the unary operation.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
		public extern MethodInfo Method
		{
			get;
		}

		/// <summary>Gets a value that indicates whether the expression tree node represents a lifted call to an operator.</summary>
		/// <returns>true if the node represents a lifted call; otherwise, false.</returns>
		public extern bool IsLifted
		{
			get;
		}

		/// <summary>Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return type is lifted to a nullable type.</summary>
		/// <returns>true if the operator's return type is lifted to a nullable type; otherwise, false.</returns>
		public extern bool IsLiftedToNull
		{
			get;
		}

		/// <summary>Gets a value that indicates whether the expression tree node can be reduced.</summary>
		/// <returns>True if a node can be reduced, otherwise false.</returns>
		public extern override bool CanReduce
		{
			get;
		}
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Reduces the expression node to a simpler expression. </summary>
		/// <returns>The reduced expression.</returns>
		public extern override Expression Reduce();

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="operand">The <see cref="P:System.Linq.Expressions.UnaryExpression.Operand" /> property of the result.</param>
		public extern UnaryExpression Update(Expression operand);
	}
}
