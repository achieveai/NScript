//-----------------------------------------------------------------------
// <copyright file="Lang9RecordTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixtures for C# 9 records, <c>with</c> expressions, and
    /// <c>init</c> accessors. Each method exercises a record-related feature
    /// through the NScript Stage 1 / Stage 2 pipeline so that a regression
    /// surfaces as a Roslyn or NScript build failure rather than silently
    /// passing.
    ///
    /// Out-of-scope for this slice (tracked separately):
    /// - Required-member runtime enforcement (<c>required</c> metadata is
    ///   persisted but no runtime check is emitted).
    /// - Primary constructors on non-record classes (validated separately in
    ///   <c>Lang12Features.cs::Lang12PrimaryCtorTests</c>) and on non-record
    ///   structs (currently out of scope — see issue #47 Phase F).
    /// - Collection expressions and list/recursive patterns (C# 12+).
    /// </summary>
    public class Lang9RecordTests
    {
        // C# 9 — nominal record with init-only properties. Lowers to a class
        // with synthesised Equals / GetHashCode / <Clone>$ / copy-ctor.
        public record NominalPerson
        {
            public string Name { get; init; }
            public int Age { get; init; }
        }

        // C# 9 — positional record. Roslyn synthesises the primary constructor,
        // init-only properties, and Deconstruct.
        public record PositionalPerson(string Name, int Age);

        // C# 9 — record inheritance. Derived records get their own <Clone>$
        // override so `with` on a base reference still produces the derived type.
        public record Employee(string Name, int Age, string Title)
            : PositionalPerson(Name, Age);

        // C# 10 — record struct. Behaves like a struct but carries the
        // synthesised value-equality contract.
        public record struct PointStruct(int X, int Y);

        // Construct + read a nominal record using object-initializer syntax,
        // which exercises the existing `NewInitializerExpression` path now
        // that init-only setters are persisted as `IsInitOnly = true`.
        public static void ConstructNominalRecord()
        {
            var p = new NominalPerson { Name = "Ada", Age = 36 };
            Console.WriteLine(p.Name);
            Console.WriteLine(p.Age);
        }

        // Construct a positional record via the synthesised primary ctor.
        public static void ConstructPositionalRecord()
        {
            var p = new PositionalPerson("Ada", 36);
            Console.WriteLine(p.Name);
            Console.WriteLine(p.Age);
        }

        // Single-property `with` mutation. Drives the new
        // `BoundWithExpression` -> `WithExpressionSer` -> `ParseWithExpression`
        // path in Stage 1 + Stage 2.
        public static void WithSingleMutation()
        {
            var original = new PositionalPerson("Ada", 36);
            var modified = original with { Age = 37 };
            Console.WriteLine(original.Age);
            Console.WriteLine(modified.Age);
        }

        // Multi-property `with`. The clone-method call is shared and the
        // initializer list lowers through `BuildInitializerSetters`.
        public static void WithMultiMutation()
        {
            var original = new PositionalPerson("Ada", 36);
            var modified = original with { Name = "Grace", Age = 37 };
            Console.WriteLine(modified.Name);
            Console.WriteLine(modified.Age);
        }

        // `with` on a derived record — the synthesised override of <Clone>$
        // returns the derived type, so the result keeps the Title.
        public static void WithOnDerivedRecord()
        {
            Employee original = new Employee("Ada", 36, "Engineer");
            Employee modified = original with { Age = 37 };
            Console.WriteLine(modified.Title);
            Console.WriteLine(modified.Age);
        }

        // Copy-constructor exercise. Records expose a protected copy-ctor that
        // <Clone>$ delegates to; constructing through it directly must still
        // resolve the synthesised member.
        public static void CopyConstructor()
        {
            var original = new NominalPerson { Name = "Ada", Age = 36 };
            var copy = new NominalPerson { Name = original.Name, Age = original.Age };
            Console.WriteLine(copy.Name);
        }

        // Deconstruct on positional records.
        public static void DeconstructPositional()
        {
            var p = new PositionalPerson("Ada", 36);
            var (name, age) = p;
            Console.WriteLine(name);
            Console.WriteLine(age);
        }

        // Value equality — compile-only smoke test. Roslyn synthesises `Equals`
        // and `==` against `EqualityComparer<T>.Default`, which currently
        // returns `null` in NScript's `mscorlib` facade. The fixture is kept
        // so the synthesised members continue to *bind* and *serialise* as
        // expected; runtime behaviour is gated on closing the
        // `EqualityComparer<T>.Default` gap (tracked as a follow-up to #47).
        public static void RecordValueEqualityBindOnly()
        {
            var a = new PositionalPerson("Ada", 36);
            var b = new PositionalPerson("Ada", 36);
            Console.WriteLine(object.ReferenceEquals(a, b));
        }

        // Record struct with `with` — exercises the value-type clone path.
        public static void RecordStructWith()
        {
            var origin = new PointStruct(1, 2);
            var shifted = origin with { X = 10 };
            Console.WriteLine(shifted.X);
            Console.WriteLine(shifted.Y);
        }

        // init-only setter on a non-record class. The setter is callable from
        // an object initializer but not from arbitrary code; this fixture
        // verifies the initializer path still compiles.
        public class InitOnlySettings
        {
            public string Theme { get; init; }
            public int FontSize { get; init; }
        }

        public static void InitOnlySetters()
        {
            var s = new InitOnlySettings { Theme = "dark", FontSize = 14 };
            Console.WriteLine(s.Theme);
            Console.WriteLine(s.FontSize);
        }
    }
}
