//-----------------------------------------------------------------------
// <copyright file="ScopeResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser
{
    using System;
    using System.Collections.Generic;
    using NScript.JST;

    public class ScopeResolver
    {
        /// <summary>
        /// dictionary etc.
        /// </summary>
        List<Dictionary<string, IIdentifier>> scopeDictionaries =
            new List<Dictionary<string,IIdentifier>>();

        /// <summary>
        /// Backing field for resolver.
        /// </summary>
        private readonly IResolver resolver;

        /// <summary>
        /// Backing field for scopes.
        /// </summary>
        List<IdentifierScope> scopes = new List<IdentifierScope>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeResolver"/> class.
        /// </summary>
        /// <param name="parentScope">The parent scope.</param>
        /// <param name="resolver">The resolver.</param>
        public ScopeResolver(
            IdentifierScope parentScope,
            IResolver resolver)
        {
            this.resolver = resolver;
            this.scopes.Add(parentScope);
            this.scopeDictionaries.Add(new Dictionary<string,IIdentifier>());
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public IdentifierScope Scope
        {
            get { return this.scopes[0]; }
        }

        /// <summary>
        /// Resolves the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Resolved identifier.</returns>
        public Expression Resolve(string identifier)
        {
            IIdentifier returnValue = null;

            for (int iDict = 0; iDict < this.scopeDictionaries.Count; iDict++)
            {
                if (this.scopeDictionaries[iDict].TryGetValue(identifier, out returnValue))
                {
                return new IdentifierExpression(
                    returnValue,
                    this.Scope);
                }
            }

            return this.resolver.ResolveIdentifier(
                this.Scope,
                identifier);
        }

        /// <summary>
        /// Creates the identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Newly created identifier.</returns>
        public IIdentifier CreateIdentifier(string identifier)
        {
            IIdentifier rv = SimpleIdentifier.CreateScopeIdentifier(
                this.Scope,
                identifier,
                false);

            if (this.scopeDictionaries[0].ContainsKey(identifier))
            {
                throw new InvalidProgramException(
                    string.Format("{0} declared twice", identifier));
            }
            this.scopeDictionaries[0].Add(identifier, rv);

            return rv;
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// Expression for resolved type.
        /// </returns>
        public Expression ResolveType(
            Tuple<string,string> typeName)
        {
            return this.resolver.ResolveType(
                this.Scope,
                typeName);
        }

        /// <summary>
        /// Resolves the field.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="isInstance">if set to <c>true</c> [is instance].</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Expression for resolved field.
        /// </returns>
        public Expression ResolveField(
            Tuple<string, string> typeName,
            bool isInstance,
            string fieldName)
        {
            return this.resolver.ResolveField(
                this.Scope,
                typeName,
                isInstance,
                fieldName);
        }

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="argTypeNames">The arg type names.</param>
        /// <returns>
        /// Expression for referncing given method.
        /// </returns>
        public Expression ResolveMethod(
            Tuple<string, string> typeName,
            bool isInstance,
            string methodName,
            params Tuple<string, string>[] argTypeNames)
        {
            return this.resolver.ResolveMethod(
                this.Scope,
                typeName,
                isInstance,
                methodName,
                argTypeNames);
        }

        /// <summary>
        /// Pushes the scope.
        /// </summary>
        /// <param name="args">The args.</param>
        internal void PushScope(List<string> args)
        {
            IdentifierScope newScope = new IdentifierScope(
                this.Scope,
                args,
                false);

            this.scopeDictionaries.Insert(0, new Dictionary<string, IIdentifier>());
            this.scopes.Insert(0, newScope);

            for (int iArg = 0; iArg < args.Count; iArg++)
            {
                this.scopeDictionaries[0].Add(
                    args[iArg],
                    newScope.ParameterIdentifiers[iArg]);
            }
        }

        /// <summary>
        /// Pops the scope.
        /// </summary>
        internal void PopScope()
        {
            this.scopes.RemoveAt(0);
            this.scopeDictionaries.RemoveAt(0);
        }
    }
}
