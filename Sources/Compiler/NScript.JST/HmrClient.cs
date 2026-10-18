namespace NScript.JST
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>Where a dev bundle's live patch client connects: the build service's socket and its token.</summary>
    public sealed record HmrEndpoint(string Url, string Token);

    /// <summary>
    /// Dev patch emit: the live patch client baked into each dev bundle after its closure, in
    /// the finally of a try around it, so a closure that throws while it loads still gets a
    /// client. Three lines: <see cref="Text"/>, the <see cref="Marker"/>, and the boot call
    /// <c>__nsHmrBoot("file|bundle id|ws url|token|gen:seq")</c>. Each patch writes the boot
    /// call's literal again with the patch's <c>gen:seq</c>, in the same write as the patch, so
    /// any copy of the bundle says which code it holds. The client is fixed text: only the
    /// boot literal changes, and it names nothing the compiler renames.
    /// </summary>
    public static class HmrClient
    {
        /// <summary>The marker of the boot call's line.</summary>
        public static readonly string Marker = MarkerStatement.ForLiteral("NScript.Hmr");

        /// <summary>
        /// The client, one line. Per bundle file it keeps <c>window.__nsHmr[file]</c>, takes
        /// the socket's records in order through the bundle's <c>__nsPatch</c> hook, and
        /// reloads the page only when its own script, fetched again, holds other code
        /// (at most 2 reloads in 60 s). No socket: it checks its script with backoff.
        /// </summary>
        public static readonly string Text = Bake(Source());

        /// <summary>
        /// One line from the client source: lines trimmed, full-line <c>//</c> comments dropped,
        /// the rest joined with spaces (every statement ends in ';' or '}').
        /// </summary>
        public static string Bake(string source)
            => string.Join(
                " ",
                source.Split('\n')
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0 && !line.StartsWith("//", StringComparison.Ordinal)));

        private static string Source()
        {
            using var stream = typeof(HmrClient).Assembly.GetManifestResourceStream("NScript.JST.HmrClient.js")
                ?? throw new InvalidOperationException("NScript.JST.HmrClient.js is not embedded.");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// The boot literal up to its <c>gen:seq</c>. A null endpoint (a bundle built outside
        /// the build service) leaves the url empty: the client then only checks its script.
        /// </summary>
        public static string BootPrefix(string fileName, string bundleId, HmrEndpoint endpoint)
            => fileName + "|" + bundleId + "|" + (endpoint?.Url ?? string.Empty) + "|" + (endpoint?.Token ?? string.Empty) + "|";

        /// <summary>The boot call line for <paramref name="value"/>, the whole boot literal.</summary>
        public static string BootLine(string value) => "__nsHmrBoot(\"" + Utils.ToJSString(value) + "\");";

        /// <summary>
        /// The bundle's id: 12 hex chars of the SHA-256 of its full, lower-cased path. A copy
        /// of the bundle keeps its id.
        /// </summary>
        public static string BundleId(string jsFileName)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(jsFileName).ToLowerInvariant())))
                .Substring(0, 12)
                .ToLowerInvariant();

        /// <summary>A new generation: one per full emit, never the same across daemon restarts.</summary>
        public static string NewGen() => Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
    }
}
