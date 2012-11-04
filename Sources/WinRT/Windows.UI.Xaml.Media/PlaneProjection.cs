using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Media3D;
namespace Windows.UI.Xaml.Media
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IPlaneProjectionStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class PlaneProjection : Projection, IPlaneProjection
	{
		public extern double CenterOfRotationX
		{
			get;
			set;
		}
		public extern double CenterOfRotationY
		{
			get;
			set;
		}
		public extern double CenterOfRotationZ
		{
			get;
			set;
		}
		public extern double GlobalOffsetX
		{
			get;
			set;
		}
		public extern double GlobalOffsetY
		{
			get;
			set;
		}
		public extern double GlobalOffsetZ
		{
			get;
			set;
		}
		public extern double LocalOffsetX
		{
			get;
			set;
		}
		public extern double LocalOffsetY
		{
			get;
			set;
		}
		public extern double LocalOffsetZ
		{
			get;
			set;
		}
		public extern Matrix3D ProjectionMatrix
		{
			get;
		}
		public extern double RotationX
		{
			get;
			set;
		}
		public extern double RotationY
		{
			get;
			set;
		}
		public extern double RotationZ
		{
			get;
			set;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty CenterOfRotationXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty CenterOfRotationYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty CenterOfRotationZProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty GlobalOffsetXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty GlobalOffsetYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty GlobalOffsetZProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty LocalOffsetXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty LocalOffsetYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty LocalOffsetZProperty
		{
			get;
		}
		public static extern DependencyProperty ProjectionMatrixProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty RotationXProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty RotationYProperty
		{
			get;
		}
        [ConditionallyIndependentlyAnimatable]
		public static extern DependencyProperty RotationZProperty
		{
			get;
		}
		public extern PlaneProjection();
	}
}
