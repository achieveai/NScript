using System;
namespace Windows.Foundation.Metadata
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue), Version(100794368u)]
	public sealed class VariantAttribute : Attribute
	{
		public extern VariantAttribute();
	}
}
