namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [ScriptNamespace("ss"), Extended]
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

