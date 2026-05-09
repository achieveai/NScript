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
    /// This class is intentionally excluded from the explicit Roslyn-driven
    /// build list in <c>Test/Compiler/NScript.Csc.Lib.Test/TestResources.cs</c>
    /// (mirrors the <c>Lang11RequiredTests.cs</c> precedent): the
    /// <c>NewCollectionInitializerExpression</c> /
    /// <c>CollectionExpressionSer</c> / <c>CollectionExpressionElementSer</c>
    /// shapes synthesised by Roslyn for collection expressions are not
    /// currently round-trippable through the in-test <c>BondToAst</c>
    /// deserializer. The MSBuild Framework build still globs this file via
    /// <c>Sources/Framework/Directory.Build.props</c>, so the fixtures are
    /// exercised end-to-end by the framework prebuild and downstream
    /// integration tests.
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
    /// The <c>LiteralElementSer</c> (tag 229) and <c>SpreadElementSer</c>
    /// (tag 230) dispatch paths under the abstract
    /// <c>CollectionExpressionElementSer</c> base were introduced in PR #59
    /// (Phase F1). They are exercised end-to-end by the fixtures in this
    /// file via the MSBuild Framework build; no separate Csc.Lib unit-test
    /// file targets the round-trip directly.
    ///
    /// Phase F5 extends coverage to the five list-shaped BCL interface
    /// targets (<c>IEnumerable&lt;T&gt;</c>, <c>IList&lt;T&gt;</c>,
    /// <c>ICollection&lt;T&gt;</c>, <c>IReadOnlyList&lt;T&gt;</c>,
    /// <c>IReadOnlyCollection&lt;T&gt;</c>) and to <c>IEnumerable&lt;T&gt;</c>
    /// spread sources for both <c>T[]</c> and <c>List&lt;T&gt;</c> targets.
    /// All five interfaces collapse to the same Phase F4 lowering — the
    /// element type is recovered from the interface's single type argument
    /// and a constructed <c>List&lt;T&gt;</c> is materialised. The
    /// <c>IEnumerable&lt;T&gt;</c> spread bridge for <c>T[]</c> targets
    /// synthesises <c>new List&lt;T&gt;(); AddRange(src); ToArray()</c> so
    /// the F1 array-source converter handles the result uniformly.
    ///
    /// Out of scope for this slice (deferred to Phase F6):
    /// - <c>[CollectionBuilder]</c>-attributed user types.
    /// - Index/range residuals on element-position sub-spreads
    ///   (<c>[..src[1..3]]</c>).
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

        // List<T> target with interleaved literal-spread-literal — exercises
        // Add(literal) → AddRange(src) → Add(literal) ordering correctness on
        // the List<T> initialiser path (distinct from the array path which
        // uses Array.concat with bunched literal segments).
        public static void ListTargetMixedLiteralsAndSpread()
        {
            System.Collections.Generic.List<int> src = [10, 20];
            System.Collections.Generic.List<int> dst = [1, ..src, 99];
            Console.WriteLine(dst.Count);
            Console.WriteLine(dst[0]);
            Console.WriteLine(dst[3]);
        }

        // -----------------------------------------------------------------
        // Phase F5 — list-shaped BCL interface targets and IEnumerable<T>
        // spread sources. All five interfaces collapse to the same Phase F4
        // List<T> lowering; the JS runtime carries no "interface type", so
        // the static-type information is preserved at the C# call site only.
        // -----------------------------------------------------------------

        // IEnumerable<T> target — collapses to `new List<int>()` + Add chain.
        public static void IEnumerableTarget()
        {
            System.Collections.Generic.IEnumerable<int> xs = [1, 2, 3];
            int sum = 0;
            foreach (int v in xs)
            {
                sum += v;
            }

            Console.WriteLine(sum);
        }

        // IList<T> target — same lowering as List<T> directly.
        public static void IListTarget()
        {
            System.Collections.Generic.IList<int> xs = [1, 2, 3];
            Console.WriteLine(xs.Count);
            Console.WriteLine(xs[2]);
        }

        // ICollection<T> target — exercises Count via the ICollection<T>
        // interface dispatch.
        public static void ICollectionTarget()
        {
            System.Collections.Generic.ICollection<int> xs = [10, 20, 30];
            Console.WriteLine(xs.Count);
        }

        // IReadOnlyList<T> target — exercises indexer dispatch through
        // the read-only interface.
        public static void IReadOnlyListTarget()
        {
            System.Collections.Generic.IReadOnlyList<int> xs = [7, 8, 9];
            Console.WriteLine(xs.Count);
            Console.WriteLine(xs[1]);
        }

        // IReadOnlyCollection<T> target — exercises Count via the
        // read-only collection interface.
        public static void IReadOnlyCollectionTarget()
        {
            System.Collections.Generic.IReadOnlyCollection<int> xs = [4, 5, 6];
            Console.WriteLine(xs.Count);
        }

        // Interface target with mixed literal + array spread — exercises
        // the same Add(literal) → AddRange(spread) ordering as the
        // direct-List target path.
        public static void IListTargetWithSpread()
        {
            int[] src = [10, 20];
            System.Collections.Generic.IList<int> dst = [1, ..src, 99];
            Console.WriteLine(dst.Count);
            Console.WriteLine(dst[3]);
        }

        // IEnumerable<T> spread source into a T[] target — bridged via
        // `new List<int>(); AddRange(src); ToArray()` so the F1
        // array-source converter handles the result uniformly.
        public static void SpreadFromEnumerableIntoArray()
        {
            System.Collections.Generic.IEnumerable<int> src = ProduceEnumerable();
            int[] dst = [0, ..src, 99];
            Console.WriteLine(dst.Length);
            Console.WriteLine(dst[1]);
            Console.WriteLine(dst[dst.Length - 1]);
        }

        // IEnumerable<T> spread source into a List<T> target — exercises
        // the AddRange(IEnumerable<T>) overload resolution path.
        public static void SpreadFromEnumerableIntoList()
        {
            System.Collections.Generic.IEnumerable<int> src = ProduceEnumerable();
            System.Collections.Generic.List<int> dst = [..src, 42];
            Console.WriteLine(dst.Count);
            Console.WriteLine(dst[dst.Count - 1]);
        }

        // Helper that returns an IEnumerable<int> backed by a List<int>.
        // Inlined into the spread-source fixtures so the static type at the
        // spread element is `IEnumerable<int>` (not `List<int>` / `int[]`),
        // forcing the Phase F5 dispatch arm.
        private static System.Collections.Generic.IEnumerable<int> ProduceEnumerable()
        {
            System.Collections.Generic.List<int> backing = [1, 2, 3];
            return backing;
        }
    }
}
