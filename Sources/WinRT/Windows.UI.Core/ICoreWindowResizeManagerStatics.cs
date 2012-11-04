using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Core
{
	[ExclusiveTo(typeof(CoreWindowResizeManager)), Guid(2924122181u, 28016, 18907, 142, 104, 70, 255, 189, 23, 211, 141), Version(100794368u), WebHostHidden]
	internal interface ICoreWindowResizeManagerStatics
	{
		CoreWindowResizeManager GetForCurrentView();
	}
}
