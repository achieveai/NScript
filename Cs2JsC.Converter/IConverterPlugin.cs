//-----------------------------------------------------------------------
// <copyright file="IConverterPlugin.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter
{
    using System;
    using System.Collections.Generic;
using Mono.Cecil;
using Cs2JsC.CLR;
using Cs2JsC.JST;
using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for IConverterPlugin
    /// </summary>
    public interface IConverterPlugin
    {
        /// <summary>
        /// Initializes the specified CLR context.
        /// </summary>
        /// <param name="clrContext">The CLR context.</param>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        void Initialize(
            ClrContext clrContext,
            RuntimeScopeManager runtimeScopeManager);

        /// <summary>
        /// Parses the args.
        /// </summary>
        /// <param name="args">The args.</param>
        void ParseArgs(IList<Tuple<string, string>> args);

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
