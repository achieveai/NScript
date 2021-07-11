//-----------------------------------------------------------------------
// <copyright file="UnaryOperationBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

	/// <summary>Represents the unary dynamic operation at the call site, providing the binding semantic and the details about the operation.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class UnaryOperationBinder : DynamicMetaObjectBinder
	{
		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public extern sealed override Type ReturnType
		{
			get;
		}
		/// <summary>The unary operation kind.</summary>
		/// <returns>The object of the <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents the unary operation kind.</returns>
		public extern ExpressionType Operation
		{
			get;
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.BinaryOperationBinder" /> class.</summary>
		/// <param name="operation">The unary operation kind.</param>
		protected UnaryOperationBinder(ExpressionType operation)
		{
		}
		/// <summary>Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic unary operation.</param>
		public extern DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target);
		/// <summary>Performs the binding of the unary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic unary operation.</param>
		/// <param name="errorSuggestion">The binding result in case the binding fails, or null.</param>
		public abstract DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion);
		/// <summary>Performs the binding of the dynamic unary operation.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic operation.</param>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
		public extern sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
	}
}
