namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, ScriptNamespace("ss")]
    public interface INotifyCollectionChanged
    {
        event CollectionChangedEventHandler CollectionChanged;
    }
}

