using System;
using Windows.Foundation.Metadata;
namespace Windows.UI.Xaml
{
	[MarshalingBehavior(MarshalingType.Agile), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden]
	public sealed class WindowCreatedEventArgs : IWindowCreatedEventArgs
	{
		public extern Window Window
		{
			get;
		}
	}
}
