using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
namespace Windows.UI.Xaml.Media
{
	[ExclusiveTo(typeof(PlaneProjection)), Guid(2912001127u, 15324, 18517, 137, 105, 209, 249, 163, 173, 194, 125), Version(100794368u), WebHostHidden]
	internal interface IPlaneProjectionStatics
	{
		DependencyProperty CenterOfRotationXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty CenterOfRotationYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty CenterOfRotationZProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty GlobalOffsetXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty GlobalOffsetYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty GlobalOffsetZProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty LocalOffsetXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty LocalOffsetYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty LocalOffsetZProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty ProjectionMatrixProperty
		{
			get;
		}
		DependencyProperty RotationXProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty RotationYProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
		DependencyProperty RotationZProperty
		{
			[ConditionallyIndependentlyAnimatable]
			get;
		}
	}
}
