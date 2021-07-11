//-----------------------------------------------------------------------
// <copyright file="ITypeConverterPlugin.cs" company="">
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

    /// <summary>
    /// Definition for ITypeConverterPlugin.
    /// </summary>
    public interface ITypeConverterPlugin : IConverterPlugin
    {
        /// <summary>
        /// Gets interest level.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        /// The interest level.
        /// </returns>
        IntrestLevel GetInterestLevel(TypeDefinition type);

        /// <summary>
        /// Gets pre insertion statements.
        /// </summary>
        /// <param name="typeConverter"> The type converter. </param>
        /// <returns>
        /// The pre insertion statements.
        /// </returns>
        List<Statement> GetPreInsertionStatements(TypeConverter typeConverter);

        /// <summary>
        /// Gets post insertion statements.
        /// </summary>
        /// <param name="typeConverter"> The type converter. </param>
        /// <returns>
        /// The post insertion statements.
        /// </returns>
        List<Statement> GetPostInsertionStatements(TypeConverter typeConverter);

        /// <summary>
        /// Gets encapsulation statements.
        /// </summary>
        /// <param name="typeConverter">  The type converter. </param>
        /// <param name="typeStatements"> The type statements. </param>
        /// <returns>
        /// The encapsulation statements.
        /// </returns>
        List<Statement> GetEncapsulationStatements(TypeConverter typeConverter, List<Statement> typeStatements);

        /// <summary>
        /// Gets an overwrite.
        /// </summary>
        /// <param name="typeConverter"> The type converter. </param>
        /// <returns>
        /// The overwrite.
        /// </returns>
        List<Statement> GetOverwrite(TypeConverter typeConverter);
    }
}
