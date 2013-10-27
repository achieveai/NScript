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
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", false, "TryFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturn.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", false, "TryFinallyWithReturnRetail.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", false, "TryCatchSimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturnDebug.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", false, "TryCatchWithReturn.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", false, "TryCatchFinallySimple.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ExceptionHandlingTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", false, "TryFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturnMcs.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", false, "TryFinallyWithReturnMcs.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", false, "TryCatchSimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturnMcs.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", false, "TryCatchWithReturnMcs.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.xml")]
        [TestCase(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", false, "TryCatchFinallySimple.xml")]
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
