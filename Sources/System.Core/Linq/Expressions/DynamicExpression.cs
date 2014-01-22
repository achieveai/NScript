//-----------------------------------------------------------------------
// <copyright file="DynamicExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a dynamic operation.</summary>
	public class DynamicExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.DynamicExpression.Type" /> that represents the static type of the expression.</returns>
		public extern override Type Type
		{
			get;
		}

		/// <summary>Returns the node type of this expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />, which determines the runtime behavior of the dynamic site.</summary>
		/// <returns>The <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" />, which determines the runtime behavior of the dynamic site.</returns>
		public extern CallSiteBinder Binder
		{
			get;
		}

		/// <summary>Gets the type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the type of the delegate used by the <see cref="T:System.Runtime.CompilerServices.CallSite" />.</returns>
		public extern Type DelegateType
		{
			get;
		}

		/// <summary>Gets the arguments to the dynamic operation.</summary>
		/// <returns>The read-only collections containing the arguments to the dynamic operation.</returns>
		public extern ReadOnlyCollection<Expression> Arguments
		{
			get;
		}
		/// <summary>Dispatches to the specific visit method for this node type. For example, <see cref="T:System.Linq.Expressions.MethodCallExpression" /> calls the <see cref="M:System.Linq.Expressions.ExpressionVisitor.VisitMethodCall(System.Linq.Expressions.MethodCallExpression)" />.</summary>
		/// <returns>The result of visiting this node.</returns>
		/// <param name="visitor">The visitor to visit this node with.</param>
		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.DynamicExpression.Arguments" /> property of the result.</param>
		public extern DynamicExpression Update(IEnumerable<Expression> arguments);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, params Expression[] arguments);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, IEnumerable<Expression> arguments);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2);

		public extern new static DynamicExpression Dynamic(CallSiteBinder binder, Type returnType, Expression arg0, Expression arg1, Expression arg2, Expression arg3);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, IEnumerable<Expression> arguments);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, params Expression[] arguments);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2);

		public extern new static DynamicExpression MakeDynamic(Type delegateType, CallSiteBinder binder, Expression arg0, Expression arg1, Expression arg2, Expression arg3);
	}
}
