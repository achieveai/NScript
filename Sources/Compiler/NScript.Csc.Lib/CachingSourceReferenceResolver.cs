namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Remembers <see cref="SourceReferenceResolver.NormalizePath"/> for one compilation.
    /// Roslyn normalizes a source path per method body (debug documents, behind a 16-entry
    /// cache that a project of a thousand files thrashes) and per binder factory, each a
    /// Path.GetFullPath; profiling a 1317-file project put that at ~18% of compile CPU.
    /// The answer for a path does not change within a compile, so every call after the first
    /// is a lookup. All other members go to the wrapped resolver.
    /// </summary>
    public sealed class CachingSourceReferenceResolver : SourceReferenceResolver
    {
        private readonly SourceReferenceResolver inner;

        private readonly ConcurrentDictionary<(string path, string baseFilePath), string> normalized =
            new ConcurrentDictionary<(string path, string baseFilePath), string>();

        public CachingSourceReferenceResolver(SourceReferenceResolver inner)
        {
            this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override string NormalizePath(string path, string baseFilePath)
        {
            var key = (path, baseFilePath);
            if (!this.normalized.TryGetValue(key, out var result))
            {
                result = this.inner.NormalizePath(path, baseFilePath);
                this.normalized.TryAdd(key, result);
            }

            return result;
        }

        public override string ResolveReference(string path, string baseFilePath) => this.inner.ResolveReference(path, baseFilePath);

        public override Stream OpenRead(string resolvedPath) => this.inner.OpenRead(resolvedPath);

        public override SourceText ReadText(string resolvedPath) => this.inner.ReadText(resolvedPath);

        public override bool Equals(object other) => other is CachingSourceReferenceResolver caching && this.inner.Equals(caching.inner);

        public override int GetHashCode() => this.inner.GetHashCode();
    }
}
