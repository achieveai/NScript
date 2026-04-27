//-----------------------------------------------------------------------
// <copyright file="Lang12Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

// C# 12 — alias any type. The using-alias resolves to a constructed generic
// (or array, tuple, pointer) at symbol-resolution time; bound tree sees the
// underlying type.
using IntList = System.Collections.Generic.List<int>;
using Pair = System.ValueTuple<int, int>;

/// <summary>
/// Compile-only fixtures for transparent C# 12 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
///
/// NOTE: Collection expressions (<c>[1, 2, 3]</c>) and primary constructors
/// on classes are tracked under later phases (E and F). Default lambda
/// parameter values are NOT exercised here because they introduce new bound
/// node shape that has not been audited yet.
/// </summary>
public class Lang12Features
{
    public static void AliasAnyType()
    {
        IntList list = new IntList();
        list.Add(1);
        list.Add(2);

        Pair p = new Pair(3, 4);
        Console.WriteLine(p.Item1);
        Console.WriteLine(p.Item2);

        Console.WriteLine(list.Count);
    }
}
