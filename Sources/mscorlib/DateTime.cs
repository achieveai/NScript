namespace System
{
    using System.Runtime.CompilerServices;

    [ScriptName("Date"), IgnoreNamespace, Extended]
    public class DateTime
    {
        public static readonly DateTime Empty = new DateTime(0);

        public extern DateTime();

        public extern DateTime(int milliseconds);

        public extern DateTime(string date);

        public extern DateTime(int year, int month, int date);

        public extern DateTime(int year, int month, int date, int hours);

        public extern DateTime(int year, int month, int date, int hours, int minutes);

        public extern DateTime(int year, int month, int date, int hours, int minutes, int seconds);

        public extern DateTime(int year, int month, int date, int hours, int minutes, int seconds, int milliseconds);

        public extern string Format(string format);

        public extern int GetDate();

        public extern int GetDay();

        public extern int GetFullYear();

        public extern int GetHours();

        public extern int GetMilliseconds();

        public extern int GetMinutes();

        public extern int GetMonth();

        public extern int GetSeconds();

        public extern int GetTime();

        public extern int GetTimezoneOffset();

        public extern int GetUTCDate();

        public extern int GetUTCDay();

        public extern int GetUTCFullYear();

        public extern int GetUTCHours();

        public extern int GetUTCMilliseconds();

        public extern int GetUTCMinutes();

        public extern int GetUTCMonth();

        public extern int GetUTCSeconds();

        public extern static bool IsEmpty(DateTime d);

        public extern string LocaleFormat(string format);

        [IntrinsicOperator]
        public extern static bool operator ==(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static bool operator >(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static bool operator >=(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static bool operator !=(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static bool operator <(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static bool operator <=(DateTime a, DateTime b);

        [IntrinsicOperator]
        public extern static long operator -(DateTime a, DateTime b);

        [Script(@"return new Date(a.valueOf() + n);")]
        public extern static DateTime operator +(DateTime a, Number n);

        [Script(@"return new Date(a.valueOf() - n);")]
        public extern static DateTime operator -(DateTime a, Number n);

        [ScriptAlias("Date.parseDate")]
        public extern static DateTime Parse(string value);

        public extern void SetDate(int date);

        public extern void SetFullYear(int year);

        public extern void SetHours(int hours);

        public extern void SetMilliseconds(int milliseconds);

        public extern void SetMinutes(int minutes);

        public extern void SetMonth(int month);

        public extern void SetSeconds(int seconds);

        public extern void SetTime(int milliseconds);

        public extern void SetUTCDate(int date);

        public extern void SetUTCFullYear(int year);

        public extern void SetUTCHours(int hours);

        public extern void SetUTCMilliseconds(int milliseconds);

        public extern void SetUTCMinutes(int minutes);

        public extern void SetUTCMonth(int month);

        public extern void SetUTCSeconds(int seconds);

        public extern void SetYear(int year);

        public extern string ToDateString();

        public extern string ToLocaleDateString();

        public extern string ToLocaleTimeString();

        public extern string ToTimeString();

        public extern string ToUTCString();

        [PreserveCase]
        public extern static int UTC(int year, int month, int day);

        [PreserveCase]
        public extern static int UTC(int year, int month, int day, int hours);

        [PreserveCase]
        public extern static int UTC(int year, int month, int day, int hours, int minutes);

        [PreserveCase]
        public extern static int UTC(int year, int month, int day, int hours, int minutes, int seconds);

        [PreserveCase]
        public extern static int UTC(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds);

        public static DateTime Now
        {
            get
            {
                return new DateTime();
            }
        }

        public static DateTime Today
        {
            get
            {
                var now = new DateTime();
                return new DateTime(now.GetFullYear(), now.GetMonth(), now.GetDate());
            }
        }
    }
}