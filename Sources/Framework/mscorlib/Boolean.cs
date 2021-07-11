namespace System
{
    using System.Runtime.CompilerServices;

    [ImportedType, IgnoreNamespace, ScriptName("Boolean")]
    public sealed class BooleanNative
    {
        [IntrinsicOperator]
        public static extern implicit operator bool?(BooleanNative obj);

        [IntrinsicOperator]
        public static extern implicit operator BooleanNative(bool b);

        [IntrinsicOperator]
        public static extern implicit operator BooleanNative(bool? b);
    }

    public struct Boolean
    {
        public static bool Parse(string s)
        {
            if (s == "true")
            {
                return true;
            }
            else if (s == "false")
            {
                return false;
            }
            else
            {
                throw new Exception("Invalid format");
            }
        }

        public override string ToString()
        {
            return this ? "true" : "false";
        }
    }
}

