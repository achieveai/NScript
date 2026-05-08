//-----------------------------------------------------------------------
// <copyright file="Lang12Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

// C# 12 — alias any type. The using-alias relaxation specifically permits
// **tuple, array, and pointer** types in alias position (closed generic
// aliases were already legal pre-C# 12 and are not exercised here). The
// underlying type is what reaches the bound tree.
using Pair = (int X, int Y);    // tuple-syntax alias — illegal pre-C# 12
using Numbers = int[];          // array-syntax alias — illegal pre-C# 12

/// <summary>
/// Compile-only fixtures for transparent C# 12 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
///
/// NOTE: Collection expressions (<c>[1, 2, 3]</c>) live in their own fixture
/// (<c>Lang12CollectionExpressionTests.cs</c>) because they introduce a new
/// bound node shape (<c>BoundCollectionExpression</c>). Primary constructors
/// on non-record classes are exercised in <c>Lang12PrimaryCtorTests</c>
/// below — Roslyn lowers them transparently to backing-field reads at bind
/// time, so no new bound-tree shape reaches Stage 1. Default lambda parameter
/// values are not exercised here because they introduce new bound node shape
/// that has not been audited yet.
/// </summary>
public class Lang12Features
{
    public static void AliasAnyType()
    {
        // Tuple-syntax alias — exercises the C# 12 grammar relaxation.
        Pair p = (3, 4);
        Console.WriteLine(p.X);
        Console.WriteLine(p.Y);

        // Array-syntax alias — exercises the C# 12 grammar relaxation.
        Numbers ns = new int[] { 1, 2, 3 };
        Console.WriteLine(ns.Length);
    }
}

/// <summary>
/// C# 12 primary constructors on plain (non-record) classes. Roslyn does NOT
/// auto-promote primary-ctor parameters to public properties (records do); it
/// synthesises private backing fields for captured parameters and rewrites
/// references at bind time. The bound tree therefore resolves to existing
/// <c>BoundFieldAccess</c> / <c>BoundParameter</c> shapes already covered by
/// the Stage 1 visitor — primary ctors on classes are a transparent C# 12
/// feature for the NScript pipeline. Records (<c>record class Foo(int X)</c>)
/// flow through their own synthesised-property path validated in
/// <c>Lang9RecordTests.cs</c> and are NOT exercised here.
/// </summary>
// CS9124: parameter captured AND used to initialise a field — intentional in
// `Counter(int initial)` to exercise the read-after-init path. CS9107:
// parameter captured AND forwarded to base — intentional in
// `DerivedHolder(int x) : BaseHolder(x)` to exercise base-call lowering.
#pragma warning disable 9107, 9124
public class Lang12PrimaryCtorTests
{
    // Captured parameter referenced from instance methods. Roslyn synthesises
    // a private backing field for `initial` and rewrites the references.
    public class Counter(int initial)
    {
        int _count = initial;
        public int Add(int n) => _count += n;
        public int Reset() => _count = initial;
        public int Initial => initial;
    }

    public static void PrimaryCtorOnClass()
    {
        var c = new Counter(10);
        Console.WriteLine(c.Add(5));
        Console.WriteLine(c.Add(2));
        Console.WriteLine(c.Reset());
        Console.WriteLine(c.Initial);
    }

    // Primary-ctor argument forwarded to base. Exercises base-call argument
    // lowering for primary ctors.
    public class BaseHolder(int seed)
    {
        public int Seed => seed;
    }

    public class DerivedHolder(int x) : BaseHolder(x)
    {
        public int Local => x;
    }

    public static void PrimaryCtorWithBaseCall()
    {
        var d = new DerivedHolder(7);
        Console.WriteLine(d.Seed);
        Console.WriteLine(d.Local);
    }

    // Multiple parameters with disjoint reference sites — `a` and `b` are
    // consumed in a field initializer, `b` is also captured into a property,
    // and `c` is captured into a property without participating in any field
    // initializer. Exercises the disambiguation path across multiple
    // synthesised backing fields.
    public class Triple(int a, int b, int c)
    {
        int _sum = a + b;
        public int Sum => _sum;
        public int B => b;
        public int Both => a + c;
    }

    public static void PrimaryCtorMultipleParams()
    {
        var t = new Triple(1, 2, 3);
        Console.WriteLine(t.Sum);
        Console.WriteLine(t.B);
        Console.WriteLine(t.Both);
    }
}
#pragma warning restore 9107, 9124
