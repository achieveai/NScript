using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Core
{
	[MarshalingBehavior(MarshalingType.Agile), Static(typeof(ICoreWindowResizeManagerStatics), 100794368u), Version(100794368u), WebHostHidden]
	public sealed class CoreWindowResizeManager : ICoreWindowResizeManager
	{
		public extern void NotifyLayoutCompleted();
		public static extern CoreWindowResizeManager GetForCurrentView();
	}
}
