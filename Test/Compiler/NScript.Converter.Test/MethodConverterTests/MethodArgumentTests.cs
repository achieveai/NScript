namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodArgumentTests
    {
        private const string TestClassNameStr = @"TestMethodArguments";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.MethodArgument.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "TestConstructor", "Constructor.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestConstructor", "Constructor.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestLocalMethod", "LocalMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestLocalMethod", "LocalMethod.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestInstanceMethods", "InstanceMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestInstanceMethods", "InstanceMethod.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestStaticMethods", "StaticMethods.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestStaticMethods", "StaticMethods.js", TestType.All, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }
    }
}
