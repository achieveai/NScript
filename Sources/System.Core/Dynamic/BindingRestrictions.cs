//-----------------------------------------------------------------------
// <copyright file="BindingRestrictions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a set of binding restrictions on the <see cref="T:System.Dynamic.DynamicMetaObject" /> under which the dynamic binding is valid.</summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class BindingRestrictions
	{
		/// <summary>Represents an empty set of binding restrictions. This field is read only.</summary>
        public static readonly BindingRestrictions Empty = null;
		/// <summary>Merges the set of binding restrictions with the current binding restrictions.</summary>
		/// <returns>The new set of binding restrictions.</returns>
		/// <param name="restrictions">The set of restrictions with which to merge the current binding restrictions.</param>
		public extern BindingRestrictions Merge(BindingRestrictions restrictions);
		/// <summary>Creates the binding restriction that check the expression for runtime type identity.</summary>
		/// <returns>The new binding restrictions.</returns>
		/// <param name="expression">The expression to test.</param>
		/// <param name="type">The exact type to test.</param>
		public extern static BindingRestrictions GetTypeRestriction(Expression expression, Type type);
		/// <summary>Creates the binding restriction that checks the expression for object instance identity.</summary>
		/// <returns>The new binding restrictions.</returns>
		/// <param name="expression">The expression to test.</param>
		/// <param name="instance">The exact object instance to test.</param>
		public extern static BindingRestrictions GetInstanceRestriction(Expression expression, object instance);
		/// <summary>Creates the binding restriction that checks the expression for arbitrary immutable properties.</summary>
		/// <returns>The new binding restrictions.</returns>
		/// <param name="expression">The expression representing the restrictions.</param>
		public extern static BindingRestrictions GetExpressionRestriction(Expression expression);
		/// <summary>Combines binding restrictions from the list of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances into one set of restrictions.</summary>
		/// <returns>The new set of binding restrictions.</returns>
		/// <param name="contributingObjects">The list of <see cref="T:System.Dynamic.DynamicMetaObject" /> instances from which to combine restrictions.</param>
		public extern static BindingRestrictions Combine(IList<DynamicMetaObject> contributingObjects);
		/// <summary>Creates the <see cref="T:System.Linq.Expressions.Expression" /> representing the binding restrictions.</summary>
		/// <returns>The expression tree representing the restrictions.</returns>
		public extern Expression ToExpression();
	}
}
