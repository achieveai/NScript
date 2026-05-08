namespace System.Diagnostics.CodeAnalysis
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable, AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class SetsRequiredMembersAttribute : Attribute
    {
    }
}
