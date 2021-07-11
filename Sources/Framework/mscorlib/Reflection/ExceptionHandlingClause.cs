//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlingClause.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Identifies kinds of exception-handling clauses.</summary>
	[Flags, ComVisible(true)]
	public enum ExceptionHandlingClauseOptions
	{
		/// <summary>The clause accepts all exceptions that derive from a specified type.</summary>
		Clause = 0,
		/// <summary>The clause contains user-specified instructions that determine whether the exception should be ignored (that is, whether normal execution should resume), be handled by the associated handler, or be passed on to the next clause.</summary>
		Filter = 1,
		/// <summary>The clause is executed whenever the try block exits, whether through normal control flow or because of an unhandled exception.</summary>
		Finally = 2,
		/// <summary>The clause is executed if an exception occurs, but not on completion of normal control flow.</summary>
		Fault = 4
	}

	/// <summary>Represents a clause in a structured exception-handling block.</summary>
	[ComVisible(true)]
	public class ExceptionHandlingClause
	{
		/// <summary>Gets a value indicating whether this exception-handling clause is a finally clause, a type-filtered clause, or a user-filtered clause.</summary>
		/// <returns>An <see cref="T:System.Reflection.ExceptionHandlingClauseOptions" /> value that indicates what kind of action this clause performs.</returns>
		public extern virtual ExceptionHandlingClauseOptions Flags
		{
			get;
		}

		/// <summary>The offset within the method, in bytes, of the try block that includes this exception-handling clause.</summary>
		/// <returns>An integer that represents the offset within the method, in bytes, of the try block that includes this exception-handling clause.</returns>
		public extern virtual int TryOffset
		{
			get;
		}

		/// <summary>The total length, in bytes, of the try block that includes this exception-handling clause.</summary>
		/// <returns>The total length, in bytes, of the try block that includes this exception-handling clause.</returns>
		public extern virtual int TryLength
		{
			get;
		}

		/// <summary>Gets the offset within the method body, in bytes, of this exception-handling clause.</summary>
		/// <returns>An integer that represents the offset within the method body, in bytes, of this exception-handling clause.</returns>
		public extern virtual int HandlerOffset
		{
			get;
		}

		/// <summary>Gets the length, in bytes, of the body of this exception-handling clause.</summary>
		/// <returns>An integer that represents the length, in bytes, of the MSIL that forms the body of this exception-handling clause.</returns>
		public extern virtual int HandlerLength
		{
			get;
		}

		/// <summary>Gets the offset within the method body, in bytes, of the user-supplied filter code.</summary>
		/// <returns>The offset within the method body, in bytes, of the user-supplied filter code. The value of this property has no meaning if the <see cref="P:System.Reflection.ExceptionHandlingClause.Flags" /> property has any value other than <see cref="F:System.Reflection.ExceptionHandlingClauseOptions.Filter" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">Cannot get the offset because the exception handling clause is not a filter.</exception>
		public extern virtual int FilterOffset
		{
			get;
		}

		/// <summary>Gets the type of exception handled by this clause.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents that type of exception handled by this clause, or null if the <see cref="P:System.Reflection.ExceptionHandlingClause.Flags" /> property is <see cref="F:System.Reflection.ExceptionHandlingClauseOptions.Filter" /> or <see cref="F:System.Reflection.ExceptionHandlingClauseOptions.Finally" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">Invalid use of property for the object's current state.</exception>
		public extern virtual Type CatchType
		{
			get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.ExceptionHandlingClause" /> class.</summary>
		protected ExceptionHandlingClause()
		{
		}
	}
}
