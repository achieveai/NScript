//-----------------------------------------------------------------------
// <copyright file="SwitchCase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents one case of a <see cref="T:System.Linq.Expressions.SwitchExpression" />.</summary>
	public sealed class SwitchCase
	{
		/// <summary>Gets the values of this case. This case is selected for execution when the <see cref="P:System.Linq.Expressions.SwitchExpression.SwitchValue" /> matches any of these values.</summary>
		/// <returns>The read-only collection of the values for this case block.</returns>
        public extern ReadOnlyCollection<Expression> TestValues
        { get; }

		/// <summary>Gets the body of this case.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> object that represents the body of the case block.</returns>
        public extern Expression Body
        { get; }

		/// <summary>Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.</returns>
		public extern override string ToString();

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="testValues">The <see cref="P:System.Linq.Expressions.SwitchCase.TestValues" /> property of the result.</param>
		/// <param name="body">The <see cref="P:System.Linq.Expressions.SwitchCase.Body" /> property of the result.</param>
		public extern SwitchCase Update(IEnumerable<Expression> testValues, Expression body);
	}
}
