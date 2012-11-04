namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss"), Extended]
    public delegate void CollectionChangedEventHandler(object sender, CollectionChangedEventArgs e);
}

