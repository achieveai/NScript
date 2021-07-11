//-----------------------------------------------------------------------
// <copyright file="ModuleHandle.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Represents a runtime handle for a module.</summary>
	/// <filterpriority>2</filterpriority>
	[ComVisible(true)]
	public struct ModuleHandle
	{
		/// <summary>Gets the metadata stream version.</summary>
		/// <returns>A 32-bit integer representing the metadata stream version. The high-order two bytes represent the major version number, and the low-order two bytes represent the minor version number.</returns>
		public extern int MDStreamVersion
		{
			get;
		}

		internal extern bool IsNullHandle();


		/// <summary>Tests whether two <see cref="T:System.ModuleHandle" /> structures are equal.</summary>
		/// <returns>true if the <see cref="T:System.ModuleHandle" /> structures are equal; otherwise, false.</returns>
		/// <param name="left">The <see cref="T:System.ModuleHandle" /> structure to the left of the equality operator.</param>
		/// <param name="right">The <see cref="T:System.ModuleHandle" /> structure to the right of the equality operator.</param>
		/// <filterpriority>3</filterpriority>
		public extern static bool operator ==(ModuleHandle left, ModuleHandle right);

		/// <summary>Tests whether two <see cref="T:System.ModuleHandle" /> structures are unequal.</summary>
		/// <returns>true if the <see cref="T:System.ModuleHandle" /> structures are unequal; otherwise, false.</returns>
		/// <param name="left">The <see cref="T:System.ModuleHandle" /> structure to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="T:System.ModuleHandle" /> structure to the right of the inequality operator.</param>
		/// <filterpriority>3</filterpriority>
		public extern static bool operator !=(ModuleHandle left, ModuleHandle right);

		/// <summary>Returns a runtime type handle for the type identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeTypeHandle" /> for the type identified by <paramref name="typeToken" />.</returns>
		/// <param name="typeToken">A metadata token that identifies a type in the module.</param>
		/// <filterpriority>2</filterpriority>
		public extern RuntimeTypeHandle GetRuntimeTypeHandleFromMetadataToken(int typeToken);

		/// <summary>Returns a runtime type handle for the type identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeTypeHandle" /> for the type identified by <paramref name="typeToken" />.</returns>
		/// <param name="typeToken">A metadata token that identifies a type in the module.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="typeToken" /> is not a valid metadata token for a type in the current module.-or-<paramref name="metadataToken" /> is not a token for a type in the scope of the current module.-or-<paramref name="metadataToken" /> is a TypeSpec whose signature contains element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty type handle.</exception>
		/// <filterpriority>1</filterpriority>
		public extern RuntimeTypeHandle ResolveTypeHandle(int typeToken);

		/// <summary>Returns a runtime type handle for the type identified by the specified metadata token, specifying the generic type arguments of the type and method where the token is in scope.</summary>
		/// <returns>A <see cref="T:System.RuntimeTypeHandle" /> for the type identified by <paramref name="typeToken" />.</returns>
		/// <param name="typeToken">A metadata token that identifies a type in the module.</param>
		/// <param name="typeInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures representing the generic type arguments of the type where the token is in scope, or null if that type is not generic. </param>
		/// <param name="methodInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures objects representing the generic type arguments of the method where the token is in scope, or null if that method is not generic.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="typeToken" /> is not a valid metadata token for a type in the current module.-or-<paramref name="metadataToken" /> is not a token for a type in the scope of the current module.-or-<paramref name="metadataToken" /> is a TypeSpec whose signature contains element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty type handle.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="typeToken " />is not a valid token.</exception>
		public extern RuntimeTypeHandle ResolveTypeHandle(int typeToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext);

		/// <summary>Returns a runtime method handle for the method or constructor identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeMethodHandle" /> for the method or constructor identified by <paramref name="methodToken" />.</returns>
		/// <param name="methodToken">A metadata token that identifies a method or constructor in the module.</param>
		/// <filterpriority>2</filterpriority>
		public extern RuntimeMethodHandle GetRuntimeMethodHandleFromMetadataToken(int methodToken);

		/// <summary>Returns a runtime method handle for the method or constructor identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeMethodHandle" /> for the method or constructor identified by <paramref name="methodToken" />.</returns>
		/// <param name="methodToken">A metadata token that identifies a method or constructor in the module.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="methodToken" /> is not a valid metadata token for a method in the current module.-or-<paramref name="metadataToken" /> is not a token for a method or constructor in the scope of the current module.-or-<paramref name="metadataToken" /> is a MethodSpec whose signature contains element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty method handle.</exception>
		/// <filterpriority>1</filterpriority>
		public extern RuntimeMethodHandle ResolveMethodHandle(int methodToken);

		/// <summary>Returns a runtime method handle for the method or constructor identified by the specified metadata token, specifying the generic type arguments of the type and method where the token is in scope.</summary>
		/// <returns>A <see cref="T:System.RuntimeMethodHandle" /> for the method or constructor identified by <paramref name="methodToken" />.</returns>
		/// <param name="methodToken">A metadata token that identifies a method or constructor in the module.</param>
		/// <param name="typeInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures representing the generic type arguments of the type where the token is in scope, or null if that type is not generic. </param>
		/// <param name="methodInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures representing the generic type arguments of the method where the token is in scope, or null if that method is not generic.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="methodToken" /> is not a valid metadata token for a method in the current module.-or-<paramref name="metadataToken" /> is not a token for a method or constructor in the scope of the current module.-or-<paramref name="metadataToken" /> is a MethodSpec whose signature contains element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty method handle.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="methodToken " />is not a valid token.</exception>
		public extern RuntimeMethodHandle ResolveMethodHandle(int methodToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext);

		/// <summary>Returns a runtime handle for the field identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeFieldHandle" /> for the field identified by <paramref name="fieldToken" />.</returns>
		/// <param name="fieldToken">A metadata token that identifies a field in the module.</param>
		/// <filterpriority>2</filterpriority>
		public extern RuntimeFieldHandle GetRuntimeFieldHandleFromMetadataToken(int fieldToken);

		/// <summary>Returns a runtime handle for the field identified by the specified metadata token.</summary>
		/// <returns>A <see cref="T:System.RuntimeFieldHandle" /> for the field identified by <paramref name="fieldToken" />.</returns>
		/// <param name="fieldToken">A metadata token that identifies a field in the module.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.-or-<paramref name="metadataToken" /> is not a token for a field in the scope of the current module.-or-<paramref name="metadataToken" /> identifies a field whose parent TypeSpec has a signature containing element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty field handle.</exception>
		/// <filterpriority>1</filterpriority>
		public extern RuntimeFieldHandle ResolveFieldHandle(int fieldToken);

		/// <summary>Returns a runtime field handle for the field identified by the specified metadata token, specifying the generic type arguments of the type and method where the token is in scope.</summary>
		/// <returns>A <see cref="T:System.RuntimeFieldHandle" /> for the field identified by <paramref name="fieldToken" />.</returns>
		/// <param name="fieldToken">A metadata token that identifies a field in the module.</param>
		/// <param name="typeInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures representing the generic type arguments of the type where the token is in scope, or null if that type is not generic. </param>
		/// <param name="methodInstantiationContext">An array of <see cref="T:System.RuntimeTypeHandle" /> structures representing the generic type arguments of the method where the token is in scope, or null if that method is not generic.</param>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.-or-<paramref name="metadataToken" /> is not a token for a field in the scope of the current module.-or-<paramref name="metadataToken" /> identifies a field whose parent TypeSpec has a signature containing element type var or mvar.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method is called on an empty field handle.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="fieldToken " />is not a valid token.</exception>
		public extern RuntimeFieldHandle ResolveFieldHandle(int fieldToken, RuntimeTypeHandle[] typeInstantiationContext, RuntimeTypeHandle[] methodInstantiationContext);
	}
}
