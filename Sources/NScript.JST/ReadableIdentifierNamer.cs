//-----------------------------------------------------------------------
// <copyright file="ReadableIdentifierNamer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifier scope.
    /// </summary>
    public partial class IdentifierScope
    {
        /// <summary>
        /// Definition for ReadableIdentifierNamer
        /// </summary>
        public class ReadableIdentifierNamer
        {
            Dictionary<SimpleIdentifier, List<SimpleIdentifier>> identifierMap
                = new Dictionary<SimpleIdentifier, List<SimpleIdentifier>>();

            Dictionary<SimpleIdentifier, string> assignedNames
                = new Dictionary<SimpleIdentifier, string>();

            static string[][] intToSuffix =
                new string[10][];

            const string CharMap = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public ReadableIdentifierNamer(
                params IdentifierScope[] rootScopes)
            {
                this.ProcessScopes(
                    rootScopes,
                    this.CollectSimpleIdentifiers);

                this.ProcessScopes(
                    rootScopes,
                    this.AssignNames);
            }

            public string GetAssignedName(SimpleIdentifier identifier)
            {
                return this.assignedNames[identifier];
            }

            private void AssignNames(IdentifierScope scope)
            {
                scope.assignedNames = this.assignedNames;
                if (scope.paramaterIdentifiers != null)
                {
                    foreach (var ident in scope.paramaterIdentifiers)
                    {
                        this.AssignName(scope, ident);
                    }
                }

                foreach (var ident in scope.scopedIdentifiers)
                {
                    this.AssignName(scope, ident);
                }
            }

            private void AssignName(IdentifierScope scope, SimpleIdentifier ident)
            {
                if (ident.ShouldEnforceSuggestion)
                {
                    this.assignedNames.Add(ident, ident.SuggestedName);
                    return;
                }

                string prefix = ident.SuggestedName;
                for (int i = 0; i < 1000; i++)
                {
                    string postfix = ReadableIdentifierNamer.GetPostfix(i);
                    bool clash = false;
                    List<SimpleIdentifier> conflicts = null;
                    if (this.identifierMap.TryGetValue(ident, out conflicts))
                    {
                        foreach (var conflictIdent in conflicts)
                        {
                            string name;
                            if (this.assignedNames.TryGetValue(conflictIdent, out name))
                            {
                                if (postfix.Length == 0)
                                {
                                    clash = name == prefix;
                                    if (clash) { break; }
                                }
                                else if (name.Length == prefix.Length + postfix.Length)
                                {
                                    bool match = true;
                                    for (int iName = prefix.Length - 1; match && iName >= 0; iName--)
                                    { match = name[iName] == prefix[iName]; }

                                    for (int jPostFix = 0; match && jPostFix < postfix.Length; jPostFix++)
                                    { match = name[name.Length - jPostFix - 1] == postfix[postfix.Length - jPostFix - 1]; }

                                    if (match)
                                    {
                                        clash = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (!clash)
                    {
                        this.assignedNames.Add(ident, prefix + postfix);
                        return;
                    }
                }
            }

            private void CollectSimpleIdentifiers(IdentifierScope scope)
            {
                if (scope.paramaterIdentifiers != null)
                {
                    foreach(var ident in scope.paramaterIdentifiers)
                    {
                        this.AddIdentifierConflicts(scope, ident);
                    }
                }

                foreach (var ident in scope.scopedIdentifiers)
                {
                    this.AddIdentifierConflicts(scope, ident);
                }
            }

            private void AddIdentifierConflicts(IdentifierScope scope, SimpleIdentifier ident)
            {
                var currentScope = scope;
                while(currentScope != null)
                {
                    this.AddConflict(currentScope, ident);

                    for (int iConflict = 0; iConflict < currentScope.conflictingScopes.Count; iConflict++)
                    {
                        var conflictScope = currentScope.conflictingScopes[iConflict];
                        this.AddConflict(conflictScope, ident);
                    }

                    currentScope = currentScope.parentScope;
                }
            }

            private void AddConflict(IdentifierScope scope, SimpleIdentifier ident)
            {
                if (scope.paramaterIdentifiers != null)
                {
                    for (int iIdent = 0;
                        iIdent < scope.paramaterIdentifiers.Count;
                        iIdent++)
                    {
                        var matchIdent = scope.paramaterIdentifiers[iIdent];
                        if (matchIdent != ident
                            && matchIdent.SuggestedName == ident.SuggestedName)
                        {
                            this.AddConflictIdentifiers(ident, matchIdent);
                        }
                    }
                }

                for (int iIdent = 0;
                    iIdent < scope.scopedIdentifiers.Count;
                    iIdent++)
                {
                    var matchIdent = scope.scopedIdentifiers[iIdent];
                    if (matchIdent != ident
                        && matchIdent.SuggestedName == ident.SuggestedName)
                    {
                        this.AddConflictIdentifiers(ident, matchIdent);
                    }
                }
            }

            private void AddConflictIdentifiers(SimpleIdentifier left, SimpleIdentifier right)
            {
                List<SimpleIdentifier> identifiers;
                if (!this.identifierMap.TryGetValue(left, out identifiers))
                {
                    identifiers = new List<SimpleIdentifier>();
                    this.identifierMap.Add(left, identifiers);
                }

                identifiers.Add(right);

                if (!this.identifierMap.TryGetValue(right, out identifiers))
                {
                    identifiers = new List<SimpleIdentifier>();
                    this.identifierMap.Add(right, identifiers);
                }

                identifiers.Add(left);
            }

            private void ProcessScopes(
                IList<IdentifierScope> scopes,
                Action<IdentifierScope> processor)
            {
                HashSet<IdentifierScope> processedScopes = new HashSet<IdentifierScope>();
                Queue<IdentifierScope> processingQueue = new Queue<IdentifierScope>(scopes);

                while(processingQueue.Count > 0)
                {
                    IdentifierScope scopeToProcess = processingQueue.Dequeue();
                    if (processedScopes.Contains(scopeToProcess))
                    {
                        continue;
                    }

                    processedScopes.Add(scopeToProcess);
                    processor(scopeToProcess);

                    scopeToProcess.childScopes.ForEach(c => processingQueue.Enqueue(c));
                    scopeToProcess.conflictingScopes.ForEach(c => processingQueue.Enqueue(c));
                }
            }

            private static string GetPostfix(int i)
            {
                int x = i & 0xfff;
                int y = i >> 12;

                if (ReadableIdentifierNamer.intToSuffix[y] == null)
                {
                    ReadableIdentifierNamer.intToSuffix[y] = new string[0x1000];
                }

                if (ReadableIdentifierNamer.intToSuffix[y][x] == null)
                {
                    ReadableIdentifierNamer.intToSuffix[y][x] = ReadableIdentifierNamer.CreatePostfix(i);
                }

                return ReadableIdentifierNamer.intToSuffix[y][x];
            }

            private static string CreatePostfix(int i)
            {
                if (i == 0)
                {
                    return string.Empty;
                }

                var sb = new System.Text.StringBuilder(2);
                while(i > 0)
                {
                    sb.Append((i - 1) % ReadableIdentifierNamer.CharMap.Length);
                    i = i / ReadableIdentifierNamer.CharMap.Length;
                }

                return sb.ToString();
            }
        }
    }
}
