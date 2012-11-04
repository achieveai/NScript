namespace System.Collections
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public delegate object ArrayAggregator(object aggregatedValue, object value, int index, Array array);
}

