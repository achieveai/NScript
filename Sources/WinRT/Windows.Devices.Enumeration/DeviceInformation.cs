using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
namespace Windows.Devices.Enumeration
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IDeviceInformationStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public sealed class DeviceInformation : IDeviceInformation
	{
		public extern EnclosureLocation EnclosureLocation
		{
			get;
		}
		public extern string Id
		{
			get;
		}
		public extern bool IsDefault
		{
			get;
		}
		public extern bool IsEnabled
		{
			get;
		}
		public extern string Name
		{
			get;
		}
		public extern IMapView<string, object> Properties
		{
			[return: HasVariant]
			get;
		}
		public extern void Update([In] DeviceInformationUpdate updateInfo);
		public extern IAsyncOperation<DeviceThumbnail> GetThumbnailAsync();
		public extern IAsyncOperation<DeviceThumbnail> GetGlyphThumbnailAsync();
		[Overload("CreateFromIdAsync")]
		public static extern IAsyncOperation<DeviceInformation> CreateFromIdAsync([In] string id);
		[Overload("CreateFromIdAsyncAdditionalProperties")]
		public static extern IAsyncOperation<DeviceInformation> CreateFromIdAsync([In] string id, [In] IIterable<string> additionalProperties);
		[Overload("FindAllAsync")]
		public static extern IAsyncOperation<DeviceInformationCollection> FindAllAsync();
		[DefaultOverload, Overload("FindAllAsyncDeviceClass")]
		public static extern IAsyncOperation<DeviceInformationCollection> FindAllAsync([In] DeviceClass deviceClass);
		[Overload("FindAllAsyncAqsFilter")]
		public static extern IAsyncOperation<DeviceInformationCollection> FindAllAsync([In] string aqsFilter);
		[Overload("FindAllAsyncAqsFilterAndAdditionalProperties")]
		public static extern IAsyncOperation<DeviceInformationCollection> FindAllAsync([In] string aqsFilter, [In] IIterable<string> additionalProperties);
		[Overload("CreateWatcher")]
		public static extern DeviceWatcher CreateWatcher();
		[DefaultOverload, Overload("CreateWatcherDeviceClass")]
		public static extern DeviceWatcher CreateWatcher([In] DeviceClass deviceClass);
		[Overload("CreateWatcherAqsFilter")]
		public static extern DeviceWatcher CreateWatcher([In] string aqsFilter);
		[Overload("CreateWatcherAqsFilterAndAdditionalProperties")]
		public static extern DeviceWatcher CreateWatcher([In] string aqsFilter, [In] IIterable<string> additionalProperties);
	}
}
