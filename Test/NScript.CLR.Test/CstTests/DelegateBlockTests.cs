//-----------------------------------------------------------------------
// <copyright file="DelegateBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;

    /// <summary>
    /// Definition for DelegateBlockTests
    /// </summary>
    [TestFixture]
    public class DelegateBlockTests
    {
        private const string TestClassNameStr = "DelegateBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.DelegateBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegate.csast")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegate.csast")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegate.csast")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegate.csast")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.csast")]
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
