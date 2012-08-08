namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [NonScriptable, EditorBrowsable(EditorBrowsableState.Never)]
    public struct IntPtr
    {
        [Script("return i;")]
        public extern static implicit operator Number(IntPtr i);

        [Script("return parseInt(s);")]
        public extern static int Parse(string s);

        [Script("return parseInt(s, radix);")]
        public extern static int Parse(string s, int radix);

        [Script("return left === right;")]
        public static extern bool operator ==(IntPtr left, IntPtr right);

        [Script("return left !== right;")]
        public static extern bool operator !=(IntPtr left, IntPtr right);

        public string Format(string format)
        {
            return this.ToString(10);
        }

        public string LocaleFormat(string format)
        {
            return this.Format(format);
        }

        [Script("return this.toString(radix);")]
        public extern string ToString(int radix);

        public override string ToString()
        {
            return this.ToString(10);
        }

        public override string ToLocaleString()
        {
            return this.ToString();
        }
    }
}