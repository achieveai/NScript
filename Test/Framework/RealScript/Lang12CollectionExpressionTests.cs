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
    /// Array targets lower to a JS array through <c>InlineArrayInitialization</c>
    /// for literal-only inputs (Phase E) and <c>ArrayWithSpreadsInitialization</c>
    /// for inputs that contain one or more spread elements (Phase F1).
    ///
    /// Phase F1 ships the wire-format and converter infrastructure for spread
    /// elements in <c>T[]</c> targets. Framework E2E fixtures that exercise the
    /// spread shape at the source level (<c>[..src]</c>, <c>[a, ..src, b]</c>) are
    /// deferred to a follow-up because Roslyn's collection-expression lowering
    /// requires <c>System.Collections.Generic.List&lt;T&gt;..ctor()</c> to be
    /// resolvable as a well-known member, which NScript's <c>mscorlib</c> facade
    /// does not currently satisfy. The serialization round-trip tests in
    /// <c>NScript.Csc.Lib.Test/CollectionExpressionRoundTripTests.cs</c> cover both
    /// <c>LiteralElementSer</c> (tag 229) and <c>SpreadElementSer</c> (tag 230)
    /// dispatch paths through the abstract <c>CollectionExpressionElementSer</c>
    /// base.
    ///
    /// Out of scope for this slice (tracked under WI #47 Phase F4):
    /// - Interface targets (<c>IEnumerable&lt;T&gt;</c>, <c>IList&lt;T&gt;</c>,
    ///   <c>IReadOnlyList&lt;T&gt;</c>, <c>ICollection&lt;T&gt;</c>,
    ///   <c>IReadOnlyCollection&lt;T&gt;</c>).
    /// - <c>List&lt;T&gt;</c> targets and <c>[CollectionBuilder]</c>-attributed types.
    /// - Spread sources whose static type is not <c>T[]</c> (<c>List&lt;T&gt;</c>,
    ///   <c>IEnumerable&lt;T&gt;</c>) — Phase F1 only accepts array-typed spread
    ///   sources because JS <c>Array.prototype.concat</c> flattens them natively.
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
