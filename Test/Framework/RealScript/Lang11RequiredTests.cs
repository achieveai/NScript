//-----------------------------------------------------------------------
// <copyright file="Lang11RequiredTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
    ///
    /// This class lives in its own file (not in <c>Lang11Features.cs</c>)
    /// because <c>Lang11Features.cs</c> is in the explicit Roslyn-driven
    /// build list in <c>NScript.Csc.Lib.Test/TestResources.cs</c>; the
    /// synthesised <c>init</c> / <c>required</c> method-reference shapes
    /// emitted by Roslyn are not yet round-trippable through the in-test
    /// <c>BondToAst</c> deserializer, so we keep them out of that path
    /// (same precedent as <c>Lang9RecordTests.cs</c>). The MSBuild Framework
    /// build still globs this file and exercises it end-to-end through
    /// NScript's compiler.
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
}
