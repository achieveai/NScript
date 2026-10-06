namespace SpreadsheetApp.Formula
{
    using System;

    /// <summary>
    /// Display format for a cell value. Formats are immutable; the sheet keeps
    /// one per column, row, or cell and resolves cell &gt; row &gt; column.
    /// </summary>
    public class CellFormat
    {
        public const int General = 0;
        public const int Number = 1;
        public const int Percent = 2;
        public const int Currency = 3;
        public const int Date = 4;
        public const int Time = 5;
        public const int DateTime = 6;

        public static readonly CellFormat Default = new CellFormat(General, 2, false);

        private int kind;
        private int decimals;
        private bool thousands;

        public CellFormat(int kind, int decimals, bool thousands)
        {
            this.kind = kind;
            this.decimals = decimals;
            this.thousands = thousands;
        }

        public int Kind { get { return this.kind; } }
        public int Decimals { get { return this.decimals; } }
        public bool Thousands { get { return this.thousands; } }

        public string Name
        {
            get
            {
                switch (this.kind)
                {
                    case Number: return "Number";
                    case Percent: return "Percent";
                    case Currency: return "Currency";
                    case Date: return "Date";
                    case Time: return "Time";
                    case DateTime: return "Date time";
                }
                return "General";
            }
        }

        /// <summary>Parses the format name used by the toolbar ("number:2,thousands", "date").</summary>
        public static CellFormat FromName(string name)
        {
            string n = name.ToLowerCase();
            if (n == "percent") return new CellFormat(Percent, 1, false);
            if (n == "currency") return new CellFormat(Currency, 2, true);
            if (n == "date") return new CellFormat(Date, 0, false);
            if (n == "time") return new CellFormat(Time, 0, false);
            if (n == "datetime") return new CellFormat(DateTime, 0, false);
            if (n == "integer") return new CellFormat(Number, 0, true);
            if (n == "number") return new CellFormat(Number, 2, true);
            return Default;
        }

        public string Format(CellValue value)
        {
            if (value.IsEmpty) return "";
            if (value.IsError) return value.ErrorCode;
            if (value.IsBool) return value.BoolValue ? "TRUE" : "FALSE";
            if (value.IsText) return value.TextValue;

            double n = value.NumberValue;
            switch (this.kind)
            {
                case Number: return Group(n.ToFixed(this.decimals), this.thousands);
                case Percent: return Group((n * 100).ToFixed(this.decimals), this.thousands) + "%";
                case Currency: return (n < 0 ? "-$" : "$") + Group(Math.Abs(n).ToFixed(this.decimals), this.thousands);
                case Date: return FormatDate(n, true, false);
                case Time: return FormatDate(n, false, true);
                case DateTime: return FormatDate(n, true, true);
            }
            return GeneralText(n);
        }

        /// <summary>General: up to 10 significant decimals, trailing zeros trimmed.</summary>
        public static string GeneralText(double n)
        {
            if (n == Math.Floor(n) && Math.Abs(n) < 1e15) return n.ToFixed(0);
            string s = n.ToFixed(10);
            int end = s.Length;
            while (end > 0 && s.Substring(end - 1, 1) == "0") end--;
            if (end > 0 && s.Substring(end - 1, 1) == ".") end--;
            return s.Substring(0, end);
        }

        /// <summary>Inserts thousands separators into a fixed-point string.</summary>
        public static string Group(string fixedText, bool thousands)
        {
            if (!thousands) return fixedText;
            string sign = "";
            string s = fixedText;
            if (s.StartsWith("-")) { sign = "-"; s = s.Substring(1); }
            int dot = s.IndexOf(".");
            string whole = dot < 0 ? s : s.Substring(0, dot);
            string frac = dot < 0 ? "" : s.Substring(dot);
            string grouped = "";
            int count = 0;
            for (int i = whole.Length - 1; i >= 0; i--)
            {
                if (count == 3) { grouped = "," + grouped; count = 0; }
                grouped = whole.Substring(i, 1) + grouped;
                count++;
            }
            return sign + grouped + frac;
        }

        private static string FormatDate(double millis, bool date, bool time)
        {
            DateTime d = FromMillis(millis);
            string s = "";
            if (date)
                s = d.GetFullYear().ToString() + "-" + Pad2(d.GetMonth() + 1) + "-" + Pad2(d.GetDate());
            if (time)
                s = s + (date ? " " : "") + Pad2(d.GetHours()) + ":" + Pad2(d.GetMinutes());
            return s;
        }

        /// <summary>Int casts truncate to 32 bits in NScript, so build the date in JS.</summary>
        [System.Runtime.CompilerServices.Script("return new Date(ms);")]
        private static extern DateTime FromMillis(double ms);

        private static string Pad2(int n)
        {
            return n < 10 ? "0" + n.ToString() : n.ToString();
        }
    }
}
