//-----------------------------------------------------------------------
// <copyright file="BinaryOperationBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

	/// <summary>Represents the binary dynamic operation at the call site, providing the binding semantic and the details about the operation.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class BinaryOperationBinder : DynamicMetaObjectBinder
	{
		/// <summary>The result type of the operation.</summary>
		/// <returns>The result type of the operation.</returns>
        public extern sealed override Type ReturnType
        {
            get;
        }
		/// <summary>The binary operation kind.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> object representing the kind of binary operation.</returns>
		public extern ExpressionType Operation
		{
			get;
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.BinaryOperationBinder" /> class.</summary>
		/// <param name="operation">The binary operation kind.</param>
		protected extern BinaryOperationBinder(ExpressionType operation);
		/// <summary>Performs the binding of the binary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic binary operation.</param>
		/// <param name="arg">The right hand side operand of the dynamic binary operation.</param>
		public extern DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg);
		/// <summary>When overridden in the derived class, performs the binding of the binary dynamic operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic binary operation.</param>
		/// <param name="arg">The right hand side operand of the dynamic binary operation.</param>
		/// <param name="errorSuggestion">The binding result if the binding fails, or null.</param>
		public abstract DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion);
		/// <summary>Performs the binding of the dynamic binary operation.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic operation.</param>
		/// <param name="args">An array of arguments of the dynamic operation.</param>
        public extern sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
	}
}
