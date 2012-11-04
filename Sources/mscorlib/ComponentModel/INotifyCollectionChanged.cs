namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, ScriptNamespace("ss")]
    public interface INotifyCollectionChanged
    {
        event CollectionChangedEventHandler CollectionChanged;
    }
}

