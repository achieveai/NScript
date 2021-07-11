//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    /// <summary>
    /// Definition for ExceptionHandlingTests
    /// </summary>
    [TestClass]
    public class ExceptionHandlingTests
    {
        private const string TestClassNameStr = "ExceptionHandlerSamples";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.ExceptionHandling.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(ExceptionHandlingTests.TestClassNameStr, "TryFinallySimple", true,  "TryFinallySimple.csast")]
        [DataRow(ExceptionHandlingTests.TestClassNameStr, "TryFinallyWithReturn", true,  "TryFinallyWithReturn.csast")]
        [DataRow(ExceptionHandlingTests.TestClassNameStr, "TryCatchSimple", true,  "TryCatchSimple.csast")]
        [DataRow(ExceptionHandlingTests.TestClassNameStr, "TryCatchWithReturn", true,  "TryCatchWithReturn.csast")]
        [DataRow(ExceptionHandlingTests.TestClassNameStr, "TryCatchFinallySimple", true,  "TryCatchFinallySimple.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ExceptionHandlingTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }
   }
}
