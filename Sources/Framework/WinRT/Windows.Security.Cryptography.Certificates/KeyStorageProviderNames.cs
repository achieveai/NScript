using System;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IKeyStorageProviderNamesStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public static class KeyStorageProviderNames
	{
		public static extern string PlatformKeyStorageProvider
		{
			get;
		}
		public static extern string SmartcardKeyStorageProvider
		{
			get;
		}
		public static extern string SoftwareKeyStorageProvider
		{
			get;
		}
	}
}
