namespace NScript.Csc.Lib.Test
{
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// #156: Dictionary&lt;K,V&gt; has no runtime, so creating one must fail the build rather
    /// than throw in the browser. Naming the type stays legal.
    /// </summary>
    [TestClass]
    public class GenericDictionaryCompileErrorTests
    {
        private static Diagnostic[] Compile(string source)
        {
            TestAssemblyLoader.LoadAssemblies();
            var mscorlib = MetadataReference.CreateFromFile(
                Path.Combine(TestResources.FixtureDirectory, "mscorlib.dll"));
            var compilation = CSharpCompilation.Create(
                "DictionaryProbe",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { mscorlib },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return compilation.GetDiagnostics().ToArray();
        }

        [TestMethod]
        public void CreatingOne_IsACompileErrorThatNamesTheAlternatives()
        {
            var diagnostics = Compile(
                "class P { object M() { return new System.Collections.Generic.Dictionary<string, int>(1); } }");

            Assert.IsTrue(
                diagnostics.Any(d => d.Id == "CS0619"
                    && d.GetMessage().Contains("StringDictionary")
                    && d.GetMessage().Contains("NumberDictionary")),
                string.Join(", ", diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
        }

        [TestMethod]
        public void NamingTheType_StillCompiles()
        {
            var diagnostics = Compile(
                "class P { void M(System.Collections.Generic.Dictionary<string, int> d) { } string N() { return nameof(System.Collections.Generic.Dictionary<,>.Count); } }");

            Assert.IsFalse(
                diagnostics.Any(d => d.Id == "CS0619"),
                string.Join(", ", diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
        }
    }
}
