//-----------------------------------------------------------------------
// <copyright file="CreateInstanceBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents the dynamic create operation at the call site, providing the binding semantic and the details about the operation.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class CreateInstanceBinder : DynamicMetaObjectBinder
	{
		/// <summary>The result type of the operation.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the result type of the operation.</returns>
		public extern sealed override Type ReturnType
		{
			get;
		}
		/// <summary>Gets the signature of the arguments at the call site.</summary>
		/// <returns>The signature of the arguments at the call site.</returns>
		public extern CallInfo CallInfo
		{
			get;
		}
		/// <summary>Initializes a new intsance of the <see cref="T:System.Dynamic.CreateInstanceBinder" />.</summary>
		/// <param name="callInfo">The signature of the arguments at the call site.</param>
		protected CreateInstanceBinder(CallInfo callInfo)
		{
		}
		/// <summary>Performs the binding of the dynamic create operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic create operation.</param>
		/// <param name="args">The arguments of the dynamic create operation.</param>
		public extern DynamicMetaObject FallbackCreateInstance(DynamicMetaObject target, DynamicMetaObject[] args);
		/// <summary>When overridden in the derived class, performs the binding of the dynamic create operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic create operation.</param>
		/// <param name="args">The arguments of the dynamic create operation.</param>
		/// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
		public abstract DynamicMetaObject FallbackCreateInstance(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion);
		/// <summary>Performs the binding of the dynamic create operation.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic create operation.</param>
		/// <param name="args">An array of arguments of the dynamic create operation.</param>
		public extern sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
	}
}
