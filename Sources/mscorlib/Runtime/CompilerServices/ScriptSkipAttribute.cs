namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=false), Extended, NonScriptable]
    public sealed class ScriptSkipAttribute : Attribute
    {
    }
}

