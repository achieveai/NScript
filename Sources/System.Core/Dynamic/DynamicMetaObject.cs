//-----------------------------------------------------------------------
// <copyright file="DynamicMetaObject.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

	/// <summary>Represents the dynamic binding and a binding logic of an object participating in the dynamic binding.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public class DynamicMetaObject
	{
		/// <summary>Represents an empty array of type <see cref="T:System.Dynamic.DynamicMetaObject" />. This field is read only.</summary>
		public static readonly DynamicMetaObject[] EmptyMetaObjects = null;
		/// <summary>The expression representing the <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</summary>
		/// <returns>The expression representing the <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</returns>
		public extern Expression Expression
		{
			get;
		}
		/// <summary>The set of binding restrictions under which the binding is valid.</summary>
		/// <returns>The set of binding restrictions.</returns>
		public extern BindingRestrictions Restrictions
		{
			get;
		}
		/// <summary>The runtime value represented by this <see cref="T:System.Dynamic.DynamicMetaObject" />.</summary>
		/// <returns>The runtime value represented by this <see cref="T:System.Dynamic.DynamicMetaObject" />.</returns>
		public extern object Value
		{
			get;
		}
		/// <summary>Gets a value indicating whether the <see cref="T:System.Dynamic.DynamicMetaObject" /> has the runtime value.</summary>
		/// <returns>True if the <see cref="T:System.Dynamic.DynamicMetaObject" /> has the runtime value, otherwise false.</returns>
		public extern bool HasValue
		{
			get;
		}
		/// <summary>Gets the <see cref="T:System.Type" /> of the runtime value or null if the <see cref="T:System.Dynamic.DynamicMetaObject" /> has no value associated with it.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the runtime value or null.</returns>
		public extern Type RuntimeType
		{
			get;
		}
		/// <summary>Gets the limit type of the <see cref="T:System.Dynamic.DynamicMetaObject" />.</summary>
		/// <returns>
		///   <see cref="P:System.Dynamic.DynamicMetaObject.RuntimeType" /> if runtime value is available, a type of the <see cref="P:System.Dynamic.DynamicMetaObject.Expression" /> otherwise.</returns>
		public extern Type LimitType
		{
			get;
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.DynamicMetaObject" /> class.</summary>
		/// <param name="expression">The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</param>
		/// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
		public DynamicMetaObject(Expression expression, BindingRestrictions restrictions)
		{
		}
		/// <summary>Initializes a new instance of the <see cref="T:System.Dynamic.DynamicMetaObject" /> class.</summary>
		/// <param name="expression">The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</param>
		/// <param name="restrictions">The set of binding restrictions under which the binding is valid.</param>
		/// <param name="value">The runtime value represented by the <see cref="T:System.Dynamic.DynamicMetaObject" />.</param>
        public DynamicMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : this(expression, restrictions)
        { }
		/// <summary>Performs the binding of the dynamic conversion operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.ConvertBinder" /> that represents the details of the dynamic operation.</param>
		public extern virtual DynamicMetaObject BindConvert(ConvertBinder binder);
		/// <summary>Performs the binding of the dynamic get member operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetMemberBinder" /> that represents the details of the dynamic operation.</param>
		public extern virtual DynamicMetaObject BindGetMember(GetMemberBinder binder);
		/// <summary>Performs the binding of the dynamic set member operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.SetMemberBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="value">The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the value for the set member operation.</param>
		public extern virtual DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value);
		/// <summary>Performs the binding of the dynamic delete member operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.DeleteMemberBinder" /> that represents the details of the dynamic operation.</param>
		public extern virtual DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder);
		/// <summary>Performs the binding of the dynamic get index operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.GetIndexBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="indexes">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - indexes for the get index operation.</param>
		public extern virtual DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes);
		/// <summary>Performs the binding of the dynamic set index operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.SetIndexBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="indexes">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - indexes for the set index operation.</param>
		/// <param name="value">The <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the value for the set index operation.</param>
		public extern virtual DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value);
		/// <summary>Performs the binding of the dynamic delete index operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.DeleteIndexBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="indexes">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - indexes for the delete index operation.</param>
		public extern virtual DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes);
		/// <summary>Performs the binding of the dynamic invoke member operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.InvokeMemberBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="args">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - arguments to the invoke member operation.</param>
		public extern virtual DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args);
		/// <summary>Performs the binding of the dynamic invoke operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.InvokeBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="args">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - arguments to the invoke operation.</param>
		public extern virtual DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args);
		/// <summary>Performs the binding of the dynamic create instance operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.CreateInstanceBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="args">An array of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances - arguments to the create instance operation.</param>
		public extern virtual DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args);
		/// <summary>Performs the binding of the dynamic unary operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.UnaryOperationBinder" /> that represents the details of the dynamic operation.</param>
		public extern virtual DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder);
		/// <summary>Performs the binding of the dynamic binary operation.</summary>
		/// <returns>The new <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the result of the binding.</returns>
		/// <param name="binder">An instance of the <see cref="T:System.Dynamic.BinaryOperationBinder" /> that represents the details of the dynamic operation.</param>
		/// <param name="arg">An instance of the <see cref="T:System.Dynamic.DynamicMetaObject" /> representing the right hand side of the binary operation.</param>
		public extern virtual DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg);
		/// <summary>Returns the enumeration of all dynamic member names.</summary>
		/// <returns>The list of dynamic member names.</returns>
		public extern virtual IEnumerable<string> GetDynamicMemberNames();
		/// <summary>Creates a meta-object for the specified object.</summary>
		/// <returns>If the given object implements <see cref="T:System.Dynamic.IDynamicMetaObjectProvider" /> and is not a remote object from outside the current AppDomain, returns the object's specific meta-object returned by <see cref="M:System.Dynamic.IDynamicMetaObjectProvider.GetMetaObject(System.Linq.Expressions.Expression)" />. Otherwise a plain new meta-object with no restrictions is created and returned.</returns>
		/// <param name="value">The object to get a meta-object for.</param>
		/// <param name="expression">The expression representing this <see cref="T:System.Dynamic.DynamicMetaObject" /> during the dynamic binding process.</param>
		public extern static DynamicMetaObject Create(object value, Expression expression);
	}
}
