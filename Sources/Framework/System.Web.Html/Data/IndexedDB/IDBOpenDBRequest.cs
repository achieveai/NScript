namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Specialized <see cref="IDBRequest"/> returned by
    /// <see cref="IDBFactory.Open(string, int)"/>. Surfaces upgrade/blocked events.
    /// </summary>
    [IgnoreNamespace]
    public class IDBOpenDBRequest : IDBRequest
    {
        public extern event Action<IDBOpenDBRequest, Event> OnBlocked;

        public extern event Action<IDBOpenDBRequest, Event> OnUpgradeNeeded;
    }
}
