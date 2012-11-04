using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.Security.Cryptography.Core
{
	[ExclusiveTo(typeof(AsymmetricKeyAlgorithmProvider)), Guid(3906142007u, 25177, 20104, 183, 224, 148, 25, 31, 222, 105, 158), Version(100794368u)]
	internal interface IAsymmetricKeyAlgorithmProvider
	{
		string AlgorithmName
		{
			get;
		}
		CryptographicKey CreateKeyPair([In] uint keySize);
		[Overload("ImportDefaultPrivateKeyBlob")]
		CryptographicKey ImportKeyPair([In] IBuffer keyBlob);
		[Overload("ImportKeyPairWithBlobType")]
		CryptographicKey ImportKeyPair([In] IBuffer keyBlob, [In] CryptographicPrivateKeyBlobType BlobType);
		[Overload("ImportDefaultPublicKeyBlob")]
		CryptographicKey ImportPublicKey([In] IBuffer keyBlob);
		[Overload("ImportPublicKeyWithBlobType")]
		CryptographicKey ImportPublicKey([In] IBuffer keyBlob, [In] CryptographicPublicKeyBlobType BlobType);
	}
}
