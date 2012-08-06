namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=false), Imported, NonScriptable]
    public sealed class ScriptSkipAttribute : Attribute
    {
    }
}

