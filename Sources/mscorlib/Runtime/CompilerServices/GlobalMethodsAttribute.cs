namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false), NonScriptable, Extended]
    public sealed class GlobalMethodsAttribute : Attribute
    {
    }
}

