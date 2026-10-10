//-----------------------------------------------------------------------
// <copyright file="ServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Text;
    using NScript.Utils;

    /// <summary>
    /// Client side of the build service: forwards one stage invocation to the daemon that
    /// serves this toolset and replays its output. Any failure to get a real result prints
    /// <c>warning NSS001</c> and returns null so the caller compiles locally; the build never
    /// fails because the service is missing.
    /// </summary>
    public static class ServiceClient
    {
        private const int ConnectTimeoutMs = 200;

        private const int LaunchWaitMs = 10_000;

        /// <summary>
        /// Runs <paramref name="args"/> on the daemon.
        /// </summary>
        /// <param name="kind"><see cref="ServiceProtocol.KindCompile"/> or <see cref="ServiceProtocol.KindEmitJs"/>.</param>
        /// <param name="args">Args with the service flag already stripped.</param>
        /// <param name="toolName">Origin printed in the NSS001 warning (<c>csc</c> or <c>nscript</c>).</param>
        /// <param name="watch">Ask the daemon to record this request for watch mode.</param>
        /// <returns>The daemon's exit code, or null when the caller must compile locally.</returns>
        public static int? TryRun(string kind, string[] args, string toolName, bool watch = false)
        {
            var request = NewRequest(kind, args, watch);
            var log = CompilerLog.ForComponent("ServiceClient");
            var total = Stopwatch.StartNew();
            string reason;
            long hashMs = 0, connectMs = 0, shadowCopyMs = 0, launchMs = 0;

            try
            {
                var hashSw = Stopwatch.StartNew();
                var identity = ServiceIdentity.ForToolset(AppContext.BaseDirectory);
                hashMs = hashSw.ElapsedMilliseconds;

                var connectSw = Stopwatch.StartNew();
                var pipe = TryConnect(identity.PipeName, ConnectTimeoutMs);
                connectMs = connectSw.ElapsedMilliseconds;
                reason = null;
                if (pipe == null && (reason = ServiceLauncher.MissingDaemonReason(identity)) == null)
                {
                    var copySw = Stopwatch.StartNew();
                    var shadowDir = ServiceLauncher.EnsureShadowCopy(identity);
                    shadowCopyMs = copySw.ElapsedMilliseconds;

                    var launchSw = Stopwatch.StartNew();
                    reason = ServiceLauncher.Launch(identity, shadowDir);
                    if (reason == null)
                    {
                        // Poll until the new daemon listens. If another client launched one at
                        // the same time, ours loses the lifetime mutex and exits; either way
                        // somebody answers on the pipe.
                        while (pipe == null && launchSw.ElapsedMilliseconds < LaunchWaitMs)
                        {
                            pipe = TryConnect(identity.PipeName, 500);
                        }

                        if (pipe == null)
                        {
                            reason = $"launched a daemon but it did not listen on {identity.PipeName} within {LaunchWaitMs}ms";
                        }
                    }

                    launchMs = launchSw.ElapsedMilliseconds;
                    log.Information(
                        "ServiceClient.Launch ShadowDir={ShadowDir} ShadowCopyMs={ShadowCopyMs} LaunchMs={LaunchMs} Connected={Connected}",
                        shadowDir, shadowCopyMs, launchMs, pipe != null);
                }

                using var connection = pipe;
                if (pipe == null)
                {
                    reason ??= "no daemon listening on " + identity.PipeName;
                }
                else
                {
                    ServiceProtocol.WriteMessage(pipe, request);

                    var response = ServiceProtocol.ReadMessage<ServiceResponse>(pipe);
                    if (response == null)
                    {
                        reason = "the daemon closed the connection before replying";
                    }
                    else if (response.InternalError)
                    {
                        reason = "daemon internal error: " + response.Message;
                    }
                    else
                    {
                        Replay(response);
                        total.Stop();
                        Console.Out.WriteLine(
                            "NScript service: pid={0} request={1} {2}ms (client {3}ms){4}",
                            response.DaemonPid,
                            response.RequestId,
                            response.ElapsedMs,
                            total.ElapsedMilliseconds,
                            watch ? " watch" : string.Empty);
                        log.Information(
                            "ServiceClient.Result {Mode} {Kind} ExitCode={ExitCode} DaemonPid={DaemonPid} RequestId={RequestId} HashMs={HashMs} ConnectMs={ConnectMs} ShadowCopyMs={ShadowCopyMs} LaunchMs={LaunchMs} TotalMs={TotalMs}",
                            "service", kind, response.ExitCode, response.DaemonPid, response.RequestId, hashMs, connectMs, shadowCopyMs, launchMs, total.ElapsedMilliseconds);
                        return response.ExitCode;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is TimeoutException || ex is System.Text.Json.JsonException)
            {
                reason = ex.GetType().Name + ": " + ex.Message;
            }

            Console.Out.WriteLine(
                "{0} : warning NSS001: NScript service unavailable ({1}); compiling locally{2}",
                toolName,
                reason,
                watch ? "; watch not active" : string.Empty);
            log.Warning(
                "ServiceClient.Result {Mode} {Kind} Reason={Reason} HashMs={HashMs} ConnectMs={ConnectMs} ShadowCopyMs={ShadowCopyMs} LaunchMs={LaunchMs} TotalMs={TotalMs}",
                "fallback", kind, reason, hashMs, connectMs, shadowCopyMs, launchMs, total.ElapsedMilliseconds);
            return null;
        }

        /// <summary>
        /// The request <see cref="TryRun"/> sends. A watch compile also carries what Sdk.targets
        /// put in csc's environment: the NScript.Sdk folder, the evaluation time and the
        /// compile properties hash.
        /// </summary>
        public static ServiceRequest NewRequest(string kind, string[] args, bool watch)
        {
            bool watchCompile = watch && kind == ServiceProtocol.KindCompile;
            string propsHash = watchCompile ? Environment.GetEnvironmentVariable(ServiceArgs.WatchPropsHashEnvVar) : null;
            return new ServiceRequest
            {
                Kind = kind,
                ClientPid = Environment.ProcessId,
                Cwd = Directory.GetCurrentDirectory(),
                Args = args,
                Watch = watch,
                WatchSdkDir = watchCompile ? Environment.GetEnvironmentVariable(ServiceArgs.WatchSdkDirEnvVar) : null,
                WatchEvaluatedUtcTicks = watchCompile ? ServiceArgs.ReadWatchEvaluatedUtcTicks() : null,
                WatchPropsHash = string.IsNullOrEmpty(propsHash) ? null : propsHash,
            };
        }

        /// <summary>
        /// Connects to <paramref name="pipeName"/>, or returns null when nobody is listening
        /// within <paramref name="timeoutMs"/>.
        /// </summary>
        public static NamedPipeClientStream TryConnect(string pipeName, int timeoutMs)
        {
            var pipe = new NamedPipeClientStream(
                ".",
                pipeName,
                PipeDirection.InOut,
                PipeOptions.CurrentUserOnly);
            try
            {
                pipe.Connect(timeoutMs);
                return pipe;
            }
            catch (TimeoutException)
            {
                pipe.Dispose();
                return null;
            }
        }

        private static void Replay(ServiceResponse response)
        {
            if (response.Utf8Output)
            {
                // The local csc switches the console to UTF-8 for /utf8output
                // (ConsoleUtil.RunWithUtf8Output); keep the replayed bytes identical.
                Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            }

            if (!string.IsNullOrEmpty(response.Stdout))
            {
                Console.Out.Write(response.Stdout);
                Console.Out.Flush();
            }

            if (!string.IsNullOrEmpty(response.Stderr))
            {
                Console.Error.Write(response.Stderr);
                Console.Error.Flush();
            }
        }
    }
}
