//-----------------------------------------------------------------------
// <copyright file="Lang14Features.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

using System;

/// <summary>
/// The receiver type for the C# 14 extension members in <see cref="Lang14Extensions"/>.
/// </summary>
public class Lang14Box
{
    public int V;
}

/// <summary>
/// A generic receiver type for the generic extension block.
/// </summary>
public class Lang14Holder<T>
{
    public T Value;
}

/// <summary>
/// C# 14 instance compound assignment operators (#116).
/// </summary>
public class Lang14Acc
{
    public int Total;

    public void operator +=(int x) { Total += x; }

    public void operator ++() { Total += 100; }

    public void operator --() { Total -= 100; }
}

/// <summary>
/// Only static operators, so <c>+=</c> and <c>++</c> fall back to them.
/// </summary>
public class Lang14Money
{
    public int Cents;

    public static Lang14Money operator +(Lang14Money a, int b) => new Lang14Money { Cents = a.Cents + b };

    public static Lang14Money operator ++(Lang14Money a) => new Lang14Money { Cents = a.Cents + 1 };
}

/// <summary>
/// C# 14 extension members (#115). Roslyn emits each member as a static
/// implementation method on this class, with the receiver as the first parameter.
/// </summary>
public static class Lang14Extensions
{
    extension(Lang14Box b)
    {
        public int Twice => b.V * 2;

        public int AddTo(int x) => b.V + x;

        public static Lang14Box Make(int v) => new Lang14Box { V = v };

        public int Half
        {
            get => b.V / 2;
            set => b.V = value * 2;
        }

        public void operator +=(int x) { b.V += x; }
    }

    extension<T>(Lang14Holder<T> h)
    {
        public T Get() => h.Value;
    }
}

/// <summary>
/// Converter fixtures for C# 14 features.
/// </summary>
public class Lang14Features
{
    public int ExtensionProperty(Lang14Box b) => b.Twice;

    public int ExtensionMethod(Lang14Box b) => b.AddTo(3);

    public int ExtensionStaticMethod() => Lang14Box.Make(4).V;

    public void ExtensionSetter(Lang14Box b)
    {
        b.Half = 5;
    }

    public string ExtensionGeneric(Lang14Holder<string> h) => h.Get();

    public void CompoundInstance(Lang14Acc a)
    {
        a += 5;
    }

    public void IncrementInstance(Lang14Acc a)
    {
        a++;
        --a;
    }

    public Lang14Money CompoundStaticFallback(Lang14Money m)
    {
        m += 5;
        return m;
    }

    public Lang14Money IncrementStatic(Lang14Money m)
    {
        m++;
        return m;
    }

    public void CompoundExtension(Lang14Box b)
    {
        b += 7;
    }
}
