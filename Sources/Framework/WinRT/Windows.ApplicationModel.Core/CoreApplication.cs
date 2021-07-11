using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
namespace Windows.ApplicationModel.Core
{
	[MarshalingBehavior(MarshalingType.Agile), Muse(Version = 100794368u), Version(100794368u), WebHostHidden]
	public static class CoreApplication
	{
		public static extern event EventHandler<object> Exiting;
		public static extern event EventHandler<object> Resuming;
		public static extern event EventHandler<SuspendingEventArgs> Suspending;
		public static extern CoreApplicationView MainView
		{
			get;
		}
		public static extern IVectorView<CoreApplicationView> Views
		{
			get;
		}
		public static extern string Id
		{
			get;
		}
		public static extern IPropertySet Properties
		{
			get;
		}
		public static extern void IncrementApplicationUseCount();
		public static extern void DecrementApplicationUseCount();
		public static extern CoreApplicationView CreateNewView([In] string runtimeType, [In] string entryPoint);
		public static extern void Exit();
		public static extern CoreApplicationView GetCurrentView();
		public static extern void Run([In] IFrameworkViewSource viewSource);
		public static extern void RunWithActivationFactories([In] IGetActivationFactory activationFactoryCallback);
	}
}
