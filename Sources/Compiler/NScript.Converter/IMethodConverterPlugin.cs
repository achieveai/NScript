//-----------------------------------------------------------------------
// <copyright file="IMethodConverterPlugin.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Values that represent IntrestLevel.
    /// </summary>
    public enum IntrestLevel
    {
        /// <summary>
        /// .
        /// </summary>
        None,
        /// <summary>
        /// .
        /// </summary>
        PreEmitStatements,
        /// <summary>
        /// .
        /// </summary>
        PostEmitStatements,
        /// <summary>
        /// .
        /// </summary>
        Encapsulate,
        /// <summary>
        /// .
        /// </summary>
        Overwrite
    }

    /// <summary>
    /// Interface for method converter plugin.
    /// </summary>
    public interface IMethodConverterPlugin : IConverterPlugin
    {
        /// <summary>
        /// Gets interest level.
        /// </summary>
        /// <param name="methodDefinition"> The method. </param>
        /// <param name="converterContext"> Context for the converter. </param>
        /// <returns>
        /// The interest level.
        /// </returns>
        IntrestLevel GetInterestLevel(MethodDefinition methodDefinition, ConverterContext converterContext);

        /// <summary>
        /// Gets pre insertion statements.
        /// </summary>
        /// <param name="scopeConverter"> The scope converter. </param>
        /// <param name="methodConverter"> The method. </param>
        /// <returns>
        /// The pre insertion statements.
        /// </returns>
        List<Statement> GetPreInsertionStatements(MethodConverter methodConverter);

        /// <summary>
        /// Gets post insertion statements.
        /// </summary>
        /// <param name="scopeConverter"> The scope converter. </param>
        ///
        /// <param name="methodConverter"> The method. </param>
        /// <returns>
        /// The post insertion statements.
        /// </returns>
        List<Statement> GetPostInsertionStatements(MethodConverter methodConverter);

        /// <summary>
        /// Gets encapsulation statements.
        /// </summary>
        /// <param name="scopeConverter"> The scope converter. </param>
        /// <param name="methodConverter"> The method. </param>
        /// <param name="methodStatments"> The method statments. </param>
        /// <returns>
        /// The encapsulation statements.
        /// </returns>
        List<Statement> GetEncapsulationStatements(MethodConverter methodConverter, List<Statement> methodStatments);

        /// <summary>
        /// Gets an overwrite.
        /// </summary>
        /// <param name="scopeConverter"> The scope converter. </param>
        /// <param name="methodConverter"> The method. </param>
        /// <returns>
        /// The overwrite.
        /// </returns>
        List<Statement> GetOverwrite(MethodConverter methodConverter);

        /// <summary>
        /// True when the method cache may replay this plugin's conversion of the method: the
        /// conversion keeps no plugin state, and every resolver call it makes goes through the
        /// method converter (so the recording sees it). Default false: never cached.
        /// </summary>
        bool CanReplayConversion(MethodDefinition methodDefinition, ConverterContext converterContext) => false;
    }
}
