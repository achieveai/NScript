//-----------------------------------------------------------------------
// <copyright file="DynamicMetaObjectBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

	/// <summary>The dynamic call site binder that participates in the <see cref="T:System.Dynamic.DynamicMetaObject" /> binding protocol.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class DynamicMetaObjectBinder : CallSiteBinder
	{
		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public extern virtual Type ReturnType
		{
			get;
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.DynamicMetaObjectBinder" /> class.</summary>
		protected DynamicMetaObjectBinder()
		{
		}
		/// <summary>Performs the runtime binding of the dynamic operation on a set of arguments.</summary>
		/// <returns>An Expression that performs tests on the dynamic operation arguments, and performs the dynamic operation if the tests are valid. If the tests fail on subsequent occurrences of the dynamic operation, Bind will be called again to produce a new <see cref="T:System.Linq.Expressions.Expression" /> for the new argument types.</returns>
		/// <param name="args">An array of arguments to the dynamic operation.</param>
		/// <param name="parameters">The array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> instances that represent the parameters of the call site in the binding process.</param>
		/// <param name="returnLabel">A LabelTarget used to return the result of the dynamic binding.</param>
		public extern sealed override Expression Bind(object[] args, ReadOnlyCollection<ParameterExpression> parameters, LabelTarget returnLabel);
		/// <summary>When overridden in the derived class, performs the binding of the dynamic operation.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic operation.</param>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
		public abstract DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
		/// <summary>Gets an expression that will cause the binding to be updated. It indicates that the expression's binding is no longer valid. This is typically used when the "version" of a dynamic object has changed.</summary>
		/// <returns>The update expression.</returns>
		/// <param name="type">The <see cref="P:System.Linq.Expressions.Expression.Type" /> property of the resulting expression; any type is allowed.</param>
		public extern Expression GetUpdateExpression(Type type);
		/// <summary>Defers the binding of the operation until later time when the runtime values of all dynamic operation arguments have been computed.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic operation.</param>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
		public extern DynamicMetaObject Defer(DynamicMetaObject target, params DynamicMetaObject[] args);
		/// <summary>Defers the binding of the operation until later time when the runtime values of all dynamic operation arguments have been computed.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
		public extern DynamicMetaObject Defer(params DynamicMetaObject[] args);
	}
}
