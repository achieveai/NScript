//-----------------------------------------------------------------------
// <copyright file="LabelTarget.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Used to represent the target of a <see cref="T:System.Linq.Expressions.GotoExpression" />.</summary>
	public sealed class LabelTarget
	{
		/// <summary>Gets the name of the label.</summary>
		/// <returns>The name of the label.</returns>
		public extern string Name
		{
			get;
		}

		/// <summary>The type of value that is passed when jumping to the label (or <see cref="T:System.Void" /> if no value should be passed).</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the type of the value that is passed when jumping to the label or <see cref="T:System.Void" /> if no value should be passed</returns>
		public extern Type Type
		{
			get;
		}

	}
}
