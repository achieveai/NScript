//-----------------------------------------------------------------------
// <copyright file="ParameterInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Discovers the attributes of a parameter and provides access to parameter metadata.</summary>
	[Serializable]
	public class ParameterInfo : ICustomAttributeProvider
	{
		/// <summary>Gets the Type of this parameter.</summary>
		/// <returns>The Type object that represents the Type of this parameter.</returns>
		public extern virtual Type ParameterType
		{
			get;
		}

		/// <summary>Gets the name of the parameter.</summary>
		/// <returns>The simple name of this parameter.</returns>
		public extern virtual string Name
		{
			get;
		}

		public extern virtual bool HasDefaultValue
		{
			get;
		}

		/// <summary>Gets a value indicating the default value if the parameter has a default value.</summary>
		/// <returns>The default value of the parameter, or <see cref="F:System.DBNull.Value" /> if the parameter has no default value.</returns>
		public extern virtual object DefaultValue
		{
			get;
		}

		/// <summary>Gets a value indicating the default value if the parameter has a default value.</summary>
		/// <returns>The default value of the parameter, or <see cref="F:System.DBNull.Value" /> if the parameter has no default value.</returns>
		public extern virtual object RawDefaultValue
		{
			get;
		}

		/// <summary>Gets the zero-based position of the parameter in the formal parameter list.</summary>
		/// <returns>An integer representing the position this parameter occupies in the parameter list.</returns>
		public extern virtual int Position
		{
			get;
		}

		/// <summary>Gets the attributes for this parameter.</summary>
		/// <returns>A ParameterAttributes object representing the attributes for this parameter.</returns>
		public extern virtual ParameterAttributes Attributes
		{
			get;
		}

		/// <summary>Gets a value indicating the member in which the parameter is implemented.</summary>
		/// <returns>The member which implanted the parameter represented by this <see cref="T:System.Reflection.ParameterInfo" />.</returns>
		public extern virtual MemberInfo Member
		{
			get;
		}

		/// <summary>Gets a value indicating whether this is an input parameter.</summary>
		/// <returns>true if the parameter is an input parameter; otherwise, false.</returns>
		public extern bool IsIn
		{
			get;
		}

		/// <summary>Gets a value indicating whether this is an output parameter.</summary>
		/// <returns>true if the parameter is an output parameter; otherwise, false.</returns>
		public extern bool IsOut
		{
			get;
		}

		/// <summary>Gets a value indicating whether this parameter is a locale identifier (lcid).</summary>
		/// <returns>true if the parameter is a locale identifier; otherwise, false.</returns>
		public extern bool IsLcid
		{
			get;
		}

		/// <summary>Gets a value indicating whether this is a Retval parameter.</summary>
		/// <returns>true if the parameter is a Retval; otherwise, false.</returns>
		public extern bool IsRetval
		{
			get;
		}

		/// <summary>Gets a value indicating whether this parameter is optional.</summary>
		/// <returns>true if the parameter is optional; otherwise, false.</returns>
		public extern bool IsOptional
		{
			get;
		}

		/// <summary>Gets a value that identifies this parameter in metadata.</summary>
		/// <returns>A value which, in combination with the module, uniquely identifies this parameter in metadata.</returns>
		public extern virtual int MetadataToken
		{
			get;
		}

		public extern virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get;
		}

		/// <summary>Initializes a new instance of the ParameterInfo class.</summary>
		protected ParameterInfo()
		{
		}
		internal extern void SetName(string name);

		internal extern void SetAttributes(ParameterAttributes attributes);

		/// <summary>Gets the required custom modifiers of the parameter.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that identify the required custom modifiers of the current parameter, such as <see cref="T:System.Runtime.CompilerServices.IsConst" /> or <see cref="T:System.Runtime.CompilerServices.IsImplicitlyDereferenced" />.</returns>
		public extern virtual Type[] GetRequiredCustomModifiers();

		/// <summary>Gets the optional custom modifiers of the parameter.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that identify the optional custom modifiers of the current parameter, such as <see cref="T:System.Runtime.CompilerServices.IsConst" /> or <see cref="T:System.Runtime.CompilerServices.IsImplicitlyDereferenced" />.</returns>
		public extern virtual Type[] GetOptionalCustomModifiers();

		/// <summary>Gets the parameter type and name represented as a string.</summary>
		/// <returns>A string containing the type and the name of the parameter.</returns>
		public extern override string ToString();

		/// <summary>Gets all the custom attributes defined on this parameter.</summary>
		/// <returns>An array that contains all the custom attributes applied to this parameter.</returns>
		/// <param name="inherit">This argument is ignored for objects of this type. See Remarks.</param>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type could not be loaded. </exception>
		public extern virtual object[] GetCustomAttributes(bool inherit);

		/// <summary>Gets the custom attributes of the specified type or its derived types that are applied to this parameter.</summary>
		/// <returns>An array that contains the custom attributes of the specified type or its derived types.</returns>
		/// <param name="attributeType">The custom attributes identified by type. </param>
		/// <param name="inherit">This argument is ignored for objects of this type. See Remarks.</param>
		/// <exception cref="T:System.ArgumentException">The type must be a type provided by the underlying runtime system.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is null.</exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type could not be loaded. </exception>
		public extern virtual object[] GetCustomAttributes(Type attributeType, bool inherit);

		/// <summary>Determines whether the custom attribute of the specified type or its derived types is applied to this parameter.</summary>
		/// <returns>true if one or more instances of <paramref name="attributeType" /> or its derived types are applied to this parameter; otherwise, false.</returns>
		/// <param name="attributeType">The Type object to search for. </param>
		/// <param name="inherit">This argument is ignored for objects of this type. See Remarks.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is null. </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="attributeType" /> is not a <see cref="T:System.Type" /> object supplied by the common language runtime.</exception>
		public extern virtual bool IsDefined(Type attributeType, bool inherit);

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects for the current parameter, which can be used in the reflection-only context.</summary>
		/// <returns>A generic list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the current parameter.</returns>
		public extern virtual IList<CustomAttributeData> GetCustomAttributesData();
	}
}
