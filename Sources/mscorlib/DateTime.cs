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

        public extern DateTime(DateTime dateTime);

        public extern DateTime(int year, int month, int date);

        public extern DateTime(int year, int month, int date, int hours);

        public extern DateTime(int year, int month, int date, int hours, int minutes);

        public extern DateTime(int year, int month, int date, int hours, int minutes, int seconds);

        public extern DateTime(int year, int month, int date, int hours, int minutes, int seconds, int milliseconds);

        public DateTime Date
        { get { return new DateTime(this.GetFullYear(), this.GetMonth(), this.GetDate()); } }

        public extern string Format(string format);

        public extern int ValueOf();

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

        [Script(@"return new Date(value);")]
        public extern static DateTime Parse(string value);

        [Script("return this.getTimezoneOffset() * 60000;")]
        public extern int UtcOffset();

        public DateTime ToUTC()
        { return this + this.UtcOffset(); }

        public DateTime ToLocal()
        { return this - this.UtcOffset(); }

        [ScriptName("now")]
        public extern static long NowTicks();

        /// <summary>
        /// Sets a date.
        /// </summary>
        /// <param name="date"> The date. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetDate(int date);

        /// <summary>
        /// Sets full year.
        /// </summary>
        /// <param name="year"> The year. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetFullYear(int year);

        /// <summary>
        /// Sets the hours.
        /// </summary>
        /// <param name="hours"> The hours. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetHours(int hours);

        /// <summary>
        /// Sets the milliseconds.
        /// </summary>
        /// <param name="milliseconds"> The milliseconds. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetMilliseconds(int milliseconds);

        /// <summary>
        /// Sets the minutes.
        /// </summary>
        /// <param name="minutes"> The minutes. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetMinutes(int minutes);

        /// <summary>
        /// Sets a month.
        /// </summary>
        /// <param name="month"> The month. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetMonth(int month);

        /// <summary>
        /// Sets the seconds.
        /// </summary>
        /// <param name="seconds"> The seconds. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetSeconds(int seconds);

        /// <summary>
        /// Sets a time.
        /// </summary>
        /// <param name="milliseconds"> The milliseconds. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetTime(int milliseconds);

        /// <summary>
        /// Sets UTC date.
        /// </summary>
        /// <param name="date"> The date. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCDate(int date);

        /// <summary>
        /// Sets UTC full year.
        /// </summary>
        /// <param name="year"> The year. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCFullYear(int year);

        /// <summary>
        /// Sets UTC hours.
        /// </summary>
        /// <param name="hours"> The hours. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCHours(int hours);

        /// <summary>
        /// Sets UTC milliseconds.
        /// </summary>
        /// <param name="milliseconds"> The milliseconds. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCMilliseconds(int milliseconds);

        /// <summary>
        /// Sets UTC minutes.
        /// </summary>
        /// <param name="minutes"> The minutes. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCMinutes(int minutes);

        /// <summary>
        /// Sets UTC month.
        /// </summary>
        /// <param name="month"> The month. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCMonth(int month);

        /// <summary>
        /// Sets UTC seconds.
        /// </summary>
        /// <param name="seconds"> The seconds. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetUTCSeconds(int seconds);

        /// <summary>
        /// Sets a year.
        /// </summary>
        /// <param name="year"> The year. </param>
        /// <returns>
        /// Ticks for updated DateTime.
        /// </returns>
        public extern int SetYear(int year);

        /// <summary>
        /// Converts this object to a date string.
        /// (day MMM dd YYYY) format.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public extern string ToDateString();

        /// <summary>
        /// Converts this object to an ISO string.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public extern string ToISOString();

        /// <summary>
        /// Converts this object to a locale date string.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public extern string ToLocaleDateString();

        public extern string ToLocaleTimeString();

        /// <summary>
        /// Converts this object to a time string.
        /// (hh:mm:ss GMT-tttt (TMZ))
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public extern string ToTimeString();

        public extern string ToGMTString();

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

        public DateTime AddDays(int days)
        {
            return this + days * 86400000;
        }

        public DateTime AddMonths(int months)
        {
            DateTime rv = new DateTime(this);
            rv.SetMonth(this.GetMonth() + months);
            return rv;
        }

        public DateTime AddYears(int years)
        {
            DateTime rv = new DateTime(this);
            rv.SetFullYear(this.GetFullYear() + years);
            return rv;
        }

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