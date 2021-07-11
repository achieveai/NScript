using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.UI.Input
{
	[MarshalingBehavior(MarshalingType.Standard), Muse(Version = 100794368u), Static(typeof(IEdgeGestureStatics), 100794368u), Threading(ThreadingModel.STA), Version(100794368u)]
	public sealed class EdgeGesture : IEdgeGesture
	{
		public extern event TypedEventHandler<EdgeGesture, EdgeGestureEventArgs> Canceled;
		public extern event TypedEventHandler<EdgeGesture, EdgeGestureEventArgs> Completed;
        public extern event TypedEventHandler<EdgeGesture, EdgeGestureEventArgs> Starting;
		public static extern EdgeGesture GetForCurrentView();
	}
}
