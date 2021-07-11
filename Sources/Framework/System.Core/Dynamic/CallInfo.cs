//-----------------------------------------------------------------------
// <copyright file="CallInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

	/// <summary>Describes arguments in the dynamic binding process.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class CallInfo
	{
		/// <summary>The number of arguments.</summary>
		/// <returns>The number of arguments.</returns>
        public extern int ArgumentCount
        {
            get;
        }
		/// <summary>The argument names.</summary>
		/// <returns>The read-only collection of argument names.</returns>
		public extern ReadOnlyCollection<string> ArgumentNames
		{
			get;
		}
		/// <summary>Creates a new PositionalArgumentInfo.</summary>
		/// <param name="argCount">The number of arguments.</param>
		/// <param name="argNames">The argument names.</param>
        public CallInfo(int argCount, params string[] argNames)
            : this(argCount, (IEnumerable<string>)argNames)
        { }
		/// <summary>Creates a new CallInfo that represents arguments in the dynamic binding process.</summary>
		/// <param name="argCount">The number of arguments.</param>
		/// <param name="argNames">The argument names.</param>
		public CallInfo(int argCount, IEnumerable<string> argNames)
        { }
		/// <summary>Serves as a hash function for the current <see cref="T:System.Dynamic.CallInfo" />.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Dynamic.CallInfo" />.</returns>
		public extern override int GetHashCode();
		/// <summary>Determines whether the specified CallInfo instance is considered equal to the current.</summary>
		/// <returns>true if the specified instance is equal to the current one otherwise, false.</returns>
		/// <param name="obj">The instance of <see cref="T:System.Dynamic.CallInfo" /> to compare with the current instance.</param>
		public extern override bool Equals(object obj);
	}
}
