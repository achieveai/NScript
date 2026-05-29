// -----------------------------------------------------------------------
// <copyright file="LogIngestionOptions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Services
{
    /// <summary>
    /// Configuration options for <see cref="LogIngestionService"/>. Wired
    /// through the options pattern by <c>AddSunlightLogIngestion</c>.
    /// </summary>
    public sealed class LogIngestionOptions
    {
        /// <summary>
        /// Max number of recent ids tracked in the de-dup LRU. Keeps the
        /// service idempotent against client retransmits without unbounded
        /// growth. Default 1024.
        /// </summary>
        public int DedupCapacity { get; set; } = 1024;
    }
}
