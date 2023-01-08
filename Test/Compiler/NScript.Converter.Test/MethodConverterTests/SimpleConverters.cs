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
        [DataRow("AccessRefArgument", "AccessRefArgument.js", TestType.All, false)]
        [DataRow("AccessRefArgument", "AccessRefArgument.js", TestType.All, true)]
        [DataRow("AddAssignDelegates", "AddAssignDelegates.js", TestType.All, false)]
        [DataRow("AddAssignDelegates", "AddAssignDelegates.js", TestType.All, true)]
        [DataRow("AddDelegates", "AddDelegates.js", TestType.All, false)]
        [DataRow("AddDelegates", "AddDelegates.js", TestType.All, true)]
        [DataRow("AddIntArgToConst", "AddIntArgToConst.js", TestType.All, false)]
        [DataRow("AddIntArgToConst", "AddIntArgToConst.js", TestType.All, true)]
        [DataRow("AddIntArgs", "AddIntArgs.js", TestType.All, false)]
        [DataRow("AddIntArgs", "AddIntArgs.js", TestType.All, true)]
        [DataRow("CheckType", "CheckType.js", TestType.All, false)]
        [DataRow("CheckType", "CheckType.static.js", TestType.All, true)]
        [DataRow("IndexerGet", "IndexerGet.js", TestType.All, false)]
        [DataRow("IndexerGet", "IndexerGet.js", TestType.All, true)]
        [DataRow("IndexerSet", "IndexerSet.js", TestType.All, false)]
        [DataRow("IndexerSet", "IndexerSet.js", TestType.All, true)]
        [DataRow("IntDivideInts", "IntDivideInts.js", TestType.All, false)]
        [DataRow("IntDivideInts", "IntDivideInts.js", TestType.All, true)]
        [DataRow("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All, false)]
        [DataRow("PassArrayElementByRef", "PassArrayElementByRef.js", TestType.All, true)]
        [DataRow("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All, false)]
        [DataRow("PassObjectFieldByRef", "PassObjectFieldByRef.js", TestType.All, true)]
        [DataRow("PassVariableByRef", "PassVariableByRef.js", TestType.All, false)]
        [DataRow("PassVariableByRef", "PassVariableByRef.js", TestType.All, true)]
        [DataRow("ReturnArrayElement", "ReturnArrayElement.js", TestType.All, false)]
        [DataRow("ReturnArrayElement", "ReturnArrayElement.js", TestType.All, true)]
        [DataRow("ReturnChar", "ReturnChar.js", TestType.All, false)]
        [DataRow("ReturnChar", "ReturnChar.js", TestType.All, true)]
        [DataRow("ReturnInt", "ReturnInt.js", TestType.All, false)]
        [DataRow("ReturnInt", "ReturnInt.js", TestType.All, true)]
        [DataRow("ReturnIntArray", "ReturnIntArray.js", TestType.All, false)]
        [DataRow("ReturnIntArray", "ReturnIntArray.js", TestType.All, true)]
        [DataRow("ReturnMethodCall", "ReturnMethodCall.js", TestType.All, false)]
        [DataRow("ReturnMethodCall", "ReturnMethodCall.js", TestType.All, true)]
        [DataRow("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All, false)]
        [DataRow("ReturnMethodCallWith1Arg", "ReturnMethodCallWith1Arg.js", TestType.All, true)]
        [DataRow("ReturnNewObject", "ReturnNewObject.js", TestType.All, false)]
        [DataRow("ReturnNewObject", "ReturnNewObject.js", TestType.All, true)]
        [DataRow("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All, false)]
        [DataRow("ReturnNewObjectWithArg", "ReturnNewObjectWithArg.js", TestType.All, true)]
        [DataRow("ReturnObjectArray", "ReturnObjectArray.js", TestType.All, false)]
        [DataRow("ReturnObjectArray", "ReturnObjectArray.js", TestType.All, true)]
        [DataRow("SetArrayElement", "SetArrayElement.js", TestType.All, false)]
        [DataRow("SetArrayElement", "SetArrayElement.js", TestType.All, true)]
        [DataRow("SubAssignDelegates", "SubAssignDelegates.js", TestType.All, false)]
        [DataRow("SubAssignDelegates", "SubAssignDelegates.js", TestType.All, true)]
        [DataRow("SubDelegates", "SubDelegates.js", TestType.All, false)]
        [DataRow("SubDelegates", "SubDelegates.js", TestType.All, true)]
        [DataRow("TestComplexCondition", "ComplexCondition.js", TestType.All, false)]
        [DataRow("TestComplexCondition", "ComplexCondition.js", TestType.All, true)]
        [DataRow("TestFourPartCondition", "FourPartCondition.js", TestType.All, false)]
        [DataRow("TestFourPartCondition", "FourPartCondition.js", TestType.All, true)]
        [DataRow("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All, false)]
        [DataRow("TestSimpleAndComparision", "SimpleAndComparision.js", TestType.All, true)]
        [DataRow("TestSimpleCompare", "TestSimpleCompare.js", TestType.All, false)]
        [DataRow("TestSimpleCompare", "TestSimpleCompare.js", TestType.All, true)]
        [DataRow("TestSimpleConditional", "SimpleConditional.js", TestType.All, false)]
        [DataRow("TestSimpleConditional", "SimpleConditional.js", TestType.All, true)]
        public void TestMcs(string methodName, string resourceName, TestType testType, bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + resourceName,
                SimpleConverters.TestClassNameStr,
                methodName,
                testType,
                true,
                false,
                instanceAsStatic);
        }

        [DataTestMethod]
        public void TestComplexConditionMethod()
        {
            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + "Complex3IfCondition.static.js",
                "BasicBlockTestFunctions",
                "Complex3IfCondition",
                TestType.All,
                false,
                false,
                true);

            ConverterTestHelpers.RunTest(
                SimpleConverters.TestFilesNSStr + "Complex3IfCondition.js",
                "BasicBlockTestFunctions",
                "Complex3IfCondition",
                TestType.All,
                false,
                false,
                false);
        }
    }
}