//-----------------------------------------------------------------------
// <copyright file="FileSystemEntry.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// A file or folder from a drop, via <see cref="DataTransferItem.WebkitGetAsEntry"/>.
    /// Cast to <see cref="FileSystemFileEntry"/> or <see cref="FileSystemDirectoryEntry"/>
    /// after checking <see cref="IsFile"/> / <see cref="IsDirectory"/>.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public class FileSystemEntry
    {
        internal extern FileSystemEntry();

        /// <summary>
        /// Gets a value indicating whether this entry is a file.
        /// </summary>
        public extern bool IsFile
        { get; }

        /// <summary>
        /// Gets a value indicating whether this entry is a folder.
        /// </summary>
        public extern bool IsDirectory
        { get; }

        /// <summary>
        /// Gets the entry name.
        /// </summary>
        public extern string Name
        { get; }

        /// <summary>
        /// Gets the path from the root of the drop, such as "/folder/a.txt".
        /// </summary>
        public extern string FullPath
        { get; }
    }

    /// <summary>
    /// A file entry.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class FileSystemFileEntry : FileSystemEntry
    {
        private extern FileSystemFileEntry();

        /// <summary>
        /// Reads the entry as a <see cref="System.Web.Html.File"/>.
        /// </summary>
        /// <returns>
        /// A task with the file; it rejects with the browser's error.
        /// </returns>
        public Task<File> File()
        {
            return PromiseTask.ToTask(
                new Promise<File>((resolve, reject) => this.FileCallback(resolve, reject)));
        }

        [ScriptName("file")]
        private extern void FileCallback(Action<File> success, Action<object> error);
    }

    /// <summary>
    /// A folder entry.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class FileSystemDirectoryEntry : FileSystemEntry
    {
        private extern FileSystemDirectoryEntry();

        /// <summary>
        /// Creates a reader for the folder's children.
        /// </summary>
        public extern FileSystemDirectoryReader CreateReader();
    }

    /// <summary>
    /// Reads a folder's children in batches.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class FileSystemDirectoryReader
    {
        private extern FileSystemDirectoryReader();

        /// <summary>
        /// Reads the next batch of children. Call again until the batch is empty:
        /// browsers return at most 100 entries per call.
        /// </summary>
        /// <returns>
        /// A task with the batch; empty when every child has been read.
        /// </returns>
        public Task<NativeArray<FileSystemEntry>> ReadEntries()
        {
            return PromiseTask.ToTask(
                new Promise<NativeArray<FileSystemEntry>>(
                    (resolve, reject) => this.ReadEntriesCallback(resolve, reject)));
        }

        [ScriptName("readEntries")]
        private extern void ReadEntriesCallback(Action<NativeArray<FileSystemEntry>> success, Action<object> error);
    }

    /// <summary>
    /// Bridges a <see cref="Promise{T}"/> to a <see cref="Task{T}"/>. Both are the native
    /// Promise at runtime, so this costs nothing.
    /// </summary>
    internal static class PromiseTask
    {
        [Script("return p;")]
        internal static extern Task<T> ToTask<T>(Promise<T> p);
    }
}
