namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Event dispatched to <see cref="IDBDatabase.OnVersionChange"/> when another
    /// connection requests a version upgrade.
    /// </summary>
    [IgnoreNamespace, ImportedType]
    public class IDBVersionChangeEvent : Event
    {
        public extern int OldVersion
        { get; }

        public extern int NewVersion
        { get; }
    }
}
