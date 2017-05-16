namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public sealed class String
    {
        public const string Empty = "";

        [Implement]
        private static RegularExpression formatHelperRegex;

        [Implement]
        private static RegularExpression trimStartHelperRegex;

        [Implement]
        private static RegularExpression trimEndHelperRegex;

        private static RegularExpression FormatHelperRegex
        {
            get
            {
                if (object.IsNullOrUndefined(String.formatHelperRegex))
                { String.formatHelperRegex =  new RegularExpression(@"(\{[^\}^\{]+\})", "g"); }

                return String.formatHelperRegex;
            }
        }

        private static RegularExpression TrimEndHelperRegex
        {
            get
            {
                if (object.IsNullOrUndefined(String.trimEndHelperRegex))
                {
                    String.trimEndHelperRegex = new RegularExpression(@"^[\s\xA0]+");
                }

                return String.trimEndHelperRegex;
            }
        }

        private static RegularExpression TrimStartHelperRegex
        {
            get
            {
                if (object.IsNullOrUndefined(String.trimStartHelperRegex))
                {
                    String.trimStartHelperRegex = new RegularExpression(@"^[\s\xA0]+");
                }

                return String.trimStartHelperRegex;
            }
        }

        public extern String();

        public extern char CharCodeAt(int index);

        [Script(@"return this[index];")]
        public extern string At(int index);

        [Script(@"
            s1 = s1 || '';
            s2 = s2 || '';
            if (s1 == s2)
                return 0;
            else if (s1 > s2)
                return 1;
            else
                return -1;")]
        public extern static int Compare(string s1, string s2);

        [Script(@"
            if (ignoreCase)
            {
                if (s1) s1 = s1.toUpperCase();
                if (s2) s2 = s2.toUpperCase();
            }

            return @{[mscorlib]System.String::Compare([mscorlib]System.String, [mscorlib]System.String)}(s1, s2);
            ")]
        public extern static int Compare(string s1, string s2, bool ignoreCase);

        [Script(@"return @{[mscorlib]System.String::Compare([mscorlib]System.String, [mscorlib]System.String)}(this, s);")]
        public extern int CompareTo(string s);

        [Script(@"return @{[mscorlib]System.String::Compare([mscorlib]System.String, [mscorlib]System.String, [mscorlib]System.Boolean)}(this, s, ignoreCase);")]
        public extern int CompareTo(string s, bool ignoreCase);

        [Script(@"return strings.@{[mscorlib]System.ArrayG`1::innerArray}.join('');")]
        public extern static string Concat(params string[] strings);

        [Script(@"return s1 + s2;")]
        public extern static string Concat(string s1, string s2);

        [Script(@"return s1 + s2 + s3;")]
        public extern static string Concat(string s1, string s2, string s3);

        [Script(@"return s1 + s2 + s3 + s4;")]
        public extern static string Concat(string s1, string s2, string s3, string s4);

        [Script(@"return s1.@{[mscorlib]System.Object::ToString()}() + s2.@{[mscorlib]System.Object::ToString()}();")]
        public extern static string Concat(object s1, object s2);

        [Script(@"return s1.@{[mscorlib]System.Object::ToString()}() + s2.@{[mscorlib]System.Object::ToString()}() + s3.@{[mscorlib]System.Object::ToString()}();")]
        public extern static string Concat(object s1, object s2, object s3);

        [Script(@"return s1.@{[mscorlib]System.Object::ToString()}() + s2.@{[mscorlib]System.Object::ToString()}() + s3.@{[mscorlib]System.Object::ToString()}() + s4.@{[mscorlib]System.Object::ToString()}();")]
        public extern static string Concat(object s1, object s2, object s3, object s4);

        [Script(@"return strings.@{[mscorlib]System.ArrayG`1::innerArray}.join('');")]
        public extern static string Concat(params object[] strings);

        [MakeStaticUsage]
        [Script(@"
            if (!this.length)
                return this.charCodeAt(this.length-1) == ch;
            return false;
            ")]
        public extern bool EndsWith(char ch);

        [MakeStaticUsage]
        [Script(@"
            if (!suffix.length) {
                return true;
            }
            if (suffix.length > this.length) {
                return false;
            }
            return (this.substr(this.length - suffix.length) == suffix);
            ")]
        public extern bool EndsWith(string suffix);

        [Script(@"return @{[mscorlib]System.String::Compare([mscorlib]System.String, [mscorlib]System.String, [mscorlib]System.Boolean)}(s1, s2, ignoreCase) == 0;")]
        public extern static bool Equals(string s1, string s2, bool ignoreCase);

        [Script(@"
            return format.replace(@{[mscorlib]System.String::get_FormatHelperRegex()}(),
                  function(str, m) {
                      var index = parseInt(m.substr(1));
                      var value = values.@{[mscorlib]System.ArrayG`1::get_Item([mscorlib]System.Int32)}(index);
                      if (!value) {
                          return '';
                      }

                      return value.toString();
                  });
            ")]
        public extern static string Format(string format, params object[] values);
        public static string Format(string format, object value)
        {
            return string.Format(format, new object[] { value });
        }

        public extern static string FromCharCode(char ch);

        [MakeStaticUsage]
        [Script(@"
            return this.indexOf(String.fromCharCode(ch));
            ")]
        public extern int IndexOf(char ch);

        public extern int IndexOf(string subString);

        [MakeStaticUsage]
        [Script(@"
            return this.indexOf(String.fromCharCode(ch), startIndex);
            ")]
        public extern int IndexOf(char ch, int startIndex);

        public extern int IndexOf(string subString, int startIndex);

        [Script(@"return !s;")]
        public extern static bool IsNullOrEmpty(string s);

        [MakeStaticUsage]
        [Script(@"
            return this.lastIndexOf(String.fromCharCode(ch));
            ")]
        public extern int LastIndexOf(char ch);

        public extern int LastIndexOf(string subString);

        [MakeStaticUsage]
        [Script(@"
            return this.lastIndexOf(String.fromCharCode(ch), startIndex);
            ")]
        public extern int LastIndexOf(char ch, int startIndex);

        public extern int LastIndexOf(string subString, int startIndex);

        public extern int LocaleCompare(string string2);

        [IntrinsicOperator]
        public extern static bool operator ==(string s1, string s2);

        [IntrinsicOperator]
        public extern static bool operator !=(string s1, string s2);

        public extern string Replace(string oldText, string replaceText);

        public extern string ReplaceFirst(string oldText, string replaceText);

        public string[] Split(char ch)
        {
            return this.SplitInternal(String.FromCharCode(ch)).GetArray<string>();
        }

        public string[] Split(string str)
        {
            return this.SplitInternal(str).GetArray<string>();
        }

        [ScriptName("split")]
        internal extern NativeArray SplitInternal(string separator);

        public string[] Split(char ch, int limit)
        {
            return this.SplitInternal(String.FromCharCode(ch), limit).GetArray<string>();
        }

        public string[] Split(string str, int limit)
        {
            return this.SplitInternal(str, limit).GetArray<string>();
        }

        [ScriptName("split")]
        internal extern NativeArray SplitInternal(string separator, int limit);

        [Script(@"
            return this.length >= 1 && this.charCodeAt(0) == ch;
            ")]
        public extern bool StartsWith(char ch);

        [Script(@"
            var i;
            if (prefix.length <= this.length) {
                for (i = 0; i < prefix.length; ++i)
                    if (prefix[i] != this[i])
                        return false;
                return true;
            }
            return false;
            ")]
        public extern bool StartsWith(string prefix);

        [ScriptName("substring")]
        public extern string Substring(int startIndex);

        [ScriptName("substr")]
        public extern string Substring(int startIndex, int length);

        public extern string ToLocaleLowerCase();

        public extern string ToLocaleUpperCase();

        public extern string ToLowerCase();

        public extern string ToUpperCase();

        public string Trim()
        {
            return this.TrimStart().TrimEnd();
        }

        [MakeStaticUsage]
        [Script(@"
            return this.trimLeft ? this.trimLeft() : this.replace(@{[mscorlib]System.String::get_TrimEndHelperRegex()}(), '');
            ")]
        public extern string TrimEnd();

        [MakeStaticUsage]
        [Script(@"
            return this.trimRight ? this.trimRight() : this.replace(@{[mscorlib]System.String::get_TrimStartHelperRegex()}(), '');
            ")]
        public extern string TrimStart();

        public char this[int index]
        {
            get
            { return this.CharCodeAt(index); }
        }

        [IntrinsicProperty]
        public extern int Length
        {
            get;
        }
    }
}