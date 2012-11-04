using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.System.Profile
{
	[ExclusiveTo(typeof(HardwareIdentification)), Guid(2534564064u, 61808, 19010, 189, 85, 169, 0, 178, 18, 218, 226), Version(100794368u)]
	internal interface IHardwareIdentificationStatics
	{
		HardwareToken GetPackageSpecificToken([In] IBuffer nonce);
	}
}
