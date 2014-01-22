//-----------------------------------------------------------------------
// <copyright file="BinaryExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an expression that has a binary operator.</summary>
	public class BinaryExpression : Expression
	{
		/// <summary>Gets a value that indicates whether the expression tree node can be reduced.</summary>
		/// <returns>True if the expression tree node can be reduced, otherwise false.</returns>
		public extern override bool CanReduce
		{
			get;
		}

		/// <summary>Gets the right operand of the binary operation.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the right operand of the binary operation.</returns>
		public extern Expression Right
		{
			get;
		}

		/// <summary>Gets the left operand of the binary operation.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the left operand of the binary operation.</returns>
		public extern Expression Left
		{
			get;
		}

		/// <summary>Gets the implementing method for the binary operation.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodInfo" /> that represents the implementing method.</returns>
		public extern MethodInfo Method
		{
			get;
		}

		/// <summary>Gets the type conversion function that is used by a coalescing or compound assignment operation.</summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.LambdaExpression" /> that represents a type conversion function.</returns>
		public extern LambdaExpression Conversion
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

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="left">The <see cref="P:System.Linq.Expressions.BinaryExpression.Left" /> property of the result. </param>
		/// <param name="conversion">The <see cref="P:System.Linq.Expressions.BinaryExpression.Conversion" /> property of the result.</param>
		/// <param name="right">The <see cref="P:System.Linq.Expressions.BinaryExpression.Right" /> property of the result. </param>
		public extern BinaryExpression Update(Expression left, LambdaExpression conversion, Expression right);

		/// <summary>Reduces the binary expression node to a simpler expression.</summary>
		/// <returns>The reduced expression.</returns>
		public extern override Expression Reduce();

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);
	}
}
