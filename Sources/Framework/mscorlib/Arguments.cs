namespace System
{
    using System.Runtime.CompilerServices;

    [ScriptName("arguments"), IgnoreNamespace, Extended]
    public static class Arguments
    {
        public extern static object GetArgument(int index);

        [ScriptAlias("Array.toArray")]
        public extern static Array ToArray();

        public extern static int Length
        { get; }
    }
}

