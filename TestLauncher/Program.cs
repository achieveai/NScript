namespace TestLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new NScript.Converter.Test.MethodConverterTests.DynamicConverterTests();
            test.Setup();
            test.TestMcs(
                NScript.Converter.Test.MethodConverterTests.DynamicConverterTests.TestClassNameStr,
                "GetFoo",
                "GetFoo.js",
                NScript.Converter.Test.TestType.All);
        }
    }
}
