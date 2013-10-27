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
        [TestCase(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", false, "StaticReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", false, "InstanceReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", false, "LocalReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", false, "LocalAndInstanceReferencingDelegate.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", false, "IntDelegateTaker.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                DelegateBlockTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [TestCase(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", false, "StaticReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", false, "InstanceReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", false, "LocalReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", false, "LocalAndInstanceReferencingDelegateMcs.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.xml")]
        [TestCase(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", false, "IntDelegateTaker.xml")]
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
