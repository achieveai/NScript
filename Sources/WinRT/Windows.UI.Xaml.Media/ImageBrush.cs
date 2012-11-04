using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Xaml.Media
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IImageBrushStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class ImageBrush : TileBrush, IImageBrush
	{
		public extern event ExceptionRoutedEventHandler ImageFailed;
		public extern event RoutedEventHandler ImageOpened;
		public extern ImageSource ImageSource
		{
			get;
			set;
		}
		public static extern DependencyProperty ImageSourceProperty
		{
			get;
		}
		public extern ImageBrush();
	}
}
