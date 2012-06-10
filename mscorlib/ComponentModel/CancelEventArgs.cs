namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss"), Imported]
    public class CancelEventArgs : EventArgs
    {
        public bool Cancel
        {
            get
            {
                return false;
            }
            set
            {
            }
        }
    }
}

