using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[Guid(2062839782u, 16586, 20041, 167, 214, 219, 219, 51, 12, 209, 163), Version(100794368u)]
	public delegate void NavigatedEventHandler([In] object sender, [In] IWebUINavigatedEventArgs e);
}
