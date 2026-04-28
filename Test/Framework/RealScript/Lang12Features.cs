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
/// NOTE: Collection expressions (<c>[1, 2, 3]</c>) and primary constructors
/// on classes are tracked under later phases (E and F). Default lambda
/// parameter values are NOT exercised here because they introduce new bound
/// node shape that has not been audited yet.
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
