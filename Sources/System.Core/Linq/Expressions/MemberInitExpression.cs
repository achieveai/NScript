//-----------------------------------------------------------------------
// <copyright file="MemberInitExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents calling a constructor and initializing one or more members of the new object.</summary>
	public sealed class MemberInitExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.MemberInitExpression.Type" /> that represents the static type of the expression.</returns>
		public extern sealed override Type Type
		{
			get;
		}


		/// <summary>Gets a value that indicates whether the expression tree node can be reduced.</summary>
		/// <returns>True if the node can be reduced, otherwise false.</returns>
		public extern override bool CanReduce
		{
			get;
		}


		/// <summary>Returns the node type of this Expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the expression that represents the constructor call.</summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that represents the constructor call.</returns>
		public extern NewExpression NewExpression
		{
			get;
		}

		/// <summary>Gets the bindings that describe how to initialize the members of the newly created object.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.MemberBinding" /> objects which describe how to initialize the members.</returns>
		public extern ReadOnlyCollection<MemberBinding> Bindings
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Reduces the <see cref="T:System.Linq.Expressions.MemberInitExpression" /> to a simpler expression. </summary>
		/// <returns>The reduced expression.</returns>
		public extern override Expression Reduce();

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="newExpression">The <see cref="P:System.Linq.Expressions.MemberInitExpression.NewExpression" /> property of the result.</param>
		/// <param name="bindings">The <see cref="P:System.Linq.Expressions.MemberInitExpression.Bindings" /> property of the result.</param>
		public extern MemberInitExpression Update(NewExpression newExpression, IEnumerable<MemberBinding> bindings);
	}
}
