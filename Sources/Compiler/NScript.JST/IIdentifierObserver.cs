//-----------------------------------------------------------------------
// <copyright file="IIdentifierObserver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    /// <summary>
    /// Sees scopes and identifiers as they are made and used on one thread
    /// (<see cref="IdentifierScope.Observer"/>). The converter's method cache uses it to record
    /// what a method conversion touched outside the method's own scopes.
    /// </summary>
    public interface IIdentifierObserver
    {
        /// <summary>A scope with a parent was made.</summary>
        void ScopeCreated(IdentifierScope scope);

        /// <summary><see cref="SimpleIdentifier.CreateScopeIdentifier(IdentifierScope, string, bool, bool)"/>
        /// returned this identifier, new or (enforced) found.</summary>
        void IdentifierCreated(SimpleIdentifier identifier, string suggestedName, bool enforceSuggestion, bool dontEscape);

        /// <summary>The identifier was used.</summary>
        void IdentifierUsed(SimpleIdentifier identifier);
    }
}
