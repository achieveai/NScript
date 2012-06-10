namespace System
{
    using System.Runtime.CompilerServices;

    [ScriptName("arguments"), IgnoreNamespace, Imported]
    public static class Arguments
    {
        public static object GetArgument(int index)
        {
            return null;
        }

        [ScriptAlias("Array.toArray")]
        public static Array ToArray()
        {
            return null;
        }

        [IntrinsicProperty]
        public static int Length
        {
            get
            {
                return 0;
            }
        }
    }
}

