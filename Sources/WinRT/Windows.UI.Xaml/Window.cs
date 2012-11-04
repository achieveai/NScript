using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
namespace Windows.UI.Xaml
{
	[MarshalingBehavior(MarshalingType.Agile), Static(typeof(IWindowStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class Window : IWindow
	{
		public extern event WindowActivatedEventHandler Activated;
		public extern event WindowClosedEventHandler Closed;
		public extern event WindowSizeChangedEventHandler SizeChanged;
		public extern event WindowVisibilityChangedEventHandler VisibilityChanged;
		public extern Rect Bounds
		{
			get;
		}
		public extern UIElement Content
		{
			get;
			set;
		}
		public extern CoreWindow CoreWindow
		{
			get;
		}
		public extern CoreDispatcher Dispatcher
		{
			get;
		}
		public extern bool Visible
		{
			get;
		}
		public static extern Window Current
		{
			get;
		}
		public extern void Activate();
		public extern void Close();
	}
}
