// -----------------------------------------------------------------------
// <copyright file="SourceMapFileHandlerOptions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server
{
    using System.IO;

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
            catch
            {
                return null;
            }
        }
    }
}
