using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
namespace Windows.Networking.Proximity
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IProximityDeviceStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u)]
	public sealed class ProximityDevice : IProximityDevice
	{
		public extern event DeviceArrivedEventHandler DeviceArrived;
		public extern event DeviceDepartedEventHandler DeviceDeparted;
		public extern ulong BitsPerSecond
		{
			get;
		}
		public extern string DeviceId
		{
			get;
		}
		public extern uint MaxMessageBytes
		{
			get;
		}
		public extern long SubscribeForMessage([In] string messageType, [In] MessageReceivedHandler messageReceivedHandler);
		[Overload("PublishMessage")]
		public extern long PublishMessage([In] string messageType, [In] string message);
		[Overload("PublishMessageWithCallback")]
		public extern long PublishMessage([In] string messageType, [In] string message, [In] MessageTransmittedHandler messageTransmittedHandler);
		[Overload("PublishBinaryMessage")]
		public extern long PublishBinaryMessage([In] string messageType, [In] IBuffer message);
		[Overload("PublishBinaryMessageWithCallback")]
		public extern long PublishBinaryMessage([In] string messageType, [In] IBuffer message, [In] MessageTransmittedHandler messageTransmittedHandler);
		[Overload("PublishUriMessage")]
		public extern long PublishUriMessage([In] Uri message);
		[Overload("PublishUriMessageWithCallback")]
		public extern long PublishUriMessage([In] Uri message, [In] MessageTransmittedHandler messageTransmittedHandler);
		public extern void StopSubscribingForMessage([In] long subscriptionId);
		public extern void StopPublishingMessage([In] long messageId);
		public static extern string GetDeviceSelector();
		public static extern ProximityDevice GetDefault();
		public static extern ProximityDevice FromId([In] string deviceInterfaceId);
	}
}
