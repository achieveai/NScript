//-----------------------------------------------------------------------
// <copyright file="Lang10GlobalUsingTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    /// <summary>
    /// Compile-only fixture for the C# 10 <c>global using</c> directive.
    ///
    /// `global using System.Text;` lives in `GlobalUsings.cs` at file scope.
    /// This fixture deliberately omits the local `using System.Text;` and
    /// `using System;` to prove the project-wide directive flowed through
    /// Roslyn's symbol resolution: <c>StringBuilder</c> binds against
    /// <c>System.Text.StringBuilder</c> via the global directive, and
    /// <c>System.Console.WriteLine</c> is fully qualified.
    ///
    /// The bound tree carries fully-qualified type references after symbol
    /// resolution, so no new Stage 1 shape reaches `BoundAstToAstBase` —
    /// the directive purely affects which symbols the binder can see.
    ///
    /// This class lives in its own file (not in `Lang10Features.cs`)
    /// following the `Lang9RecordTests.cs` / `Lang11RequiredTests.cs`
    /// precedent: `Lang10Features.cs` is in the explicit Roslyn-driven
    /// build list in `NScript.Csc.Lib.Test/TestResources.cs`. The C# 10
    /// `global using` shape has not been audited through the in-test
    /// `BondToAst` deserializer, so we keep it out of that path
    /// conservatively. The MSBuild framework build still globs this file
    /// and exercises it end-to-end through NScript's compiler.
    /// </summary>
    public class Lang10GlobalUsingTests
    {
        public static void UsesGloballyImportedText()
        {
            // No local `using System.Text;` — `StringBuilder` resolves
            // only because the project-level `global using System.Text;`
            // in `GlobalUsings.cs` is in effect.
            StringBuilder sb = new StringBuilder();
            sb.Append("hello, world");

            // `System.Console` is fully qualified — no `using System;`
            // either, to keep this fixture's coverage focused on the
            // global directive's wire-through.
            System.Console.WriteLine(sb.ToString());
        }
    }
}
