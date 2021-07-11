using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(ICertificateEnrollmentManagerStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public static class CertificateEnrollmentManager
	{
		public static extern IAsyncOperation<string> CreateRequestAsync([In] CertificateRequestProperties request);
		public static extern IAsyncAction InstallCertificateAsync([In] string certificate, [In] InstallOptions installOption);
		public static extern IAsyncAction ImportPfxDataAsync([In] string pfxData, [In] string password, [In] ExportOption exportable, [In] KeyProtectionLevel keyProtectionLevel, [In] InstallOptions installOption, [In] string friendlyName);
	}
}
