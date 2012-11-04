using System;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IKeyAlgorithmNamesStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public static class KeyAlgorithmNames
	{
		public static extern string Dsa
		{
			get;
		}
		public static extern string Ecdh256
		{
			get;
		}
		public static extern string Ecdh384
		{
			get;
		}
		public static extern string Ecdh521
		{
			get;
		}
		public static extern string Ecdsa256
		{
			get;
		}
		public static extern string Ecdsa384
		{
			get;
		}
		public static extern string Ecdsa521
		{
			get;
		}
		public static extern string Rsa
		{
			get;
		}
	}
}
