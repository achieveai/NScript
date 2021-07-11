namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public static class Math
    {
        [PreserveCase]
        public static double E;
        [PreserveCase]
        public static double LN10;
        [PreserveCase]
        public static double LN2;
        [PreserveCase]
        public static double LOG10E;
        [PreserveCase]
        public static double LOG2E;
        [PreserveCase]
        public static double PI;
        [PreserveCase]
        public static double SQRT1_2;
        [PreserveCase]
        public static double SQRT2;

        public extern static double Abs(double n);
        public extern static float Abs(float n);
        public extern static int Abs(int n);
        public extern static long Abs(long n);
        public extern static short Abs(short n);

        public extern static double Acos(double n);
        public extern static float Acos(float n);

        public extern static double Asin(double n);
        public extern static float Asin(float n);

        public extern static double Atan(double n);
        public extern static float Atan(float n);

        public extern static double Atan2(double x, double y);
        public extern static float Atan2(float x, float y);

        public extern static double Ceil(double n);
        public extern static float Ceil(float n);

        public extern static double Cos(double n);
        public extern static float Cos(float n);

        public extern static double Exp(double n);
        public extern static float Exp(float n);

        public extern static double Floor(double n);
        public extern static float Floor(float n);

        public extern static double Log(double n);
        public extern static float Log(float n);

        public extern static double Max(double num1, double num2);
        public extern static float Max(float num1, float num2);
        public extern static byte Max(byte num1, byte num2);
        public extern static sbyte Max(sbyte num1, sbyte num2);
        public extern static short Max(short num1, short num2);
        public extern static ushort Max(ushort num1, ushort num2);
        public extern static int Max(int num1, int num2);
        public extern static uint Max(uint num1, uint num2);
        public extern static long Max(long num1, long num2);
        public extern static ulong Max(ulong num1, ulong num2);

        public extern static double Min(double num1, double num2);
        public extern static float Min(float num1, float num2);
        public extern static byte Min(byte num1, byte num2);
        public extern static sbyte Min(sbyte num1, sbyte num2);
        public extern static short Min(short num1, short num2);
        public extern static ushort Min(ushort num1, ushort num2);
        public extern static int Min(int num1, int num2);
        public extern static uint Min(uint num1, uint num2);
        public extern static long Min(long num1, long num2);
        public extern static ulong Min(ulong num1, ulong num2);

        // public extern static double Max(params double[] numbers);
        // public extern static float Max(params float[] numbers);
        // public extern static byte Max(params byte[] numbers);
        // public extern static sbyte Max(params sbyte[] numbers);
        // public extern static short Max(params short[] numbers);
        // public extern static ushort Max(params ushort[] numbers);
        // public extern static int Max(params int[] numbers);
        // public extern static uint Max(params uint[] numbers);
        // public extern static long Max(params long[] numbers);
        // public extern static ulong Max(params ulong[] numbers);

        // public extern static double Min(params double[] numbers);
        // public extern static float Min(params float[] numbers);
        // public extern static byte Min(params byte[] numbers);
        // public extern static sbyte Min(params sbyte[] numbers);
        // public extern static short Min(params short[] numbers);
        // public extern static ushort Min(params ushort[] numbers);
        // public extern static int Min(params int[] numbers);
        // public extern static uint Min(params uint[] numbers);
        // public extern static long Min(params long[] numbers);
        // public extern static ulong Min(params ulong[] numbers);

        public extern static double Pow(double baseNumber, double exponent);
        public extern static float Pow(float baseNumber, float exponent);

        public extern static double Random();
        public static int Random(int number)
        {
            return (int)Math.Floor(Math.Random() * number);
        }

        public extern static double Round(double n);
        public extern static float Round(float n);

        public extern static double Sin(double n);
        public extern static float Sin(float n);

        public extern static double Sqrt(double n);
        public extern static float Sqrt(float n);

        public extern static double Tan(double n);
        public extern static float Tan(float n);

        [ScriptAlias("parseInt")]
        public extern static int Truncate(double n);

        [ScriptAlias("parseInt")]
        public extern static int Truncate(Number n);

        [ScriptAlias("parseInt")]
        public extern static int Truncate(float n);
    }
}

