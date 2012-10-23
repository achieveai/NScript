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
    using System.Text;

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
    [IgnoreNamespace, PsudoType]
    public class TimeRanges
    {
        /// <summary>
        /// The length.
        /// </summary>
        [IntrinsicField]
        public int Length;

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
    [IgnoreNamespace, PsudoType]
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
        [IntrinsicField]
        public bool AutoPlay;

        /// <summary>
        /// The buffered.
        /// </summary>
        [IntrinsicField]
        public readonly TimeRanges Buffered;

        /// <summary>
        /// true to show, false to hide the controls.
        /// </summary>
        [IntrinsicField]
        [ScriptName("controls")]
        public bool ShowControls;

        /// <summary>
        /// The cross origin.
        /// </summary>
        [IntrinsicField]
        public string CrossOrigin;

        /// <summary>
        /// The current source.
        /// </summary>
        [IntrinsicField]
        public readonly string CurrentSrc;

        /// <summary>
        /// The current time.
        /// </summary>
        [IntrinsicField]
        public double CurrentTime;

        /// <summary>
        /// true if default muted.
        /// </summary>
        [IntrinsicField]
        public bool DefaultMuted;

        /// <summary>
        /// The default playback rate.
        /// </summary>
        [IntrinsicField]
        public double DefaultPlaybackRate;

        /// <summary>
        /// The duration.
        /// </summary>
        [IntrinsicField]
        public readonly double Duration;

        /// <summary>
        /// The ended.
        /// </summary>
        [IntrinsicField]
        public readonly bool Ended;

        /// <summary>
        /// The error.
        /// </summary>
        [IntrinsicField]
        public readonly MediaError? Error;

        /// <summary>
        /// Time of the initial.
        /// </summary>
        [IntrinsicField]
        public readonly double InitialTime;

        /// <summary>
        /// true to loop.
        /// </summary>
        [IntrinsicField]
        public bool Loop;

        /// <summary>
        /// true if muted.
        /// </summary>
        [IntrinsicField]
        public bool Muted;

        /// <summary>
        /// State of the network.
        /// </summary>
        [IntrinsicField]
        public readonly MediaNetworkState? NetworkState;

        /// <summary>
        /// true if paused.
        /// </summary>
        [IntrinsicField]
        public bool Paused;

        /// <summary>
        /// The playback rate.
        /// </summary>
        [IntrinsicField]
        public double PlaybackRate;

        /// <summary>
        /// The played.
        /// </summary>
        [IntrinsicField]
        public readonly TimeRanges Played;

        /// <summary>
        /// The preload.
        /// </summary>
        [IntrinsicField]
        public string Preload;

        /// <summary>
        /// State of the ready.
        /// </summary>
        [IntrinsicField]
        public MediaReadyState ReadyState;

        /// <summary>
        /// The seekable.
        /// </summary>
        [IntrinsicField]
        public readonly TimeRanges Seekable;

        /// <summary>
        /// The seeking.
        /// </summary>
        [IntrinsicField]
        public readonly bool Seeking;

        /// <summary>
        /// The volume.
        /// </summary>
        [IntrinsicField]
        public double Volume;

        /// <summary>
        /// Gets the on load start handler.
        /// </summary>
        /// <value>
        /// The on load start handler.
        /// </value>
        public event Action<ElementEvent> OnLoadStart
		{
			add { this.Bind(OnLoadStartEvtName, value); }
			remove { this.UnBind(OnLoadStartEvtName, value); }
		}

        /// <summary>
        /// Gets the on progress handler.
        /// </summary>
        /// <value>
        /// The on progress handler.
        /// </value>
		public event Action<ElementEvent> OnProgress
		{
			add { this.Bind(OnProgressEvtName, value); }
			remove { this.UnBind(OnProgressEvtName, value); }
		}

        /// <summary>
        /// Gets the on suspend handler.
        /// </summary>
        /// <value>
        /// The on suspend handler.
        /// </value>
		public event Action<ElementEvent> OnSuspend
		{
			add { this.Bind(OnSuspendEvtName, value); }
			remove { this.UnBind(OnSuspendEvtName, value); }
		}

        /// <summary>
        /// Gets the on abort handler.
        /// </summary>
        /// <value>
        /// The on abort handler.
        /// </value>
		public event Action<ElementEvent> OnAbort
		{
			add { this.Bind(OnAbortEvtName, value); }
			remove { this.UnBind(OnAbortEvtName, value); }
		}

        /// <summary>
        /// Gets the on error handler.
        /// </summary>
        /// <value>
        /// The on error handler.
        /// </value>
		public event Action<ElementEvent> OnError
		{
			add { this.Bind(OnErrorEvtName, value); }
			remove { this.UnBind(OnErrorEvtName, value); }
		}

        /// <summary>
        /// Gets the on stalled handler.
        /// </summary>
        /// <value>
        /// The on stalled handler.
        /// </value>
		public event Action<ElementEvent> OnStalled
		{
			add { this.Bind(OnStalledEvtName, value); }
			remove { this.UnBind(OnStalledEvtName, value); }
		}

        /// <summary>
        /// Gets the on emptied handler.
        /// </summary>
        /// <value>
        /// The on emptied handler.
        /// </value>
		public event Action<ElementEvent> OnEmptied
		{
			add { this.Bind(OnEmptiedEvtName, value); }
			remove { this.UnBind(OnEmptiedEvtName, value); }
		}

        /// <summary>
        /// Gets the on loaded metadata handler.
        /// </summary>
        /// <value>
        /// The on loaded metadata handler.
        /// </value>
		public event Action<ElementEvent> OnLoadedMetadata
		{
			add { this.Bind(OnLoadedMetadataEvtName, value); }
			remove { this.UnBind(OnLoadedMetadataEvtName, value); }
		}

        /// <summary>
        /// Gets the information describing the on loaded handler.
        /// </summary>
        /// <value>
        /// Information describing the on loaded handler.
        /// </value>
		public event Action<ElementEvent> OnLoadedData
		{
			add { this.Bind(OnLoadedDataEvtName, value); }
			remove { this.UnBind(OnLoadedDataEvtName, value); }
		}

        /// <summary>
        /// Gets the on can play handler.
        /// </summary>
        /// <value>
        /// The on can play handler.
        /// </value>
		public event Action<ElementEvent> OnCanPlay
		{
			add { this.Bind(OnCanPlayEvtName, value); }
			remove { this.UnBind(OnCanPlayEvtName, value); }
		}

        /// <summary>
        /// Gets the on can play through handler.
        /// </summary>
        /// <value>
        /// The on can play through handler.
        /// </value>
		public event Action<ElementEvent> OnCanPlayThrough
		{
			add { this.Bind(OnCanPlayThroughEvtName, value); }
			remove { this.UnBind(OnCanPlayThroughEvtName, value); }
		}

        /// <summary>
        /// Gets the on playing handler.
        /// </summary>
        /// <value>
        /// The on playing handler.
        /// </value>
		public event Action<ElementEvent> OnPlaying
		{
			add { this.Bind(OnPlayingEvtName, value); }
			remove { this.UnBind(OnPlayingEvtName, value); }
		}

        /// <summary>
        /// Gets the on ended handler.
        /// </summary>
        /// <value>
        /// The on ended handler.
        /// </value>
		public event Action<ElementEvent> OnEnded
		{
			add { this.Bind(OnEndedEvtName, value); }
			remove { this.UnBind(OnEndedEvtName, value); }
		}

        /// <summary>
        /// Gets the on waiting handler.
        /// </summary>
        /// <value>
        /// The on waiting handler.
        /// </value>
		public event Action<ElementEvent> OnWaiting
		{
			add { this.Bind(OnWaitingEvtName, value); }
			remove { this.UnBind(OnWaitingEvtName, value); }
		}

        /// <summary>
        /// Gets the on duration change handler.
        /// </summary>
        /// <value>
        /// The on duration change handler.
        /// </value>
		public event Action<ElementEvent> OnDurationChange
		{
			add { this.Bind(OnDurationChangeEvtName, value); }
			remove { this.UnBind(OnDurationChangeEvtName, value); }
		}

        /// <summary>
        /// Gets the on time upate handler.
        /// </summary>
        /// <value>
        /// The on time upate handler.
        /// </value>
		public event Action<ElementEvent> OnTimeUpate
		{
			add { this.Bind(OnTimeUpateEvtName, value); }
			remove { this.UnBind(OnTimeUpateEvtName, value); }
		}

        /// <summary>
        /// Gets the on play handler.
        /// </summary>
        /// <value>
        /// The on play handler.
        /// </value>
		public event Action<ElementEvent> OnPlay
		{
			add { this.Bind(OnPlayEvtName, value); }
			remove { this.UnBind(OnPlayEvtName, value); }
		}

        /// <summary>
        /// Gets the on pause handler.
        /// </summary>
        /// <value>
        /// The on pause handler.
        /// </value>
		public event Action<ElementEvent> OnPause
		{
			add { this.Bind(OnPauseEvtName, value); }
			remove { this.UnBind(OnPauseEvtName, value); }
		}

        /// <summary>
        /// Gets the on rate change handler.
        /// </summary>
        /// <value>
        /// The on rate change handler.
        /// </value>
		public event Action<ElementEvent> OnRateChange
		{
			add { this.Bind(OnRateChangeEvtName, value); }
			remove { this.UnBind(OnRateChangeEvtName, value); }
		}

        /// <summary>
        /// Gets the on volume change handler.
        /// </summary>
        /// <value>
        /// The on volume change handler.
        /// </value>
		public event Action<ElementEvent> OnVolumeChange
		{
			add { this.Bind(OnVolumeChangeEvtName, value); }
			remove { this.UnBind(OnVolumeChangeEvtName, value); }
		}

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
