//-----------------------------------------------------------------------
// <copyright file="AtomicFile.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace OwaSourceMapper
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Replaces a generated file in one step: the content goes to a unique temp file in the
    /// same folder, which is then renamed over the target. A reader (a browser reload, a test
    /// runner) sees the old file or the new one, never a truncated one, and a process killed
    /// mid-write leaves the old file in place.
    /// </summary>
    public static class AtomicFile
    {
        /// <summary>Starts the error thrown when the target stayed in use through every retry.</summary>
        public const string InUseMessagePrefix = "cannot replace ";

        // About 1 s: long enough for a browser or dev server to finish reading a bundle.
        private const int MoveAttempts = 10;

        private static readonly TimeSpan MoveRetryDelay = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Writes <paramref name="path"/> through <paramref name="write"/>. The bytes are the
        /// same as a <see cref="StreamWriter"/> opened on the path with <paramref name="encoding"/>.
        /// If the final rename still fails after retries (a reader holds the target without
        /// delete sharing), the temp file is deleted, the old target is kept, and the error is
        /// thrown: there is no fallback to an in-place write.
        /// </summary>
        public static void Write(string path, Encoding encoding, Action<TextWriter> write)
        {
            var target = Path.GetFullPath(path);

            // Unique per process and call, so two writers of the same file (parallel builds,
            // watch plus a plain build) never share a temp file.
            var temp = string.Format(
                "{0}.{1}.{2}.tmp",
                target,
                Environment.ProcessId,
                Guid.NewGuid().ToString("N").Substring(0, 8));
            try
            {
                using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream, encoding))
                {
                    write(writer);
                }

                MoveOver(temp, target);
            }
            catch
            {
                TryDelete(temp);
                throw;
            }
        }

        private static void MoveOver(string temp, string target)
        {
            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    File.Move(temp, target, overwrite: true);
                    return;
                }
                catch (Exception ex) when ((ex is IOException || ex is UnauthorizedAccessException) && attempt < MoveAttempts)
                {
                    Thread.Sleep(MoveRetryDelay);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    throw new IOException(InUseMessagePrefix + target + ": in use by another process (" + ex.Message + ")", ex);
                }
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
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
