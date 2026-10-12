//-----------------------------------------------------------------------
// <copyright file="ServiceLauncher.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib.Service
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Starts a daemon for a toolset. The daemon always runs from a shadow copy of the
    /// toolset's runtime closure so it never locks <c>NScriptToolSet/bin</c> (a compiler
    /// rebuild must keep working while a daemon is alive), and it is started without
    /// inheriting the client's handles, so MSBuild never waits on the daemon's output.
    /// </summary>
    public static class ServiceLauncher
    {
        /// <summary>Marker written last into a finished shadow copy.</summary>
        public const string CompleteMarker = ".complete";

        /// <summary>Held open (no sharing) by a daemon for the life of its shadow copy.</summary>
        public const string DaemonLockFile = ".daemon.lock";

        // The repo toolset hosts the daemon in NScript; the NuGet tool layout in Cs2Jsc.
        private static readonly string[] HostNames = { "NScript", "Cs2Jsc" };

        /// <summary>
        /// Copies the runtime closure into <see cref="ServiceIdentity.ShadowDir"/> unless a
        /// complete copy is already there. Copies into a temp sibling and renames it, so a
        /// concurrent launcher either wins the rename or reuses the winner's copy.
        /// </summary>
        /// <returns>The shadow directory.</returns>
        public static string EnsureShadowCopy(ServiceIdentity identity)
        {
            var shadow = identity.ShadowDir;
            if (File.Exists(Path.Combine(shadow, CompleteMarker)))
            {
                return shadow;
            }

            Directory.CreateDirectory(identity.ServiceRoot);
            var temp = Path.Combine(identity.ServiceRoot, $".tmp-{Environment.ProcessId}-{DateTime.UtcNow.Ticks}");
            foreach (var rel in ServiceIdentity.RuntimeClosure(identity.ToolsetDir))
            {
                var target = Path.Combine(temp, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(Path.Combine(identity.ToolsetDir, rel), target);
            }

            File.WriteAllText(Path.Combine(temp, CompleteMarker), identity.ToolsetHash);

            try
            {
                Directory.Move(temp, shadow);
            }
            catch (IOException) when (File.Exists(Path.Combine(shadow, CompleteMarker)))
            {
                // Another client finished the same copy first; use theirs.
                TryDeleteDirectory(temp);
            }

            return shadow;
        }

        /// <summary>
        /// Returns why this toolset cannot host a daemon (no NScript or Cs2Jsc host), or null
        /// when it can.
        /// </summary>
        public static string MissingDaemonReason(ServiceIdentity identity)
            => HostPath(identity.ToolsetDir) == null ? "no " + string.Join(" or ", HostFileNames()) + " in " + identity.ToolsetDir : null;

        /// <summary>The daemon host in <paramref name="dir"/>: an .exe on Windows, else a .dll; null when none.</summary>
        public static string HostPath(string dir)
            => HostFileNames().Select(name => Path.Combine(dir, name)).FirstOrDefault(File.Exists);

        private static string[] HostFileNames()
        {
            var ext = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ".dll";
            return HostNames.Select(name => name + ext).ToArray();
        }

        /// <summary>
        /// Starts <c>NScript.exe service</c> (or <c>Cs2Jsc.exe service</c>) from <paramref name="shadowDir"/> for
        /// <paramref name="identity"/>. Returns the failure reason, or null on success.
        /// </summary>
        public static string Launch(ServiceIdentity identity, string shadowDir)
        {
            // Trailing separators are trimmed: "C:\dir\" would escape its closing quote.
            var toolsetDir = Path.TrimEndingDirectorySeparator(identity.ToolsetDir);
            var serviceArgs = $"service --toolset-dir \"{toolsetDir}\" --toolset-hash {identity.ToolsetHash}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var exe = HostPath(shadowDir);
                if (exe == null)
                {
                    return "no " + string.Join(" or ", HostFileNames()) + " in " + shadowDir;
                }

                // Same recipe as Roslyn's VBCSCompiler launch (BuildServerConnection): no std
                // handles and no handle inheritance, so MSBuild's ToolTask does not wait on
                // pipes that the long-lived daemon would otherwise keep open.
                var startInfo = new STARTUPINFO();
                startInfo.cb = Marshal.SizeOf(startInfo);
                startInfo.hStdError = NativeMethods.InvalidIntPtr;
                startInfo.hStdInput = NativeMethods.InvalidIntPtr;
                startInfo.hStdOutput = NativeMethods.InvalidIntPtr;
                startInfo.dwFlags = NativeMethods.STARTF_USESTDHANDLES;

                var commandLine = new StringBuilder($"\"{exe}\" {serviceArgs}");
                bool success = NativeMethods.CreateProcess(
                    lpApplicationName: null,
                    lpCommandLine: commandLine,
                    lpProcessAttributes: NativeMethods.NullPtr,
                    lpThreadAttributes: NativeMethods.NullPtr,
                    bInheritHandles: false,
                    dwCreationFlags: NativeMethods.NORMAL_PRIORITY_CLASS | NativeMethods.CREATE_NO_WINDOW,
                    lpEnvironment: NativeMethods.NullPtr,
                    lpCurrentDirectory: shadowDir,
                    lpStartupInfo: ref startInfo,
                    lpProcessInformation: out var processInfo);

                if (!success)
                {
                    return "CreateProcess failed with error " + Marshal.GetLastWin32Error();
                }

                NativeMethods.CloseHandle(processInfo.hProcess);
                NativeMethods.CloseHandle(processInfo.hThread);
                return null;
            }

            var dll = HostPath(shadowDir);
            if (dll == null)
            {
                return "no " + string.Join(" or ", HostFileNames()) + " in " + shadowDir;
            }

            var psi = new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet",
                Arguments = $"\"{dll}\" {serviceArgs}",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = shadowDir,
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                return "Process.Start returned null";
            }

            process.StandardInput.Close();
            process.StandardOutput.Close();
            process.StandardError.Close();
            return null;
        }

        /// <summary>
        /// Deletes shadow copies and run folders of other toolset builds that no daemon is
        /// using, under every toolset key (Debug, Release, other checkouts), not only this one.
        /// A running daemon holds <see cref="DaemonLockFile"/> open, so its directory is
        /// skipped; fresh copies (a launch in progress) are skipped too. Logs are kept.
        /// Best effort.
        /// </summary>
        public static void DeleteStaleShadowCopies(ServiceIdentity identity, string currentShadowDir)
        {
            var allKeysRoot = Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(identity.ServiceRoot));
            if (allKeysRoot == null || !Directory.Exists(allKeysRoot))
            {
                return;
            }

            var current = Path.TrimEndingDirectorySeparator(Path.GetFullPath(currentShadowDir));
            foreach (var dir in SafeSubdirectories(allKeysRoot).SelectMany(SafeSubdirectories))
            {
                if (string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(dir)), current, StringComparison.OrdinalIgnoreCase)
                    || DateTime.UtcNow - Directory.GetLastWriteTimeUtc(dir) < TimeSpan.FromMinutes(10))
                {
                    continue;
                }

                var lockFile = Path.Combine(dir, DaemonLockFile);
                try
                {
                    if (File.Exists(lockFile))
                    {
                        // Fails while the owning daemon is alive.
                        File.Delete(lockFile);
                    }
                }
                catch (IOException)
                {
                    continue;
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                TryDeleteDirectory(dir);
            }
        }

        private static string[] SafeSubdirectories(string dir)
        {
            try
            {
                return Directory.GetDirectories(dir);
            }
            catch (IOException)
            {
                return Array.Empty<string>();
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<string>();
            }
        }

        private static void TryDeleteDirectory(string dir)
        {
            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
