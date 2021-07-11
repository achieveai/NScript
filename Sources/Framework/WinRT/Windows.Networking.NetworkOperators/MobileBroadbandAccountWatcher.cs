using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.Networking.NetworkOperators
{
	[Activatable(100794368u), DualApiPartition(version = 100794368u), Version(100794368u)]
	public sealed class MobileBroadbandAccountWatcher : IMobileBroadbandAccountWatcher
	{
		public extern event TypedEventHandler<MobileBroadbandAccountWatcher, MobileBroadbandAccountEventArgs> AccountAdded;
		public extern event TypedEventHandler<MobileBroadbandAccountWatcher, MobileBroadbandAccountEventArgs> AccountRemoved;
		public extern event TypedEventHandler<MobileBroadbandAccountWatcher, MobileBroadbandAccountUpdatedEventArgs> AccountUpdated;
		public extern event TypedEventHandler<MobileBroadbandAccountWatcher, object> EnumerationCompleted;
        public extern event TypedEventHandler<MobileBroadbandAccountWatcher, object> Stopped;
		public extern MobileBroadbandAccountWatcherStatus Status
		{
			get;
		}
		public extern MobileBroadbandAccountWatcher();
		public extern void Start();
		public extern void Stop();
	}
}
