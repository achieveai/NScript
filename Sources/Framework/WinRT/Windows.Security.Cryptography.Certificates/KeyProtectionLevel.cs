using System;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[Version(100794368u)]
	public enum KeyProtectionLevel
	{
		NoConsent,
		ConsentOnly,
		ConsentWithPassword
	}
}
