namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    [NonScriptable, EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Type, Inherited=false, AllowMultiple=false), Extended]
    public sealed class NonScriptableAttribute : Attribute
    {
    }
}

