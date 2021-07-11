using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.System.Profile
{
	[MarshalingBehavior(MarshalingType.Standard), Static(typeof(IHardwareIdentificationStatics), 100794368u), Version(100794368u)]
	public static class HardwareIdentification
	{
		public static extern HardwareToken GetPackageSpecificToken([In] IBuffer nonce);
	}
}
