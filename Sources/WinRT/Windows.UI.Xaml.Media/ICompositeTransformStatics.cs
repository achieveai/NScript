using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[ExclusiveTo(typeof(CompositeTransform)), Guid(790170632u, 33382, 18799, 150, 83, 161, 139, 212, 248, 54, 170), Version(100794368u), WebHostHidden]
	internal interface ICompositeTransformStatics
	{
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty CenterXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty CenterYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty RotationProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty ScaleXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty ScaleYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty SkewXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty SkewYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty TranslateXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty TranslateYProperty
		{
			get;
		}
	}
}
