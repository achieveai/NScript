namespace System.Serialization
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, IgnoreNamespace]
    public delegate object JsonParseCallback(string name, object value);
}

