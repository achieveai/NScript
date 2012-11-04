namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, ScriptNamespace("ss")]
    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}

