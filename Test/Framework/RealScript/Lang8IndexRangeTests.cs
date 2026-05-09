//-----------------------------------------------------------------------
// <copyright file="Lang8IndexRangeTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Fixtures for C# 8 indices and ranges (Phase F6).
    ///
    /// Exercise the Stage-1 lowering paths added in
    /// <c>BoundAstToAstBase</c> for <c>BoundFromEndIndexExpression</c>,
    /// <c>BoundRangeExpression</c>, and <c>BoundImplicitIndexerAccess</c>.
    /// The mscorlib facade contributes <c>System.Index</c>,
    /// <c>System.Range</c>, and <c>RuntimeHelpers.GetSubArray&lt;T&gt;</c>.
    ///
    /// Phase F6 ships <c>T[]</c> support for both <c>Index</c> and
    /// <c>Range</c> arguments, plus <c>Index</c> support on receivers that
    /// expose an <c>int</c> indexer (<c>List&lt;T&gt;</c>,
    /// <c>IList&lt;T&gt;</c>, <c>IReadOnlyList&lt;T&gt;</c>). Range slicing
    /// over <c>List&lt;T&gt;</c> and <c>string</c> is deferred — those
    /// receivers throw an actionable <c>NotSupportedException</c> in the
    /// Stage-1 visitor.
    ///
    /// Each fixture asserts expected values via <c>throw new Exception</c>
    /// so the from-end offset arithmetic is verified end-to-end whenever
    /// these are eventually wired into the V8 execution suite (today they
    /// are compile-only via <c>NScript.Csc.Lib.Test</c>).
    /// </summary>
    public class Lang8IndexRangeTests
    {
        private static void AssertEqual(int expected, int actual, string label)
        {
            if (expected != actual)
            {
                throw new Exception(label + ": expected " + expected + ", got " + actual);
            }
        }

        // From-end index against a T[] receiver — lowers to
        // `xs[idx.GetOffset(xs.Length)]`.
        public static void IndexFromEndArray()
        {
            int[] xs = new int[] { 10, 20, 30, 40 };
            int last = xs[^1];
            AssertEqual(40, last, "xs[^1]");
            int penultimate = xs[^2];
            AssertEqual(30, penultimate, "xs[^2]");
            Console.WriteLine(last);
        }

        // Explicit System.Index from-start against a T[] receiver — proves
        // the implicit-indexer dispatch handles the non-from-end case too.
        public static void IndexFromStartArray()
        {
            int[] xs = new int[] { 10, 20, 30, 40 };
            System.Index i = 0;
            int first = xs[i];
            AssertEqual(10, first, "xs[Index 0]");
            Console.WriteLine(first);
        }

        // Range slice against a T[] receiver — lowers to
        // `RuntimeHelpers.GetSubArray<int>(xs, 1..3)`.
        public static void RangeArraySlice()
        {
            int[] xs = new int[] { 10, 20, 30, 40, 50 };
            int[] mid = xs[1..3];
            AssertEqual(2, mid.Length, "xs[1..3].Length");
            AssertEqual(20, mid[0], "xs[1..3][0]");
            AssertEqual(30, mid[1], "xs[1..3][1]");
            Console.WriteLine(mid.Length);
        }

        // Open-ended ranges — exercises the null LeftOperand / RightOperand
        // branches in VisitRangeExpression that fall back to Index.Start /
        // Index.End boundary properties.
        public static void RangeArrayOpenEnded()
        {
            int[] xs = new int[] { 10, 20, 30, 40, 50 };
            int[] tail = xs[2..];
            int[] head = xs[..2];
            int[] all = xs[..];
            AssertEqual(3, tail.Length, "xs[2..].Length");
            AssertEqual(30, tail[0], "xs[2..][0]");
            AssertEqual(2, head.Length, "xs[..2].Length");
            AssertEqual(20, head[1], "xs[..2][1]");
            AssertEqual(5, all.Length, "xs[..].Length");
            AssertEqual(50, all[4], "xs[..][4]");
            Console.WriteLine(tail.Length);
        }

        // From-end index against a List<T> receiver — exercises the
        // ResolveIntIndexer path (List<T> exposes an int indexer).
        public static void IndexOnList()
        {
            List<int> list = new List<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);
            int last = list[^1];
            AssertEqual(30, last, "list[^1]");
            int first = list[^3];
            AssertEqual(10, first, "list[^3]");
            Console.WriteLine(last);
        }
    }
}
