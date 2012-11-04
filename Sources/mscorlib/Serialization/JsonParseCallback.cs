namespace System.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, IgnoreNamespace]
    public delegate object JsonParseCallback(string name, object value);
}

