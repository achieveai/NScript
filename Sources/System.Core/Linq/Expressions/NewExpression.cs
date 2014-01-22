//-----------------------------------------------------------------------
// <copyright file="NewExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a constructor call.</summary>
	public class NewExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.NewExpression.Type" /> that represents the static type of the expression.</returns>
		public extern override Type Type
		{
			get;
		}

		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the called constructor.</summary>
		/// <returns>The <see cref="T:System.Reflection.ConstructorInfo" /> that represents the called constructor.</returns>
		public extern ConstructorInfo Constructor
		{
			get;
		}

		/// <summary>Gets the arguments to the constructor.</summary>
		/// <returns>A collection of <see cref="T:System.Linq.Expressions.Expression" /> objects that represent the arguments to the constructor.</returns>
		public extern ReadOnlyCollection<Expression> Arguments
		{
			get;
		}

		/// <summary>Gets the members that can retrieve the values of the fields that were initialized with constructor arguments.</summary>
		/// <returns>A collection of <see cref="T:System.Reflection.MemberInfo" /> objects that represent the members that can retrieve the values of the fields that were initialized with constructor arguments.</returns>
		public extern ReadOnlyCollection<MemberInfo> Members
		{
			get;
		}

		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.NewExpression.Arguments" /> property of the result.</param>
		public extern NewExpression Update(IEnumerable<Expression> arguments);
	}
}
