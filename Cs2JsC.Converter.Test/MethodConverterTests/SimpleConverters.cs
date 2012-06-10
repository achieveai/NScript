//-----------------------------------------------------------------------
// <copyright file="SimpleConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    [TestFixture]
    public class SimpleConverters
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.SimpleConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row("ReturnInt", "ReturnInt.js", TestType.All)]
        [Row("AddIntArgs", "AddIntArgs.js", TestType.All)]
        [Row("AddIntArgToConst", "AddIntArgToConst.js", TestType.All)]
        [Row("ReturnMethodCall", "ReturnMethodCall.js", TestType.All)]
        [Row("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All)]
        [Row("ReturnNewObject", "ReturnNewObject.js", TestType.All)]
        [Row("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All)]
        [Row("ReturnIntArray", "ReturnIntArray.js", TestType.All)]
        [Row("ReturnObjectArray", "ReturnObjectArray.js", TestType.All)]
        [Row("ReturnArrayElement", "ReturnArrayElement.js", TestType.All)]
        [Row("SetArrayElement", "SetArrayElement.js", TestType.All)]
        [Row("CheckType", "CheckType.js", TestType.All)]
        [Row("TestSimpleCompare", "TestSimpleCompare.js", TestType.All)]
        [Row("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All)]
        [Row("TestFourPartCondition", "FourPartCondition.js", TestType.All)]
        [Row("TestSimpleConditional", "SimpleConditional.js", TestType.All)]
        [Row("TestComplexCondition", "ComplexCondition.js", TestType.All)]
        [Row("PassVariableByRef", "PassVariableByRef.js", TestType.All)]
        [Row("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All)]
        [Row("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All)]
        [Row("AccessRefArgument", "AccessRefArgument.js", TestType.All)]
        public void Test(string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + resourceName,
                SimpleConverters.TestClassNameStr,
                methodName,
                testType);
        }

        [Test]
        [Row("ReturnInt", "ReturnInt.js", TestType.All)]
        [Row("AddIntArgs", "AddIntArgs.js", TestType.All)]
        [Row("AddIntArgToConst", "AddIntArgToConst.js", TestType.All)]
        [Row("ReturnMethodCall", "ReturnMethodCall.js", TestType.All)]
        [Row("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All)]
        [Row("ReturnNewObject", "ReturnNewObject.js", TestType.All)]
        [Row("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All)]
        [Row("ReturnIntArray", "ReturnIntArray.js", TestType.All)]
        [Row("ReturnObjectArray", "ReturnObjectArray.js", TestType.All)]
        [Row("ReturnArrayElement", "ReturnArrayElement.js", TestType.All)]
        [Row("SetArrayElement", "SetArrayElement.js", TestType.All)]
        [Row("CheckType", "CheckType.js", TestType.All)]
        [Row("TestSimpleCompare", "TestSimpleCompare.js", TestType.All)]
        [Row("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All)]
        [Row("TestFourPartCondition", "FourPartCondition.js", TestType.All)]
        [Row("TestSimpleConditional", "SimpleConditional.js", TestType.All)]
        [Row("TestComplexCondition", "ComplexCondition.js", TestType.All)]
        [Row("PassVariableByRef", "PassVariableByRef.js", TestType.All)]
        [Row("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All)]
        [Row("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All)]
        [Row("AccessRefArgument", "AccessRefArgument.js", TestType.All)]
        [Row("AddDelegates", "AddDelegates.js", TestType.All)]
        [Row("AddAssignDelegates", "AddAssignDelegates.js", TestType.All)]
        [Row("SubDelegates", "SubDelegates.js", TestType.All)]
        [Row("SubAssignDelegates", "SubAssignDelegates.js", TestType.All)]
        [Row("IndexerSet", "IndexerSet.js", TestType.All)]
        [Row("IndexerGet", "IndexerGet.js", TestType.All)]
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