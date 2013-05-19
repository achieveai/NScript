//-----------------------------------------------------------------------
// <copyright file="IRuntimeConverterPlugin.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IRuntimeConverterPlugin
    /// </summary>
    public interface IRuntimeConverterPlugin : IConverterPlugin
    {
        /// <summary>
        /// Gets the methods to emit pass1.
        /// </summary>
        /// <param name="clrContext">The CLR context.</param>
        /// <returns></returns>
        List<MethodReference> GetMethodsToEmitPass1();

        /// <summary>
        /// Gets the methods to emit pass N.
        /// </summary>
        /// <param name="clrContext">The CLR context.</param>
        /// <param name="methodsEmitted">The methods emitted.</param>
        /// <returns></returns>
        List<MethodReference> GetMethodsToEmitPassN(List<MethodReference> methodsEmitted);

        /// <summary>
        /// Gets the pre javascript.
        /// </summary>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        /// <returns></returns>
        List<Statement> GetPreJavascript();

        /// <summary>
        /// Gets the post javascript.
        /// </summary>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        /// <returns></returns>
        List<Statement> GetPostJavascript();
    }
}
