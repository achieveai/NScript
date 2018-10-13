//-----------------------------------------------------------------------
// <copyright file="SimpleConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    [TestFixture]
    public class SimpleConverters
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.SimpleConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase("ReturnInt", "ReturnInt.js", TestType.All)]
        [TestCase("AddIntArgs", "AddIntArgs.js", TestType.All)]
        [TestCase("AddIntArgToConst", "AddIntArgToConst.js", TestType.All)]
        [TestCase("IntDivideInts", "IntDivideInts.js", TestType.All)]
        [TestCase("ReturnMethodCall", "ReturnMethodCall.js", TestType.All)]
        [TestCase("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All)]
        [TestCase("ReturnNewObject", "ReturnNewObject.js", TestType.All)]
        [TestCase("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All)]
        [TestCase("ReturnIntArray", "ReturnIntArray.js", TestType.All)]
        [TestCase("ReturnObjectArray", "ReturnObjectArray.js", TestType.All)]
        [TestCase("ReturnArrayElement", "ReturnArrayElement.js", TestType.All)]
        [TestCase("SetArrayElement", "SetArrayElement.js", TestType.All)]
        [TestCase("CheckType", "CheckType.js", TestType.All)]
        [TestCase("TestSimpleCompare", "TestSimpleCompare.js", TestType.All)]
        [TestCase("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All)]
        [TestCase("TestFourPartCondition", "FourPartCondition.js", TestType.All)]
        [TestCase("TestSimpleConditional", "SimpleConditional.js", TestType.All)]
        [TestCase("TestComplexCondition", "ComplexCondition.js", TestType.All)]
        [TestCase("PassVariableByRef", "PassVariableByRef.js", TestType.All)]
        [TestCase("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All)]
        [TestCase("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All)]
        [TestCase("AccessRefArgument", "AccessRefArgument.js", TestType.All)]
        [TestCase("AddDelegates", "AddDelegates.js", TestType.All)]
        [TestCase("AddAssignDelegates", "AddAssignDelegates.js", TestType.All)]
        [TestCase("SubDelegates", "SubDelegates.js", TestType.All)]
        [TestCase("SubAssignDelegates", "SubAssignDelegates.js", TestType.All)]
        [TestCase("IndexerSet", "IndexerSet.js", TestType.All)]
        [TestCase("IndexerGet", "IndexerGet.js", TestType.All)]
        public void TestMcs(string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + resourceName,
                SimpleConverters.TestClassNameStr,
                methodName,
                testType,
                true);
        }

        [Test]
        public void TestComplexConditionMethod()
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + "Complex3IfCondition.js",
                "BasicBlockTestFunctions",
                "Complex3IfCondition",
                TestType.All);
        }
    }
}