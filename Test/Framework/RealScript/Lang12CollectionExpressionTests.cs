//-----------------------------------------------------------------------
// <copyright file="Lang12CollectionExpressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixtures for C# 12 collection expressions.
    ///
    /// Phase E (literal-only <c>T[]</c>) and Phase F1 (<c>T[]</c> with spreads
    /// from another <c>T[]</c>) shipped the wire format and converter
    /// infrastructure. Phase F4 extends coverage to <c>List&lt;T&gt;</c>
    /// targets — both literal-only and with spread sources whose static type
    /// is <c>List&lt;T&gt;</c> or <c>T[]</c>. A new Stage-1 lowering branch in
    /// <c>VisitCollectionExpression</c> emits
    /// <c>NewCollectionInitializerExpression</c> (<c>new List&lt;T&gt;()</c>
    /// plus a sequence of <c>Add</c>/<c>AddRange</c> calls) which feeds the
    /// existing <c>InlineCollectionInitializationExpression</c> JST bridge,
    /// so no new ProtoBuf tag is required. Phase F4 also enables
    /// <c>List&lt;T&gt;</c> spread sources into <c>T[]</c> targets via a
    /// synthesised <c>ToArray()</c> bridge so the F1 array-source converter
    /// handles both shapes uniformly.
    ///
    /// The serialization round-trip tests in
    /// <c>NScript.Csc.Lib.Test/CollectionExpressionRoundTripTests.cs</c> cover both
    /// <c>LiteralElementSer</c> (tag 229) and <c>SpreadElementSer</c> (tag 230)
    /// dispatch paths through the abstract <c>CollectionExpressionElementSer</c>
    /// base.
    ///
    /// Out of scope for this slice (deferred to Phase F5):
    /// - The five BCL interface targets — <c>IEnumerable&lt;T&gt;</c>,
    ///   <c>IList&lt;T&gt;</c>, <c>IReadOnlyList&lt;T&gt;</c>,
    ///   <c>ICollection&lt;T&gt;</c>, <c>IReadOnlyCollection&lt;T&gt;</c>.
    ///   Roslyn's binder requires a long tail of well-known-member
    ///   signatures on these interfaces (<c>RemoveAt</c>, <c>Count</c>,
    ///   <c>IsSynchronized</c>, <c>SyncRoot</c>, etc.) before it will produce
    ///   a <c>BoundCollectionExpression</c>; supplying those facade members
    ///   ripples through every implementer (List, Dictionary, ArrayG,
    ///   ReadOnlyCollection, NumberDictionary, StringDictionary, ExpandoObject)
    ///   and is large enough to track separately.
    /// - <c>[CollectionBuilder]</c>-attributed user types.
    /// - <c>IEnumerable&lt;T&gt;</c> spread sources into a <c>T[]</c> target
    ///   (needs an iterator-based emit path; non-trivial without a
    ///   <c>System.Linq.Enumerable.ToArray</c> helper in NScript's mscorlib facade).
    /// - <c>Span&lt;T&gt;</c> / <c>ReadOnlySpan&lt;T&gt;</c> (Non-Goal — see
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

        // -----------------------------------------------------------------
        // Phase F4 — List<T> direct targets and array+List spread bridges.
        // BCL interface targets are deferred to Phase F5 (see class summary).
        // -----------------------------------------------------------------

        // Empty collection expression target-typed to List<T>.
        public static void EmptyList()
        {
            System.Collections.Generic.List<int> xs = [];
            Console.WriteLine(xs.Count);
        }

        // Literal-only List<T> target.
        public static void ListTarget()
        {
            System.Collections.Generic.List<int> xs = [1, 2, 3];
            Console.WriteLine(xs.Count);
            Console.WriteLine(xs[2]);
        }

        // List<T> target with a spread source whose static type is List<T>.
        // Lowers to `new List<int>(); AddRange(src);`.
        public static void ListTargetWithSpreadFromList()
        {
            System.Collections.Generic.List<int> src = [10, 20, 30];
            System.Collections.Generic.List<int> dst = [..src];
            Console.WriteLine(dst.Count);
            Console.WriteLine(dst[1]);
        }

        // List<T> target with a spread source whose static type is T[].
        // Lowers to `new List<int>(); AddRange(arr);` — exact-match overload
        // resolution picks `AddRange(T[])` over `AddRange(IEnumerable<T>)`.
        public static void ListTargetWithSpreadFromArray()
        {
            int[] src = [4, 5, 6];
            System.Collections.Generic.List<int> dst = [..src, 7];
            Console.WriteLine(dst.Count);
            Console.WriteLine(dst[3]);
        }

        // T[] target with a List<T> spread source — F4 normalises this through
        // a synthesised `List<T>.ToArray()` so the F1 array-source converter
        // handles both shapes uniformly.
        public static void ArrayTargetWithSpreadFromList()
        {
            System.Collections.Generic.List<int> src = [1, 2, 3];
            int[] dst = [0, ..src, 99];
            Console.WriteLine(dst.Length);
            Console.WriteLine(dst[4]);
        }
    }
}
