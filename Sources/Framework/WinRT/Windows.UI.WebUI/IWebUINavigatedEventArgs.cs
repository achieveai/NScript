using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[Guid(2807579064u, 9369, 16432, 166, 157, 21, 210, 217, 207, 229, 36), Version(100794368u)]
	public interface IWebUINavigatedEventArgs
	{
		WebUINavigatedOperation NavigatedOperation
		{
			get;
		}
	}
}
