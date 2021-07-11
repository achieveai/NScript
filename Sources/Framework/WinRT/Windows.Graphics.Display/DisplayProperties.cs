using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.Graphics.Display
{
	[MarshalingBehavior(MarshalingType.Agile), Static(typeof(IDisplayPropertiesStatics), 100794368u), Version(100794368u)]
	public static class DisplayProperties
	{
		public static extern event DisplayPropertiesEventHandler ColorProfileChanged;
		public static extern event DisplayPropertiesEventHandler DisplayContentsInvalidated;
		public static extern event DisplayPropertiesEventHandler LogicalDpiChanged;
		public static extern event DisplayPropertiesEventHandler OrientationChanged;
		public static extern event DisplayPropertiesEventHandler StereoEnabledChanged;
		public static extern DisplayOrientations AutoRotationPreferences
		{
			get;
			set;
		}
		public static extern DisplayOrientations CurrentOrientation
		{
			get;
		}
		public static extern float LogicalDpi
		{
			get;
		}
		public static extern DisplayOrientations NativeOrientation
		{
			get;
		}
		public static extern ResolutionScale ResolutionScale
		{
			get;
		}
		public static extern bool StereoEnabled
		{
			get;
		}
		public static extern IAsyncOperation<IRandomAccessStream> GetColorProfileAsync();
	}
}
