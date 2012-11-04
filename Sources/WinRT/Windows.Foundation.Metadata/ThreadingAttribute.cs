using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.Class), Version(100794368u)]
	public sealed class ThreadingAttribute : Attribute
	{
		public extern ThreadingAttribute(ThreadingModel model);
	}
}
