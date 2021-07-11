//-----------------------------------------------------------------------
// <copyright file="NewArrayExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents creating a new array and possibly initializing the elements of the new array.</summary>
	public class NewArrayExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.NewArrayExpression.Type" /> that represents the static type of the expression.</returns>
		public extern sealed override Type Type
		{
			get;
		}

		/// <summary>Gets the bounds of the array if the value of the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property is <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayBounds" />, or the values to initialize the elements of the new array if the value of the <see cref="P:System.Linq.Expressions.Expression.NodeType" /> property is <see cref="F:System.Linq.Expressions.ExpressionType.NewArrayInit" />.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.Expression" /> objects which represent either the bounds of the array or the initialization values.</returns>
		public extern ReadOnlyCollection<Expression> Expressions
		{
			get;
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="expressions">The <see cref="P:System.Linq.Expressions.NewArrayExpression.Expressions" /> property of the result.</param>
		public extern NewArrayExpression Update(IEnumerable<Expression> expressions);
	}
}
