using System;
using Windows.Foundation.Metadata;
namespace Windows.Security.Cryptography.Certificates
{
	[Flags, Version(100794368u)]
	public enum InstallOptions : uint
	{
		None = 0u,
		DeleteExpired = 1u
	}
}
