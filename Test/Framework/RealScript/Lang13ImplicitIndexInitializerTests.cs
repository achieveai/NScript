//-----------------------------------------------------------------------
// <copyright file="Lang13ImplicitIndexInitializerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixture for the C# 13 implicit index access in nested
    /// object initializers.
    ///
    /// C# 13 permits `^N` (the <c>BoundFromEndIndexExpression</c> shape)
    /// inside a nested initializer position — e.g.
    /// `new Buffer { Items = { [^1] = 0 } }` — composing the Phase F6
    /// `Index`/`Range` lowering with the existing element-assignment shape
    /// inside an object initializer. The standalone form `arr[^1] = 0` is
    /// already covered by `Lang8IndexRangeTests.cs`; this fixture exercises
    /// the nested-initializer composition specifically.
    ///
    /// Bound-tree expectation: the inner `[^1] = 0` lowers to a
    /// `BoundObjectInitializerMember`-like element assignment whose index
    /// argument is a `BoundFromEndIndexExpression` against `^1`. The F6
    /// lowering already converts that into the equivalent
    /// `Items[Items.Length - 1] = 0` JS assignment.
    ///
    /// This class lives in its own file (not in `Lang13Features.cs`)
    /// following the `Lang9RecordTests.cs` / `Lang11RequiredTests.cs`
    /// precedent: `Lang13Features.cs` is in the explicit Roslyn-driven
    /// build list in `NScript.Csc.Lib.Test/TestResources.cs`. The nested
    /// initializer shape composed with `^N` has not been audited through
    /// the in-test `BondToAst` deserializer, so we keep it out of that
    /// path conservatively. The MSBuild framework build still globs this
    /// file and exercises it end-to-end through NScript's compiler.
    /// </summary>
    public class Lang13ImplicitIndexInitializerTests
    {
        public class Buffer
        {
            public int[] Items;

            public Buffer()
            {
                this.Items = new int[] { 1, 2, 3 };
            }
        }

        public static void ImplicitIndexInInitializer()
        {
            // C# 13 — `^1` inside a nested object initializer. Compiles
            // only because the implicit-index-on-initializer grammar is
            // permitted under <LangVersion>13</LangVersion>.
            var x = new Buffer { Items = { [^1] = 0 } };
            Console.WriteLine(x.Items[2]);
        }
    }
}
