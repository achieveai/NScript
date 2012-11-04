using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Xaml.Documents
{
	[ExclusiveTo(typeof(Paragraph)), Guid(4010313882u, 21339, 20044, 141, 132, 40, 59, 51, 233, 138, 55), Version(100794368u), WebHostHidden]
	internal interface IParagraphStatics
	{
		DependencyProperty TextIndentProperty
		{
			get;
		}
	}
}
