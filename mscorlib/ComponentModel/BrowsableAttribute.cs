namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited=true, AllowMultiple=false), NonScriptable]
    public sealed class BrowsableAttribute : Attribute
    {
    }
}

