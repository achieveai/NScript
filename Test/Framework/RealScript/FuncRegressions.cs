//-----------------------------------------------------------------------
// <copyright file="FuncRegressions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections;

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

        public static bool TestPassByRefAssignment(List<int> list, ref int value)
        {
            if (list.Count > 10)
            {
                value = list[10];
                return true;
            }

            value = default(int);
            return false;
        }

        public static string EscapesInString()
        {
            return @"\+.?/\r\n" + "\\+?.\r\n\"";
        }

        public static void CollapsingForInIfRegression(string[] headerPair)
        {
            var request = new MyDictionary<string, string>();
            if (headerPair != null)
            {
                for (int iHeader = 0; iHeader < headerPair.Length - 1; iHeader+=2)
                {
                    request.Add(headerPair[iHeader], headerPair[iHeader + 1]);
                }
            }
        }

        public static string RegressionWithLastIndexOfString(string str)
        {
            if (str.LastIndexOf('/') > 0)
            {
                return str + "10";
            }

            return str;
        }

        public static void OddNativeArrayPushBehavior(Foo fooObject)
        {
            var items = new NativeArray(1);
            items.Push<Foo>(fooObject);
        }

        public static string DeclarationWithOut(
            System.Collections.Generic.Dictionary<string, Func<string, string>> dict)
        {
            Func<string, string> func;
            if (dict.TryGetValue("foo", out func))
            {
                return func("foo");
            }

            return null;
        }
    }
}
