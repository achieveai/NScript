namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited=true, AllowMultiple=false), NonScriptable]
    public sealed class PreserveCaseAttribute : Attribute
    {
    }
}

