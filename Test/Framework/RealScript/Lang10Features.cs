//-----------------------------------------------------------------------
// <copyright file="Lang10Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// C# 10 — file-scoped namespace. Purely syntactic: Roslyn produces the same
// bound tree as a braced namespace, so the existing visitor pipeline is
// unchanged.
namespace RealScript;

using System;

/// <summary>
/// Compile-only fixtures for transparent C# 10 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
///
/// NOTE: Constant interpolated strings (<c>const string s = $"a{X}b";</c>)
/// look like a transparent fold but they currently surface a
/// <c>BoundInterpolatedString</c> to the Stage-1 visitor, where
/// <c>VisitInterpolatedString</c> throws <c>NotImplementedException</c>.
/// That is a pre-existing C# 6 gap, not a C# 10 gap. Tracked separately.
/// </summary>
public class Lang10Features
{
    // C# 10 — assignment in deconstruction (mix of declaration and existing
    // variables). Roslyn lowers this to a sequence of plain assignments.
    public static void MixedDeconstructionAssignment()
    {
        int existing = 0;
        (existing, int created) = (10, 20);
        Console.WriteLine(existing);
        Console.WriteLine(created);
    }

    // C# 10 — extended property pattern syntax `{ A.B: ... }`. Roslyn lowers
    // this to an ordinary recursive pattern (which we do NOT yet support);
    // tracked under Phase C — pattern family.

    // C# 10 — natural type for lambdas inferred via `var`. The
    // delegate-bound case is identical to C# 9 lambdas at the bound level;
    // only the var-binding `var f = () => 0;` is new C# 10 territory and
    // synthesizes a delegate type at bind time.
    public static void NaturalLambdaType()
    {
        var produce = () => 42;
        Console.WriteLine(produce());

        var combine = (int a, int b) => a + b;
        Console.WriteLine(combine(1, 2));
    }

    // C# 10 — lambda with an explicit return type (here `int`).
    // Bound tree carries the explicit return-type symbol on the anonymous
    // function shape; otherwise identical to a natural-typed lambda.
    public static void LambdaExplicitReturnType()
    {
        var explicitReturn = int (int a) => a + 1;
        Console.WriteLine(explicitReturn(41));
    }

    // C# 10 — `ParenthesizedPattern`. Allowed in C# 10 wherever a pattern is
    // allowed; lowers to the inner pattern (transparent for already-supported
    // patterns like constant/declaration).
    public static void ParenthesizedConstantPattern()
    {
        object o = 5;
        bool isFive = o is (5);
        Console.WriteLine(isFive);
    }

    // C# 10 — single-overload method group natural type. Roslyn synthesises
    // the delegate type at bind time; the bound expression is identical to
    // one written with an explicit delegate type. C# 13 extends this with
    // overload-pruning (see Lang13Features.MethodGroupOverloadPruning).
    public static void MethodGroupNaturalType()
    {
        var f = ProduceInt;
        Console.WriteLine(f().ToString());

        var g = TakeInt;
        g(42);
    }

    private static int ProduceInt() => 99;

    private static void TakeInt(int x) => Console.WriteLine(x.ToString());

    // C# 10 — explicit parameterless constructor on a `struct`. Roslyn lowers
    // an explicit parameterless struct ctor to an ordinary
    // `BoundConstructor` shape; the surrounding struct codegen path was
    // exercised under the Phase F2 record-struct work without surfacing new
    // bound-tree gaps. The construction site is an ordinary
    // `BoundObjectCreationExpression`.
    public struct ExplicitDefaultStruct
    {
        public int X;
        public int Y;

        public ExplicitDefaultStruct()
        {
            X = 7;
            Y = 11;
        }
    }

    public static void ParameterlessStructConstructor()
    {
        var s = new ExplicitDefaultStruct();
        Console.WriteLine(s.X);
        Console.WriteLine(s.Y);
    }

    // C# 10 — attributes on lambdas and lambda parameters. The attribute is
    // stored on the synthesised lambda method symbol's metadata; the bound
    // tree shape is the same `BoundLambda` as an unattributed lambda. Use a
    // local attribute (not `[Obsolete]`) so the lambda invocation does not
    // raise an obsolete-method warning at the call site.
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter, Inherited = false)]
    public sealed class LambdaMarkerAttribute : Attribute
    {
        public LambdaMarkerAttribute(string label)
        {
            Label = label;
        }

        public string Label { get; }
    }

    public static void LambdaWithAttribute()
    {
        // Attribute on the lambda itself.
        Func<int, int> increment = [LambdaMarker("inc")] (int x) => x + 1;
        Console.WriteLine(increment(41));

        // Attribute on a lambda parameter.
        Func<int, int> doubler = ([LambdaMarker("p")] int x) => x * 2;
        Console.WriteLine(doubler(21));
    }

    // C# 10 — [CallerArgumentExpression]. Roslyn folds the captured argument
    // expression to a literal `string` default at every call site at bind
    // time, so the bound tree at the callee is identical to an ordinary
    // optional `string` parameter and the callee body is identical to one
    // that receives a literal. The wire-through is transparent for Stage 1 /
    // Stage 2 — no new bound-tree shape reaches `BoundAstToAstBase`. This
    // fixture pins both the facade resolution and the call-site fold.
    public static string DescribeArgument(
        int value,
        [System.Runtime.CompilerServices.CallerArgumentExpression("value")] string expression = "")
    {
        return expression + "=" + value.ToString();
    }

    public static void CallerArgumentExpression()
    {
        int x = 41;
        // Roslyn folds the second argument to the literal `"x + 1"` here.
        Console.WriteLine(DescribeArgument(x + 1));
    }
}
