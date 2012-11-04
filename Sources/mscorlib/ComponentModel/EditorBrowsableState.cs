namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable]
    public enum EditorBrowsableState
    {
        Always,
        Never,
        Advanced
    }
}

