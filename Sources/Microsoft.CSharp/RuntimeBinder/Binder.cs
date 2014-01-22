//-----------------------------------------------------------------------
// <copyright file="Binder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.CSharp.RuntimeBinder
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime;
    using System.Runtime.CompilerServices;

	/// <summary>Contains factory methods to create dynamic call site binders for CSharp.</summary>
	public static class Binder
	{
		/// <summary>Initializes a new CSharp binary operation binder.</summary>
		/// <returns>Returns a new CSharp binary operation binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="operation">The binary operation kind.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder BinaryOperation(CSharpBinderFlags flags, ExpressionType operation, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp convert binder.</summary>
		/// <returns>Returns a new CSharp convert binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="type">The type to convert to.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		public extern static CallSiteBinder Convert(CSharpBinderFlags flags, Type type, Type context);

		/// <summary>Initializes a new CSharp get index binder.</summary>
		/// <returns>Returns a new CSharp get index binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder GetIndex(CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp get member binder.</summary>
		/// <returns>Returns a new CSharp get member binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="name">The name of the member to get.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder GetMember(CSharpBinderFlags flags, string name, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp invoke binder.</summary>
		/// <returns>Returns a new CSharp invoke binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder Invoke(CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp invoke member binder.</summary>
		/// <returns>Returns a new CSharp invoke member binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="name">The name of the member to invoke.</param>
		/// <param name="typeArguments">The list of type arguments specified for this invoke.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder InvokeMember(CSharpBinderFlags flags, string name, IEnumerable<Type> typeArguments, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp invoke constructor binder.</summary>
		/// <returns>Returns a new CSharp invoke constructor binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder InvokeConstructor(CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp is event binder.</summary>
		/// <returns>Returns a new CSharp is event binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="name">The name of the event to look for.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		public extern static CallSiteBinder IsEvent(CSharpBinderFlags flags, string name, Type context);

		/// <summary>Initializes a new CSharp set index binder.</summary>
		/// <returns>Returns a new CSharp set index binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder SetIndex(CSharpBinderFlags flags, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp set member binder.</summary>
		/// <returns>Returns a new CSharp set member binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder SetMember(CSharpBinderFlags flags, string name, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);

		/// <summary>Initializes a new CSharp unary operation binder.</summary>
		/// <returns>Returns a new CSharp unary operation binder.</returns>
		/// <param name="flags">The flags with which to initialize the binder.</param>
		/// <param name="operation">The unary operation kind.</param>
		/// <param name="context">The <see cref="T:System.Type" /> that indicates where this operation is used.</param>
		/// <param name="argumentInfo">The sequence of <see cref="T:Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" /> instances for the arguments to this operation.</param>
		public extern static CallSiteBinder UnaryOperation(CSharpBinderFlags flags, ExpressionType operation, Type context, IEnumerable<CSharpArgumentInfo> argumentInfo);
	}
}
