namespace NScript.Csc.Lib.Test
{
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class TestMscorlibBuild
    {
        private Dictionary<IMethodSymbol, MethodBody> _compilationResults;

        [SetUp]
        public void Setup()
        {
            var resources = TestResources.sources["mscorlib"];
            const string nscriptGitPath = @"E:\repos\cs2jsc";
            var trees = resources
                .files
                .Select(_ =>
                    Path.Combine(
                        Path.GetFullPath(Path.Combine(nscriptGitPath,resources.directory)),
                        _))
                .Select(_ =>
                    CSharpSyntaxTree.ParseText(
                        File.ReadAllText(_),
                        CSharpParseOptions.Default,
                        _))
                .ToArray();

            var compilerOptions = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                false,
                null,
                null,
                null,
                null,
                OptimizationLevel.Debug,
                false,
                true,
                null,
                Path.Combine(Path.Combine(nscriptGitPath, resources.directory), resources.keyFile),
                default(ImmutableArray<byte>),
                null,
                Platform.AnyCpu,
                ReportDiagnostic.Warn,
                4,
                null,
                false,
                false,
                null,
                null,
                null,
                null,
                null,
                false,
                MetadataImportOptions.Public);

            var compilation = CSharpCompilation.Create(
                "TestCompilation",
                syntaxTrees: trees,
                options: compilerOptions);

            _compilationResults = SerializationHelper.ExpressionVisitMap(
                compilation);
        }

        [Test]
        public void ValidateString()
        {
        }
    }
}
