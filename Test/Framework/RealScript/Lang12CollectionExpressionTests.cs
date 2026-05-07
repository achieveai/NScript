//-----------------------------------------------------------------------
// <copyright file="Lang12CollectionExpressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixtures for C# 12 collection expressions targeting <c>T[]</c>.
    /// Array targets lower to a JS array through <c>InlineArrayInitialization</c>.
    ///
    /// Out of scope for this slice (tracked under WI #47 Phase F):
    /// - Interface targets (<c>IEnumerable&lt;T&gt;</c>, <c>IList&lt;T&gt;</c>,
    ///   <c>IReadOnlyList&lt;T&gt;</c>, <c>ICollection&lt;T&gt;</c>,
    ///   <c>IReadOnlyCollection&lt;T&gt;</c>) — these depend on Roslyn finding
    ///   <c>System.Collections.Generic.List&lt;T&gt;..ctor()</c> as a well-known
    ///   member, which NScript's <c>mscorlib</c> facade does not currently satisfy.
    /// - <c>List&lt;T&gt;</c> targets and <c>[CollectionBuilder]</c>-attributed types.
    /// - Spread elements (<c>..source</c>).
    /// - <c>Span&lt;T&gt;</c> / <c>ReadOnlySpan&lt;T&gt;</c> (non-goal — see
    ///   <c>docs/language/limitations.md</c>).
    /// </summary>
    public class Lang12CollectionExpressionTests
    {
        // Empty collection expression target-typed to T[].
        public static void EmptyArray()
        {
            int[] xs = [];
            Console.WriteLine(xs.Length);
        }

        // Single-element array.
        public static void SingleElementArray()
        {
            int[] xs = [42];
            Console.WriteLine(xs.Length);
            Console.WriteLine(xs[0]);
        }

        // Multi-element constant array.
        public static void ConstantArray()
        {
            int[] xs = [1, 2, 3];
            Console.WriteLine(xs.Length);
            Console.WriteLine(xs[2]);
        }

        // Element-typed string array — proves element-type metadata round-trips.
        public static void StringArray()
        {
            string[] names = ["Ada", "Grace", "Margaret"];
            Console.WriteLine(names[0]);
            Console.WriteLine(names.Length);
        }

        // Computed elements: collection expressions accept any expression in
        // element position, not just constants.
        public static void ComputedElements()
        {
            int a = 2;
            int b = 3;
            int[] xs = [a, b, a + b];
            Console.WriteLine(xs.Length);
            Console.WriteLine(xs[2]);
        }

        // Nested collection expression — exercises element-typing for T[][].
        public static void NestedArray()
        {
            int[][] grid = [[1, 2], [3, 4]];
            Console.WriteLine(grid.Length);
            Console.WriteLine(grid[1][0]);
        }
    }
}
