using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(ICompositeTransformStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class CompositeTransform : Transform, ICompositeTransform
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
		public extern double Rotation
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
		public extern double SkewX
		{
			get;
			set;
		}
		public extern double SkewY
		{
			get;
			set;
		}
		public extern double TranslateX
		{
			get;
			set;
		}
		public extern double TranslateY
		{
			get;
			set;
		}
		public static extern DependencyProperty CenterXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty CenterYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty RotationProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty ScaleXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty ScaleYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty SkewXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty SkewYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty TranslateXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public static extern DependencyProperty TranslateYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		public extern CompositeTransform();
	}
}
