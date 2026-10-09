//-----------------------------------------------------------------------
// <copyright file="ServiceProtocol.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Text.Json;

    /// <summary>
    /// One request per pipe connection, sent as a single JSON line.
    /// </summary>
    public sealed class ServiceRequest
    {
        public string Kind { get; set; }

        public int ClientPid { get; set; }

        public string Cwd { get; set; }

        public string[] Args { get; set; }

        /// <summary>
        /// Watch mode (<c>NScriptWatch=true</c>): the daemon records this request and replays
        /// it when one of its inputs changes on disk.
        /// </summary>
        public bool Watch { get; set; }

        /// <summary>
        /// Compile requests in watch mode: the NScript.Sdk folder whose props/targets the
        /// project imports. Edits there make the daemon ask for a <c>dotnet build</c>.
        /// </summary>
        public string WatchSdkDir { get; set; }
    }

    /// <summary>
    /// The single JSON-line reply to a <see cref="ServiceRequest"/>.
    /// </summary>
    public sealed class ServiceResponse
    {
        public int ExitCode { get; set; }

        public string Stdout { get; set; }

        public string Stderr { get; set; }

        /// <summary>
        /// The daemon failed in a way that says nothing about the inputs (an exception
        /// escaped the stage). The client falls back to a local compile.
        /// </summary>
        public bool InternalError { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Stage 1 only: the compile ran with <c>/utf8output</c>, so the client switches its
        /// console to UTF-8 before replaying, as the local csc does.
        /// </summary>
        public bool Utf8Output { get; set; }

        public int DaemonPid { get; set; }

        public long RequestId { get; set; }

        public long ElapsedMs { get; set; }

        /// <summary>
        /// Status reply only: name/value pairs printed by <c>service --status</c>.
        /// </summary>
        public Dictionary<string, string> Status { get; set; }
    }

    /// <summary>
    /// Wire format shared by the client and the daemon.
    /// </summary>
    public static class ServiceProtocol
    {
        public const string KindCompile = "compile";
        public const string KindEmitJs = "emitJs";
        public const string KindStatus = "status";
        public const string KindStop = "stop";

        /// <summary>
        /// <c>nscript service --sync</c>: Args are the project's obj DLL and the wait in whole
        /// seconds; the reply's ExitCode is the answer (0 current, 1 not, 2 busy).
        /// </summary>
        public const string KindSync = "sync";

        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        public static void WriteMessage<T>(Stream stream, T message)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte((byte)'\n');
            stream.Flush();
        }

        /// <summary>
        /// Reads one JSON line. Returns null when the stream ends before a full line arrives
        /// (the peer died or hung up).
        /// </summary>
        public static T ReadMessage<T>(Stream stream)
            where T : class
        {
            var buffer = new MemoryStream();
            var chunk = new byte[64 * 1024];
            while (true)
            {
                int read = stream.Read(chunk, 0, chunk.Length);
                if (read <= 0)
                {
                    return null;
                }

                // One message per connection and the peer sends nothing after the newline,
                // so a chunk that ends with '\n' completes the message.
                buffer.Write(chunk, 0, read);
                if (chunk[read - 1] == (byte)'\n')
                {
                    break;
                }
            }

            var text = Utf8NoBom.GetString(buffer.GetBuffer(), 0, (int)buffer.Length - 1);
            return JsonSerializer.Deserialize<T>(text);
        }
    }

    /// <summary>
    /// Identity of one toolset build: where it lives, a hash over the files the daemon loads,
    /// and the names derived from both. A rebuilt toolset gets a new hash and therefore a new
    /// pipe name, so a stale daemon is never reused; it idles out.
    /// </summary>
    public sealed class ServiceIdentity
    {
        /// <summary>Environment variable overriding the daemon log path.</summary>
        public const string LogPathEnvVar = "NSCRIPT_SERVICE_LOG";

        private ServiceIdentity(string toolsetDir, string toolsetHash)
        {
            this.ToolsetDir = toolsetDir;
            this.ToolsetHash = toolsetHash;
            this.Key = ComputeKey(toolsetDir);
            this.PipeName = "nscript-" + this.Key + "-" + toolsetHash.Substring(0, 12);
            this.ServiceRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NScript",
                "service",
                this.Key);
        }

        /// <summary>The original (not shadow) toolset directory, with a trailing separator.</summary>
        public string ToolsetDir { get; }

        /// <summary>Hex SHA-256 over the runtime closure of <see cref="ToolsetDir"/>.</summary>
        public string ToolsetHash { get; }

        /// <summary>16 hex chars over toolset dir, user and elevation.</summary>
        public string Key { get; }

        public string PipeName { get; }

        /// <summary>The lifetime mutex: one daemon per pipe name.</summary>
        public string MutexName => @"Local\" + this.PipeName;

        /// <summary><c>%LOCALAPPDATA%\NScript\service\&lt;key&gt;</c>: log and shadow copies.</summary>
        public string ServiceRoot { get; }

        public string ShadowDir => Path.Combine(this.ServiceRoot, this.ToolsetHash.Substring(0, 16));

        /// <summary>
        /// <c>&lt;ServiceRoot&gt;\&lt;hash16&gt;.run</c>: state owned by the one daemon of this
        /// toolset build (lock, pid file, rsp snapshots, watch.log). A sibling of
        /// <see cref="ShadowDir"/>, never inside it.
        /// </summary>
        public string RunDir => Path.Combine(this.ServiceRoot, this.ToolsetHash.Substring(0, 16) + ".run");

        public string LogPath
        {
            get
            {
                var overridePath = Environment.GetEnvironmentVariable(LogPathEnvVar);
                return string.IsNullOrWhiteSpace(overridePath)
                    ? Path.Combine(this.ServiceRoot, "service.jsonl")
                    : overridePath.Trim();
            }
        }

        public static ServiceIdentity ForToolset(string toolsetDir)
        {
            var dir = NormalizeDir(toolsetDir);
            return new ServiceIdentity(dir, ComputeToolsetHash(dir));
        }

        /// <summary>Used by a daemon started from a shadow copy: the launching client passes both.</summary>
        public static ServiceIdentity FromKnown(string toolsetDir, string toolsetHash)
            => new ServiceIdentity(NormalizeDir(toolsetDir), toolsetHash);

        /// <summary>
        /// The files a daemon loads: top-level assemblies, deps/runtimeconfig json and
        /// PluginConfig.xml, plus the native and RID-specific assets for this OS. Excludes
        /// publish/, .playwright/, pdbs and other RIDs, which the daemon never loads.
        /// Paths are relative to <paramref name="toolsetDir"/>, sorted ordinally.
        /// </summary>
        public static List<string> RuntimeClosure(string toolsetDir)
        {
            var dir = NormalizeDir(toolsetDir);
            var files = new List<string>();
            foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileName(file);
                var ext = Path.GetExtension(name).ToLowerInvariant();
                if (ext == ".dll" || ext == ".exe" || ext == ".json"
                    || string.Equals(name, "PluginConfig.xml", StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(name);
                }
            }

            foreach (var rid in CurrentRuntimeDirectories())
            {
                var ridDir = Path.Combine(dir, "runtimes", rid);
                if (!Directory.Exists(ridDir))
                {
                    continue;
                }

                foreach (var file in Directory.EnumerateFiles(ridDir, "*", SearchOption.AllDirectories))
                {
                    files.Add(Path.GetRelativePath(dir, file));
                }
            }

            files.Sort(StringComparer.Ordinal);
            return files;
        }

        /// <summary>
        /// SHA-256 over the sorted lines <c>relpath|length|mtimeTicks</c> of <see cref="RuntimeClosure"/>.
        /// </summary>
        public static string ComputeToolsetHash(string toolsetDir)
        {
            var dir = NormalizeDir(toolsetDir);
            var sb = new StringBuilder();
            foreach (var rel in RuntimeClosure(dir))
            {
                var info = new FileInfo(Path.Combine(dir, rel));
                sb.Append(rel.ToLowerInvariant()).Append('|')
                    .Append(info.Length).Append('|')
                    .Append(info.LastWriteTimeUtc.Ticks).Append('\n');
            }

            return Hex(SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString())));
        }

        private static string ComputeKey(string toolsetDir)
        {
            // Elevation is part of the key: a non-elevated client cannot open an elevated
            // daemon's CurrentUserOnly pipe (its owner is Administrators), so they must not share.
            var seed = toolsetDir.ToLowerInvariant() + "|" + Environment.UserName + "|" + (IsElevated() ? "T" : "F");
            return Hex(SHA256.HashData(Encoding.UTF8.GetBytes(seed))).Substring(0, 16);
        }

        private static bool IsElevated()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

#pragma warning disable CA1416 // Validate platform compatibility
            using var identity = WindowsIdentity.GetCurrent();
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
#pragma warning restore CA1416
        }

        private static IEnumerable<string> CurrentRuntimeDirectories()
        {
            string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                : "linux";
            string arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
            yield return os;
            yield return os + "-" + arch;
            if (os != "win")
            {
                yield return "unix";
            }
        }

        private static string NormalizeDir(string dir)
            => Path.TrimEndingDirectorySeparator(Path.GetFullPath(dir)) + Path.DirectorySeparatorChar;

        private static string Hex(byte[] bytes) => Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
