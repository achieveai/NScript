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
        Lang13Features.MethodGroupOverloadPruning();
        Lang13Features.ParamsCollections();
    }
}
