//-----------------------------------------------------------------------
// <copyright file="LoopExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an infinite loop. It can be exited with "break".</summary>
	public sealed class LoopExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.LoopExpression.Type" /> that represents the static type of the expression.</returns>
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

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.Expression" /> that is the body of the loop.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> that is the body of the loop.</returns>
		public extern Expression Body
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.LabelTarget" /> that is used by the loop body as a break statement target.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.LabelTarget" /> that is used by the loop body as a break statement target.</returns>
		public extern LabelTarget BreakLabel
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Linq.Expressions.LabelTarget" /> that is used by the loop body as a continue statement target.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.LabelTarget" /> that is used by the loop body as a continue statement target.</returns>
		public extern LabelTarget ContinueLabel
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="breakLabel">The <see cref="P:System.Linq.Expressions.LoopExpression.BreakLabel" /> property of the result.</param>
		/// <param name="continueLabel">The <see cref="P:System.Linq.Expressions.LoopExpression.ContinueLabel" /> property of the result.</param>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.LoopExpression.Body" /> property of the result.</param>
		public extern LoopExpression Update(LabelTarget breakLabel, LabelTarget continueLabel, Expression body);
	}
}
