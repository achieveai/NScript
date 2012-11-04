using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.RuntimeClass), Version(100794368u)]
	public sealed class MuseAttribute : Attribute
	{
		public uint Version;
		public extern MuseAttribute();
	}
}
