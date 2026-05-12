//-----------------------------------------------------------------------
// <copyright file="Lang9PatternExecutionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// V8 runtime execution coverage for the C# 9-10 pattern-matching family.
    /// Mirrors the harness shape of <c>Lang8FeatureExecutionTests</c>: a single
    /// <c>Main()</c> drives feature-specific helpers whose <c>Console.WriteLine</c>
    /// output is captured by <c>JsConsole</c> and snapshot-compared.
    ///
    /// Output convention: helpers emit string tokens (e.g. "rel:y" / "rel:n")
    /// so the snapshot stays stable regardless of <c>bool.ToString()</c>
    /// casing differences between .NET and the JS runtime.
    /// </summary>
    public class Lang9PatternExecutionTests
    {
        public static void Main()
        {
            TestRelationalPattern();
            TestBinaryAndPattern();
            TestBinaryOrPattern();
            TestNegatedPattern();
            TestParenthesizedConstantPattern();
            TestSwitchRelationalArm();
            TestTypeTestPattern();
        }

        private static void TestRelationalPattern()
        {
            int x = 10;
            Console.WriteLine(x is > 5 ? "rel:y" : "rel:n");
            Console.WriteLine(x is > 50 ? "rel:y" : "rel:n");
        }

        private static void TestBinaryAndPattern()
        {
            int a = 50;
            Console.WriteLine(a is > 0 and < 100 ? "and:y" : "and:n");
            int b = 150;
            Console.WriteLine(b is > 0 and < 100 ? "and:y" : "and:n");
        }

        private static void TestBinaryOrPattern()
        {
            int a = -5;
            Console.WriteLine(a is < 0 or > 100 ? "or:y" : "or:n");
            int b = 50;
            Console.WriteLine(b is < 0 or > 100 ? "or:y" : "or:n");
        }

        private static void TestNegatedPattern()
        {
            int a = 42;
            Console.WriteLine(a is not 42 ? "not:y" : "not:n");
            int b = 43;
            Console.WriteLine(b is not 42 ? "not:y" : "not:n");
        }

        private static void TestParenthesizedConstantPattern()
        {
            object a = 5;
            Console.WriteLine(a is (5) ? "paren:y" : "paren:n");
            object b = 7;
            Console.WriteLine(b is (5) ? "paren:y" : "paren:n");
        }

        private static void TestSwitchRelationalArm()
        {
            Console.WriteLine(Sign(-10));
            Console.WriteLine(Sign(0));
            Console.WriteLine(Sign(10));
        }

        private static int Sign(int x) => x switch
        {
            < 0 => -1,
            0 => 0,
            _ => 1
        };

        private static void TestTypeTestPattern()
        {
            BaseClass d = new Derived();
            Console.WriteLine(d is Derived ? "type:y" : "type:n");
            BaseClass b = new BaseClass();
            Console.WriteLine(b is Derived ? "type:y" : "type:n");
        }

        private class BaseClass { }

        private class Derived : BaseClass { }
    }
}
