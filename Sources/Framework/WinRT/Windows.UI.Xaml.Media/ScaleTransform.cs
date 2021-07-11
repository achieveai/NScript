using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IScaleTransformStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class ScaleTransform : Transform, IScaleTransform
	{
		public extern double CenterX
		{
			get;
			set;
		}
		public extern double CenterY
		{
			get;
			set;
		}
		public extern double ScaleX
		{
			get;
			set;
		}
		public extern double ScaleY
		{
			get;
			set;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty CenterXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty CenterYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty ScaleXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty ScaleYProperty
		{
			get;
		}
		public extern ScaleTransform();
	}
}
