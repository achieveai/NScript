namespace System.Net
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, IgnoreNamespace, NumericValues]
    public enum ReadyState
    {
        Uninitialized,
        Open,
        Sent,
        Receiving,
        Loaded
    }
}

