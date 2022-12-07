//-----------------------------------------------------------------------
// <copyright file="SimpleConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SimpleConverters
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.SimpleConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow("ReturnInt", "ReturnInt.js", TestType.All)]
        [DataRow("ReturnChar", "ReturnChar.js", TestType.All)]
        [DataRow("AddIntArgs", "AddIntArgs.js", TestType.All)]
        [DataRow("AddIntArgToConst", "AddIntArgToConst.js", TestType.All)]
        [DataRow("IntDivideInts", "IntDivideInts.js", TestType.All)]
        [DataRow("ReturnMethodCall", "ReturnMethodCall.js", TestType.All)]
        [DataRow("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All)]
        [DataRow("ReturnNewObject", "ReturnNewObject.js", TestType.All)]
        [DataRow("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All)]
        [DataRow("ReturnIntArray", "ReturnIntArray.js", TestType.All)]
        [DataRow("ReturnObjectArray", "ReturnObjectArray.js", TestType.All)]
        [DataRow("ReturnArrayElement", "ReturnArrayElement.js", TestType.All)]
        [DataRow("SetArrayElement", "SetArrayElement.js", TestType.All)]
        [DataRow("CheckType", "CheckType.js", TestType.All)]
        [DataRow("TestSimpleCompare", "TestSimpleCompare.js", TestType.All)]
        [DataRow("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All)]
        [DataRow("TestFourPartCondition", "FourPartCondition.js", TestType.All)]
        [DataRow("TestSimpleConditional", "SimpleConditional.js", TestType.All)]
        [DataRow("TestComplexCondition", "ComplexCondition.js", TestType.All)]
        [DataRow("PassVariableByRef", "PassVariableByRef.js", TestType.All)]
        [DataRow("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All)]
        [DataRow("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All)]
        [DataRow("AccessRefArgument", "AccessRefArgument.js", TestType.All)]
        [DataRow("AddDelegates", "AddDelegates.js", TestType.All)]
        [DataRow("AddAssignDelegates", "AddAssignDelegates.js", TestType.All)]
        [DataRow("SubDelegates", "SubDelegates.js", TestType.All)]
        [DataRow("SubAssignDelegates", "SubAssignDelegates.js", TestType.All)]
        [DataRow("IndexerSet", "IndexerSet.js", TestType.All)]
        [DataRow("IndexerGet", "IndexerGet.js", TestType.All)]
        public void TestMcs(string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + resourceName,
                SimpleConverters.TestClassNameStr,
                methodName,
                testType,
                true);
        }

        [DataTestMethod]
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