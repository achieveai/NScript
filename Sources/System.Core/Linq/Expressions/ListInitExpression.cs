//-----------------------------------------------------------------------
// <copyright file="ListInitExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a constructor call that has a collection initializer.</summary>
	public sealed class ListInitExpression : Expression
	{
		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.ListInitExpression.Type" /> that represents the static type of the expression.</returns>
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

		/// <summary>Gets the expression that contains a call to the constructor of a collection type.</summary>
		/// <returns>A <see cref="T:System.Linq.Expressions.NewExpression" /> that represents the call to the constructor of a collection type.</returns>
		public extern NewExpression NewExpression
		{
			get;
		}

		/// <summary>Gets the element initializers that are used to initialize a collection.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.ElementInit" /> objects which represent the elements that are used to initialize the collection.</returns>
		public extern ReadOnlyCollection<ElementInit> Initializers
		{
			get;
		}

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Reduces the binary expression node to a simpler expression.</summary>
		/// <returns>The reduced expression.</returns>
		public extern override Expression Reduce();

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="newExpression">The <see cref="P:System.Linq.Expressions.ListInitExpression.NewExpression" /> property of the result.</param>
		/// <param name="initializers">The <see cref="P:System.Linq.Expressions.ListInitExpression.Initializers" /> property of the result.</param>
		public extern ListInitExpression Update(NewExpression newExpression, IEnumerable<ElementInit> initializers);
	}
}
