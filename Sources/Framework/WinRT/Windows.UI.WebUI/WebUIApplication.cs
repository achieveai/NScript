using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.WebUI
{
	[MarshalingBehavior(MarshalingType.Standard), Static(typeof(IWebUIActivationStatics), 100794368u), Version(100794368u)]
	public static class WebUIApplication
	{
		public static extern event ActivatedEventHandler Activated;
		public static extern event NavigatedEventHandler Navigated;
		public static extern event ResumingEventHandler Resuming;
		public static extern event SuspendingEventHandler Suspending;
	}
}
