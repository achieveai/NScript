namespace System.Runtime.CompilerServices
{
    using System;

    [NonScriptable, Extended, AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public sealed class AttachedPropertyAttribute : Attribute
    {
    }
}

