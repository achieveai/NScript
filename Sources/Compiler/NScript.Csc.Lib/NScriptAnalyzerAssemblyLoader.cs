//-----------------------------------------------------------------------
// <copyright file="NScriptAnalyzerAssemblyLoader.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Definition for NScriptAnalyzerAssemblyLoader
    /// </summary>
    internal class NScriptAnalyzerAssemblyLoader : AnalyzerAssemblyLoader
    {
        private int _hookedAssemblyResolve;

        protected override Assembly LoadFromPathImpl(string fullPath)
        {
            if (Interlocked.CompareExchange(ref _hookedAssemblyResolve, 0, 1) == 0)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }

            return LoadImpl(fullPath);
        }

        protected virtual Assembly LoadImpl(string fullPath) => Assembly.LoadFrom(fullPath);

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                return Load(AppDomain.CurrentDomain.ApplyPolicy(args.Name));
            }
            catch
            {
                return null;
            }
        }
    }
}
