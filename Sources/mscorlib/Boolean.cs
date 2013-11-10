namespace System
{
    using System.Runtime.CompilerServices;

    [ImportedType, IgnoreNamespace, ScriptName("Boolean")]
    public class BooleanNative
    {
        [Script("return obj;")]
        public static extern implicit operator bool?(BooleanNative obj);

        [Script("return b;")]
        public static extern implicit operator BooleanNative(bool b);

        [Script("return b;")]
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

