using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[ExclusiveTo(typeof(WebUINavigatedOperation)), Guid(2056675080u, 33154, 19081, 171, 103, 132, 146, 232, 117, 13, 75), Version(100794368u)]
	internal interface IWebUINavigatedOperation
	{
		WebUINavigatedDeferral GetDeferral();
	}
}
