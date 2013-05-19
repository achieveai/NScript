//-----------------------------------------------------------------------
// <copyright file="MediaElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Values that represent MediaReadyState.
    /// </summary>
    public enum MediaReadyState
    {
        /// <summary>
        /// Have nothing
        /// </summary>
        HaveNothing = 0,

        /// <summary>
        /// Have Metadata
        /// </summary>
        HaveMetadata = 1,

        /// <summary>
        /// Have Current Data
        /// </summary>
        HaveCurrentData = 2,

        /// <summary>
        /// Have Future Data
        /// </summary>
        HaveFutureData = 3,

        /// <summary>
        /// Have Enough Data
        /// </summary>
        HaveEnoughtData = 4
    }

    /// <summary>
    /// Values that represent MediaError.
    /// </summary>
    public enum MediaError
    {
        /// <summary>
        /// No error
        /// </summary>
        None = 0,

        /// <summary>
        /// Aborted
        /// </summary>
        Aborted = 1,

        /// <summary>
        /// Network error
        /// </summary>
        Network = 2,

        /// <summary>
        /// Decoding related error
        /// </summary>
        Decode = 3,

        /// <summary>
        /// Media not supported.
        /// </summary>
        NotSupported = 4
    }

    /// <summary>
    /// Values that represent MediaNetworkState.
    /// </summary>
    public enum MediaNetworkState
    {
        /// <summary>
        /// Nothing loaded
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Idle
        /// </summary>
        Idle = 1,

        /// <summary>
        /// Loading media
        /// </summary>
        Loading = 2,

        /// <summary>
        /// No source specified
        /// </summary>
        NoSource = 3
    }

    /// <summary>
    /// Time ranges.
    /// </summary>
    [IgnoreNamespace]
    public class TimeRanges
    {
        /// <summary>
        /// Default constructor for TimeRanges
        /// </summary>
        private extern TimeRanges();

        /// <summary>
        /// The length.
        /// </summary>
        public extern int Length
        { get; set; }

        /// <summary>
        /// Starts.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern double Start(int index);

        /// <summary>
        /// Ends.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern double End(int index);
    }

    /// <summary>
    /// Media element.
    /// </summary>
    [IgnoreNamespace]
    public abstract class MediaElement : SourceElement
    {
        /// <summary>
        /// Name of the on load start event.
        /// </summary>
        public const string OnLoadStartEvtName = "loadstart";

        /// <summary>
        /// Name of the on progress event.
        /// </summary>
        public const string OnProgressEvtName = "progress";

        /// <summary>
        /// Name of the on suspend event.
        /// </summary>
        public const string OnSuspendEvtName = "suspend";

        /// <summary>
        /// Name of the on abort event.
        /// </summary>
        public const string OnAbortEvtName = "abort";

        /// <summary>
        /// Name of the on error event.
        /// </summary>
        public const string OnErrorEvtName = "error";

        /// <summary>
        /// Name of the on stalled event.
        /// </summary>
        public const string OnStalledEvtName = "stalled";

        /// <summary>
        /// Name of the on emptied event.
        /// </summary>
        public const string OnEmptiedEvtName = "emptied";

        /// <summary>
        /// Name of the on loaded metadata event.
        /// </summary>
        public const string OnLoadedMetadataEvtName = "loadedmetadata";

        /// <summary>
        /// Name of the on loaded data event.
        /// </summary>
        public const string OnLoadedDataEvtName = "loadeddata";

        /// <summary>
        /// Name of the on can play event.
        /// </summary>
        public const string OnCanPlayEvtName = "canplay";

        /// <summary>
        /// Name of the on can play through event.
        /// </summary>
        public const string OnCanPlayThroughEvtName = "canplaythrough";

        /// <summary>
        /// Name of the on playing event.
        /// </summary>
        public const string OnPlayingEvtName = "playing";

        /// <summary>
        /// Name of the on ended event.
        /// </summary>
        public const string OnEndedEvtName = "ended";

        /// <summary>
        /// Name of the on waiting event.
        /// </summary>
        public const string OnWaitingEvtName = "waiting";

        /// <summary>
        /// Name of the on duration change event.
        /// </summary>
        public const string OnDurationChangeEvtName = "durationchange";

        /// <summary>
        /// Name of the on time upate event.
        /// </summary>
        public const string OnTimeUpateEvtName = "timeupdate";

        /// <summary>
        /// Name of the on play event.
        /// </summary>
        public const string OnPlayEvtName = "play";

        /// <summary>
        /// Name of the on pause event.
        /// </summary>
        public const string OnPauseEvtName = "pause";

        /// <summary>
        /// Name of the on rate change event.
        /// </summary>
        public const string OnRateChangeEvtName = "ratechange";

        /// <summary>
        /// Name of the on volume change event.
        /// </summary>
        public const string OnVolumeChangeEvtName = "volumechange";

        /// <summary>
        /// true to automatic play.
        /// </summary>
        public extern bool AutoPlay
        { get; set; }

        /// <summary>
        /// The buffered.
        /// </summary>
        public extern TimeRanges Buffered
        { get; }

        /// <summary>
        /// true to show, false to hide the controls.
        /// </summary>
        [ScriptName("controls")]
        public extern bool ShowControls
        { get; set; }

        /// <summary>
        /// The cross origin.
        /// </summary>
        public extern string CrossOrigin
        { get; set; }

        /// <summary>
        /// The current source.
        /// </summary>
        public extern string CurrentSrc
        { get; }

        /// <summary>
        /// The current time.
        /// </summary>
        public extern double CurrentTime
        { get; set; }

        /// <summary>
        /// true if default muted.
        /// </summary>
        public extern bool DefaultMuted
        { get; set; }

        /// <summary>
        /// The default playback rate.
        /// </summary>
        public extern double DefaultPlaybackRate
        { get; set; }

        /// <summary>
        /// The duration.
        /// </summary>
        public extern double Duration
        { get; }

        /// <summary>
        /// The ended.
        /// </summary>
        public extern bool Ended
        { get; }

        /// <summary>
        /// The error.
        /// </summary>
        public extern MediaError? Error
        { get; }

        /// <summary>
        /// Time of the initial.
        /// </summary>
        public extern double InitialTime
        { get; }

        /// <summary>
        /// true to loop.
        /// </summary>
        public extern bool Loop
        { get; set; }

        /// <summary>
        /// true if muted.
        /// </summary>
        public extern bool Muted
        { get; set; }

        /// <summary>
        /// State of the network.
        /// </summary>
        public extern MediaNetworkState? NetworkState
        { get; }

        /// <summary>
        /// true if paused.
        /// </summary>
        public extern bool Paused
        { get; set; }

        /// <summary>
        /// The playback rate.
        /// </summary>
        public extern double PlaybackRate
        { get; set; }

        /// <summary>
        /// The played.
        /// </summary>
        public extern TimeRanges Played
        { get; }

        /// <summary>
        /// The preload.
        /// </summary>
        public extern string Preload
        { get; set; }

        /// <summary>
        /// State of the ready.
        /// </summary>
        public extern MediaReadyState ReadyState
        { get; set; }

        /// <summary>
        /// The seekable.
        /// </summary>
        public extern TimeRanges Seekable
        { get; }

        /// <summary>
        /// The seeking.
        /// </summary>
        public extern bool Seeking
        { get; }

        /// <summary>
        /// The volume.
        /// </summary>
        public extern double Volume
        { get; set; }

        /// <summary>
        /// Gets the on load start handler.
        /// </summary>
        /// <value>
        /// The on load start handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnLoadStart;

        /// <summary>
        /// Gets the on progress handler.
        /// </summary>
        /// <value>
        /// The on progress handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnProgress;

        /// <summary>
        /// Gets the on suspend handler.
        /// </summary>
        /// <value>
        /// The on suspend handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnSuspend;

        /// <summary>
        /// Gets the on abort handler.
        /// </summary>
        /// <value>
        /// The on abort handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnAbort;

        /// <summary>
        /// Gets the on error handler.
        /// </summary>
        /// <value>
        /// The on error handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnError;

        /// <summary>
        /// Gets the on stalled handler.
        /// </summary>
        /// <value>
        /// The on stalled handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnStalled;

        /// <summary>
        /// Gets the on emptied handler.
        /// </summary>
        /// <value>
        /// The on emptied handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnEmptied;

        /// <summary>
        /// Gets the on loaded metadata handler.
        /// </summary>
        /// <value>
        /// The on loaded metadata handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnLoadedMetadata;

        /// <summary>
        /// Gets the information describing the on loaded handler.
        /// </summary>
        /// <value>
        /// Information describing the on loaded handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnLoadedData;

        /// <summary>
        /// Gets the on can play handler.
        /// </summary>
        /// <value>
        /// The on can play handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnCanPlay;

        /// <summary>
        /// Gets the on can play through handler.
        /// </summary>
        /// <value>
        /// The on can play through handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnCanPlayThrough;

        /// <summary>
        /// Gets the on playing handler.
        /// </summary>
        /// <value>
        /// The on playing handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnPlaying;

        /// <summary>
        /// Gets the on ended handler.
        /// </summary>
        /// <value>
        /// The on ended handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnEnded;

        /// <summary>
        /// Gets the on waiting handler.
        /// </summary>
        /// <value>
        /// The on waiting handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnWaiting;

        /// <summary>
        /// Gets the on duration change handler.
        /// </summary>
        /// <value>
        /// The on duration change handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnDurationChange;

        /// <summary>
        /// Gets the on time upate handler.
        /// </summary>
        /// <value>
        /// The on time upate handler.
        /// </value>
		public extern event Action<MediaElement, ElementEvent> OnTimeUpdate;

        /// <summary>
        /// Gets the on play handler.
        /// </summary>
        /// <value>
        /// The on play handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnPlay;

        /// <summary>
        /// Gets the on pause handler.
        /// </summary>
        /// <value>
        /// The on pause handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnPause;

        /// <summary>
        /// Gets the on rate change handler.
        /// </summary>
        /// <value>
        /// The on rate change handler.
        /// </value>
        public extern event Action<MediaElement, ElementEvent> OnRateChange;

        /// <summary>
        /// Gets the on volume change handler.
        /// </summary>
        /// <value>
        /// The on volume change handler.
        /// </value>
		public extern event Action<ElementEvent> OnVolumeChange;

        /// <summary>
        /// Can play type.
        /// </summary>
        /// <param name="mediaType"> Type of the media. </param>
        /// <returns>
        /// "maybe", "probably" or "". Empty string means -ve response.
        /// </returns>
        public extern string CanPlayType(string mediaType);

        /// <summary>
        /// Loads this object.
        /// </summary>
        public extern void Load();

        /// <summary>
        /// Pauses this object.
        /// </summary>
        public extern void Pause();

        /// <summary>
        /// Plays this object.
        /// </summary>
        public extern void Play();
    }
}
