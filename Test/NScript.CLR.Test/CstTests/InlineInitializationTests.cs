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
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArray.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", false, "ReturnInlineObjectArray.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "TestVarArgs", false, "TestVarArgs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", false, "ReturnInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", false, "CallMethodWithInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", false, "ReturnInlineConstIntArray.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                InlineInitializationTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArrayMcs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", false, "ReturnInlineObjectArrayMcs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "TestVarArgs", false, "TestVarArgs.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", false, "ReturnInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", false, "CallMethodWithInlineSettersElement.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.xml")]
        [TestCase(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", false, "ReturnInlineConstIntArray.xml")]
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