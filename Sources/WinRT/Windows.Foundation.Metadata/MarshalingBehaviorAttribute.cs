using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.Class), Version(100794368u)]
	public sealed class MarshalingBehaviorAttribute : Attribute
	{
		public extern MarshalingBehaviorAttribute(MarshalingType behavior);
	}
}
