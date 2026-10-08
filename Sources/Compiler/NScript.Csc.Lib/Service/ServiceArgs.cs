//-----------------------------------------------------------------------
// <copyright file="ServiceArgs.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Command-line handling for the build-service opt-in flags. The flags are stripped
    /// before the stage's own parser runs, so a local fallback sees exactly the args the
    /// daemon would have seen.
    /// </summary>
    public static class ServiceArgs
    {
        /// <summary>
        /// Environment variable that opts csc.exe into the service. MSBuild's Csc task has no
        /// free-form argument parameter, so Sdk.targets sets it through CscEnvironment.
        /// </summary>
        public const string ServiceEnvVar = "NSCRIPT_SERVICE";

        /// <summary>Environment variable that opts csc.exe into watch mode (implies the service).</summary>
        public const string WatchEnvVar = "NSCRIPT_WATCH";

        /// <summary>Environment variable naming the NScript.Sdk folder (watched for props/targets edits).</summary>
        public const string WatchSdkDirEnvVar = "NSCRIPT_WATCH_SDKDIR";

        private static readonly string[] ReleaseFlags = { "-minify", "-uglify", "-optimize" };

        /// <summary>
        /// Removes every <c>-service</c> / <c>/service</c> (any case) from <paramref name="args"/>.
        /// </summary>
        /// <returns>True when at least one service flag was present.</returns>
        public static bool TryStripServiceFlag(string[] args, out string[] stripped)
        {
            var kept = new List<string>(args.Length);
            bool found = false;
            foreach (var arg in args)
            {
                if (string.Equals(arg, "-service", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(arg, "/service", StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    continue;
                }

                kept.Add(arg);
            }

            stripped = kept.ToArray();
            return found;
        }

        /// <summary>
        /// Removes every <c>-watch</c> / <c>/watch</c> (any case) from <paramref name="args"/>.
        /// </summary>
        /// <returns>True when at least one watch flag was present.</returns>
        public static bool TryStripWatchFlag(string[] args, out string[] stripped)
        {
            var kept = new List<string>(args.Length);
            bool found = false;
            foreach (var arg in args)
            {
                if (string.Equals(arg, "-watch", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(arg, "/watch", StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    continue;
                }

                kept.Add(arg);
            }

            stripped = kept.ToArray();
            return found;
        }

        /// <summary>
        /// True when <see cref="WatchEnvVar"/> is <c>1</c> or <c>true</c>.
        /// </summary>
        public static bool IsWatchRequestedByEnvironment()
        {
            var value = Environment.GetEnvironmentVariable(WatchEnvVar);
            return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// True when the stage-1 opt-in environment variable is set to <c>1</c> or <c>true</c>.
        /// </summary>
        public static bool IsServiceRequestedByEnvironment()
        {
            var value = Environment.GetEnvironmentVariable(ServiceEnvVar);
            return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// True when any release-only stage-2 flag is present. Such builds never go to the
        /// service (Release output stays batch).
        /// </summary>
        public static bool HasReleaseFlags(string[] args)
            => args.Any(arg => ReleaseFlags.Any(flag => string.Equals(arg, flag, StringComparison.OrdinalIgnoreCase)));

        /// <summary>
        /// Appends <c>-devMode</c> unless it is already present.
        /// </summary>
        public static string[] EnsureDevMode(string[] args)
            => args.Any(arg => string.Equals(arg, "-devMode", StringComparison.OrdinalIgnoreCase))
                ? args
                : args.Concat(new[] { "-devMode" }).ToArray();
    }
}
