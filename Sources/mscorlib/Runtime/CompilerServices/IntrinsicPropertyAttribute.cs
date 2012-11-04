namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, NonScriptable, AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
    public sealed class IntrinsicPropertyAttribute : Attribute
    {
    }
}

