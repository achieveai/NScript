//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

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
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArray.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", false, "ReturnInlineObjectArray.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "TestVarArgs", false, "TestVarArgs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", false, "ReturnInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", false, "CallMethodWithInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", false, "ReturnInlineConstIntArray.xml")]
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
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", true,  "ReturnInlineObjectArrayMcs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineObjectArray", false, "ReturnInlineObjectArrayMcs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "TestVarArgs", true,  "TestVarArgs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "TestVarArgs", false, "TestVarArgs.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", true,  "ReturnInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineSettersElement", false, "ReturnInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", true,  "CallMethodWithInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "CallMethodWithInlineSettersElement", false, "CallMethodWithInlineSettersElement.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", true,  "ReturnInlineConstIntArray.xml")]
        [Row(InlineInitializationTests.TestClassNameStr, "ReturnInlineConstIntArray", false, "ReturnInlineConstIntArray.xml")]
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

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}