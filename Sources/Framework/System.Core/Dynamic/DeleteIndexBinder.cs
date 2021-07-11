//-----------------------------------------------------------------------
// <copyright file="DeleteIndexBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents the dynamic delete index operation at the call site, providing the binding semantic and the details about the operation.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class DeleteIndexBinder : DynamicMetaObjectBinder
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
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.DeleteIndexBinder" />.</summary>
		/// <param name="callInfo">The signature of the arguments at the call site.</param>
		protected DeleteIndexBinder(CallInfo callInfo)
		{
		}
		/// <summary>Performs the binding of the dynamic delete index operation.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic delete index operation.</param>
		/// <param name="args">An array of arguments of the dynamic delete index operation.</param>
		public extern sealed override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args);
		/// <summary>Performs the binding of the dynamic delete index operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic delete index operation.</param>
		/// <param name="indexes">The arguments of the dynamic delete index operation.</param>
		public extern DynamicMetaObject FallbackDeleteIndex(DynamicMetaObject target, DynamicMetaObject[] indexes);
		/// <summary>When overridden in the derived class, performs the binding of the dynamic delete index operation if the target dynamic object cannot bind.</summary>
		/// <returns>The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="target">The target of the dynamic delete index operation.</param>
		/// <param name="indexes">The arguments of the dynamic delete index operation.</param>
		/// <param name="errorSuggestion">The binding result to use if binding fails, or null.</param>
		public abstract DynamicMetaObject FallbackDeleteIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion);
	}
}
