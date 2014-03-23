//-----------------------------------------------------------------------
// <copyright file="MethodBody.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

	/// <summary>Provides access to the metadata and MSIL for the body of a method.</summary>
	[ComVisible(true)]
	public class MethodBody
	{
		/// <summary>Gets a metadata token for the signature that describes the local variables for the method in metadata.</summary>
		/// <returns>An integer that represents the metadata token.</returns>
		public extern virtual int LocalSignatureMetadataToken
		{
			get;
		}

		/// <summary>Gets the list of local variables declared in the method body.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.LocalVariableInfo" /> objects that describe the local variables declared in the method body.</returns>
		public extern virtual IList<LocalVariableInfo> LocalVariables
		{
			get;
		}

		/// <summary>Gets the maximum number of items on the operand stack when the method is executing.</summary>
		/// <returns>The maximum number of items on the operand stack when the method is executing.</returns>
		public extern virtual int MaxStackSize
		{
			get;
		}

		/// <summary>Gets a value indicating whether local variables in the method body are initialized to the default values for their types.</summary>
		/// <returns>true if the method body contains code to initialize local variables to null for reference types, or to the zero-initialized value for value types; otherwise, false.</returns>
		public extern virtual bool InitLocals
		{
			get;
		}

		/// <summary>Gets a list that includes all the exception-handling clauses in the method body.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Reflection.ExceptionHandlingClause" /> objects representing the exception-handling clauses in the body of the method.</returns>
		public extern virtual IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.MethodBody" /> class.</summary>
		protected MethodBody()
		{
		}

		/// <summary>Returns the MSIL for the method body, as an array of bytes.</summary>
		/// <returns>An array of type <see cref="T:System.Byte" /> that contains the MSIL for the method body. </returns>
		public extern virtual byte[] GetILAsByteArray();
	}
}
