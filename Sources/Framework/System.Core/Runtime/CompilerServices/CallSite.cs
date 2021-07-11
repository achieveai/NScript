//-----------------------------------------------------------------------
// <copyright file="CallSite.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

	public abstract class CallSiteBinder
	{
		/// <summary>Gets a label that can be used to cause the binding to be updated. It indicates that the expression's binding is no longer valid. This is typically used when the "version" of a dynamic object has changed.</summary>
		/// <returns>The <see cref="T:System.Linq.Expressions.LabelTarget" /> object representing a label that can be used to trigger the binding update.</returns>
		public extern static LabelTarget UpdateLabel
		{
			get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> class.</summary>
		protected CallSiteBinder()
		{
		}
		/// <summary>Performs the runtime binding of the dynamic operation on a set of arguments.</summary>
		/// <returns>An Expression that performs tests on the dynamic operation arguments, and performs the dynamic operation if the tests are valid. If the tests fail on subsequent occurrences of the dynamic operation, Bind will be called again to produce a new <see cref="T:System.Linq.Expressions.Expression" /> for the new argument types.</returns>
		/// <param name="args">An array of arguments to the dynamic operation.</param>
		/// <param name="parameters">The array of <see cref="T:System.Linq.Expressions.ParameterExpression" /> instances that represent the parameters of the call site in the binding process.</param>
		/// <param name="returnLabel">A LabelTarget used to return the result of the dynamic binding.</param>
		public abstract Expression Bind(object[] args, ReadOnlyCollection<ParameterExpression> parameters, LabelTarget returnLabel);

		/// <summary>Provides low-level runtime binding support. Classes can override this and provide a direct delegate for the implementation of rule. This can enable saving rules to disk, having specialized rules available at runtime, or providing a different caching policy.</summary>
		/// <returns>A new delegate which replaces the CallSite Target.</returns>
		/// <param name="site">The CallSite the bind is being performed for.</param>
		/// <param name="args">The arguments for the binder.</param>
		/// <typeparam name="T">The target type of the CallSite.</typeparam>
		public extern virtual T BindDelegate<T>(CallSite<T> site, object[] args) where T : class;

		/// <summary>Adds a target to the cache of known targets. The cached targets will be scanned before calling BindDelegate to produce the new rule.</summary>
		/// <param name="target">The target delegate to be added to the cache.</param>
		/// <typeparam name="T">The type of target being added.</typeparam>
		protected extern void CacheTarget<T>(T target) where T : class;
	}

	/// <summary>A dynamic call site base class. This type is used as a parameter type to the dynamic site targets.</summary>
	public class CallSite
	{
		/// <summary>Class responsible for binding dynamic operations on the dynamic site.</summary>
		/// <returns>The <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> object responsible for binding dynamic operations.</returns>
        public extern CallSiteBinder Binder
        {
            get;
        }
		/// <summary>Creates a call site with the given delegate type and binder.</summary>
		/// <returns>The new call site.</returns>
		/// <param name="delegateType">The call site delegate type.</param>
		/// <param name="binder">The call site binder.</param>
        public extern static CallSite Create(Type delegateType, CallSiteBinder binder);
	}
	/// <summary>Dynamic site type.</summary>
	/// <typeparam name="T">The delegate type.</typeparam>
	public class CallSite<T> : CallSite where T : class
	{
		/// <summary>The Level 0 cache - a delegate specialized based on the site history.</summary>
		public T Target;
		/// <summary>The update delegate. Called when the dynamic site experiences cache miss.</summary>
		/// <returns>The update delegate.</returns>
		public extern T Update
		{
			get;
		}


		/// <summary>Creates an instance of the dynamic call site, initialized with the binder responsible for the runtime binding of the dynamic operations at this call site.</summary>
		/// <returns>The new instance of dynamic call site.</returns>
		/// <param name="binder">The binder responsible for the runtime binding of the dynamic operations at this call site.</param>
        public extern static CallSite<T> Create(CallSiteBinder binder);
	}
}
