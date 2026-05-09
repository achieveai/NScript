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
    /// Compile-only fixtures for C# 8 indices and ranges (Phase F6).
    ///
    /// These fixtures exercise the Stage-1 lowering paths added in
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
    /// </summary>
    public class Lang8IndexRangeTests
    {
        // From-end index against a T[] receiver — lowers to
        // `xs[idx.GetOffset(xs.Length)]`.
        public static void IndexFromEndArray()
        {
            int[] xs = new int[] { 10, 20, 30, 40 };
            int last = xs[^1];
            Console.WriteLine(last);
        }

        // Explicit System.Index from-start against a T[] receiver — proves
        // the implicit-indexer dispatch handles the non-from-end case too.
        public static void IndexFromStartArray()
        {
            int[] xs = new int[] { 10, 20, 30, 40 };
            System.Index i = 0;
            int first = xs[i];
            Console.WriteLine(first);
        }

        // Range slice against a T[] receiver — lowers to
        // `RuntimeHelpers.GetSubArray<int>(xs, 1..3)`.
        public static void RangeArraySlice()
        {
            int[] xs = new int[] { 10, 20, 30, 40, 50 };
            int[] mid = xs[1..3];
            Console.WriteLine(mid.Length);
            Console.WriteLine(mid[0]);
            Console.WriteLine(mid[1]);
        }

        // Open-ended ranges — exercises the null LeftOperand / RightOperand
        // branches in VisitRangeExpression that fall back to Index.Start /
        // Index.End boundary properties.
        public static void RangeArrayOpenEnded()
        {
            int[] xs = new int[] { 10, 20, 30, 40, 50 };
            int[] tail = xs[2..];
            int[] head = xs[..2];
            Console.WriteLine(tail.Length);
            Console.WriteLine(tail[0]);
            Console.WriteLine(head.Length);
            Console.WriteLine(head[1]);
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
            Console.WriteLine(last);
        }
    }
}
