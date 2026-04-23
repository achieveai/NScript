// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandlerOptions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;

    /// <summary>
    /// Options for the ASP.NET Core source-map file handler. Controls where <c>.map</c>
    /// files are located and how the resolved source file paths are validated before
    /// being served to the client.
    /// </summary>
    public sealed class SourceMapFileHandlerOptions
    {
        /// <summary>
        /// Directory containing the <c>.map</c> files this handler serves against.
        /// Requests of the form <c>/{prefix}/{mapName}/{sourceName}</c> look for
        /// <c>{MapsDirectory}/{mapName}.map</c>.
        /// </summary>
        public string MapsDirectory { get; set; }

        /// <summary>
        /// When <c>true</c> (the default), the handler serves the file whose absolute path
        /// was recorded in the map's <c>sourcesLong</c> array. Set to <c>false</c> in
        /// deployments where source files are NOT available on the hosting machine's local
        /// filesystem (the handler will return 404 for every request).
        /// </summary>
        public bool ServeFromSourcesLong { get; set; } = true;

        /// <summary>
        /// Optional allow-list of root directories that source paths (from <c>sourcesLong</c>)
        /// must reside within. When <c>null</c> or empty, the handler falls back to requiring
        /// the resolved file to live under <see cref="MapsDirectory"/>'s parent hierarchy — the
        /// conservative default. Deployments where source files live outside the maps tree MUST
        /// set this explicitly; otherwise every request returns 404.
        /// </summary>
        /// <remarks>
        /// This is the primary defence against arbitrary-file-read via a tampered map: even if an
        /// attacker controls a map's <c>sourcesLong</c> entries, only files rooted under one of
        /// these directories will be served.
        /// </remarks>
        public IReadOnlyList<string> AllowedSourceRoots { get; set; }

        /// <summary>
        /// Hard cap on the byte length of a <c>.map</c> file this handler will parse. Maps larger
        /// than this are treated as 404 rather than pulled fully into memory. Default: 16 MB.
        /// </summary>
        public long MaxMapFileSizeBytes { get; set; } = 16L * 1024 * 1024;

        /// <summary>
        /// Hard cap on the byte length of a resolved source file this handler will stream back.
        /// Files larger than this are treated as 404. Default: 16 MB.
        /// </summary>
        public long MaxSourceFileSizeBytes { get; set; } = 16L * 1024 * 1024;

        /// <summary>
        /// Validates that <see cref="MapsDirectory"/> is set and exists. Returns the
        /// absolute, fully-qualified directory on success; <c>null</c> otherwise.
        /// </summary>
        /// <returns> Normalized absolute path, or <c>null</c> when the directory is not usable. </returns>
        public string ResolveMapsDirectory()
        {
            if (string.IsNullOrEmpty(this.MapsDirectory))
            {
                return null;
            }

            try
            {
                var full = Path.GetFullPath(this.MapsDirectory);
                return Directory.Exists(full) ? full : null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (PathTooLongException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (SecurityException)
            {
                return null;
            }
        }

        /// <summary>
        /// Computes the effective allow-list of absolute, normalized root directories used to
        /// contain resolved source paths. Invalid or non-existent entries are silently dropped
        /// (they can never match anyway). When no entries remain, falls back to <see cref="MapsDirectory"/>
        /// so that misconfiguration fails closed — the handler will only serve files that live
        /// within the maps directory tree.
        /// </summary>
        internal IReadOnlyList<string> ResolveAllowedRoots(string resolvedMapsDir)
        {
            var result = new List<string>();
            if (this.AllowedSourceRoots != null)
            {
                foreach (var raw in this.AllowedSourceRoots)
                {
                    if (string.IsNullOrEmpty(raw))
                    {
                        continue;
                    }

                    string full;
                    try
                    {
                        full = Path.GetFullPath(raw);
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                    catch (PathTooLongException)
                    {
                        continue;
                    }
                    catch (NotSupportedException)
                    {
                        continue;
                    }
                    catch (SecurityException)
                    {
                        continue;
                    }

                    result.Add(NormalizeDirectory(full));
                }
            }

            if (result.Count == 0 && !string.IsNullOrEmpty(resolvedMapsDir))
            {
                result.Add(NormalizeDirectory(resolvedMapsDir));
            }

            return result;
        }

        private static string NormalizeDirectory(string path)
        {
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                || path.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                return path;
            }

            return path + Path.DirectorySeparatorChar;
        }
    }
}
