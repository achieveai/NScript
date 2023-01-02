//-----------------------------------------------------------------------
// <copyright file="IdentifierScope.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using MoreLinq;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Scopes for variables.
    /// </summary>
    public partial class IdentifierScope
    {
        /// <summary>
        /// Backing field if IsExecutionScope
        /// </summary>
        private readonly bool isExecutionScope = false;

        /// <summary>
        /// Scope variable identifiers.
        /// </summary>
        private readonly HashSet<SimpleIdentifier> scopedIdentifiersSet =
            new HashSet<SimpleIdentifier>();

        /// <summary>
        /// Identifier used in current scope.
        /// </summary>
        private readonly Dictionary<SimpleIdentifier, int> usedIdentifiersSet =
            new Dictionary<SimpleIdentifier, int>();

        /// <summary>
        /// backing list for ParameterIdentifiers.
        /// </summary>
        private readonly List<SimpleIdentifier> paramaterIdentifiers;

        /// <summary>
        /// backing list for ScopedIdentifiers.
        /// </summary>
        private readonly List<SimpleIdentifier> scopedIdentifiers =
            new List<SimpleIdentifier>();

        /// <summary>
        /// backing list for ChildScopes.
        /// </summary>
        private readonly List<IdentifierScope> childScopes =
            new List<IdentifierScope>();

        /// <summary>
        /// Backing list for UsedIdentifiers.
        /// </summary>
        private readonly List<SimpleIdentifier> usedIdentifiers =
            new List<SimpleIdentifier>();

        /// <summary>
        /// Backing field for used local identifiers.
        /// </summary>
        private readonly List<SimpleIdentifier> usedLocalIdentifiers =
            new List<SimpleIdentifier>();

        /// <summary>
        /// List of names of the assigneds.
        /// </summary>
        private Func<SimpleIdentifier, string> assignedNames;

        /// <summary>
        /// Backing field for ParameterIdentifiers.
        /// </summary>
        private readonly ReadOnlyCollection<SimpleIdentifier> readonlyParameterIdentifiers;

        /// <summary>
        /// Backing field for ScopedIdentifiers.
        /// </summary>
        private readonly ReadOnlyCollection<SimpleIdentifier> readonlyScopedIdentifiers;

        /// <summary>
        /// Backing field for UsedIdentifiers.
        /// </summary>
        private readonly ReadOnlyCollection<SimpleIdentifier> readonlyUsedIdentifiers;

        /// <summary>
        /// Backing field for ChildScopes.
        /// </summary>
        private readonly ReadOnlyCollection<IdentifierScope> readonlyChildScopes;

        /// <summary>
        /// Backing field for usedLocalIdentifiers.
        /// </summary>
        private readonly ReadOnlyCollection<SimpleIdentifier> readonlyUsedLocalIdentifiers;

        /// <summary>
        /// mapping between enforced names and identifiers.
        /// </summary>
        private readonly Dictionary<string, SimpleIdentifier> knownNameMap =
            new Dictionary<string, SimpleIdentifier>();

        /// <summary>
        /// Dictionary for keeping track of which names are used by which identifier.
        /// This is used for generating correct name.
        /// </summary>
        private readonly Dictionary<string, List<SimpleIdentifier>> identifierNameGroups =
            new Dictionary<string, List<SimpleIdentifier>>();
        private readonly string scopeName;

        /// <summary>
        /// Parent scope.
        /// </summary>
        private readonly IdentifierScope parentScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierScope"/> class.
        /// </summary>
        /// <param name="isExecutionScope">if set to <c>true</c> [is execution scope].</param>
        public IdentifierScope(bool isExecutionScope)
        {
            this.isExecutionScope = isExecutionScope;

            if (isExecutionScope)
            {
                this.paramaterIdentifiers = new List<SimpleIdentifier>();
                this.readonlyParameterIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.paramaterIdentifiers);
            }
            else
            {
                this.scopeName = "JSObject instance";
            }

            this.readonlyUsedLocalIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.usedLocalIdentifiers);
            this.readonlyScopedIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.scopedIdentifiers);
            this.readonlyUsedIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.usedIdentifiers);
            this.readonlyChildScopes = new ReadOnlyCollection<IdentifierScope>(this.childScopes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierScope"/> class.
        /// </summary>
        /// <param name="parentScope">The parent scope.</param>
        public IdentifierScope(
            IdentifierScope parentScope,
            string scopeName = default)
        {
            if (parentScope == null)
            {
                throw new ArgumentNullException("parentScope");
            }

            this.scopeName = scopeName;
            this.parentScope = parentScope;
            this.isExecutionScope = parentScope.isExecutionScope;
            this.parentScope.childScopes.Add(this);

            this.readonlyUsedLocalIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.usedLocalIdentifiers);
            this.readonlyScopedIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.scopedIdentifiers);
            this.readonlyUsedIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.usedIdentifiers);
            this.readonlyChildScopes = new ReadOnlyCollection<IdentifierScope>(this.childScopes);

            if (this.isExecutionScope)
            {
                this.paramaterIdentifiers = new List<SimpleIdentifier>();
                this.readonlyParameterIdentifiers = new ReadOnlyCollection<SimpleIdentifier>(this.paramaterIdentifiers);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierScope"/> class.
        /// </summary>
        /// <param name="parentScope">The parent scope.</param>
        public IdentifierScope(
            IdentifierScope parentScope,
            int parameterCount)
            : this(parentScope)
        {
            if (!parentScope.isExecutionScope)
            {
                throw new System.InvalidOperationException("Non execution scopes can't take parameters");
            }

            SimpleIdentifier.CreateParameterIdentifier(this, parameterCount, this.paramaterIdentifiers);

            for (int iParam = 0; iParam < this.paramaterIdentifiers.Count; iParam++)
            {
                this.AddIdentifierToTrack(this.paramaterIdentifiers[iParam]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifierScope"/> class.
        /// </summary>
        /// <param name="parentScope">The parent scope.</param>
        /// <param name="suggestedParameterNames">The suggested parameter names.</param>
        /// <param name="enforceSuggestion">if set to <c>true</c> [enforce suggestion].</param>
        public IdentifierScope(
            IdentifierScope parentScope,
            IList<string> suggestedParameterNames,
            bool enforceSuggestion)
            : this(parentScope)
        {
            if (!parentScope.isExecutionScope)
            {
                throw new System.InvalidOperationException("Non execution scopes can't take parameters");
            }

            SimpleIdentifier.CreateParameterIdentifier(
                this,
                suggestedParameterNames,
                enforceSuggestion,
                this.paramaterIdentifiers);

            for (int iParam = 0; iParam < this.paramaterIdentifiers.Count; iParam++)
            {
                this.AddIdentifierToTrack(this.paramaterIdentifiers[iParam]);
            }
        }

        public bool IsExecutionScope
        { get { return this.isExecutionScope; } }

        public IdentifierScope ParentScope
            => this.parentScope;

        public IList<SimpleIdentifier> ParameterIdentifiers
            => this.readonlyParameterIdentifiers;

        public IList<SimpleIdentifier> ScopedIdentifiers
            =>   this.readonlyScopedIdentifiers;

        public IList<SimpleIdentifier> UsedLocalIdentifiers
            => this.readonlyUsedLocalIdentifiers;

        public IList<SimpleIdentifier> UsedIdentifiers
            => this.readonlyUsedIdentifiers;

        public IList<IdentifierScope> ChildScopes
            => this.readonlyChildScopes;

        public void ResetUsageCounter()
        {
            this.usedIdentifiers.Clear();
            this.usedIdentifiersSet.Clear();
            this.usedLocalIdentifiers.Clear();
            this.ParameterIdentifiers.ForEach(parm => parm.ResetUsage());
            this.scopedIdentifiers.ForEach(si => si.ResetUsage());
            this.childScopes.ForEach(cs => cs.ResetUsageCounter());
        }

        internal void AddIdentifier(SimpleIdentifier identifier)
        {
            if (identifier.ShouldEnforceSuggestion)
            {
                this.knownNameMap[identifier.SuggestedName] = identifier;
            }

            this.scopedIdentifiersSet.Add(identifier);
            this.scopedIdentifiers.Add(identifier);

            this.AddIdentifierToTrack(identifier);
        }

        /// <summary>
        /// Identifiers the used.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        internal void IdentifierUsed(SimpleIdentifier identifier)
        {
            if (!this.usedIdentifiersSet.ContainsKey(identifier))
            {
                this.usedIdentifiersSet.Add(identifier, 1);
                this.usedIdentifiers.Add(identifier);
                if (this.scopedIdentifiersSet.Contains(identifier))
                {
                    this.usedLocalIdentifiers.Add(identifier);
                }
            }
            else
            {
                this.usedIdentifiersSet[identifier]++;
            }
        }

        /// <summary>
        /// Gets the known identifier.
        /// </summary>
        /// <param name="identifierName">Name of the identifier.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// true if knownIdentifier is already present, false otherwise
        /// </returns>
        internal bool TryGetKnownIdentifier(string identifierName, out SimpleIdentifier identifier)
        {
            return this.knownNameMap.TryGetValue(identifierName, out identifier);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Name for this identifier.</returns>
        internal string GetName(SimpleIdentifier identifier)
        {
            SimpleIdentifier knownIdentifier;

            if (this.assignedNames != null)
            {
                return this.assignedNames(identifier);
            }

            if (this.knownNameMap.TryGetValue(identifier.SuggestedName, out knownIdentifier)
                && knownIdentifier == identifier)
            {
                return identifier.SuggestedName;
            }

            IdentifierScope parentScope = this.ParentScope;
            List<SimpleIdentifier> sameNamedIdentifiers;

            int slotIndex = 0;

            while (parentScope != null)
            {
                if (parentScope.identifierNameGroups.TryGetValue(
                    identifier.SuggestedName,
                    out sameNamedIdentifiers))
                {
                    slotIndex += sameNamedIdentifiers.Count;
                }

                parentScope = parentScope.ParentScope;
            }

            if (this.identifierNameGroups.TryGetValue(identifier.SuggestedName, out sameNamedIdentifiers))
            {
                slotIndex += sameNamedIdentifiers.IndexOf(identifier);
            }

            if (slotIndex > 0)
            {
                return identifier.SuggestedName + Utils.GetCompressedInt(slotIndex - 1);
            }

            return identifier.SuggestedName;
        }
    
        private void AddIdentifierToTrack(SimpleIdentifier identifier)
        {
            List<SimpleIdentifier> identifierGroup;

            if (!this.identifierNameGroups.TryGetValue(identifier.SuggestedName, out identifierGroup))
            {
                identifierGroup = new List<SimpleIdentifier>();
                this.identifierNameGroups.Add(identifier.SuggestedName, identifierGroup);
            }

            identifierGroup.Add(identifier);
        }

        public void Optimize()
        {
        }
    }
}
