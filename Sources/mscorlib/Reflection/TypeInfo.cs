//-----------------------------------------------------------------------
// <copyright file="TypeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	[Serializable]
	public abstract class TypeInfo : Type, IReflectableType
	{
		public extern virtual Type[] GenericTypeParameters
		{
			get;
		}

		public extern virtual IEnumerable<ConstructorInfo> DeclaredConstructors
		{
			get;
		}

		public extern virtual IEnumerable<EventInfo> DeclaredEvents
		{
			get;
		}

		public extern virtual IEnumerable<FieldInfo> DeclaredFields
		{
			get;
		}

		public extern virtual IEnumerable<MemberInfo> DeclaredMembers
		{
			get;
		}

		public extern virtual IEnumerable<MethodInfo> DeclaredMethods
		{
			get;
		}

		public extern virtual IEnumerable<TypeInfo> DeclaredNestedTypes
		{
			get;
		}

		public extern virtual IEnumerable<PropertyInfo> DeclaredProperties
		{
			get;
		}

		public extern virtual IEnumerable<Type> ImplementedInterfaces
		{
			get;
		}

		internal TypeInfo()
		{
		}

		TypeInfo IReflectableType.GetTypeInfo()
		{
			return this;
		}

		public extern virtual Type AsType();

		public extern virtual bool IsAssignableFrom(TypeInfo typeInfo);

		public extern virtual EventInfo GetDeclaredEvent(string name);

		public extern virtual FieldInfo GetDeclaredField(string name);

		public extern virtual MethodInfo GetDeclaredMethod(string name);

		public extern virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name);

		public extern virtual TypeInfo GetDeclaredNestedType(string name);

		public extern virtual PropertyInfo GetDeclaredProperty(string name);
	}
}
