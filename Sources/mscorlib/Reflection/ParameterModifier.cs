//-----------------------------------------------------------------------
// <copyright file="ParameterModifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Attaches a modifier to parameters so that binding can work with parameter signatures in which the types have been modified.</summary>
	[ComVisible(true)]
	[Serializable]
	public struct ParameterModifier
	{
        internal extern bool[] IsByRefArray
        {
            get;
        }

		/// <summary>Gets or sets a value that specifies whether the parameter at the specified index position is to be modified by the current <see cref="T:System.Reflection.ParameterModifier" />.</summary>
		/// <returns>true if the parameter at this index position is to be modified by this <see cref="T:System.Reflection.ParameterModifier" />; otherwise, false.</returns>
		/// <param name="index">The index position of the parameter whose modification status is being examined or set. </param>
        public extern bool this[int index]
        {
            get;
            set;
        }

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.ParameterModifier" /> structure representing the specified number of parameters.</summary>
		/// <param name="parameterCount">The number of parameters. </param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="parameterCount" /> is negative. </exception>
        public extern ParameterModifier(int parameterCount);
	}
}
