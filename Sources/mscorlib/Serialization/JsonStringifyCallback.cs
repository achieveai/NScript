namespace System.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, IgnoreNamespace]
    public delegate object JsonStringifyCallback(string name, object value);
}

