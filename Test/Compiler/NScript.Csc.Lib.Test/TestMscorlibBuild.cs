namespace NScript.Csc.Lib.Test
{
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class TestMscorlibBuild
    {
        private Dictionary<IMethodSymbol, MethodBody> _compilationResults;

        [TestInitialize]
        public void Setup()
        {
            _compilationResults = TestResources.RealScriptMethods;
        }
    }
}
