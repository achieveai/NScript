using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Csc.Lib.Test;

namespace NScript.Converter.Test.MethodConverterTests
{
    [TestClass]
    public class Lang8FeatureTests
    {
        private const string TestClassNameStr = @"Lang8Features";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.Lang8Features.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "NullCoalescingAssignment", "TestNullCoalescing.js", TestType.All, false)]
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
