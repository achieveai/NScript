//-----------------------------------------------------------------------
// <copyright file="Lang12LambdaDefaultTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixture for the C# 12 default lambda parameter values
    /// feature.
    ///
    /// `(int x = 5) => x + 1` stores the default value on the lambda's
    /// synthesised parameter symbol; Roslyn folds the default into the
    /// argument list at the call site, so the lambda body itself sees a
    /// plain `BoundParameter` access. Wire-through is expected to be
    /// transparent — the new metadata shape on the parameter symbol just
    /// needs to round-trip through NScript's serialiser without raising
    /// from `BoundAstToAstBase`.
    ///
    /// `Func&lt;int, int&gt;` requires exactly one argument at every call
    /// site (the parameterless `f()` form does not bind), so the fixture
    /// only exercises the explicit-argument call site — per the issue body
    /// the parameterless form is out of scope for this fixture.
    ///
    /// This class lives in its own file (not in `Lang12Features.cs`)
    /// following the `Lang9RecordTests.cs` / `Lang11RequiredTests.cs`
    /// precedent: `Lang12Features.cs` is in the explicit Roslyn-driven
    /// build list in `NScript.Csc.Lib.Test/TestResources.cs`. The new
    /// parameter-symbol metadata shape (the stored default value on a
    /// lambda parameter) has not been audited through the in-test
    /// `BondToAst` deserializer, so we keep it out of that path
    /// conservatively. The MSBuild framework build still globs this file
    /// and exercises it end-to-end through NScript's compiler.
    /// </summary>
    // CS9099: parameter has a default in the lambda but `Func<int,int>` (the
    // target delegate type) has no default — informational only; the fixture's
    // whole point is to exercise the lambda-parameter default-value metadata.
#pragma warning disable 9099
    public class Lang12LambdaDefaultTests
    {
        public static void LambdaDefaultParameter()
        {
            // C# 12 — default value on a lambda parameter. The default is
            // recorded on the parameter symbol but not consumed at the
            // explicit-argument call site below.
            Func<int, int> f = (int x = 5) => x + 1;

            // Explicit-argument call site. `Func<int,int>` requires one
            // argument — the parameterless form is grammatically out.
            int result = f(7);
            Console.WriteLine(result);
        }
    }
#pragma warning restore 9099
}
