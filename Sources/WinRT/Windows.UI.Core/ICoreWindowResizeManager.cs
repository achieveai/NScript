using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Core
{
	[ExclusiveTo(typeof(CoreWindowResizeManager)), Guid(3102783781u, 45904, 18611, 161, 152, 92, 26, 132, 112, 2, 67), Version(100794368u), WebHostHidden]
	internal interface ICoreWindowResizeManager
	{
		void NotifyLayoutCompleted();
	}
}
