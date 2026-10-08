//-----------------------------------------------------------------------
// <copyright file="ClrContextResolveTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System;
    using System.Linq;
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
    }
}
