namespace TestLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new NScript.Converter.Test.MethodConverterTests.RegressionTests();
            test.Setup();
            test.TestMcs(
                NScript.Converter.Test.MethodConverterTests.RegressionTests.TestClassNameStr,
                "DeclarationWithOut",
                "DeclarationWithOut.js",
                NScript.Converter.Test.TestType.All);
        }
    }
}
