//-----------------------------------------------------------------------
// <copyright file="DelegateBlockTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

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
        [Row(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", false, "StaticReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", false, "InstanceReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", false, "LocalReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", false, "LocalAndInstanceReferencingDelegate.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", false, "IntDelegateTaker.xml")]
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
        [Row(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", true,  "StaticReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "StaticReferencingDelegate", false, "StaticReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", true,  "InstanceReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "InstanceReferencingDelegate", false, "InstanceReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", true,  "LocalReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalReferencingDelegate", false, "LocalReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", true,  "LocalAndInstanceReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "LocalAndInstanceReferencingDelegate", false, "LocalAndInstanceReferencingDelegateMcs.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", true,  "IntDelegateTaker.xml")]
        [Row(DelegateBlockTests.TestClassNameStr, "IntDelegateTaker", false, "IntDelegateTaker.xml")]
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
