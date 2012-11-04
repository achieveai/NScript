using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[ExclusiveTo(typeof(CompositeTransform)), Guid(790170632u, 33382, 18799, 150, 83, 161, 139, 212, 248, 54, 170), Version(100794368u), WebHostHidden]
	internal interface ICompositeTransformStatics
	{
		DependencyProperty CenterXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty CenterYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty RotationProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty ScaleXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty ScaleYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty SkewXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty SkewYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty TranslateXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty TranslateYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
	}
}
