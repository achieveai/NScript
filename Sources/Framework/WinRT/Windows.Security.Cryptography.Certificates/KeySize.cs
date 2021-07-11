using System;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[Version(100794368u)]
	public enum KeySize
	{
		Invalid,
		Rsa2048 = 2048,
		Rsa4096 = 4096
	}
}
