//-----------------------------------------------------------------------
// <copyright file="MemberExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

	/// <summary>Represents accessing a field or property.</summary>
	public class MemberExpression : Expression
	{
		/// <summary>Gets the field or property to be accessed.</summary>
		/// <returns>The <see cref="T:System.Reflection.MemberInfo" /> that represents the field or property to be accessed.</returns>
		public extern MemberInfo Member
		{
			get;
		}

		/// <summary>Gets the containing object of the field or property.</summary>
		/// <returns>An <see cref="T:System.Linq.Expressions.Expression" /> that represents the containing object of the field or property.</returns>
		public extern Expression Expression
		{
			get;
		}

		/// <summary>Returns the node type of this <see cref="P:System.Linq.Expressions.MemberExpression.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="expression">The <see cref="P:System.Linq.Expressions.MemberExpression.Expression" /> property of the result.</param>
		public extern MemberExpression Update(Expression expression);
	}
}
