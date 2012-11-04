using System;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
namespace Windows.UI.ApplicationSettings
{
	[MarshalingBehavior(MarshalingType.None), Muse(Version = 100794368u), Version(100794368u)]
	public sealed class SettingsPaneCommandsRequest : ISettingsPaneCommandsRequest
	{
		public extern IVector<SettingsCommand> ApplicationCommands
		{
			get;
		}
	}
}
