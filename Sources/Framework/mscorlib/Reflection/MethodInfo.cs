//-----------------------------------------------------------------------
// <copyright file="MethodInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    // using System.Security.Permissions;

    /// <summary>Discovers the attributes of a method and provides access to method metadata.</summary>
    [Serializable]
    public abstract class MethodInfo : MethodBase
    {
        /// <summary>Gets a <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a method.</summary>
        /// <returns>A <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a method.</returns>
        public override MemberTypes MemberType
        {
            get
            {
                return MemberTypes.Method;
            }
        }
        /// <summary>Gets the return type of this method.</summary>
        /// <returns>The return type of this method.</returns>
        public virtual Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets a <see cref="T:System.Reflection.ParameterInfo" /> object that contains information about the return type of the method, such as whether the return type has custom modifiers. </summary>
        /// <returns>A <see cref="T:System.Reflection.ParameterInfo" /> object that contains information about the return type.</returns>
        /// <exception cref="T:System.NotImplementedException">This method is not implemented.</exception>
        public virtual ParameterInfo ReturnParameter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Gets the custom attributes for the return type.</summary>
        /// <returns>An ICustomAttributeProvider object representing the custom attributes for the return type.</returns>
        public abstract ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get;
        }
        /// <summary>Initializes a new instance of the <see cref="T:System.Reflection.MethodInfo" /> class.</summary>
        protected MethodInfo()
        {
        }
        /// <summary>Indicates whether two <see cref="T:System.Reflection.MethodInfo" /> objects are equal.</summary>
        /// <returns>true if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        public extern static bool operator ==(MethodInfo left, MethodInfo right);

        /// <summary>Indicates whether two <see cref="T:System.Reflection.MethodInfo" /> objects are not equal.</summary>
        /// <returns>true if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, false.</returns>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        public extern static bool operator !=(MethodInfo left, MethodInfo right);

        /// <summary>When overridden in a derived class, returns the MethodInfo object for the method on the direct or indirect base class in which the method represented by this instance was first declared.</summary>
        /// <returns>A MethodInfo object for the first implementation of this method.</returns>
        public abstract MethodInfo GetBaseDefinition();

        /// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic method or the type parameters of a generic method definition.</summary>
        /// <returns>An array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic method or the type parameters of a generic method definition. Returns an empty array if the current method is not a generic method.</returns>
        /// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
        public extern override Type[] GetGenericArguments();

        /// <summary>Returns a <see cref="T:System.Reflection.MethodInfo" /> object that represents a generic method definition from which the current method can be constructed.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object representing a generic method definition from which the current method can be constructed.</returns>
        /// <exception cref="T:System.InvalidOperationException">The current method is not a generic method. That is, <see cref="P:System.Reflection.MethodInfo.IsGenericMethod" /> returns false. </exception>
        /// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
        public extern virtual MethodInfo GetGenericMethodDefinition();

        /// <summary>Substitutes the elements of an array of types for the type parameters of the current generic method definition, and returns a <see cref="T:System.Reflection.MethodInfo" /> object representing the resulting constructed method.</summary>
        /// <returns>A <see cref="T:System.Reflection.MethodInfo" /> object that represents the constructed method formed by substituting the elements of <paramref name="typeArguments" /> for the type parameters of the current generic method definition.</returns>
        /// <param name="typeArguments">An array of types to be substituted for the type parameters of the current generic method definition.</param>
        /// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Reflection.MethodInfo" /> does not represent a generic method definition. That is, <see cref="P:System.Reflection.MethodInfo.IsGenericMethodDefinition" /> returns false.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="typeArguments" /> is null.-or- Any element of <paramref name="typeArguments" /> is null. </exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in <paramref name="typeArguments" /> is not the same as the number of type parameters of the current generic method definition.-or- An element of <paramref name="typeArguments" /> does not satisfy the constraints specified for the corresponding type parameter of the current generic method definition. </exception>
        /// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
        public extern virtual MethodInfo MakeGenericMethod(params Type[] typeArguments);

        public extern virtual Delegate CreateDelegate(Type delegateType);

        public extern virtual Delegate CreateDelegate(Type delegateType, object target);
    }
}