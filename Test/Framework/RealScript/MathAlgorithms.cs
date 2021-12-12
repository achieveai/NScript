//-----------------------------------------------------------------------
// <copyright file="MathAlgorithms.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    public static class Console
    {
        [Script(@"console.info(str);")]
        public static extern void WriteLine(string str);

        [Script(@"console.info(i.toString());")]
        public static extern void WriteLine(int i);

        [Script(@"console.info(l.toString());")]
        public static extern void WriteLine(long l);

        [Script(@"console.info(d.toString());")]
        public static extern void WriteLine(double d);

        [Script(@"console.info(d.toString());")]
        public static extern void WriteLine(float d);

        [Script(@"console.info(o.@{[mscorlib]System.Object::ToString()}());")]
        public static extern void WriteLine(object o);

        [Script("console.setTimeout(action, delay);")]
        public static extern void SetTimeout(Action action, int delay);
    }

    /// <summary>
    /// Definition for MathAlgorithms
    /// </summary>
    public static class FactorialCalculator
    {
        public static void Main()
        {
            Console.WriteLine(FactorialCalculator.Calculate(10).ToString());
        }

        public static int Calculate(int i)
        {
            if (i <= 1)
            {
                return 1;
            }

            return i * FactorialCalculator.Calculate(i - 1);
        }
    }

    public static class GetMin
    {
        public static void RunTest()
        {
            int[] array = new int[]
            {
                10, 11, 0, -1, 29991, 1213
            };

            Console.WriteLine(
                GetMin.Calculate(array).ToString());
        }

        public static int Calculate(int[] array)
        {
            int minValue = Number.MAX_VALUE;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] < minValue)
                {
                    minValue = array[i];
                }
            }

            return minValue;
        }
    }

    public static class QuickSort
    {
        public static void RunTest()
        {
            int[] randomArray = new int[500];
            for (int i = 0; i < randomArray.Length; i++)
            {
                randomArray[i] = Math.Random(10000);
            }
            // Console.WriteLine(QuickSort.Stringify(randomArray));
            QuickSort.QSort(randomArray, 0, randomArray.Length);
            // Console.WriteLine(QuickSort.Stringify(randomArray));
            Console.WriteLine("Verification: " + QuickSort.Verify(randomArray).ToString());
        }

        public static void QSort(int[] array, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            int randomPoint = startIndex + Math.Random(endIndex - startIndex);
            int randomPointValue = array[randomPoint];
            array[randomPoint] = array[endIndex - 1];
            int i = startIndex, j = endIndex - 2;

            while (i < j)
            {
                while (array[i] <= randomPointValue && i < endIndex - 2) i++;
                while (array[j] >= randomPointValue && j > startIndex) j--;

                if (i < j)
                {
                    int tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                }
            }

            if (i < endIndex - 2
                || array[i] > randomPointValue)
            {
                array[endIndex - 1] = array[i];
                array[i] = randomPointValue;
            }
            else
            {
                array[endIndex - 1] = randomPointValue;
                i = endIndex - 1;
            }

            QuickSort.QSort(array, startIndex, i);
            QuickSort.QSort(array, i + 1, endIndex);
        }

        public static string Stringify(int[] array)
        {
            string rv = "[";
            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                {
                    rv = rv + "," + array[i].ToString();
                }
                else
                {
                    rv += array[i].ToString();
                }
            }

            rv += "]";

            return rv;
        }

        public static bool Verify(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] > array[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
