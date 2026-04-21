namespace Sunlight.Framework.Data.WebStore
{
    /// <summary>
    /// Scope of a <see cref="WebStoreTransaction"/>, mapped to the underlying
    /// IDB transaction mode string when submitted to the browser.
    /// </summary>
    public enum TransactionKind
    {
        /// <summary>Readonly scope — fastest, permits concurrent readers.</summary>
        Read,
        /// <summary>Readwrite scope — serializes against other writers.</summary>
        ReadWrite,
        /// <summary>Versionchange scope — only valid during database upgrade.</summary>
        VersionChange,
    }
}
