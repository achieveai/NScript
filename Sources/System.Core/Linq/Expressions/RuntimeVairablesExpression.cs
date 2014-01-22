//-----------------------------------------------------------------------
// <copyright file="RuntimeVairablesExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>An expression that provides runtime read/write permission for variables.</summary>
	public sealed class RuntimeVariablesExpression : Expression
	{
		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Type" /> that represents the static type of the expression.</returns>
        public extern sealed override Type Type
        { get; }

		/// <summary>Returns the node type of this Expression. Extension nodes should return <see cref="F:System.Linq.Expressions.ExpressionType.Extension" /> when overriding this method.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
        public extern sealed override ExpressionType NodeType
        { get; }

		/// <summary>The variables or parameters to which to provide runtime access.</summary>
		/// <returns>The read-only collection containing parameters that will be provided the runtime access.</returns>
        public extern ReadOnlyCollection<ParameterExpression> Variables
        { get; }

		protected extern override Expression Accept(ExpressionVisitor visitor);

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="variables">The <see cref="P:System.Linq.Expressions.RuntimeVariablesExpression.Variables" /> property of the result.</param>
		public extern RuntimeVariablesExpression Update(IEnumerable<ParameterExpression> variables);
	}
}
