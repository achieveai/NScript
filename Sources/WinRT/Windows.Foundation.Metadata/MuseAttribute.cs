using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.Class), Version(100794368u)]
	public sealed class MuseAttribute : Attribute
	{
		public uint Version;
		public extern MuseAttribute();
	}
}
