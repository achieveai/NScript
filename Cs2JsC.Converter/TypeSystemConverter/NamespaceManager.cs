//-----------------------------------------------------------------------
// <copyright file="NamespaceManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System.Collections.Generic;
    using Cs2JsC.JST;

    /// <summary>
    /// Definition for NamespaceManager
    /// </summary>
    public class NamespaceManager
    {
        /// <summary>
        /// Backing field for CurrentScope.
        /// </summary>
        private readonly IdentifierScope currentScope;

        /// <summary>
        /// Backing field for ParentNamespace
        /// </summary>
        private readonly NamespaceManager parentNamespace;

        /// <summary>
        /// Backing field for Identifier.
        /// </summary>
        private readonly Identifier scopeIdentifier;

        /// <summary>
        /// These are not 
        /// </summary>
        private readonly Dictionary<Identifier, IdentifierScope> childrenScopes =
            new Dictionary<Identifier, IdentifierScope>();

        /// <summary>
        /// Backing collection of all the childNamespaces.
        /// </summary>
        private readonly Dictionary<string, NamespaceManager> childNamesaces =
            new Dictionary<string, NamespaceManager>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManager"/> class.
        /// </summary>
        /// <param name="parentNamespace">The parent namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="enforceName">if set to <c>true</c> [enforce name].</param>
        public NamespaceManager(
            NamespaceManager parentNamespace,
            string name,
            bool enforceName)
        {
            this.parentNamespace = parentNamespace;
            this.scopeIdentifier = Identifier.CreateScopeIdentifier(
                this.parentNamespace.Scope,
                name,
                enforceName);

            this.currentScope = parentNamespace.GetScope(this.scopeIdentifier);
            this.parentNamespace.childNamesaces.Add(name, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManager"/> class.
        /// </summary>
        public NamespaceManager()
        {
            this.currentScope = new IdentifierScope(true);
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public IdentifierScope Scope
        { get { return this.currentScope; } }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public NamespaceManager Parent
        {get { return this.parentNamespace; }}

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Identifier Identifier
        {
            get { return this.scopeIdentifier; }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <param name="nameSpace">The name space.</param>
        /// <returns></returns>
        public NamespaceManager GetNamespace(
            string nameSpace,
            bool enforceName = false)
        {
            int dotIndex = nameSpace.IndexOf('.');
            string curPart = nameSpace;

            if (dotIndex > 0)
            {
                curPart = nameSpace.Substring(0, dotIndex);
                nameSpace = nameSpace.Substring(dotIndex + 1);
            }
            else
            {
                curPart = nameSpace;
                nameSpace = null;
            }

            NamespaceManager returnValue;
            if (!this.childNamesaces.TryGetValue(curPart, out returnValue))
            {
                returnValue = new NamespaceManager(
                    this,
                    curPart,
                    enforceName);
            }

            if (string.IsNullOrEmpty(nameSpace))
            {
                return returnValue;
            }

            return returnValue.GetNamespace(nameSpace, enforceName);
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>IdentifierScope keyed with given identifier.</returns>
        public IdentifierScope GetScope(Identifier identifier)
        {
            IdentifierScope identifierScope;

            if (!this.childrenScopes.TryGetValue(identifier, out identifierScope))
            {
                identifierScope = new IdentifierScope(false);
                this.childrenScopes.Add(identifier, identifierScope);
            }

            return identifierScope;
        }
    }
}
