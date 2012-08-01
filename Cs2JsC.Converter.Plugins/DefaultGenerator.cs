//-----------------------------------------------------------------------
// <copyright file="DefaultGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Plugins
{
    using System;
    using Mono.Cecil;
    using System.Collections.Generic;
    using CLR;
    using JST;
    using TypeSystemConverter;

    /// <summary>
    /// Definition for DefaultGenerator
    /// </summary>
    public class DefaultGenerator : IConverterPlugin
    {
        private ClrContext clrContext;

        private RuntimeScopeManager runtimeScopeManager;

        public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            this.clrContext = clrContext;
            this.runtimeScopeManager = runtimeScopeManager;
        }

        public void ParseArgs(IList<Tuple<string, string>> args)
        {
        }

        /// <summary>
        /// Gets the methods to emit pass1.
        /// </summary>
        /// <returns>List of methods to include in final JS.</returns>
        public List<MethodReference> GetMethodsToEmitPass1()
        {
            List<MethodReference> rv = new List<MethodReference>();
            foreach (var typeDefinition in clrContext.GetTypeDefinitions())
            {
                if (!typeDefinition.Namespace.StartsWith("System"))
                {
                    foreach (var method in typeDefinition.Methods)
                    {
                        if (method.HasGenericParameters) { continue; }

                        rv.Add(method);
                    }
                }
            }

            return rv;
        }

        public List<MethodReference> GetMethodsToEmitPassN(List<MethodReference> methodsEmitted)
        {
            return methodsEmitted;
        }

        public List<Statement> GetPreJavascript()
        {
            return null;
        }

        public List<Statement> GetPostJavascript()
        {
            return null;
        }
    }
}
