//-----------------------------------------------------------------------
// <copyright file="Lang13ExecutionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

/// <summary>
/// V8 runtime execution coverage for the runtime-shaped C# 13 features
/// that are clean end-to-end through Roslyn → JST → JS → V8:
/// the <c>\e</c> ESC escape sequence and <c>params</c> collections for the
/// two list-shaped BCL targets that the runtime supports today
/// (<c>List&lt;T&gt;</c> and <c>IEnumerable&lt;T&gt;</c>).
/// Delegates directly to the helpers in <c>Lang13Features.cs</c>.
///
/// Excluded by design (compile-only coverage retained in <c>Lang13Features.cs</c>):
/// <list type="bullet">
///   <item><description><c>MethodGroupOverloadPruning</c> — runtime gap: the
///     constraint-pruned generic candidate is dead-code-eliminated yet a
///     downstream type reference still resolves to its <c>!!0</c> type
///     parameter (<c>ConverterLocationException</c>).</description></item>
///   <item><description><c>params IList&lt;T&gt;</c> / <c>ICollection&lt;T&gt;</c> /
///     <c>IReadOnlyList&lt;T&gt;</c> / <c>IReadOnlyCollection&lt;T&gt;</c> —
///     runtime gap on <c>xs.Count</c>: the interface-suffixed getter slot is
///     not present on the underlying <c>List&lt;T&gt;</c> facade
///     (<c>TypeError</c> in V8).</description></item>
/// </list>
/// Both are tracked in <c>docs/language/csharp9-13-status.md</c>.
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
