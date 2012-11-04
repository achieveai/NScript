using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[MarshalingBehavior(MarshalingType.Standard), Version(100794368u)]
	public sealed class WebUINavigatedEventArgs : IWebUINavigatedEventArgs
	{
		public extern WebUINavigatedOperation NavigatedOperation
		{
			get;
		}
	}
}
