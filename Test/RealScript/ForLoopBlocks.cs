//-----------------------------------------------------------------------
// <copyright file="ForLoopBlocks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections;

    /// <summary>
    /// Definition for ForLoopBlocks
    /// </summary>
    public class ForLoopBlocks
    {
        public static void ForLoopBasic(int i)
        {
            for (int j = 0; j < i; ++j)
            {
                Class1.staticBar = Class1.GetMoreStatic(i);
            }
        }

        public static void ForLoopWithContinue(int num)
        {
            for (int j = 0; j < num; ++j)
            {
                Class1.staticBar = Class1.GetMoreStatic(num);

                if (num == 100)
                {
                    num /= 2;
                    continue;
                }

                num += Class1.staticBar;
            }
        }

        public static void ForLoopWithBreak(int num)
        {
            for (int j = 0; j < num; ++j)
            {
                Class1.staticBar = Class1.GetMoreStatic(num);

                if (num == 100)
                {
                    num /= 2;
                    break;
                }

                num += Class1.staticBar;
            }
        }

        public static void ForLoopPadded(int i)
        {
            TmpC.Foo("bool");

            for (int j = 0; j < i; ++j)
            {
                Class1.staticBar = Class1.GetMoreStatic(i);
            }

            TmpC.Foo("after");
        }

        public static void ForEachLoop(Dictionary dict)
        {
            foreach (DictionaryEntry kvPair in dict)
            {
                string.Format("k:{0}, v:{1}", kvPair.Key, kvPair.Value);
            }
        }

        public static void ForEachLoopArr(Class1[] arr)
        {
            foreach (Class1 kvPair in arr)
            {
                string.Format("k:{0}, v:{1}", kvPair.instanceFoo, kvPair.Func1(10, 11));
            }
        }
    }
}