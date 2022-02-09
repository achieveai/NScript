namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
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
        [DataRow(TestClassNameStr, "TestConstructor", "Constructor.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestLocalMethod", "LocalMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestInstanceMethods", "InstanceMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestStaticMethods", "StaticMethods.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}
