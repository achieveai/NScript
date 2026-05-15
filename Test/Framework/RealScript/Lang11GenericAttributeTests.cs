//-----------------------------------------------------------------------
// <copyright file="Lang11GenericAttributeTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixtures for C# 11 generic attributes
    /// (<c>class FooAttribute&lt;T&gt; : Attribute</c> and usages of the form
    /// <c>[Foo&lt;int&gt;]</c>).
    ///
    /// Each method/type here exercises a path Roslyn synthesises around
    /// generic-attribute metadata so a regression in the symbol/serializer
    /// pass surfaces as a build failure rather than silently passing.
    ///
    /// This class lives in its own file (not in <c>Lang11Features.cs</c>)
    /// because <c>Lang11Features.cs</c> is in the explicit Roslyn-driven
    /// build list in <c>NScript.Csc.Lib.Test/TestResources.cs</c>; the
    /// generic-attribute metadata shapes emitted by Roslyn may surface a
    /// previously-unmodelled deserializer path in the in-test
    /// <c>BondToAst</c> / <c>MemberReferenceDeserializer</c> pipeline. To
    /// follow the <c>Lang9RecordTests.cs</c> precedent we keep this file
    /// out of that explicit list; the MSBuild Framework build still globs
    /// the file and drives it end-to-end through Stage 1 (Roslyn → DLL with
    /// embedded <c>$$BstInfo$$</c>) and Stage 2 (<c>cs2jsc</c> → JS).
    /// </summary>
    public class Lang11GenericAttributeTests
    {
        // C# 11 — generic attribute definition. Roslyn emits this as an
        // ordinary generic class deriving from `Attribute`; the metadata
        // path is the same as any other generic class declaration except
        // that the type symbol now reaches `SymbolSerializer` in attribute
        // position when applied to other declarations.
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        public sealed class MarkerAttribute<T> : Attribute
        {
            public MarkerAttribute()
            {
            }

            public MarkerAttribute(string label)
            {
                this.Label = label;
            }

            public string Label { get; }

            public Type Captured { get; } = typeof(T);
        }

        // C# 11 — generic-attribute application with a concrete reference
        // type argument. Cecil reads the `CustomAttribute.AttributeType` as
        // a `GenericInstanceType` whose generic argument is `string`; the
        // symbol-side metadata path through `SymbolSerializer` must
        // round-trip the type-argument so the attribute resolves cleanly
        // on the consumer side.
        [Marker<string>("class")]
        public class TargetedClass
        {
            // C# 11 — generic-attribute application on a method with a
            // value-type argument. Exercises the boxing-free generic
            // instantiation path through metadata.
            [Marker<int>("method")]
            public void MarkedMethod()
            {
            }

            // C# 11 — generic-attribute application on a property with a
            // user-defined reference type argument. Confirms that
            // metadata-side resolution of `GenericInstanceType` is not
            // tied to the BCL primitive types.
            [Marker<TargetedClass>("property")]
            public int MarkedProperty { get; set; }
        }

        // C# 11 — generic-attribute application using the no-arg ctor. Pins
        // the constructor-resolution path against generic-attribute
        // metadata for the parameterless overload.
        [Marker<long>]
        public class NoArgMarkedClass
        {
        }

        // Compile-only happy-path: instantiate the marked types and read
        // the `typeof(T)` capture so the IL referencing the generic
        // attribute's instance members also drives demand-driven
        // conversion through Stage 2.
        public static void ReadMarkedTypes()
        {
            var marker = new MarkerAttribute<int>("inline");
            Console.WriteLine(marker.Label);
            Console.WriteLine(marker.Captured.FullName);

            var t = new TargetedClass();
            t.MarkedMethod();
            t.MarkedProperty = 7;
            Console.WriteLine(t.MarkedProperty);

            var n = new NoArgMarkedClass();
            Console.WriteLine(n.GetType().FullName);
        }
    }
}
