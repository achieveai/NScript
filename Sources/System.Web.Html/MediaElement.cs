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
    public abstract class MediaElement : Element
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
        /// Source for the.
        /// </summary>
        [IntrinsicField]
        public string Src;

        /// <summary>
        /// The volume.
        /// </summary>
        [IntrinsicField]
        public double Volume;

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
