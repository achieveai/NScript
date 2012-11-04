namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable]
    public abstract class MarshalByRefObject
    {
        protected MarshalByRefObject()
        {
        }
    }
}

