//-----------------------------------------------------------------------
// <copyright file="Lang11Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

/// <summary>
/// Compile-only fixtures for transparent C# 11 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
/// </summary>
public class Lang11Features
{
    // C# 11 — raw string literals (single and multi-line). Roslyn produces
    // ordinary `string` constants at parse time; the bound tree only sees
    // `BoundLiteral`.
    private const string SingleLineRaw = """no escapes needed: \n stays literal""";

    private const string MultiLineRaw = """
        line one
        line two
        """;

    // C# 11 — newlines allowed inside interpolation holes is a syntactic
    // refinement of `$"..."`; not exercised here because the underlying
    // `BoundInterpolatedString` visit is a pre-existing gap.
    public static void RawStrings()
    {
        Console.WriteLine(SingleLineRaw);
        Console.WriteLine(MultiLineRaw);
    }

    // C# 11 — UTF-8 string literals. Lowered to `ReadOnlySpan<byte>` and
    // therefore not transparent for NScript (no `Span<T>` on JS — see
    // Non-Goals in the issue and the limitations doc). Test omitted.

    // C# 11 — `nameof(parameter)` inside an attribute on the enclosing method
    // referencing one of that method's own parameters. This was illegal pre-
    // C# 11; Roslyn now resolves the parameter symbol at attribute-bind time
    // and folds to a constant string.
    [Obsolete("renamed parameter: " + nameof(value))]
    public static void NameOfParameterInAttribute(string value)
    {
        Console.WriteLine(value);
    }

    // Body-position `nameof(local)` is a C# 6 fold; kept here so the matrix
    // row that mentions both call sites has a representative pair.
    public static void NameOfLocal()
    {
        int counter = 0;
        Console.WriteLine(nameof(counter));
    }

    // C# 11 — `nameof` extended to type parameters of a generic method.
    // Roslyn folds `nameof(T)` to the literal type-parameter name at bind
    // time; bound tree sees a constant string.
    public static void NameOfTypeParameter<T>()
    {
        Console.WriteLine(nameof(T));
    }

    // C# 11 — file-local types. The `file` modifier scopes the type to the
    // current source file. Bound tree sees an ordinary class with mangled
    // metadata name; member access binds normally.
    public static void UseFileLocalHelper()
    {
        var helper = new FileLocalHelper(11);
        Console.WriteLine(helper.Compute());

        // Force the attribute-bind path on `NameOfParameterInAttribute` to be
        // exercised by referencing the method (the bound site is the
        // attribute application itself; this call just keeps the method
        // reachable in the demand-driven converter pass).
        NameOfParameterInAttribute("hello");
        NameOfLocal();
    }

    // C# 11 — list/slice/relational/logical/negated patterns are tracked under
    // Phase C; not exercised here.

    // C# 11 — auto-default of unused fields in struct constructors. Bound
    // tree shape unchanged (compiler synthesizes default-init prologue).
    public static void StructAutoDefault()
    {
        SimpleStruct s = new SimpleStruct(7);
        Console.WriteLine(s.A);
        Console.WriteLine(s.B);
    }

    public struct SimpleStruct
    {
        public int A;
        public int B;

        public SimpleStruct(int a)
        {
            this.A = a;
            // B intentionally unset — auto-defaulted to 0 under C# 11.
        }
    }
}

file class FileLocalHelper
{
    private readonly int seed;

    public FileLocalHelper(int seed)
    {
        this.seed = seed;
    }

    public int Compute() => this.seed * 2;
}
