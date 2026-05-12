//-----------------------------------------------------------------------
// <copyright file="Lang13Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

/// <summary>
/// Compile-only fixtures for transparent C# 13 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
///
/// NOTE: <c>params</c> collections (C# 13) are tracked under Phase E with
/// the broader collection-expression work. <c>System.Threading.Lock</c> and
/// the new <c>field</c> keyword in property accessors are explicit
/// non-goals (see <c>docs/language/csharp9-13-status.md</c>).
/// </summary>
public class Lang13Features
{
    // C# 13 — escape sequence \e for ESC (0x1B). Roslyn folds it into a
    // constant char or string at lex time; bound tree sees a literal.
    public static void EscapeSequenceEsc()
    {
        char esc = '\e';
        Console.WriteLine(((int)esc).ToString());

        string ansiClear = "\e[2J";
        Console.WriteLine(ansiClear.Length.ToString());
    }

    // C# 13 — method group overload pruning. C# 10 introduced single-
    // overload natural types (covered in Lang10Features); C# 13 extends
    // overload resolution to **prune** candidates whose generic
    // type-parameter constraints fail at this scope, leaving a single
    // applicable candidate. Pre-C# 13 the same code reports an ambiguity
    // error because the failing generic candidate is still in play during
    // method-group conversion (and equivalently during natural-type
    // inference for `var`).
    //
    // Here `Take<T>(T)` requires `T : class`; for `int` the constraint
    // fails, so under C# 13 only the non-generic `Take(int)` survives
    // pruning and the method-group conversion to `Action<int>` succeeds.
    public static void MethodGroupOverloadPruning()
    {
        Action<int> act = Take;
        act(7);
    }

    private static void Take<T>(T x) where T : class
    {
        Console.WriteLine("class:" + (x == null ? "null" : x.ToString()));
    }

    private static void Take(int x) => Console.WriteLine("int:" + x.ToString());

    // C# 13 — params collections with a `List<T>` parameter. Roslyn synthesises
    // a List<T>-shaped collection expression at the call site, which routes
    // through the same Phase F4 lowering that handles explicit
    // `List<T> xs = [...]` target-typing.
    //
    // Phase F5 extends coverage to the five list-shaped BCL interfaces
    // (IEnumerable<T>, IList<T>, ICollection<T>, IReadOnlyList<T>,
    // IReadOnlyCollection<T>) as `params` shapes — Roslyn target-types the
    // synthesised call-site collection expression to the interface, which
    // the F5 dispatch arm collapses to `new List<int>()` + Add chain.
    public static void ParamsCollections()
    {
        Sum(1, 2, 3);
        Sum(10, 20);
        SumEnumerable(1, 2, 3, 4);
    }

    // Interface-typed `params` collection variants. These remain defined so
    // that Stage 1 binding still exercises the Phase F5 call-site synthesis
    // for `IList<T>`/`ICollection<T>`/`IReadOnlyList<T>`/`IReadOnlyCollection<T>`
    // shapes (the compile-only coverage that #47 Phase F5 cares about).
    //
    // They are NOT invoked from `ParamsCollections()` because `.Count` on the
    // synthesised `List<T>` instance dispatches through the interface-suffixed
    // getter slot at runtime, which the underlying List<T> facade does not
    // expose — V8 raises `TypeError: xs.V_get_Count_<suffix> is not a function`.
    // Tracked as a runtime gap in `docs/language/csharp9-13-status.md`.
    public static void ParamsCollectionsInterfaceCount_RuntimeGap()
    {
        CountIList(10, 20, 30);
        CountICollection(7, 8);
        CountIReadOnlyList(11, 12, 13, 14);
        CountIReadOnlyCollection(100, 200);
    }

    private static void Sum(params System.Collections.Generic.List<int> xs)
    {
        int total = 0;
        foreach (int v in xs)
        {
            total += v;
        }

        Console.WriteLine(total);
    }

    private static void SumEnumerable(params System.Collections.Generic.IEnumerable<int> xs)
    {
        int total = 0;
        foreach (int v in xs)
        {
            total += v;
        }

        Console.WriteLine(total);
    }

    private static void CountIList(params System.Collections.Generic.IList<int> xs)
    {
        Console.WriteLine(xs.Count);
    }

    private static void CountICollection(params System.Collections.Generic.ICollection<int> xs)
    {
        Console.WriteLine(xs.Count);
    }

    private static void CountIReadOnlyList(params System.Collections.Generic.IReadOnlyList<int> xs)
    {
        Console.WriteLine(xs.Count);
    }

    private static void CountIReadOnlyCollection(
        params System.Collections.Generic.IReadOnlyCollection<int> xs)
    {
        Console.WriteLine(xs.Count);
    }
}
