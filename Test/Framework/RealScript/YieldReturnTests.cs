//-----------------------------------------------------------------------
// <copyright file="YieldReturnTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections.Generic;
    using System.Collections;

    /// <summary>
    /// Definition for YieldReturnTests
    /// </summary>
    public static class YieldReturnTests
    {
        public static void Main()
        {
            var ator = F();
            while (ator.MoveNext())
            {
                Console.WriteLine(ator.Current);
            }

            foreach (var el in G())
            {
                Console.WriteLine(el);
            }

            foreach(var el in I())
            {
                Console.WriteLine(el);
            }

            foreach(var el in H<string>("first", "second", "third"))
            {
                Console.WriteLine(el);
            }

            var x = 1112;
        }

        public static IEnumerator F()
        {
            yield return 12;
            yield return 90;
        }

        public static IEnumerable G()
        {
            yield return "asdf";
            yield return "iopp";
        }

        public static IEnumerable<string> I()
        {
            yield return "789";
            yield return "0123";
        }

        public static IEnumerable<T> H<T>(params T[] args)
        {
            foreach(var arg in args)
            {
                yield return arg;
            }
        }

        public static IEnumerator<string> GetEnumeratorNestedForeach(System.Collections.Generic.IList<string> strList)
        {
            foreach (var kv in strList)
                yield return kv;
        }

        public static IEnumerator<string> GetEnumeratorNestedFor(System.Collections.Generic.IList<string> strList)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    yield return strList[i];
                }
            }
        }
    }
}