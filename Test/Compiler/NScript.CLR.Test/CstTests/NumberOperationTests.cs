//-----------------------------------------------------------------------
// <copyright file="NumberOperationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for NumberOperationTests
    /// </summary>
    public class NumberOperationTests
    {
        private const string TestClassNameStr = "NumberOperations";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.NumberOperations.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(NumberOperationTests.TestClassNameStr, "Div", true,  "IntDivide.csast")]
        [DataRow(NumberOperationTests.TestClassNameStr, "DoubleDivide", true,   "DoubleDivide.csast")]
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
