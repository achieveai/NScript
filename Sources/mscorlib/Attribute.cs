namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [EditorBrowsable(EditorBrowsableState.Never), Imported, NonScriptable]
    [AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=false)]
    public abstract class Attribute
    {
        protected Attribute()
        {
        }
    }
}

