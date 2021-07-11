using System;
namespace Windows.Foundation.Metadata
{
	[Version(100794368u)]
	public sealed class VersionAttribute : Attribute
	{
		public extern VersionAttribute(uint version);
	}
}
