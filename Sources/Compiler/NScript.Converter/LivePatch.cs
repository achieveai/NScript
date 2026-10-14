namespace NScript.Converter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// What one dev build with the live patch client (<c>NSCRIPT_PATCH_EMIT=1</c>) means for
    /// pages that have its bundle open. <see cref="Seq"/> 0 is a full emit: a new
    /// <see cref="Gen"/>, so open pages reload. A patch has the next seq of its gen and either
    /// <see cref="Js"/>, which a page runs through the bundle's <c>__nsPatch</c> hook, or a
    /// <see cref="Reload"/> reason. A null gen asks open pages to reload now.
    /// </summary>
    public sealed record LivePatch(string Gen, int Seq, string Js, int Methods, string Reload, IReadOnlyList<string> Notes)
    {
        /// <summary>A full emit of generation <paramref name="gen"/>.</summary>
        public static LivePatch NewBuild(string gen) => new LivePatch(gen, 0, null, 0, "new build", Array.Empty<string>());

        /// <summary>Open pages reload for <paramref name="reason"/>, whatever they hold.</summary>
        public static LivePatch ReloadAll(string reason) => new LivePatch(null, 0, null, 0, reason, Array.Empty<string>());
    }
}
