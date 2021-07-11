namespace System.Runtime.CompilerServices
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited=true, AllowMultiple=false), NonScriptable, Extended]
    public sealed class AlternateSignatureAttribute : Attribute
    {
    }
}

