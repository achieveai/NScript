//-----------------------------------------------------------------------
// <copyright file="GotoExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Represents an unconditional jump. This includes return statements, break and continue statements, and other jumps.</summary>
	public sealed class GotoExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.GotoExpression.Type" /> that represents the static type of the expression.</returns>
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

		/// <summary>The value passed to the target, or null if the target is of type System.Void.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the value passed to the target or null.</returns>
		public extern Expression Value
		{
			get;
		}

		/// <summary>The target label where this node jumps to.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.LabelTarget" /> object representing the target label for this node.</returns>
		public extern LabelTarget Target
		{
			get;
		}

		/// <summary>The kind of the "go to" expression. Serves information purposes only.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.GotoExpressionKind" /> object representing the kind of the "go to" expression.</returns>
		public extern GotoExpressionKind Kind
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="target">The <see cref="P:System.Linq.Expressions.GotoExpression.Target" /> property of the result. </param>
		/// <param name="value">The <see cref="P:System.Linq.Expressions.GotoExpression.Value" /> property of the result. </param>
		public extern GotoExpression Update(LabelTarget target, Expression value);
	}
}
