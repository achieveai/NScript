//-----------------------------------------------------------------------
// <copyright file="CustomAttributeData.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Provides access to custom attribute data for assemblies, modules, types, members and parameters that are loaded into the reflection-only context.</summary>
	[Serializable]
	public class CustomAttributeData
	{
		public extern Type AttributeType
		{
			get;
		}

		/// <summary>Returns a <see cref="T:System.Reflection.ConstructorInfo" /> object representing the constructor that would have initialized the custom attribute.</summary>
		/// <returns>A <see cref="T:System.Reflection.ConstructorInfo" /> object representing the constructor that would have initialized the custom attribute represented by the current instance of the <see cref="T:System.Reflection.CustomAttributeData" /> class.</returns>
		[ComVisible(true)]
		public new extern virtual ConstructorInfo Constructor
		{
			get;
		}

		/// <summary>Gets the list of positional arguments specified for the attribute instance represented by the <see cref="T:System.Reflection.CustomAttributeData" /> object.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeTypedArgument" /> structures representing the positional arguments specified for the custom attribute instance.</returns>
		public extern virtual IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get;
		}

		/// <summary>Gets the list of named arguments specified for the attribute instance represented by the <see cref="T:System.Reflection.CustomAttributeData" /> object.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeNamedArgument" /> structures representing the named arguments specified for the custom attribute instance.</returns>
		public extern virtual IList<CustomAttributeNamedArgument> NamedArguments
		{
			get;
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target member.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target member.</returns>
		/// <param name="target">A <see cref="T:System.Reflection.MemberInfo" /> object representing the member whose attribute data is to be retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is null.</exception>
		public extern static IList<CustomAttributeData> GetCustomAttributes(MemberInfo target);

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target module.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target module.</returns>
		/// <param name="target">The module whose custom attribute data is to be retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is null.</exception>
		public extern static IList<CustomAttributeData> GetCustomAttributes(Module target);

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target assembly.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target assembly.</returns>
		/// <param name="target">The assembly whose custom attribute data is to be retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is null.</exception>
		public extern static IList<CustomAttributeData> GetCustomAttributes(Assembly target);

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target parameter.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target parameter.</returns>
		/// <param name="target">A <see cref="T:System.Reflection.ParameterInfo" /> object representing the parameter whose attribute data is to be retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is null.</exception>
		public extern static IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target);

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeData" /> class.</summary>
		protected extern CustomAttributeData();
	}
}
