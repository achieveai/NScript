//-----------------------------------------------------------------------
// <copyright file="CatchBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a catch statement in a try block.</summary>
	public sealed class CatchBlock
	{
		/// <summary>Gets a reference to the <see cref="T:System.Exception" /> object caught by this handler.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ParameterExpression" /> object representing a reference to the <see cref="T:System.Exception" /> object caught by this handler.</returns>
		public extern ParameterExpression Variable
		{
			get;
		}

		/// <summary>Gets the type of <see cref="T:System.Exception" /> this handler catches.</summary>
		/// <returns>The <see cref="T:System.Type" /> object representing the type of <see cref="T:System.Exception" /> this handler catches.</returns>
		public extern Type Test
		{
			get;
		}

		/// <summary>Gets the body of the catch block.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the catch body.</returns>
		public extern Expression Body
		{
			get;
		}

		/// <summary>Gets the body of the <see cref="T:System.Linq.Expressions.CatchBlock" /> filter.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object representing the body of the <see cref="T:System.Linq.Expressions.CatchBlock" /> filter.</returns>
		public extern Expression Filter
		{
			get;
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="variable">The <see cref="P:System.Linq.Expressions.CatchBlock.Variable" /> property of the result.</param>
		/// <param name="filter">The <see cref="P:System.Linq.Expressions.CatchBlock.Filter" /> property of the result.</param>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.CatchBlock.Body" /> property of the result.</param>
		public extern CatchBlock Update(ParameterExpression variable, Expression filter, Expression body);
	}
}
