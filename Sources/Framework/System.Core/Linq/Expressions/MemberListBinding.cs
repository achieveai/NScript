//-----------------------------------------------------------------------
// <copyright file="MemberListBinding.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;


	/// <summary>Represents initializing the elements of a collection member of a newly created object.</summary>
	public sealed class MemberListBinding : MemberBinding
	{
		/// <summary>Gets the element initializers for initializing a collection member of a newly created object.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of <see cref="T:System.Linq.Expressions.ElementInit" /> objects to initialize a collection member with.</returns>
		public extern ReadOnlyCollection<ElementInit> Initializers
		{
			get;
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="initializers">The <see cref="P:System.Linq.Expressions.MemberListBinding.Initializers" /> property of the result.</param>
		public extern MemberListBinding Update(IEnumerable<ElementInit> initializers);
	}
}
