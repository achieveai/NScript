//-----------------------------------------------------------------------
// <copyright file="NumberOperationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Definition for NumberOperationTests
    /// </summary>
    public class NumberOperationTests
    {
        private const string TestClassNameStr = "NumberOperations";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.NumberOperations.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(NumberOperationTests.TestClassNameStr, "Div", true,  "IntDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "Div", false,  "IntDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "DoubleDivide", true,   "DoubleDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "DoubleDivide", false,  "DoubleDivide.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                NumberOperationTests.ResourceFileNamepace + resourceName,
                TestHelpers.GetAST(testClassName, methodName, isDebug).RootBlock);
        }

        [Test]
        [TestCase(NumberOperationTests.TestClassNameStr, "Div", true,  "IntDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "Div", false,  "IntDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "DoubleDivide", true,   "DoubleDivide.xml")]
        [TestCase(NumberOperationTests.TestClassNameStr, "DoubleDivide", false,  "DoubleDivide.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                NumberOperationTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }
    }
}
