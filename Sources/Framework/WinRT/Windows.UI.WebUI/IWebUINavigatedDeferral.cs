using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[ExclusiveTo(typeof(WebUINavigatedDeferral)), Guid(3624149069u, 33567, 18146, 180, 50, 58, 252, 226, 17, 249, 98), Version(100794368u)]
	internal interface IWebUINavigatedDeferral
	{
		void Complete();
	}
}
