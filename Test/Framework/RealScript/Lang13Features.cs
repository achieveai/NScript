//-----------------------------------------------------------------------
// <copyright file="Lang13Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

/// <summary>
/// Compile-only fixtures for transparent C# 13 syntactic features.
/// See <c>Lang9Features.cs</c> for the contract describing "transparent".
///
/// NOTE: <c>params</c> collections (C# 13) are tracked under Phase E with
/// the broader collection-expression work. <c>System.Threading.Lock</c> and
/// the new <c>field</c> keyword in property accessors are explicit
/// non-goals (see <c>docs/language/csharp9-13-status.md</c>).
/// </summary>
public class Lang13Features
{
    // C# 13 — escape sequence \e for ESC (0x1B). Roslyn folds it into a
    // constant char or string at lex time; bound tree sees a literal.
    public static void EscapeSequenceEsc()
    {
        char esc = '\e';
        Console.WriteLine(((int)esc).ToString());

        string ansiClear = "\e[2J";
        Console.WriteLine(ansiClear.Length.ToString());
    }

    // C# 13 — method group natural type. Roslyn synthesizes the delegate
    // type at bind time; the bound expression is identical to one written
    // with an explicit delegate type.
    public static void MethodGroupNaturalType()
    {
        var f = ProduceInt;
        Console.WriteLine(f().ToString());

        var g = TakeInt;
        g(42);
    }

    private static int ProduceInt() => 99;

    private static void TakeInt(int x) => Console.WriteLine(x.ToString());
}
