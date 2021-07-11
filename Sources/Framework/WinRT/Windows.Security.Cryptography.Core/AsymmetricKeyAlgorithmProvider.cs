using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.Security.Cryptography.Core
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IAsymmetricKeyAlgorithmProviderStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public sealed class AsymmetricKeyAlgorithmProvider : IAsymmetricKeyAlgorithmProvider
	{
		public extern string AlgorithmName
		{
			get;
		}
		public extern CryptographicKey CreateKeyPair([In] uint keySize);
		[Overload("ImportDefaultPrivateKeyBlob")]
		public extern CryptographicKey ImportKeyPair([In] IBuffer keyBlob);
		[Overload("ImportKeyPairWithBlobType")]
		public extern CryptographicKey ImportKeyPair([In] IBuffer keyBlob, [In] CryptographicPrivateKeyBlobType BlobType);
		[Overload("ImportDefaultPublicKeyBlob")]
		public extern CryptographicKey ImportPublicKey([In] IBuffer keyBlob);
		[Overload("ImportPublicKeyWithBlobType")]
		public extern CryptographicKey ImportPublicKey([In] IBuffer keyBlob, [In] CryptographicPublicKeyBlobType BlobType);
		public static extern AsymmetricKeyAlgorithmProvider OpenAlgorithm([In] string algorithm);
	}
}
