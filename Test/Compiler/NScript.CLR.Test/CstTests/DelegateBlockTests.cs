//-----------------------------------------------------------------------
// <copyright file="DelegateBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for DelegateBlockTests
    /// </summary>
    [TestClass]
    public class DelegateBlockTests
    {
        private const string TestClassNameStr = "DelegateBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.DelegateBlocks.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegate.csast")]
        [DataRow(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegate.csast")]
        [DataRow(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegate.csast")]
        [DataRow(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegate.csast")]
        [DataRow(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                DelegateBlockTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }
    }
}
