//-----------------------------------------------------------------------
// <copyright file="Lang9Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixtures for transparent C# 9 syntactic features.
    /// "Transparent" means Roslyn lowers them to bound nodes already supported by the
    /// NScript Stage 1 visitor (<c>BoundAstToAstBase</c>) — so they ride the existing
    /// pipeline without new visitors, serializer entries, or converter cases.
    ///
    /// If a method here fails to compile through the NScript csc, the corresponding
    /// row in <c>docs/language/csharp9-13-status.md</c> must move from "Supported"
    /// to "Needs implementation".
    ///
    /// NOTE — features deliberately NOT exercised here:
    /// - Target-typed <c>new()</c>: surfaces <c>ConversionKind.ObjectCreation</c>
    ///   in <c>VisitConversion</c> (currently throws NotImplementedException);
    ///   tracked under Phase B/C.
    /// - Covariant return types: require the
    ///   <c>RuntimeFeature.CovariantReturnsOfClasses</c> metadata flag, absent on
    ///   our <c>netstandard2.1</c> target.
    /// - Records, <c>with</c>, <c>init</c>: tracked under Phase D.
    /// - Pattern-matching enhancements: tracked under Phase C.
    /// </summary>
    public class Lang9Features
    {
        // C# 9 — discard parameters in lambdas. Lowers to an ordinary lambda whose
        // parameter names are `_` (no captures, no special bound node).
        public static void LambdaDiscardParameters()
        {
            Func<int, int, int> alwaysOne = (_, _) => 1;
            int v = alwaysOne(10, 20);
            Console.WriteLine(v);

            Action<int, string> ignoreBoth = (_, _) => Console.WriteLine("ignored");
            ignoreBoth(0, "x");
        }

        // C# 9 — static anonymous functions. The `static` keyword forbids captures
        // but produces an ordinary lambda bound node.
        public static void StaticLambdas()
        {
            Func<int, int> doubleIt = static x => x * 2;
            Console.WriteLine(doubleIt(21));

            Action<string> log = static s => Console.WriteLine(s);
            log("static");
        }

        // C# 9 — `static` local function modifier (technically introduced in
        // C# 8 but exercised here as part of the C# 9 lambda/closure family).
        // Forbids implicit captures from the enclosing scope; lowers to an
        // ordinary static local function bound node.
        public static void LocalFunctionWithStaticModifier()
        {
            int seed = 5;
            int outer = Compute(2);
            Console.WriteLine(outer);
            Console.WriteLine(seed);

            static int Compute(int x) => x * x;
        }
    }
}
