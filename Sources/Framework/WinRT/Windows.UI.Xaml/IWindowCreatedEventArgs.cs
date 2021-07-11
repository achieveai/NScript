using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Xaml
{
	[ExclusiveTo(typeof(WindowCreatedEventArgs)), Guid(834081904u, 65279, 18004, 175, 72, 155, 57, 138, 181, 119, 43), Version(100794368u), WebHostHidden]
	internal interface IWindowCreatedEventArgs
	{
		Window Window
		{
			get;
		}
	}
}
