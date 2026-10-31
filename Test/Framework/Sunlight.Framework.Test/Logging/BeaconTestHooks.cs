//-----------------------------------------------------------------------
// <copyright file="BeaconTestHooks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test.Logging
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;

    /// <summary>
    /// Browser hooks for driving <see cref="HttpLogSink"/>'s real unload path:
    /// swap <c>navigator.sendBeacon</c> for a recorder, fire <c>pagehide</c> or <c>beforeunload</c>,
    /// and read a beacon <c>Blob</c> back. Kept out of the test fixture so
    /// fixture classes carry no <c>[Script]</c> externs.
    /// </summary>
    public static class BeaconTestHooks
    {
        [Script(@"return window.navigator.sendBeacon;")]
        public static extern object GetSendBeacon();

        [Script(@"window.navigator.sendBeacon = beacon;")]
        public static extern void SetSendBeacon(object beacon);

        [Script(@"window.navigator.sendBeacon = beacon;")]
        public static extern void SetSendBeacon(Func<string, object, bool> beacon);

        [Script(@"var e = window.document.createEvent('Event'); e.initEvent('pagehide', false, false); window.dispatchEvent(e);")]
        public static extern void DispatchPageHide();

        [Script(@"var e = window.document.createEvent('Event'); e.initEvent('beforeunload', false, false); window.dispatchEvent(e);")]
        public static extern void DispatchBeforeUnload();

        [Script(@"return console.warn;")]
        public static extern object GetConsoleWarn();

        [Script(@"console.warn = warn;")]
        public static extern void SetConsoleWarn(object warn);

        [Script(@"console.warn = warn;")]
        public static extern void SetConsoleWarn(Action<string, object> warn);

        [Script(@"return data instanceof window.Blob;")]
        public static extern bool IsBlob(object data);

        [Script(@"blob.text().then(done);")]
        public static extern void ReadText(Blob blob, Action<string> done);
    }
}
