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
        /// <returns></returns>
        List<MethodReference> GetMethodsToEmitPassN();

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

        /// <summary>
        /// Dev patch emit: the name of the section a dev bundle marks after this plugin's
        /// post JavaScript (<c>/*@S name*/</c>), or null for none.
        /// </summary>
        string PatchSectionName => null;

        /// <summary>
        /// Dev patch emit, after a change to embedded resources only: adds what the bundle
        /// needs before this plugin's section marker to <paramref name="statements"/>, using
        /// the state of the full emit this plugin instance made. Returns null, or why it
        /// cannot patch (the build then emits in full). A plugin that does not know whether
        /// its output depends on the change cannot patch.
        /// </summary>
        string TryPatchResources(List<Statement> statements) => "plugin " + this.GetType().Name;
    }
}
