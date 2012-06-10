namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false), NonScriptable, Imported]
    public sealed class GlobalMethodsAttribute : Attribute
    {
    }
}

