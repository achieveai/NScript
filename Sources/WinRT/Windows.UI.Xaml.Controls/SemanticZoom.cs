using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Markup;
namespace Windows.UI.Xaml.Controls
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(ISemanticZoomStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden, ContentProperty(Name = "ZoomedInView")]
	public sealed class SemanticZoom : Control, ISemanticZoom
	{
		public extern event SemanticZoomViewChangedEventHandler ViewChangeCompleted;
        public extern event SemanticZoomViewChangedEventHandler ViewChangeStarted;
		public extern bool CanChangeViews
		{
			get;
			set;
		}
		public extern bool IsZoomOutButtonEnabled
		{
			get;
			set;
		}
		public extern bool IsZoomedInViewActive
		{
			get;
			set;
		}
		public extern ISemanticZoomInformation ZoomedInView
		{
			get;
			set;
		}
		public extern ISemanticZoomInformation ZoomedOutView
		{
			get;
			set;
		}
		public static extern DependencyProperty CanChangeViewsProperty
		{
			get;
		}
		public static extern DependencyProperty IsZoomOutButtonEnabledProperty
		{
			get;
		}
		public static extern DependencyProperty IsZoomedInViewActiveProperty
		{
			get;
		}
		public static extern DependencyProperty ZoomedInViewProperty
		{
			get;
		}
		public static extern DependencyProperty ZoomedOutViewProperty
		{
			get;
		}
		public extern SemanticZoom();
		public extern void ToggleActiveView();
	}
}
