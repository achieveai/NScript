namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, AttributeUsage(AttributeTargets.Type, Inherited=true, AllowMultiple=false), NonScriptable]
    public sealed class IgnoreNamespaceAttribute : Attribute
    {
    }
}

