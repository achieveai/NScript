//-----------------------------------------------------------------------
// <copyright file="FuncRegressions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for FuncRegressions
    /// </summary>
    public static class FuncRegressions
    {
        public static string IfElseInForBlock(string[] array)
        {
            string rv = "[";
            for (int i = 0; i < array.Length; ++i)
            {
                if (i > 0)
                {
                    rv = rv + "," + array[i];
                }
                else
                {
                    rv = rv + array[i];
                }
            }

            return rv + "]";
        }

        public static void NestedWhileLoops(int[] array)
        {
            int randomValue = 11;
            int i = 0, j = 10;

            while (i < j)
            {
                while (array[i] <= randomValue) ++i;
                while (array[j] >= randomValue) --j;

                if (i < j)
                {
                    int tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                }
            }

            array[11] = array[i];
        }

        public static void NestedWhileLoops2(int[] array)
        {
            int randomValue = 11;
            int i = 0, j = 10;

            while (i < j)
            {
                while (array[i] <= randomValue && i < 12) ++i;
                while (array[j] >= randomValue && j >= i && j > 0) --j;

                if (i < j)
                {
                    int tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                }
            }

            array[11] = array[i];
        }

        public static void TwoConsicutivePreIncrements()
        {
            var i = 0;
            TestArithmetics.Unary(++i, ++i);
            TestArithmetics.Unary(i++, i++);
        }
    }
}
