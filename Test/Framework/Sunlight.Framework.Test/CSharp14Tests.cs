//-----------------------------------------------------------------------
// <copyright file="CSharp14Tests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using System.Collections.Generic;
    using SunlightUnit;

    public class Cs14Box
    {
        public int V;
    }

    public class Cs14Holder<T>
    {
        public T Value;
    }

    /// <summary>
    /// C# 14 extension members. Extension blocks must sit in a top-level
    /// static class.
    /// </summary>
    public static class Cs14Extensions
    {
        extension(Cs14Box b)
        {
            public int Twice => b.V * 2;

            public int AddTo(int x) => b.V + x;

            public static Cs14Box Make(int v) => new Cs14Box { V = v };

            public int Half
            {
                get => b.V / 2;
                set => b.V = value * 2;
            }

            public void operator +=(int x) { b.V += x; }
        }

        extension<T>(Cs14Holder<T> h)
        {
            public T Get() => h.Value;
        }
    }

    /// <summary>
    /// Browser tests for every C# 14 feature NScript supports (#117).
    /// </summary>
    [TestFixture]
    public class CSharp14Tests
    {
        private class FieldHolder
        {
            public int Doubled { get => field; set => field = value * 2; }

            public string Name { get; set => field = value ?? "none"; } = "init";
        }

        private class Node
        {
            public int V;
            public Node Next;
            public int P { get; set; }
        }

        private delegate bool TryGet(string s, out int v);

        private delegate void Bump(ref int x);

        private partial class Widget
        {
            public partial Widget(int v);

            public partial event Action Changed;
        }

        private partial class Widget
        {
            public int V;
            private Action handlers;

            public partial Widget(int v) { V = v; }

            public partial event Action Changed { add { handlers += value; } remove { handlers -= value; } }

            public void Raise() => handlers?.Invoke();
        }

        private class Acc
        {
            public int Total;

            public void operator +=(int x) { Total += x; }

            public void operator ++() { Total += 100; }

            public void operator --() { Total -= 100; }
        }

        private class Money
        {
            public int Cents;

            public static Money operator +(Money a, int b) => new Money { Cents = a.Cents + b };

            public static Money operator ++(Money a) => new Money { Cents = a.Cents + 1 };
        }

        private class Wallet
        {
            public Money M = new Money();

            public int SelfReads;

            public Wallet Self
            {
                get
                {
                    SelfReads++;
                    return this;
                }
            }
        }

        [Test]
        public static void FieldKeyword(Assert assert)
        {
            var h = new FieldHolder();
            h.Doubled = 4;
            assert.Equal(h.Doubled, 8, "setter writes the backing field");
            assert.Equal(h.Name, "init", "initializer sets the backing field");
            h.Name = null;
            assert.Equal(h.Name, "none", "setter logic runs");
        }

        [Test]
        public static void NullConditionalAssignment(Assert assert)
        {
            var n = new Node();
            n?.V = 7;
            assert.Equal(n.V, 7, "assigns when not null");
            n?.P += 2;
            assert.Equal(n.P, 2, "compound assigns when not null");

            Node none = null;
            int calls = 0;
            none?.V = Count(ref calls);
            assert.Equal(calls, 0, "right side is skipped when null");
            n.Next?.V = 9;
            assert.IsTrue(n.Next == null, "nested null is skipped");
        }

        [Test]
        public static void NameofUnboundGeneric(Assert assert)
        {
            assert.Equal(nameof(List<>), "List", "unbound generic type");
            assert.Equal(nameof(Dictionary<,>.Count), "Count", "member of unbound generic");
        }

        [Test]
        public static void SimpleLambdaParameterModifiers(Assert assert)
        {
            TryGet t = (s, out v) => { v = s.Length; return true; };
            assert.IsTrue(t("abc", out var len), "out lambda returns");
            assert.Equal(len, 3, "out parameter is set");

            Bump b = (ref x) => x += 10;
            int y = 1;
            b(ref y);
            assert.Equal(y, 11, "ref parameter is updated");
        }

        [Test]
        public static void PartialConstructorAndEvent(Assert assert)
        {
            var w = new Widget(4);
            assert.Equal(w.V, 4, "partial constructor");
            int hits = 0;
            w.Changed += () => hits++;
            w.Raise();
            assert.Equal(hits, 1, "partial event");
        }

        [Test]
        public static void ExtensionMembers(Assert assert)
        {
            var b = Cs14Box.Make(5);
            assert.Equal(b.V, 5, "static extension method");
            assert.Equal(b.Twice, 10, "extension property");
            assert.Equal(b.AddTo(3), 8, "extension method");
            b.Half = 6;
            assert.Equal(b.V, 12, "extension setter");
            assert.Equal(b.Half, 6, "extension getter");
            b += 1;
            assert.Equal(b.V, 13, "extension compound operator");

            var h = new Cs14Holder<string> { Value = "x" };
            assert.Equal(h.Get(), "x", "generic extension block");
        }

        [Test]
        public static void UserDefinedCompoundAssignment(Assert assert)
        {
            var a = new Acc();
            a += 5;
            assert.Equal(a.Total, 5, "instance +=");
            a++;
            assert.Equal(a.Total, 105, "instance ++");
            --a;
            assert.Equal(a.Total, 5, "instance --");

            var m = new Money();
            m += 5;
            assert.Equal(m.Cents, 5, "+= falls back to static +");
            m++;
            assert.Equal(m.Cents, 6, "++ falls back to static ++");
        }

        [Test]
        public static void CompoundAssignmentShapes(Assert assert)
        {
            var b = Cs14Box.Make(4);
            b.Half += 3;
            assert.Equal(b.V, 10, "extension property +=");
            b.Half++;
            assert.Equal(b.V, 12, "extension property ++");
            int old = b.Half++;
            assert.Equal(old, 6, "extension property postfix yields the old value");
            assert.Equal(b.V, 14, "extension property postfix stores");
            assert.Equal(b.Half -= 1, 6, "extension property compound yields the new value");

            var a = new Acc();
            var r = a += 5;
            assert.IsTrue(r == a, "instance += yields the target");
            assert.Equal(a.Total, 5, "instance += used as a value runs once");
            assert.IsTrue(++a == a, "instance prefix ++ yields the target");
            assert.Equal(a.Total, 105, "instance prefix ++ used as a value runs once");

            int picks = 0;
            var w = new Wallet();
            Func<Wallet> pick = () => { picks++; return w; };
            pick().M += 5;
            pick().M++;
            assert.Equal(w.M.Cents, 6, "static fallback on a call receiver");
            assert.Equal(picks, 2, "the receiver is evaluated once per operator");

            var m = new Money();
            var before = m++;
            assert.Equal(before.Cents, 0, "static postfix ++ yields the old value");
            assert.Equal(m.Cents, 1, "static postfix ++ stores the new value");
        }

        [Test]
        public static void CompoundAssignmentEvaluationOrder(Assert assert)
        {
            var b = Cs14Box.Make(0);
            int v = b.Half = 5;
            assert.Equal(v, 5, "extension property assignment yields the value");
            assert.Equal(b.V, 10, "extension property assignment stores");

            var m = Cs14Box.Make(5);
            int mv = m.Half = m.V;
            assert.Equal(mv, 5, "the value is read before the setter changes it");
            assert.Equal(m.V, 10, "the setter got the old value");

            var w = new Wallet();
            w.Self.M += 5;
            assert.Equal(w.M.Cents, 5, "static fallback through a property receiver");
            assert.Equal(w.SelfReads, 1, "the property receiver is read once");

            var first = new[] { new Money() };
            var xs = first;
            Func<int> shift = () => { xs = new[] { new Money() }; return 0; };
            xs[shift()] += 5;
            assert.Equal(first[0].Cents, 5, "the array is read before the index runs");
            assert.Equal(xs[0].Cents, 0, "the new array is untouched");

            var box = Cs14Box.Make(4);
            var kept = box;
            Func<int> rebind = () => { box = Cs14Box.Make(0); return 1; };
            int half = box.Half += rebind();
            assert.Equal(half, 3, "extension property += yields the new value");
            assert.Equal(kept.V, 6, "the receiver is read before the right side runs");
            assert.Equal(box.V, 0, "the rebound receiver is untouched");

            var a = new Acc();
            var original = a;
            Func<int> swap = () => { a = new Acc(); return 1; };
            var r = a += swap();
            assert.IsTrue(r == original, "instance += yields the target read before the argument");
            assert.Equal(original.Total, 1, "instance += runs on the original target");
        }

        private static int Count(ref int c)
        {
            c++;
            return 1;
        }
    }
}
