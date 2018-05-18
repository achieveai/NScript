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
            _compilationResults = TestResources.RealScriptMethods;
        }

        [Test]
        public void ValidateString()
        {
        }
    }
}
