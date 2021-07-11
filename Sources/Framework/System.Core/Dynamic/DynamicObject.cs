//-----------------------------------------------------------------------
// <copyright file="DynamicObject.cs" company="">
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

	/// <summary>Provides a base class for specifying dynamic behavior at run time. This class must be inherited from; you cannot instantiate it directly.</summary>
	[Serializable]
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public class DynamicObject : IDynamicMetaObjectProvider
	{
		/// <summary>Enables derived types to initialize a new instance of the <see cref="T:System.Dynamic.DynamicObject" /> type.</summary>
		protected DynamicObject()
		{
		}
		/// <summary>Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
		public extern virtual bool TryGetMember(GetMemberBinder binder, out object result);
		/// <summary>Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
		public extern virtual bool TrySetMember(SetMemberBinder binder, object value);
		/// <summary>Provides the implementation for operations that delete an object member. This method is not intended for use in C# or Visual Basic.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the deletion.</param>
		public extern virtual bool TryDeleteMember(DeleteMemberBinder binder);
		/// <summary>Provides the implementation for operations that invoke a member. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as calling a method.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleMethod". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
		/// <param name="args">The arguments that are passed to the object member during the invoke operation. For example, for the statement sampleObject.SampleMethod(100), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[0]" /> is equal to 100.</param>
		/// <param name="result">The result of the member invocation.</param>
		public extern virtual bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result);
		/// <summary>Provides implementation for type conversion operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that convert an object from one type to another.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the conversion operation. The binder.Type property provides the type to which the object must be converted. For example, for the statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic), where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Type returns the <see cref="T:System.String" /> type. The binder.Explicit property provides information about the kind of conversion that occurs. It returns true for explicit conversion and false for implicit conversion.</param>
		/// <param name="result">The result of the type conversion operation.</param>
		public extern virtual bool TryConvert(ConvertBinder binder, out object result);
		/// <summary>Provides the implementation for operations that initialize a new instance of a dynamic object. This method is not intended for use in C# or Visual Basic.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the initialization operation.</param>
		/// <param name="args">The arguments that are passed to the object during initialization. For example, for the new SampleType(100) operation, where SampleType is the type derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[0]" /> is equal to 100.</param>
		/// <param name="result">The result of the initialization.</param>
		public extern virtual bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result);
		/// <summary>Provides the implementation for operations that invoke an object. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as invoking an object or a delegate.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
		/// <param name="binder">Provides information about the invoke operation.</param>
		/// <param name="args">The arguments that are passed to the object during the invoke operation. For example, for the sampleObject(100) operation, where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="args[0]" /> is equal to 100.</param>
		/// <param name="result">The result of the object invocation.</param>
		public extern virtual bool TryInvoke(InvokeBinder binder, object[] args, out object result);
		/// <summary>Provides implementation for binary operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as addition and multiplication.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the binary operation. The binder.Operation property returns an <see cref="T:System.Linq.Expressions.ExpressionType" /> object. For example, for the sum = first + second statement, where first and second are derived from the DynamicObject class, binder.Operation returns ExpressionType.Add.</param>
		/// <param name="arg">The right operand for the binary operation. For example, for the sum = first + second statement, where first and second are derived from the DynamicObject class, <paramref name="arg" /> is equal to second.</param>
		/// <param name="result">The result of the binary operation.</param>
		public extern virtual bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result);
		/// <summary>Provides implementation for unary operations. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as negation, increment, or decrement.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the unary operation. The binder.Operation property returns an <see cref="T:System.Linq.Expressions.ExpressionType" /> object. For example, for the negativeNumber = -number statement, where number is derived from the DynamicObject class, binder.Operation returns "Negate".</param>
		/// <param name="result">The result of the unary operation.</param>
		public extern virtual bool TryUnaryOperation(UnaryOperationBinder binder, out object result);
		/// <summary>Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for indexing operations.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the operation. </param>
		/// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes[0]" /> is equal to 3.</param>
		/// <param name="result">The result of the index operation.</param>
		public extern virtual bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result);
		/// <summary>Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations that access objects by a specified index.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.</returns>
		/// <param name="binder">Provides information about the operation. </param>
		/// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="indexes[0]" /> is equal to 3.</param>
		/// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, <paramref name="value" /> is equal to 10.</param>
		public extern virtual bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value);
		/// <summary>Provides the implementation for operations that delete an object by index. This method is not intended for use in C# or Visual Basic.</summary>
		/// <returns>true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)</returns>
		/// <param name="binder">Provides information about the deletion.</param>
		/// <param name="indexes">The indexes to be deleted.</param>
		public extern virtual bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes);
		/// <summary>Returns the enumeration of all dynamic member names. </summary>
		/// <returns>A sequence that contains dynamic member names.</returns>
		public extern virtual IEnumerable<string> GetDynamicMemberNames();
		/// <summary>Provides a <see cref="T:System.Dynamic.DynamicMetaObject" /> that dispatches to the dynamic virtual methods. The object can be encapsulated inside another <see cref="T:System.Dynamic.DynamicMetaObject" /> to provide custom behavior for individual actions. This method supports the Dynamic Language Runtime infrastructure for language implementers and it is not intended to be used directly from your code.</summary>
		/// <returns>An object of the <see cref="T:System.Dynamic.DynamicMetaObject" /> type.</returns>
		/// <param name="parameter">The expression that represents <see cref="T:System.Dynamic.DynamicMetaObject" /> to dispatch to the dynamic virtual methods.</param>
		public extern virtual DynamicMetaObject GetMetaObject(Expression parameter);
	}
}
