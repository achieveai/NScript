using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
namespace Windows.Foundation
{
	[ExclusiveTo(typeof(Uri)), Guid(3251909306u, 51236, 17490, 167, 253, 81, 43, 195, 187, 233, 161), Version(100794368u)]
	internal interface IUriEscapeStatics
	{
		string UnescapeComponent([In] string toUnescape);
		string EscapeComponent([In] string toEscape);
	}
}
