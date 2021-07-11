//-----------------------------------------------------------------------
// <copyright file="IndexExpression.cs" company="">
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

	/// <summary>Represents indexing a property or array.</summary>
	public sealed class IndexExpression : Expression
	{
		/// <summary>Returns the node type of this <see cref="T:System.Linq.Expressions.Expression" />.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.ExpressionType" /> that represents this expression.</returns>
		public extern sealed override ExpressionType NodeType
		{
			get;
		}

		/// <summary>Gets the static type of the expression that this <see cref="T:System.Linq.Expressions.Expression" /> represents.</summary>
		/// <returns>The <see cref="P:System.Linq.Expressions.IndexExpression.Type" /> that represents the static type of the expression.</returns>
		public extern sealed override Type Type
		{
			get;
		}

		/// <summary>An object to index.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.Expression" /> representing the object to index.</returns>
		public extern Expression Object
		{
			get;
		}

		/// <summary>Gets the <see cref="T:System.Reflection.PropertyInfo" /> for the property if the expression represents an indexed property, returns null otherwise.</summary>
		/// <returns>The <see cref="T:System.Reflection.PropertyInfo" /> for the property if the expression represents an indexed property, otherwise null.</returns>
		public extern PropertyInfo Indexer
		{
			get;
		}

		/// <summary>Gets the arguments that will be used to index the property or array.</summary>
		/// <returns>The read-only collection containing the arguments that will be used to index the property or array.</returns>
		public extern ReadOnlyCollection<Expression> Arguments
		{
			get;
		}

		/// <summary>Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.</summary>
		/// <returns>This expression if no children are changed or an expression with the updated children.</returns>
		/// <param name="object">The <see cref="P:System.Linq.Expressions.IndexExpression.Object" /> property of the result.</param>
		/// <param name="arguments">The <see cref="P:System.Linq.Expressions.IndexExpression.Arguments" /> property of the result.</param>
		public extern IndexExpression Update(Expression @object, IEnumerable<Expression> arguments);
		protected extern override Expression Accept(ExpressionVisitor visitor);
	}
}
