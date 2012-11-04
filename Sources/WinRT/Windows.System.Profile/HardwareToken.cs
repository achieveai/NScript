using System;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.System.Profile
{
	[MarshalingBehavior(MarshalingType.Agile), Version(100794368u)]
	public sealed class HardwareToken : IHardwareToken
	{
		public extern IBuffer Certificate
		{
			get;
		}
		public extern IBuffer Id
		{
			get;
		}
		public extern IBuffer Signature
		{
			get;
		}
	}
}
