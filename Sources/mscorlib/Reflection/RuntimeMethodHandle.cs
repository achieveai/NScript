//-----------------------------------------------------------------------
// <copyright file="RuntimeMethodHandle.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>
	///   <see cref="T:System.RuntimeMethodHandle" /> is a handle to the internal metadata representation of a method.</summary>
	/// <filterpriority>2</filterpriority>
	[Serializable]
	public struct RuntimeMethodHandle
	{
		internal static RuntimeMethodHandle EmptyHandle
		{
			get
			{
				return default(RuntimeMethodHandle);
			}
		}
		/// <summary>Gets the value of this instance.</summary>
		/// <returns>A <see cref="T:System.RuntimeMethodHandle" /> that is the internal metadata representation of a method.</returns>
		/// <filterpriority>2</filterpriority>
		public extern IntPtr Value
		{
			get;
		}

		/// <summary>Indicates whether two instances of <see cref="T:System.RuntimeMethodHandle" /> are equal.</summary>
		/// <returns>true if the value of <paramref name="left" /> is equal to the value of <paramref name="right" />; otherwise, false.</returns>
		/// <param name="left">A <see cref="T:System.RuntimeMethodHandle" /> to compare to <paramref name="right" />.</param>
		/// <param name="right">A <see cref="T:System.RuntimeMethodHandle" /> to compare to <paramref name="left" />.</param>
		/// <filterpriority>3</filterpriority>
		public extern static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right);

		/// <summary>Indicates whether two instances of <see cref="T:System.RuntimeMethodHandle" /> are not equal.</summary>
		/// <returns>true if the value of <paramref name="left" /> is unequal to the value of <paramref name="right" />; otherwise, false.</returns>
		/// <param name="left">A <see cref="T:System.RuntimeMethodHandle" /> to compare to <paramref name="right" />.</param>
		/// <param name="right">A <see cref="T:System.RuntimeMethodHandle" /> to compare to <paramref name="left" />.</param>
		/// <filterpriority>3</filterpriority>
		public extern static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right);

		/// <summary>Indicates whether this instance is equal to a specified <see cref="T:System.RuntimeMethodHandle" />.</summary>
		/// <returns>true if <paramref name="handle" /> is equal to the value of this instance; otherwise, false.</returns>
		/// <param name="handle">A <see cref="T:System.RuntimeMethodHandle" /> to compare to this instance.</param>
		/// <filterpriority>2</filterpriority>
		public extern bool Equals(RuntimeMethodHandle handle);

		internal extern bool IsNullHandle();

		/// <summary>Obtains a pointer to the method represented by this instance.</summary>
		/// <returns>A pointer to the method represented by this instance.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the necessary permission to perform this operation.</exception>
		/// <filterpriority>2</filterpriority>
		public extern IntPtr GetFunctionPointer();
	}
}
