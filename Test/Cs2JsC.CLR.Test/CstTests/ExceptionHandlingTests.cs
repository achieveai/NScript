//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for ExceptionHandlingTests
    /// </summary>
    [TestFixture]
    public class ExceptionHandlingTests
    {
        private const string TestClassNameStr = "ExceptionHandlerSamples";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.ExceptionHandling.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", false, "TryFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturn.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", false, "TryFinallyWithReturnRetail.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", false, "TryCatchSimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturnDebug.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", false, "TryCatchWithReturn.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", false, "TryCatchFinallySimple.xml")]
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
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", false, "TryFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturnMcs.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", false, "TryFinallyWithReturnMcs.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", false, "TryCatchSimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturnMcs.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", false, "TryCatchWithReturnMcs.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.xml")]
        [Row(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", false, "TryCatchFinallySimple.xml")]
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

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
