//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    /// <summary>
    /// Definition for InlineInitializationTests
    /// </summary>
    [TestClass]
    public class InlineInitializationTests
    {
        private const string TestClassNameStr = "InlineComplexStatements";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.InlineInitializations.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArray.csast")]
        [DataRow(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.csast")]
        [DataRow(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.csast")]
        [DataRow(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.csast")]
        [DataRow(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                InlineInitializationTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        // [DataTestMethod]
        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                "System.Collections.Generic.StringDictionary`1",
                "Add",
                true).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}