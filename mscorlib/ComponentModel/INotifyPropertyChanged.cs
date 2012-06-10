namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, ScriptNamespace("ss")]
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}

