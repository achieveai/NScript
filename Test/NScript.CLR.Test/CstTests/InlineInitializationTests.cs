//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Definition for InlineInitializationTests
    /// </summary>
    [TestFixture]
    public class InlineInitializationTests
    {
        private const string TestClassNameStr = "InlineComplexStatements";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.InlineInitializations.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArray.csast")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.csast")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.csast")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.csast")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                InlineInitializationTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        // [Test]
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