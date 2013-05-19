//-----------------------------------------------------------------------
// <copyright file="IConverterPlugin.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;
    using NScript.Converter.TypeSystemConverter;

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
    }
}
