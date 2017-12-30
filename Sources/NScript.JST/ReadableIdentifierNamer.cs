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
            Dictionary<SimpleIdentifier, int> identifierMapping = new Dictionary<SimpleIdentifier, int>();
            List<SimpleIdentifier> identifiers = new List<SimpleIdentifier>();
            List<List<int>> conflictMap = new List<List<int>>();
            string[] assignedNames = null;

            static string[][] intToSuffix =
                new string[10][];

            const string CharMap = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public ReadableIdentifierNamer(IdentifierScope rootScopes)
            {
                this.ProcessScopes(
                    rootScopes,
                    this.CollectSimpleIdentifiers);

                this.assignedNames = new string[this.identifiers.Count];
                this.ProcessScopes(
                    rootScopes,
                    this.AssignNames);
            }

            private void AssignNames(IdentifierScope scope)
            {
                scope.assignedNames = (si) => this.assignedNames[identifierMapping[si]];

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
                var identIdx = identifierMapping[ident];
                if (ident.ShouldEnforceSuggestion)
                {
                    this.assignedNames[identIdx] = ident.SuggestedName;
                    return;
                }

                string prefix = ident.SuggestedName;
                var prefixLength = prefix.Length;

                for (int i = 0; i < 1000; i++)
                {
                    string postfix = ReadableIdentifierNamer.GetPostfix(i);
                    var postfixLength = postfix.Length;
                    bool clash = false;
                    List<int> conflicts = this.conflictMap[identIdx];
                    if (conflicts != null) 
                    {
                        foreach (var conflictIdent in conflicts)
                        {
                            string name = this.assignedNames[conflictIdent];
                            if (name != null)
                            {
                                var nameLength = name.Length;
                                if (postfixLength == 0)
                                {
                                    clash = name == prefix;
                                    if (clash) { break; }
                                }
                                else if (nameLength == prefixLength + postfixLength)
                                {
                                    bool match = true;

                                    for (int jPostFix = 0; match && jPostFix < postfix.Length; jPostFix++)
                                    { match = name[nameLength - jPostFix - 1] == postfix[postfixLength - jPostFix - 1]; }

                                    for (int iName = prefixLength - 3; match && iName < prefixLength; iName++)
                                    { match = name[iName] == prefix[iName]; }

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
                        this.assignedNames[identIdx] = prefix + postfix;
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
                        var identIdx = this.GetIdentifierIndex(ident);
                        // this.AddIdentifierConflicts(scope, ident);
                        for (int j = 0; j < scope.usedIdentifiers.Count; j++)
                        {
                            var jId = scope.usedIdentifiers[j];
                            if (ident == jId)
                            { continue; }

                            this.AddConflictIdentifiers(identIdx, jId);
                        }
                    }
                }

                if (scope.IsExecutionScope)
                {
                    for (int i = 0; i < scope.scopedIdentifiers.Count; i++)
                    {
                        var iId = scope.scopedIdentifiers[i];
                        var identIdx = this.GetIdentifierIndex(iId);

                        for (int j = 0; j < scope.usedIdentifiers.Count; j++)
                        {
                            var jId = scope.usedIdentifiers[j];
                            if (iId == jId)
                            { continue; }

                            this.AddConflictIdentifiers(identIdx, jId);
                        }
                    }
                }
                else
                {
                    foreach (var ident in scope.scopedIdentifiers)
                    {
                        this.AddIdentifierConflicts(
                            scope,
                            this.GetIdentifierIndex(ident));
                    }
                }
            }

            private void AddIdentifierConflicts(IdentifierScope scope, int identIdx)
            {
                var currentScope = scope;
                while(currentScope != null)
                {
                    this.AddConflict(currentScope, identIdx);
                    currentScope = currentScope.parentScope;
                }
            }

            private void AddConflict(IdentifierScope scope, int identIdx)
            {
                if (scope.isExecutionScope)
                { throw new InvalidOperationException(); }

                for (int iIdent = 0;
                    iIdent < scope.scopedIdentifiers.Count;
                    iIdent++)
                {
                    var matchIdent = scope.scopedIdentifiers[iIdent];
                    this.AddConflictIdentifiers(identIdx, matchIdent);
                }
            }

            private void AddConflictIdentifiers(int leftIdx, SimpleIdentifier right)
            {
                if (CanConflict(this.identifiers[leftIdx].SuggestedName, right.SuggestedName))
                {
                    List<int> identifiers = this.conflictMap[leftIdx];
                    if (identifiers == null)
                    {
                        identifiers = new List<int>();
                        this.conflictMap[leftIdx] = identifiers;
                    }

                    identifiers.Add(this.GetIdentifierIndex(right));
                }

                /*
                if (CanConflict(right.SuggestedName, left.SuggestedName))
                {
                    if (!this.identifierMap.TryGetValue(right, out identifiers))
                    {
                        identifiers = new List<SimpleIdentifier>();
                        this.identifierMap.Add(right, identifiers);
                    }

                    identifiers.Add(left);
                }
                */
            }

            private int GetIdentifierIndex(SimpleIdentifier identifier)
            {
                int rv;
                if (this.identifierMapping.TryGetValue(identifier, out rv))
                { return rv; }

                this.identifierMapping[identifier] = rv = this.identifierMapping.Count;
                this.conflictMap.Add(null);
                this.identifiers.Add(identifier);
                return rv;
            }

            private static bool CanConflict(string left, string right)
            {
                int minLength = Math.Min(left.Length, right.Length);
                int maxLen = Math.Max(left.Length, right.Length);

                if (maxLen > minLength + 2)
                { return false; }

                for (int idx = minLength - 1; idx >= 0; idx--)
                {
                    if (left[idx] != right[idx])
                    { return false; }
                }

                return true;
            }

            private void ProcessScopes(
                IdentifierScope scope,
                Action<IdentifierScope> processor)
            {
                HashSet<IdentifierScope> processedScopes = new HashSet<IdentifierScope>();
                Queue<IdentifierScope> processingQueue = new Queue<IdentifierScope>();
                processingQueue.Enqueue(scope);

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
