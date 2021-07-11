//-----------------------------------------------------------------------
// <copyright file="TestTypeResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using System;
    using System.Collections.Generic;
    using NScript.JST;
    using System.Linq;

    /// <summary>
    /// Definition for TestTypeResolver
    /// </summary>
    public class TestTypeResolver : IResolver
    {
        /// <summary>
        /// backing field for scope.
        /// </summary>
        private readonly IdentifierScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTypeResolver"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public TestTypeResolver(IdentifierScope scope)
        {
            this.scope = scope;
        }

        /// <summary>
        /// Resolves the identifier.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// Resolved identifier.
        /// </returns>
        public Expression ResolveIdentifier(
            IdentifierScope scope,
            string identifier)
        {
            return new IdentifierExpression(
                this.scope.ParameterIdentifiers.Single((i) => i.SuggestedName == identifier),
                scope);
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// Expression for resolved type.
        /// </returns>
        public Expression ResolveType(
            IdentifierScope scope,
            Tuple<string, string> typeName)
        {
            return new StringLiteralExpression(
                this.scope,
                string.Format(
                    "{0}__{1}",
                    typeName.Item1,
                    typeName.Item2));
        }

        /// <summary>
        /// Resolves the field.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Expression for resolved field.
        /// </returns>
        public Expression ResolveField(
            IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string fieldName)
        {
            return new StringLiteralExpression(
                this.scope,
                string.Format(
                    "{0}{1}__{2}#{3}",
                    isInstance ? "instance " : string.Empty,
                    typeName.Item1,
                    typeName.Item2,
                    fieldName));
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodArguments">The method arguments.</param>
        /// <returns>
        /// Expression for referncing given method.
        /// </returns>
        public Expression ResolveMethod(
            IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string methodName,
            params Tuple<string, string>[] methodArguments)
        {
            return new StringLiteralExpression(
                this.scope,
                string.Format(
                    "{0}{1}__{2}#{3}(args:{4})",
                    isInstance ? "instance " : string.Empty,
                    typeName.Item1,
                    typeName.Item2,
                    methodName,
                    methodArguments.Length));
        }
    }
}
