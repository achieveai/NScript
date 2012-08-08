namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=false), NonScriptable, Imported]
    public sealed class PreserveNameAttribute : Attribute
    {
    }
}

