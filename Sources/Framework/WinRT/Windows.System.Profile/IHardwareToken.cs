using System;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.System.Profile
{
	[ExclusiveTo(typeof(HardwareToken)), Guid(687264960u, 64274, 16548, 129, 103, 127, 78, 3, 210, 114, 76), Version(100794368u)]
	internal interface IHardwareToken
	{
		IBuffer Certificate
		{
			get;
		}
		IBuffer Id
		{
			get;
		}
		IBuffer Signature
		{
			get;
		}
	}
}
