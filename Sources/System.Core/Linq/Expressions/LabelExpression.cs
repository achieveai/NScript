//-----------------------------------------------------------------------
// <copyright file="LabelExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a label, which can be put in any <see cref="T:System.Linq.Expressions.Expression" /> context. If it is jumped to, it will get the value provided by the corresponding <see cref="T:System.Linq.Expressions.GotoExpression" />. Otherwise, it receives the value in <see cref="P:System.Linq.Expressions.LabelExpression.DefaultValue" />. If the <see cref="T:System.Type" /> equals System.Void, no value should be provided.</summary>
	public sealed class LabelExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.LabelExpression.Type" /> that represents the static type of the expression.</returns>
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

		/// <summary>The <see cref="T:System.Linq.Expressions.LabelTarget" /> which this label is associated with.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.LabelTarget" /> which this label is associated with.</returns>
		public extern LabelTarget Target
		{
			get;
		}

		/// <summary>The value of the <see cref="T:System.Linq.Expressions.LabelExpression" /> when the label is reached through regular control flow (for example, is not jumped to).</summary>
		/// <returns>The Expression object representing the value of the <see cref="T:System.Linq.Expressions.LabelExpression" />.</returns>
		public extern Expression DefaultValue
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="target">The <see cref="P:System.Linq.Expressions.LabelExpression.Target" /> property of the result.</param>
		/// <param name="defaultValue">The <see cref="P:System.Linq.Expressions.LabelExpression.DefaultValue" /> property of the result</param>
		public extern LabelExpression Update(LabelTarget target, Expression defaultValue);
	}
}
