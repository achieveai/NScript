using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Xaml.Data
{
	[AttributeUsage(AttributeTargets.Class), Version(100794368u), WebHostHidden]
	public sealed class BindableAttribute : Attribute
	{
		public extern BindableAttribute();
	}
}
