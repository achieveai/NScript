using System;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.Media
{
	[MarshalingBehavior(MarshalingType.Agile), Static(typeof(IMediaControl), 100794368u), Threading(ThreadingModel.MTA), Version(100794368u)]
	public static class MediaControl
	{
		public static extern event EventHandler<object> ChannelDownPressed;
		public static extern event EventHandler<object> ChannelUpPressed;
		public static extern event EventHandler<object> FastForwardPressed;
		public static extern event EventHandler<object> NextTrackPressed;
		public static extern event EventHandler<object> PausePressed;
		public static extern event EventHandler<object> PlayPauseTogglePressed;
        public static extern event EventHandler<object> PlayPressed;
		public static extern event EventHandler<object> PreviousTrackPressed;
		public static extern event EventHandler<object> RecordPressed;
		public static extern event EventHandler<object> RewindPressed;
		public static extern event EventHandler<object> SoundLevelChanged;
		public static extern event EventHandler<object> StopPressed;
		public static extern Uri AlbumArt
		{
			get;
			set;
		}
		public static extern string ArtistName
		{
			get;
			set;
		}
		public static extern bool IsPlaying
		{
			get;
			set;
		}
		public static extern SoundLevel SoundLevel
		{
			get;
		}
		public static extern string TrackName
		{
			get;
			set;
		}
	}
}
