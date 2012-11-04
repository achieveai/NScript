using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.Class), Version(100794368u)]
	public sealed class GCPressureAttribute : Attribute
	{
		public GCPressureAmount amount;
		public extern GCPressureAttribute();
	}
}
