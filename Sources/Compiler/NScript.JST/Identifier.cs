//-----------------------------------------------------------------------
// <copyright file="Identifier.cs" company="WebAps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Identifier base class
    /// </summary>
    [DebuggerDisplay("Name={suggestedName}, enforce={enforceSuggestion}")]
    public class SimpleIdentifier : IIdentifier
    {
        /// <summary>
        /// Backing store for usageCount.
        /// </summary>
        private int usageCount;

        /// <summary>
        /// Backing field for suggested name.
        /// </summary>
        private readonly string suggestedName;

        /// <summary>
        /// Backing field for EnforceSuggestion.
        /// </summary>
        private readonly bool enforceSuggestion;
        private readonly string originalSuggestedName;

        /// <summary>
        /// Backing field for owner scope.
        /// </summary>
        private readonly IdentifierScope ownerScope;

        /// <summary>
        /// true if this object is function name.
        /// </summary>
        private bool isFunctionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleIdentifier"/> class.
        /// </summary>
        /// <param name="suggestedName">Name of the suggested.</param>
        /// <param name="ownerScope">The owner scope.</param>
        private SimpleIdentifier(
            IdentifierScope ownerScope,
            string suggestedName,
            bool enforceSuggestion,
            bool dontEscape = false)
        {
            this.ownerScope = ownerScope;
            this.enforceSuggestion = enforceSuggestion;
            this.originalSuggestedName = suggestedName;
            this.suggestedName = string.IsNullOrWhiteSpace(suggestedName)
                    ? string.Empty
                    : enforceSuggestion || dontEscape
                        ? suggestedName
                        : Utils.ToJSIdentifier(suggestedName);
        }

        /// <summary>
        /// Gets or sets the usage count.
        /// </summary>
        /// <value>The usage count.</value>
        public int UsageCount
        {
            get
            {
                return this.usageCount;
            }

            set
            {
                this.usageCount = value;
            }
        }

        /// <summary>
        /// Gets the name of the suggested.
        /// </summary>
        /// <value>The name of the suggested.</value>
        public string SuggestedName
            => this.suggestedName;

        public string OriginalSuggestedName
            => this.originalSuggestedName;

        /// <summary>
        /// Gets a value indicating whether Identifier should enforce suggestion.
        /// </summary>
        /// <value>
        /// <c>true</c> if should enforce suggestion; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldEnforceSuggestion
            => this.enforceSuggestion;

        /// <summary>
        /// Gets a value indicating whether this object is empty.
        /// </summary>
        /// <value>
        /// true if this object is empty, false if not.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                return this.enforceSuggestion && this.suggestedName.Length == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is function name.
        /// </summary>
        /// <value>
        /// true if this object is function name, false if not.
        /// </value>
        public bool IsFunctionName
        { get { return this.isFunctionName; } }

        /// <summary>
        /// Gets the owner scope.
        /// </summary>
        /// <value>The owner scope.</value>
        public IdentifierScope OwnerScope
        {
            get
            {
                return this.ownerScope;
            }
        }

        /// <summary>
        /// Creates the scope identifier.
        /// </summary>
        /// <param name="ownerScope">The owner scope.</param>
        /// <returns>Identifier that has already been added to scope.</returns>
        public static SimpleIdentifier CreateScopeIdentifier(
            IdentifierScope ownerScope)
        {
            return SimpleIdentifier.CreateScopeIdentifier(ownerScope, null, false);
        }

        /// <summary>
        /// Creates the scope identifier.
        /// </summary>
        /// <param name="ownerScope">The owner scope.</param>
        /// <param name="suggestedName">Name of the suggested.</param>
        /// <param name="enforceSuggestion">if set to <c>true</c> [enforce suggestion].</param>
        /// <returns>
        /// Identifier that has already been added to scope.
        /// </returns>
        public static SimpleIdentifier CreateScopeIdentifier(
            IdentifierScope ownerScope,
            string suggestedName,
            bool enforceSuggestion,
            bool dontEscape = false)
        {
            if (enforceSuggestion && suggestedName == null)
            {
                throw new ArgumentException("suggestedName");
            }

            SimpleIdentifier returnValue = null;
            suggestedName = suggestedName.Trim();

            if (!enforceSuggestion
                || !ownerScope.TryGetKnownIdentifier(suggestedName, out returnValue))
            {
                returnValue = new SimpleIdentifier(
                     ownerScope,
                     suggestedName,
                     enforceSuggestion,
                     dontEscape);

                ownerScope.AddIdentifier(returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Creates the parameter identifier.
        /// </summary>
        /// <param name="ownerScope">The owner scope.</param>
        /// <param name="parameterCount">The parameter count.</param>
        /// <param name="paramIdentifiers">The param identifiers.</param>
        internal static void CreateParameterIdentifier(
            IdentifierScope ownerScope,
            int parameterCount,
            List<SimpleIdentifier> paramIdentifiers)
        {
            for (int parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
            {
                paramIdentifiers.Add(
                    new SimpleIdentifier(
                        ownerScope,
                        string.Format(
                            "arg{0}",
                            parameterIndex),
                        false));
            }
        }

        /// <summary>
        /// Creates the parameter identifier.
        /// </summary>
        /// <param name="ownerScope">The owner scope.</param>
        /// <param name="identifierNames">The identifier names.</param>
        /// <param name="enforceNames">if set to <c>true</c> [enforce names].</param>
        /// <param name="paramIdentifiers">The param identifiers.</param>
        internal static void CreateParameterIdentifier(
            IdentifierScope ownerScope,
            IList<string> identifierNames,
            bool enforceNames,
            List<SimpleIdentifier> paramIdentifiers)
        {
            for (int parameterIndex = 0; parameterIndex < identifierNames.Count; parameterIndex++)
            {
                paramIdentifiers.Add(
                    new SimpleIdentifier(
                        ownerScope,
                        identifierNames[parameterIndex],
                        enforceNames));
            }
        }

        /// <summary>
        /// Adds the usage.
        /// </summary>
        /// <param name="identifierScope">The identifier scope.</param>
        public void AddUsage(IdentifierScope identifierScope)
        {
            this.usageCount++;

            if (!this.ownerScope.IsExecutionScope)
            { this.AddUsageInternal(this.ownerScope); }
            else
            { this.AddUsageInternal(identifierScope); }
        }

        /// <summary>
        /// Adds the usage internal.
        /// </summary>
        /// <param name="identifierScope">The identifier scope.</param>
        /// <returns></returns>
        private bool AddUsageInternal(IdentifierScope identifierScope)
        {
            if (identifierScope == null)
            { throw new Exception("Invalid operation exception"); }
            else if (identifierScope == this.ownerScope)
            { identifierScope.IdentifierUsed(this); return true; }
            else
            {
                this.AddUsageInternal(identifierScope.ParentScope);
                identifierScope.IdentifierUsed(this);
                return true;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>Name to be written in script</returns>
        public string GetName()
        {
            if (this.enforceSuggestion)
            {
                return this.SuggestedName;
            }
            else
            {
                return this.OwnerScope.GetName(this);
            }
        }

        public void MarkAsFunctionName()
        {
            this.isFunctionName = true;
        }

        internal void ResetUsage()
        {
            usageCount = 0;
        }
    }
}