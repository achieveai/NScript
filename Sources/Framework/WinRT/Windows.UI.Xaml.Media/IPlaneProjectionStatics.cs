using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[ExclusiveTo(typeof(PlaneProjection)), Guid(2912001127u, 15324, 18517, 137, 105, 209, 249, 163, 173, 194, 125), Version(100794368u), WebHostHidden]
	internal interface IPlaneProjectionStatics
	{
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty CenterOfRotationXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty CenterOfRotationYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty CenterOfRotationZProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty GlobalOffsetXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty GlobalOffsetYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty GlobalOffsetZProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty LocalOffsetXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty LocalOffsetYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty LocalOffsetZProperty
		{
			get;
		}
		DependencyProperty ProjectionMatrixProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty RotationXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty RotationYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		DependencyProperty RotationZProperty
		{
			get;
		}
	}
}
