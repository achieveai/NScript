namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, NonScriptable, AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
    public sealed class IntrinsicFieldAttribute : Attribute
    {
    }
}

