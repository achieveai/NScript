//-----------------------------------------------------------------------
// <copyright file="YieldReturnTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System.Collections.Generic;
    using System.Collections;
    using System;

    /// <summary>
    /// Definition for YieldReturnTests
    /// </summary>
    public static class YieldReturnTests
    {
        public static void Main()
        {
            var ator = F(90);
            while (ator.MoveNext())
            {
                Console.WriteLine(ator.Current);
            }

            ator = F(78);
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

            foreach (var el in K<int, string>(1, "2", _ => _.ToString()))
            {
                Console.WriteLine(el);
            }

            foreach(var el in J<int>(1, 2))
            {
                Console.WriteLine(el);
            }

            var x = 1112;
        }

        public static IEnumerator F(int x)
        {
            yield return 12;
            if (x == 78) yield break;
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

        public static IEnumerable<T> K<U, T>(U arg1, T arg2, Func<U, T> transform)
        {
            yield return arg2;
            var res = transform(arg1);
            yield return res;

            if (res.AreEqual(arg2))
            { yield break; }

            yield return arg2;
        }

        public static IEnumerable<string> J<T>(T arg1, T arg2)
        {
            yield return "asdf";
            yield return "jkl;";
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