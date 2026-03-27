//-----------------------------------------------------------------------
// <copyright file="NScriptAnalyzerAssemblyLoader.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using Microsoft.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// Definition for NScriptAnalyzerAssemblyLoader
    /// </summary>
    internal sealed class NScriptAnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        private readonly AnalyzerAssemblyLoader analyzerAssemblyLoader = new AnalyzerAssemblyLoader();

        public void AddDependencyLocation(string fullPath) => analyzerAssemblyLoader.AddDependencyLocation(fullPath);

        public Assembly LoadFromPath(string fullPath) => analyzerAssemblyLoader.LoadFromPath(fullPath);
    }
}
