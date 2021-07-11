//-----------------------------------------------------------------------
// <copyright file="WhileLoopBlocks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Definition for WhileLoopBlocks
    /// </summary>
    public class WhileLoopBlocks
    {
        public void DoWhileLoop(int i)
        {
            do
            {
                TmpC.Foo("{0}", i);
                --i;
            } while (i > 10);
        }

        public void WhileLoop(int i)
        {
            while(i > 10)
            {
                TmpC.Foo("{0}", i);
                --i;
            }
        }

        public int DoWhilePaddedLoop(int i)
        {
            i %= 100;

            do
            {
                TmpC.Foo("{0}", i);
                --i;
            } while (i > 10);

            return i;
        }

        public int WhilePaddedLoop(int i)
        {
            i %= 100;

            while(i > 10)
            {
                TmpC.Foo("{0}", i);
                --i;
            }

            return i;
        }

        public void WhileLoopWithBreak(int i)
        {
            while (i > 100)
            {
                if (i % 22 == 0)
                {
                    return;
                }
                
                if (i % 13 == 0)
                {
                    break;
                }

                Class1.GetMoreStatic(i);
            }

            Class1.GetMoreStatic(i + 19);
        }

        public void WhileLoopWithContinue(int i)
        {
            while (i > 100)
            {
                if (i % 22 == 0)
                {
                    return;
                }
                
                if (i % 13 == 0)
                {
                    continue;
                }

                Class1.GetMoreStatic(i);
            }

            Class1.GetMoreStatic(i + 19);
        }

        private bool WhileInfiniteLoop(int candidate, int[] primes)
        {
            var i = 1;
            while (true)
            {
                var p = primes[i];
                if (candidate < p * p)
                    return true;
                TestWhileToForConfusionRegression(i);
            }
        }

        static void TestWhileToForConfusionRegression(int arg)
        {
            Console.WriteLine("Testing while...");
            var i = 0;
            while (i < arg)
            {
                Console.WriteLine(i++);
            }
            Console.WriteLine("after");
        }

        static void TestWhileToForConfusionRegression2(int[] array, int startIndex, int endIndex)
        {
            int randomPoint = startIndex + Math.Random(endIndex - startIndex);
            int randomPointValue = array[randomPoint];
            int i = startIndex, j = endIndex - 2;

            while (i < j)
            {
                while (array[i] <= randomPointValue && i < endIndex - 2) ++i;
                while (array[j] >= randomPointValue && j > startIndex) --j;

                if (i < j)
                {
                    int tmp = array[i];
                    array[i] = array[j];
                    array[j] = tmp;
                }
            }

            QuickSort.QSort(array, startIndex, i);
        }
    }
}
