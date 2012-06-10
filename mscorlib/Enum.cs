namespace System
{
    using System.Runtime.CompilerServices;

    public abstract class Enum : ValueType
    {
        protected Enum()
        {
        }

        [Script(@"
            var rv = type.@{[mscrolib]System.Type::lowerStrToValueMap}[s];
            if (rv === undefined) {
                throw ""Can't parse "" + s + "" as enum"";
            }
            return rv;
            ")]
        public extern static Enum Parse(Enum type, string s);

        [Script(@"
            var rv = enumType.@{[mscorlib]System.Type::enumValueToStrMap}[value];
            return typeof rv === 'undefined' ? value.toString() : rv;")]
        public extern static string ToString(Enum enumType, int value);

        [Script(@"
            var enumType = this.@{[mscorlib]System.Object::Constructor};
            var value = this.@{[mscorlib]System.Type::boxedValue};
            var rv = enumType.@{[mscorlib]System.Type::enumValueToStrMap}[value];
            return typeof rv === 'undefined' ? value.toString() : rv;")]
        public extern override string ToString();
    }
}