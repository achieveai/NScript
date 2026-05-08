namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RequiredMemberAttribute : Attribute
    {
    }
}
