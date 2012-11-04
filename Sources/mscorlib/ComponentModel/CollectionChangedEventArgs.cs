namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, ScriptNamespace("ss")]
    public sealed class CollectionChangedEventArgs : EventArgs
    {
        public CollectionChangedEventArgs(CollectionChangedAction action, object item)
        {
        }

        public CollectionChangedEventArgs(CollectionChangedAction action, object item, int index)
        {
        }

        public CollectionChangedAction Action
        {
            get
            {
                return CollectionChangedAction.Reset;
            }
        }

        public int Index
        {
            get
            {
                return 0;
            }
        }

        public object Item
        {
            get
            {
                return null;
            }
        }
    }
}

