using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.Devices.Enumeration.Pnp
{
	[DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Version(100794368u)]
	public sealed class PnpObjectWatcher : IPnpObjectWatcher
	{
		public extern event TypedEventHandler<PnpObjectWatcher, PnpObject> Added;
		public extern event TypedEventHandler<PnpObjectWatcher, object> EnumerationCompleted;
		public extern event TypedEventHandler<PnpObjectWatcher, PnpObjectUpdate> Removed;
		public extern event TypedEventHandler<PnpObjectWatcher, object> Stopped;
		public extern event TypedEventHandler<PnpObjectWatcher, PnpObjectUpdate> Updated;
		public extern DeviceWatcherStatus Status
		{
			get;
		}
		public extern void Start();
		public extern void Stop();
	}
}
