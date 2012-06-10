namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, ScriptNamespace("ss")]
    public sealed class PropertyChangedEventArgs : EventArgs
    {
        public PropertyChangedEventArgs(string propertyName)
        {
        }

        public string PropertyName
        {
            get
            {
                return null;
            }
        }
    }
}

