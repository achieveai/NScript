//-----------------------------------------------------------------------
// <copyright file="SymbolDocumentInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	/// <summary>Stores information necessary to emit debugging symbol information for a source file, in particular the file name and unique language identifier.</summary>
	public class SymbolDocumentInfo
	{
		private readonly string _fileName;
		/// <summary>The source file name.</summary>
		/// <returns>The string representing the source file name.</returns>
		public extern string FileName
		{
			get;
		}
	}
}
