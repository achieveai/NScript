using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(ISkewTransformStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class SkewTransform : Transform, ISkewTransform
	{
		public extern double AngleX
		{
			get;
			set;
		}
		public extern double AngleY
		{
			get;
			set;
		}
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
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty AngleXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty AngleYProperty
		{
			get;
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
		public extern SkewTransform();
	}
}
