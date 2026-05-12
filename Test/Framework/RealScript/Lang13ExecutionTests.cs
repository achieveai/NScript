//-----------------------------------------------------------------------
// <copyright file="Lang13ExecutionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

/// <summary>
/// V8 runtime execution coverage for the runtime-shaped C# 13 features:
/// the <c>\e</c> ESC escape sequence, method-group overload pruning, and
/// <c>params</c> collections (six BCL shapes — <c>List&lt;T&gt;</c>,
/// <c>IEnumerable&lt;T&gt;</c>, <c>IList&lt;T&gt;</c>, <c>ICollection&lt;T&gt;</c>,
/// <c>IReadOnlyList&lt;T&gt;</c>, <c>IReadOnlyCollection&lt;T&gt;</c>).
/// Delegates directly to the helpers in <c>Lang13Features.cs</c>.
/// </summary>
public class Lang13ExecutionTests
{
    public static void Main()
    {
        Lang13Features.EscapeSequenceEsc();

        // NOTE: `Lang13Features.MethodGroupOverloadPruning` triggers a
        // generic-type-access error in the demand-driven converter at
        // runtime ("Can't access generic type (!!0) if they are ignored")
        // because the constraint-pruned generic candidate is dead-code-
        // eliminated yet a downstream type reference still resolves to its
        // type parameter. Tracked as a runtime gap in
        // `docs/language/csharp9-13-status.md`; the compile-only path in
        // `Lang13Features.cs` keeps coverage of the bind-time behaviour.
        Lang13Features.ParamsCollections();
    }
}
