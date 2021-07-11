//-----------------------------------------------------------------------
// <copyright file="LocalVariableInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Discovers the attributes of a local variable and provides access to local variable metadata.</summary>
	public class LocalVariableInfo
	{
		/// <summary>Gets the type of the local variable.</summary>
		/// <returns>The type of the local variable.</returns>
		public extern virtual Type LocalType
		{
			get;
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the object referred to by the local variable is pinned in memory.</summary>
		/// <returns>true if the object referred to by the variable is pinned in memory; otherwise, false.</returns>
		public extern virtual bool IsPinned
		{
			get;
		}

		/// <summary>Gets the index of the local variable within the method body.</summary>
		/// <returns>An integer value that represents the order of declaration of the local variable within the method body.</returns>
		public extern virtual int LocalIndex
		{
			get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.LocalVariableInfo" /> class.</summary>
		protected LocalVariableInfo()
		{
		}
	}
}
