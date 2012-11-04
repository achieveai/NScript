namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=false), NonScriptable, Extended]
    public sealed class PreserveNameAttribute : Attribute
    {
    }
}

