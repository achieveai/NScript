using NScript.CLR.Test;
using NUnit.Framework;

namespace NScript.Converter.Test.MethodConverterTests
{
    [TestFixture]
    public class TestCompilerGeneratedStuff
    {
        private const string TestClassNameStr = "TestCompilerGeneratedStuff";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.CompilerGeneratedStuff.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "get_IntProperty", "GetAutoProperty.js", TestType.All)]
        [TestCase(TestClassNameStr, "set_IntProperty", "SetAutoProperty.js", TestType.All)]
        [TestCase(TestClassNameStr, "add_testEvent", "AddTestEvent.js", TestType.All)]
        [TestCase(TestClassNameStr, "remove_testEvent", "RemoveTestEvent.js", TestType.All)]
        [TestCase(TestClassNameStr, "TestEventCheck", "TestEventAddRemove.js", TestType.All)]
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