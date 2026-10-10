//-----------------------------------------------------------------------
// <copyright file="ClrContextInterfaceOverridesTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The context computes a type's interface overrides once and keeps them only as long as
    /// the context itself, so a dropped context frees its modules once the build ends.
    /// </summary>
    [TestClass]
    public class ClrContextInterfaceOverridesTests
    {
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [TestMethod]
        public void GetInterfaceOverrides_RepeatedCall_ReturnsTheSameMap()
        {
            using var context = LoadMscorlib();
            var arrayList = context.GetTypeDefinition(Tuple.Create("mscorlib", "System.Collections.ArrayList"));

            var first = arrayList.GetInterfaceOverrides(context);

            Assert.IsTrue(first.Count > 0, "ArrayList implements interfaces, so it has overrides.");
            Assert.AreSame(first, arrayList.GetInterfaceOverrides(context));
        }

        [TestMethod]
        public void GetInterfaceOverrides_ContextDisposed_ModuleIsCollected()
        {
            var module = UseAndDropContext();

            // Between builds the service clears the comparer's hash cache, which is keyed by
            // Cecil objects (Builder.ResetProcessState). Do the same, so only the context's
            // own caches are under test.
            MemberReferenceComparer.Instance.ClearCache();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(module.IsAlive, "A cached interface-override map outlived its context.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference UseAndDropContext()
        {
            using var context = LoadMscorlib();
            var arrayList = context.GetTypeDefinition(Tuple.Create("mscorlib", "System.Collections.ArrayList"));
            Assert.IsTrue(arrayList.GetInterfaceOverrides(context).Count > 0);
            return new WeakReference(arrayList.Module);
        }

        private static ClrContext LoadMscorlib()
        {
            var context = new ClrContext();
            context.LoadAssembly(Path.Combine(Csc.Lib.Test.TestResources.FixtureDirectory, "mscorlib.dll"));
            return context;
        }
    }
}
