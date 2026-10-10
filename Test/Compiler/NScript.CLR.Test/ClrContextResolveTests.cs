//-----------------------------------------------------------------------
// <copyright file="ClrContextResolveTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;

    /// <summary>
    /// A reference into another loaded assembly must resolve to that assembly's loaded
    /// TypeDefinition, not to a second copy the resolver reads from disk. Two copies of one
    /// type made the converter emit an interface twice.
    /// </summary>
    [TestClass]
    public class ClrContextResolveTests
    {
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [TestMethod]
        public void Resolve_CrossAssemblyReference_ReturnsTheLoadedTypeDefinition()
        {
            var context = TestAssemblyLoader.Context;
            var loadedObject = context.GetTypeDefinition(Tuple.Create("mscorlib", "System.Object"));

            ModuleDefinition realScript;
            Assert.IsTrue(context.TryGetModuleDefinition("RealScript", out realScript));
            var objectReference = realScript.Types
                .Select(type => type.BaseType)
                .First(baseType => baseType != null
                    && !(baseType is TypeDefinition)
                    && baseType.FullName == "System.Object");

            Assert.AreSame(loadedObject, context.Resolve(objectReference));
            Assert.AreSame(loadedObject, objectReference.Resolve());
        }

        /// <summary>
        /// The context caches what Cecil resolves, but the converter makes new reference
        /// objects every build. A build service resolving them build after build must not keep
        /// them alive: with a strong cache the daemon grew about 20 MB per save.
        /// </summary>
        [TestMethod]
        public void Resolve_CachedReference_DoesNotOutliveItsCaller()
        {
            using var context = new ClrContext();
            context.LoadAssembly(Path.Combine(Csc.Lib.Test.TestResources.FixtureDirectory, "mscorlib.dll"));
            Assert.IsTrue(context.TryGetModuleDefinition("mscorlib", out var mscorlib));
            var reference = ResolveNewReference(mscorlib);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(reference.IsAlive, "The resolver cache kept a reference the caller dropped.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference ResolveNewReference(ModuleDefinition mscorlib)
        {
            var objectType = mscorlib.GetType("System.Object");
            var toString = new MethodReference("ToString", mscorlib.TypeSystem.String, new TypeReference("System", "Object", mscorlib, mscorlib))
            {
                HasThis = true,
            };
            Assert.AreSame(objectType.Methods.Single(method => method.Name == "ToString" && !method.HasParameters), toString.Resolve());
            return new WeakReference(toString);
        }
    }
}
