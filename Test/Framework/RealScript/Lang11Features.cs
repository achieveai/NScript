//-----------------------------------------------------------------------
// <copyright file="Lang11Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;
using System.Diagnostics.CodeAnalysis;

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

/// <summary>
/// Compile-only fixtures for the C# 11 <c>required</c> members feature.
///
/// "<c>required</c>" is metadata-only in NScript: the
/// <c>IsRequired</c> flag is persisted on <c>PropertySpecSer</c>/
/// <c>FieldSpecSer</c> (shipped under the records slice), and the BCL
/// attribute stubs (<c>RequiredMemberAttribute</c>,
/// <c>CompilerFeatureRequiredAttribute</c>,
/// <c>SetsRequiredMembersAttribute</c>) live in the NScript mscorlib facade
/// so Roslyn binds the C# 11 syntax.  No runtime check is emitted — this
/// matches the same "compile-time strict, runtime permissive" precedent
/// already used for nullable reference types.
///
/// Each method here exercises a path Roslyn synthesises around
/// <c>required</c> so a regression in the metadata pass surfaces as a
/// compile failure rather than silently passing.
/// </summary>
public class Lang11RequiredTests
{
    // C# 11 — required init-only property. Roslyn applies
    // [RequiredMember] to the property and a
    // [CompilerFeatureRequired("RequiredMembers")] to the containing type;
    // both attributes must resolve through the NScript mscorlib facade.
    public class Settings
    {
        public required string Theme { get; init; }

        public int FontSize { get; init; }
    }

    // C# 11 — required field. The `required` modifier on a field follows
    // the same metadata path as on a property.
    public class Profile
    {
        public required string DisplayName;

        public int Score;
    }

    // C# 11 — derived class adds another required member; both base and
    // derived `required` members must be set at construction.
    public class ThemedSettings : Settings
    {
        public required string Accent { get; init; }
    }

    // Construct an object that satisfies all required members through an
    // object initializer. This is the canonical happy-path call site for
    // `required`: Roslyn validates at the call site, so a missing
    // required member is a CS9035 *bind-time* error in the consumer's
    // compilation unit.
    public static void ConstructWithAllRequired()
    {
        var s = new Settings { Theme = "dark", FontSize = 14 };
        Console.WriteLine(s.Theme);
        Console.WriteLine(s.FontSize);
    }

    // Required field via object initializer.
    public static void ConstructWithRequiredField()
    {
        var p = new Profile { DisplayName = "Ada", Score = 100 };
        Console.WriteLine(p.DisplayName);
        Console.WriteLine(p.Score);
    }

    // Derived type — both inherited and own required members must be set.
    public static void ConstructDerivedWithAllRequired()
    {
        var t = new ThemedSettings { Theme = "dark", FontSize = 14, Accent = "blue" };
        Console.WriteLine(t.Theme);
        Console.WriteLine(t.Accent);
    }

    // C# 11 — `[SetsRequiredMembers]` on a constructor tells Roslyn that
    // the ctor itself sets every required member, so a caller does NOT
    // need to repeat them in an initializer. Exercises the
    // SetsRequiredMembersAttribute facade.
    public class Defaults
    {
        public required string Name { get; init; }

        public required int Count { get; init; }

        [SetsRequiredMembers]
        public Defaults()
        {
            this.Name = "default";
            this.Count = 0;
        }

        [SetsRequiredMembers]
        public Defaults(string name)
        {
            this.Name = name;
            this.Count = 0;
        }
    }

    public static void ConstructorSetsRequiredMembers()
    {
        var d1 = new Defaults();
        Console.WriteLine(d1.Name);
        Console.WriteLine(d1.Count);

        var d2 = new Defaults("custom");
        Console.WriteLine(d2.Name);
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
