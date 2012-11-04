using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
namespace Windows.Globalization.NumberFormatting
{
	[Guid(3567829488u, 32976, 19213, 168, 158, 136, 44, 30, 143, 131, 16), Version(100794368u)]
	public interface INumberFormatter2
	{
		string FormatInt([In] long value);
		string FormatUInt([In] ulong value);
		string FormatDouble([In] double value);
	}
}
