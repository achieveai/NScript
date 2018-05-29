//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Definition for ExceptionHandlingTests
    /// </summary>
    [TestFixture]
    public class ExceptionHandlingTests
    {
        private const string TestClassNameStr = "ExceptionHandlerSamples";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.ExceptionHandling.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.csast")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturn.csast")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.csast")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturn.csast")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ExceptionHandlingTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        [Test]
        [Timeout(20000)]
        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                "TryCatchFinallySimple",
                true).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
