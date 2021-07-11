//-----------------------------------------------------------------------
// <copyright file="RuntimeModule.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	internal class RuntimeModule : Module
	{
		public extern override int MDStreamVersion
		{
			get;
		}

		public extern override string FullyQualifiedName
		{
			get;
		}

		public extern override int MetadataToken
		{
			get;
		}

		public extern override string ScopeName
		{
			get;
		}

		public extern override string Name
		{
			get;
		}

		public extern override Assembly Assembly
		{
			get;
		}

		internal RuntimeModule()
		{
		}

		public extern override byte[] ResolveSignature(int metadataToken);

		public extern unsafe override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);

		public extern unsafe override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);

		public extern override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);

		public extern unsafe override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);

		public extern override string ResolveString(int metadataToken);

		internal extern bool IsTransientInternal();

		public extern override object[] GetCustomAttributes(bool inherit);

		public extern override object[] GetCustomAttributes(Type attributeType, bool inherit);

		public extern override bool IsDefined(Type attributeType, bool inherit);

		public extern override IList<CustomAttributeData> GetCustomAttributesData();

		[ComVisible(true)]
		public extern override Type GetType(string className, bool throwOnError, bool ignoreCase);

		internal extern string GetFullyQualifiedName();

		public extern override Type[] GetTypes();

		public extern override bool IsResource();

		public extern override FieldInfo[] GetFields(BindingFlags bindingFlags);

		public extern override FieldInfo GetField(string name, BindingFlags bindingAttr);

		public extern override MethodInfo[] GetMethods(BindingFlags bindingFlags);
	}
}
