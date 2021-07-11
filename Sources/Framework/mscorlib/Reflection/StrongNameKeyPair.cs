//-----------------------------------------------------------------------
// <copyright file="StrongNameKeyPair.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;

	/// <summary>Encapsulates access to a public or private key pair used to sign strong name assemblies.</summary>
	[ComVisible(true)]
	public class StrongNameKeyPair
	{
		/// <summary>Gets the public part of the public key or public key token of the key pair.</summary>
		/// <returns>An array of type byte containing the public key or public key token of the key pair.</returns>
		public extern byte[] PublicKey
		{
			get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.StrongNameKeyPair" /> class, building the key pair from a byte array.</summary>
		/// <param name="keyPairArray">An array of type byte containing the key pair. </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyPairArray" /> is null. </exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public extern StrongNameKeyPair(byte[] keyPairArray);

		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.StrongNameKeyPair" /> class, building the key pair from a String.</summary>
		/// <param name="keyPairContainer">A string containing the key pair. </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyPairContainer" /> is null. </exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public extern StrongNameKeyPair(string keyPairContainer);

		private extern unsafe byte[] ComputePublicKey();

		private extern bool GetKeyPair(out object arrayOrContainer);
	}
}
