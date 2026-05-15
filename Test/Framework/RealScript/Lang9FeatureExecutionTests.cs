//-----------------------------------------------------------------------
// <copyright file="Lang9FeatureExecutionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// V8 runtime execution coverage for non-pattern C# 9 features that
    /// need observable runtime behaviour beyond a compile-only fixture.
    /// Mirrors <c>Lang8FeatureExecutionTests</c>: a single <c>Main()</c>
    /// drives feature-specific helpers whose <c>Console.WriteLine</c>
    /// output is captured by <c>JsConsole</c> and snapshot-compared.
    /// </summary>
    public class Lang9FeatureExecutionTests
    {
        public static void Main()
        {
            TestTargetTypedNewParameterless();
            TestTargetTypedNewWithInitializer();
            TestTargetTypedNewWithArgs();
            TestTargetTypedNewAsReturn();
            TestModuleInitializerRanBeforeMain();
            TestModuleInitializerOrderingAcrossTypes();
        }

        // Each branch below exercises one shape of `new()` Roslyn surfaces as
        // `ConversionKind.ObjectCreation` to `VisitConversion`. The Stage 1
        // pass-through arm forwards to the inner `BoundObjectCreationExpression`
        // so the ordinary constructor pipeline emits the constructed instance.
        private static void TestTargetTypedNewParameterless()
        {
            Holder h = new();
            Console.WriteLine("ttn:par:" + h.Value);
        }

        private static void TestTargetTypedNewWithInitializer()
        {
            Holder h = new() { Value = 11 };
            Console.WriteLine("ttn:init:" + h.Value);
        }

        private static void TestTargetTypedNewWithArgs()
        {
            Holder h = new(42);
            Console.WriteLine("ttn:args:" + h.Value);
        }

        private static void TestTargetTypedNewAsReturn()
        {
            Console.WriteLine("ttn:ret:" + Make().Value);
        }

        private static Holder Make() => new(7);

        // `[ModuleInitializer]` static methods on `ModuleInitTrackerA` /
        // `ModuleInitTrackerB` are emitted before the entry point by
        // `Builder.cs`. The first assertion verifies the call ran; the
        // second verifies deterministic ordering across types — both
        // initializers append to the same buffer in declared order.
        private static void TestModuleInitializerRanBeforeMain()
        {
            Console.WriteLine("mi:ran:" + ModuleInitTrackerA.Marker);
        }

        private static void TestModuleInitializerOrderingAcrossTypes()
        {
            Console.WriteLine("mi:order:" + ModuleInitTrackerB.Trace);
        }

        public class Holder
        {
            public int Value { get; set; }

            public Holder() { Value = 1; }

            public Holder(int v) { Value = v; }
        }
    }

    internal static class ModuleInitTrackerA
    {
        public static int Marker = 0;

        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void Init()
        {
            Marker = 99;
            ModuleInitTrackerB.Trace += "A";
        }
    }

    internal static class ModuleInitTrackerB
    {
        public static string Trace = string.Empty;

        [System.Runtime.CompilerServices.ModuleInitializer]
        public static void Init()
        {
            Trace += "B";
        }
    }
}
