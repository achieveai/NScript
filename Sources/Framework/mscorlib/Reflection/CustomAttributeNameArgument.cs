//-----------------------------------------------------------------------
// <copyright file="CustomAttributeNameArgument.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Represents a named argument of a custom attribute in the reflection-only context.</summary>
	[Serializable]
	public struct CustomAttributeNamedArgument
	{
		internal extern Type ArgumentType
		{
			get;
		}

		/// <summary>Gets the attribute member that would be used to set the named argument.</summary>
		/// <returns>A <see cref="T:System.Reflection.MemberInfo" /> representing the attribute member that would be used to set the named argument.</returns>
		public extern MemberInfo MemberInfo
		{
			get;
		}

		/// <summary>Gets a <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure that can be used to obtain the type and value of the current named argument.</summary>
		/// <returns>A <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structure that can be used to obtain the type and value of the current named argument.</returns>
		public extern CustomAttributeTypedArgument TypedValue
		{
			get;
		}

		public extern string MemberName
		{
			get;
		}

		public extern bool IsField
		{
			get;
		}

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are equivalent.</summary>
		/// <returns>true if the two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are equal; otherwise, false.</returns>
		/// <param name="left">The <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structure to the left of the equality operator.</param>
		/// <param name="right">The <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structure to the right of the equality operator.</param>
		public extern static bool operator ==(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right);

		/// <summary>Tests whether two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are different.</summary>
		/// <returns>true if the two <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures are different; otherwise, false.</returns>
		/// <param name="left">The <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structure to the left of the inequality operator.</param>
		/// <param name="right">The <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structure to the right of the inequality operator.</param>
		public extern static bool operator !=(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right);

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> class, which represents the specified field or property of the custom attribute, and specifies the value of the field or property.</summary>
		/// <param name="memberInfo">A field or property of the custom attribute. The new <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> object represents this member and its value.</param>
		/// <param name="value">The value of the field or property of the custom attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="memberInfo" /> is null.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="memberInfo" /> is not a field or property of the custom attribute.</exception>
		public extern CustomAttributeNamedArgument(MemberInfo memberInfo, object value);

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> class, which represents the specified field or property of the custom attribute, and specifies a <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> object that describes the type and value of the field or property.</summary>
		/// <param name="memberInfo">A field or property of the custom attribute. The new <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> object represents this member and its value.</param>
		/// <param name="typedArgument">An object that describes the type and value of the field or property.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="memberInfo" /> is null.</exception>
		public extern CustomAttributeNamedArgument(MemberInfo memberInfo, CustomAttributeTypedArgument typedArgument);
	}
}
