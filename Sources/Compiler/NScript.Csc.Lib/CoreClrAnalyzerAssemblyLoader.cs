//-----------------------------------------------------------------------
// <copyright file="CoreClrAnalyzerAssemblyLoader.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using System;

    /// <summary>
    /// Definition for CoreClrAnalyzerAssemblyLoader
    /// </summary>
    public class CoreClrAnalyzerAssemblyLoader
    {
#if NOMORE
        private AssemblyLoadContext _loadContext;

        public CoreClrAnalyzerAssemblyLoader()
        {
        }

        protected override Assembly LoadFromPathImpl(string fullPath)
        {
            //.NET Native doesn't support AssemblyLoadContext.GetLoadContext. 
            // Initializing the _loadContext in the .ctor would cause
            // .NET Native builds to fail because the .ctor is called. 
            // However, LoadFromPathImpl is never called in .NET Native, so 
            // we do a lazy initialization here to make .NET Native builds happy.
            if (_loadContext == null)
            {
                AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(typeof(CoreClrAnalyzerAssemblyLoader).GetTypeInfo().Assembly);

                if (System.Threading.Interlocked.CompareExchange(ref _loadContext, loadContext, null) == null)
                {
                    _loadContext.Resolving += (context, name) =>
                    {
                        Debug.Assert(ReferenceEquals(context, _loadContext));
                        return Load(name.FullName);
                    };
                }
            }

            return LoadImpl(fullPath);
        }

        protected virtual Assembly LoadImpl(string fullPath) => _loadContext.LoadFromAssemblyPath(fullPath);
#endif
    }
}
