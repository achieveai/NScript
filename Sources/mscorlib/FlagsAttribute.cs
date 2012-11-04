namespace System
{
    using System.Runtime.CompilerServices;

    [NonScriptable, AttributeUsage(AttributeTargets.Enum, Inherited=false, AllowMultiple=false), Extended]
    public sealed class FlagsAttribute : Attribute
    {
    }
}

