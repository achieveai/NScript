//-----------------------------------------------------------------------
// <copyright file="Interlocked.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Threading
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Interlocked
    /// </summary>
    public class Interlocked
    {
        public static long Add(ref long num, long add)
        {
            long rv = num;
            num += add;
            return rv;
        }

        public static int Add(ref int num, int add)
        {
            int rv = num;
            num += add;
            return rv;
        }

        public static long Decrement(ref long num)
        {
            long rv = num;
            num -= 1;
            return rv;
        }

        public static int Decrement(ref int num)
        {
            int rv = num;
            num -= 1;
            return rv;
        }

        public static long Increment(ref long num)
        {
            long rv = num;
            num += 1;
            return rv;
        }

        public static int Increment(ref int num)
        {
            int rv = num;
            num += 1;
            return rv;
        }

        public static int CompareExchange(ref int num, int value, int comparand)
        {
            int rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        public static long CompareExchange(ref long num, long value, long comparand)
        {
            long rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        public static float CompareExchange(ref float num, float value, float comparand)
        {
            float rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        public static double CompareExchange(ref double num, double value, double comparand)
        {
            double rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        public static IntPtr CompareExchange(ref IntPtr num, IntPtr value, IntPtr comparand)
        {
            IntPtr rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        public static object CompareExchange(ref object num, object value, object comparand)

        {
            object rv = num;
            if (num == comparand)
            {
                num = value;
            }

            return rv;
        }

        [IgnoreGenericArguments]
        public static T CompareExchange<T>(ref T num, T value, T comparand)
        {
            T rv = num;
            if (object.Equals(num, comparand))
            {
                num = value;
            }

            return rv;
        }

        public static int Exchange(ref int num, int value)
        {
            int rv = num;
            num = value;

            return rv;
        }

        public static long Exchange(ref long num, long value)
        {
            long rv = num;
            num = value;

            return rv;
        }

        public static float Exchange(ref float num, float value)
        {
            float rv = num;
            num = value;

            return rv;
        }

        public static double Exchange(ref double num, double value)
        {
            double rv = num;
            num = value;

            return rv;
        }

        public static IntPtr Exchange(ref IntPtr num, IntPtr value)
        {
            IntPtr rv = num;
            num = value;

            return rv;
        }

        public static object Exchange(ref object num, object value)
        {
            object rv = num;
            num = value;

            return rv;
        }

        [IgnoreGenericArguments]
        public static T Exchange<T>(ref T num, T value)
        {
            T rv = num;
            num = value;

            return rv;
        }
    }
}
