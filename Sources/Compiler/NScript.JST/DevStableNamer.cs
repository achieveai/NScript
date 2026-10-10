namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    partial class IdentifierScope
    {
        /// <summary>
        /// Dev-mode naming (slice 2, design section 3). A name depends only on the identifier's
        /// identity and metadata, or, for locals, on its own function, so a body edit renames
        /// nothing outside that function and every bundle names shared slots the same way.
        /// Replaces <see cref="IdentifierMinifiedNamer"/> when dev mode is on; release naming is
        /// untouched.
        /// </summary>
        public sealed class DevStableNamer
        {
            /// <summary>JS reserved words plus the globals a local must never shadow.</summary>
            private static readonly HashSet<string> ReservedWords = new HashSet<string>(StringComparer.Ordinal)
            {
                "abstract", "arguments", "await", "async", "boolean", "break", "byte", "case",
                "catch", "char", "class", "const", "continue", "debugger", "default", "delete",
                "do", "double", "else", "enum", "eval", "export", "extends", "false", "final",
                "finally", "float", "for", "function", "goto", "if", "implements", "import",
                "in", "instanceof", "int", "interface", "let", "long", "native", "new", "null",
                "package", "private", "protected", "public", "return", "short", "static", "super",
                "switch", "synchronized", "this", "throw", "throws", "transient", "true", "try",
                "typeof", "var", "void", "volatile", "while", "with", "yield",
                "undefined", "NaN", "Infinity",
            };

            /// <summary>Runtime interface slots are <c>name + "_" + typeId</c>; dev type ids are k + 9 base62.</summary>
            private static readonly Regex InterfaceSlotSuffix = new Regex("_k[0-9A-Za-z]{9}$", RegexOptions.CultureInvariant);

            private readonly Dictionary<SimpleIdentifier, string> names = new Dictionary<SimpleIdentifier, string>();
            private readonly List<string> errors = new List<string>();
            private readonly List<string> fallbacks = new List<string>();
            private readonly Dictionary<IdentifierScope, HashSet<string>> enforcedInSubtree =
                new Dictionary<IdentifierScope, HashSet<string>>();

            private DevStableNamer()
            {
            }

            /// <summary>NSDEV001 / NSDEV002 messages. Non-empty means the bundle must not be written.</summary>
            public IReadOnlyList<string> Errors => this.errors;

            /// <summary>Suggested names of non-enforced identifiers that had no <see cref="SimpleIdentifier.StableName"/>.</summary>
            public IReadOnlyList<string> Fallbacks => this.fallbacks;

            /// <summary>Number of identifiers this namer assigned.</summary>
            public int NamedCount => this.names.Count;

            /// <summary>
            /// Names the execution tree under <paramref name="root"/> (the global scope): root
            /// identifiers get their StableName, locals and parameters get their C# name.
            /// </summary>
            public static DevStableNamer NameExecutionTree(IdentifierScope root)
            {
                if (!root.IsExecutionScope || root.ParentScope != null)
                {
                    throw new ArgumentException("Expected the root execution scope.", nameof(root));
                }

                var namer = new DevStableNamer();
                namer.NameRoot(root);
                var rootEnforced = new HashSet<string>(root.knownNameMap.Keys, StringComparer.Ordinal);
                foreach (var child in root.ChildScopes)
                {
                    namer.NameLocals(child, rootEnforced);
                }

                return namer;
            }

            /// <summary>
            /// Names the member tree under <paramref name="root"/> (the Object prototype scope).
            /// Every name must be unique along its lookup chain (NSDEV001).
            /// </summary>
            public static DevStableNamer NameTypeTree(IdentifierScope root)
            {
                if (root.IsExecutionScope || root.ParentScope != null)
                {
                    throw new ArgumentException("Expected the root member (type) scope.", nameof(root));
                }

                var namer = new DevStableNamer();
                namer.NameMembers(root, 0, new Dictionary<string, SimpleIdentifier>(StringComparer.Ordinal));
                return namer;
            }

            private string GetName(SimpleIdentifier ident)
            {
                if (ident.ShouldEnforceSuggestion)
                {
                    return ident.SuggestedName;
                }

                if (this.names.TryGetValue(ident, out var name))
                {
                    return name;
                }

                throw new InvalidOperationException(
                    "NSDEV002: identifier '" + ident.SuggestedName + "' was not named by dev naming "
                    + "(created after naming ran, or outside the named scope tree).");
            }

            private string NameOf(SimpleIdentifier ident)
                => ident.ShouldEnforceSuggestion
                    ? ident.SuggestedName
                    : this.names.TryGetValue(ident, out var name) ? name : null;

            private void NameRoot(IdentifierScope root)
            {
                root.assignedNames = this.GetName;
                var byName = new Dictionary<string, SimpleIdentifier>(StringComparer.Ordinal);
                var fallbackOrdinals = new Dictionary<string, int>(StringComparer.Ordinal);

                foreach (var ident in root.scopedIdentifiers)
                {
                    string name;
                    if (ident.ShouldEnforceSuggestion)
                    {
                        name = ident.SuggestedName;
                    }
                    else if (ident.StableName != null)
                    {
                        name = ident.StableName;
                    }
                    else
                    {
                        // Rule 5: plugin-made (or not yet covered) root identifiers. The ordinal
                        // counts only identifiers with the same suggested name.
                        if (string.IsNullOrEmpty(ident.SuggestedName))
                        {
                            this.errors.Add("NSDEV002: a non-enforced root identifier has neither a stable nor a suggested name.");
                        }

                        fallbackOrdinals.TryGetValue(ident.SuggestedName, out var ordinal);
                        fallbackOrdinals[ident.SuggestedName] = ++ordinal;
                        name = ident.SuggestedName + "$$g" + (ordinal == 1 ? string.Empty : ordinal.ToString());
                        this.fallbacks.Add(ident.SuggestedName);
                    }

                    if (name.Length == 0 && !ident.ShouldEnforceSuggestion)
                    {
                        this.errors.Add("NSDEV002: root identifier '" + ident.OriginalSuggestedName + "' ended up with no name.");
                    }

                    if (byName.TryGetValue(name, out var other))
                    {
                        if (!(other.ShouldEnforceSuggestion && ident.ShouldEnforceSuggestion))
                        {
                            this.errors.Add(
                                "NSDEV001: root name '" + name + "' is used by '" + other.OriginalSuggestedName
                                + "' and '" + ident.OriginalSuggestedName + "'.");
                        }
                    }
                    else
                    {
                        byName.Add(name, ident);
                    }

                    if (!ident.ShouldEnforceSuggestion)
                    {
                        this.names[ident] = name;
                    }
                }
            }

            /// <summary>
            /// Rule 4, parent-first: a local or parameter keeps its C# name, with <c>_2</c>,
            /// <c>_3</c> on collision. It avoids reserved words, root enforced names, every
            /// name an enclosing scope assigned, every free identifier used in this subtree and
            /// every enforced name declared in this subtree. All inputs come from this
            /// function's own subtree or from root names, which gives locality.
            /// </summary>
            private void NameLocals(IdentifierScope scope, HashSet<string> enclosing)
            {
                scope.assignedNames = this.GetName;
                var taken = new HashSet<string>(enclosing, StringComparer.Ordinal);
                taken.UnionWith(this.EnforcedInSubtree(scope));

                foreach (var used in scope.usedIdentifiers)
                {
                    if (used.OwnerScope == scope)
                    {
                        continue;
                    }

                    var usedName = this.NameOf(used);
                    if (usedName == null)
                    {
                        this.errors.Add(
                            "NSDEV002: free identifier '" + used.OriginalSuggestedName
                            + "' is used before its owner scope was named.");
                        continue;
                    }

                    taken.Add(usedName);
                }

                var own = new List<SimpleIdentifier>();
                if (scope.ParameterIdentifiers != null)
                {
                    own.AddRange(scope.ParameterIdentifiers);
                }

                own.AddRange(scope.scopedIdentifiers);

                foreach (var ident in own)
                {
                    if (ident.ShouldEnforceSuggestion || this.names.ContainsKey(ident))
                    {
                        continue;
                    }

                    var baseName = string.IsNullOrEmpty(ident.SuggestedName) ? "v" : ident.SuggestedName;
                    var name = baseName;
                    for (int suffix = 2; ReservedWords.Contains(name) || taken.Contains(name); suffix++)
                    {
                        name = baseName + "_" + suffix;
                    }

                    taken.Add(name);
                    this.names[ident] = name;
                }

                var childEnclosing = new HashSet<string>(enclosing, StringComparer.Ordinal);
                foreach (var ident in own)
                {
                    childEnclosing.Add(this.NameOf(ident));
                }

                foreach (var child in scope.ChildScopes)
                {
                    this.NameLocals(child, childEnclosing);
                }
            }

            private HashSet<string> EnforcedInSubtree(IdentifierScope scope)
            {
                if (!this.enforcedInSubtree.TryGetValue(scope, out var set))
                {
                    set = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var ident in scope.scopedIdentifiers.Concat(
                        scope.ParameterIdentifiers ?? Enumerable.Empty<SimpleIdentifier>()))
                    {
                        if (ident.ShouldEnforceSuggestion)
                        {
                            set.Add(ident.SuggestedName);
                        }
                    }

                    foreach (var child in scope.ChildScopes)
                    {
                        set.UnionWith(this.EnforcedInSubtree(child));
                    }

                    this.enforcedInSubtree.Add(scope, set);
                }

                return set;
            }

            /// <summary>
            /// Rules 1-3 for member scopes: enforced names stay, others use their StableName
            /// (<c>M(name) $ depth kind [$ sig]</c>), and identifiers without one fall back to
            /// <c>suggested $ depth g</c>. A name may not repeat along a lookup chain unless both
            /// identifiers are enforced (an imported override).
            /// </summary>
            private void NameMembers(IdentifierScope scope, int depth, Dictionary<string, SimpleIdentifier> chain)
            {
                scope.assignedNames = this.GetName;
                var local = new Dictionary<string, SimpleIdentifier>(StringComparer.Ordinal);
                var fallbackOrdinals = new Dictionary<string, int>(StringComparer.Ordinal);

                foreach (var ident in scope.scopedIdentifiers)
                {
                    string name;
                    if (ident.ShouldEnforceSuggestion)
                    {
                        name = ident.SuggestedName;
                    }
                    else if (ident.StableName != null)
                    {
                        name = ident.StableName;
                    }
                    else
                    {
                        fallbackOrdinals.TryGetValue(ident.SuggestedName, out var ordinal);
                        fallbackOrdinals[ident.SuggestedName] = ++ordinal;
                        name = ident.SuggestedName + "$" + depth + "g" + (ordinal == 1 ? string.Empty : "$" + ordinal);
                        this.fallbacks.Add(ident.SuggestedName);
                    }

                    if (!ident.ShouldEnforceSuggestion)
                    {
                        if (InterfaceSlotSuffix.IsMatch(name))
                        {
                            this.errors.Add(
                                "NSDEV001: member name '" + name + "' (" + ScopeLabel(scope)
                                + ") can collide with a runtime interface slot (name_typeId).");
                        }

                        this.names[ident] = name;
                    }

                    SimpleIdentifier other;
                    if ((local.TryGetValue(name, out other) || chain.TryGetValue(name, out other))
                        && other != ident
                        && !(other.ShouldEnforceSuggestion && ident.ShouldEnforceSuggestion))
                    {
                        this.errors.Add(
                            "NSDEV001: member name '" + name + "' is used by '" + other.OriginalSuggestedName
                            + "' (" + ScopeLabel(other.OwnerScope) + ") and '" + ident.OriginalSuggestedName
                            + "' (" + ScopeLabel(scope) + ") on one lookup chain.");
                    }

                    local[name] = ident;
                }

                if (scope.ChildScopes.Count == 0)
                {
                    return;
                }

                var childChain = new Dictionary<string, SimpleIdentifier>(chain, StringComparer.Ordinal);
                foreach (var pair in local)
                {
                    childChain[pair.Key] = pair.Value;
                }

                foreach (var child in scope.ChildScopes)
                {
                    this.NameMembers(child, depth + 1, childChain);
                }
            }

            private static string ScopeLabel(IdentifierScope scope)
                => scope.scopeName ?? "<unnamed scope>";

            /// <summary>
            /// After naming: every name a local in the subtree tried, up to the one it got, with
            /// whether a name from outside the subtree (root enforced names, enclosing scopes'
            /// own names) held it. Locals get the same names again while each answer holds.
            /// </summary>
            public static List<KeyValuePair<string, bool>> CaptureLocalProbes(IdentifierScope subtreeRoot, OuterNames outer)
            {
                var probes = new List<KeyValuePair<string, bool>>();
                var seen = new HashSet<string>(StringComparer.Ordinal);
                CaptureLocalProbes(subtreeRoot, subtreeRoot.ParentScope, outer, probes, seen);
                return probes;
            }

            /// <summary>
            /// True when each probe from <see cref="CaptureLocalProbes"/> gets the same answer
            /// for a subtree whose parent is <paramref name="parent"/>.
            /// </summary>
            public static bool ProbesMatch(IdentifierScope parent, IReadOnlyList<KeyValuePair<string, bool>> probes, OuterNames outer)
            {
                for (int i = 0; i < probes.Count; i++)
                {
                    if (outer.Contains(parent, probes[i].Key) != probes[i].Value)
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// The enforced names in a subtree. They shape the names of the enclosing scope's
            /// locals, so a stand-in for the subtree must hold them too.
            /// </summary>
            public static List<string> EnforcedNames(IdentifierScope subtreeRoot)
            {
                var namer = new DevStableNamer();
                var names = new List<string>(namer.EnforcedInSubtree(subtreeRoot));
                names.Sort(StringComparer.Ordinal);
                return names;
            }

            private static void CaptureLocalProbes(
                IdentifierScope scope,
                IdentifierScope parent,
                OuterNames outer,
                List<KeyValuePair<string, bool>> probes,
                HashSet<string> seen)
            {
                foreach (var ident in (scope.ParameterIdentifiers ?? Enumerable.Empty<SimpleIdentifier>()).Concat(scope.scopedIdentifiers))
                {
                    if (ident.ShouldEnforceSuggestion)
                    {
                        continue;
                    }

                    var baseName = string.IsNullOrEmpty(ident.SuggestedName) ? "v" : ident.SuggestedName;
                    var name = ident.GetName();
                    var last = 1;
                    if (name != baseName
                        && !(name.Length > baseName.Length + 1
                            && name.StartsWith(baseName + "_", StringComparison.Ordinal)
                            && int.TryParse(name.Substring(baseName.Length + 1), out last)))
                    {
                        throw new InvalidOperationException(
                            "Local '" + name + "' does not follow its base name '" + baseName + "'.");
                    }

                    for (int suffix = 1; suffix <= last; suffix++)
                    {
                        var candidate = suffix == 1 ? baseName : baseName + "_" + suffix;
                        if (seen.Add(candidate))
                        {
                            probes.Add(new KeyValuePair<string, bool>(candidate, outer.Contains(parent, candidate)));
                        }
                    }
                }

                foreach (var child in scope.ChildScopes)
                {
                    CaptureLocalProbes(child, parent, outer, probes, seen);
                }
            }

            /// <summary>
            /// The names outside a subtree that its locals avoid, per enclosing scope (see
            /// <see cref="NameLocals"/>): the root's enforced names, then each enclosing scope's own.
            /// Answers are cached, so make one per naming pass.
            /// </summary>
            public sealed class OuterNames
            {
                private readonly Dictionary<IdentifierScope, HashSet<string>> own =
                    new Dictionary<IdentifierScope, HashSet<string>>();

                public bool Contains(IdentifierScope scope, string name)
                {
                    for (; scope != null; scope = scope.ParentScope)
                    {
                        if (scope.ParentScope == null)
                        {
                            return scope.knownNameMap.ContainsKey(name);
                        }

                        if (this.Own(scope).Contains(name))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                private HashSet<string> Own(IdentifierScope scope)
                {
                    if (!this.own.TryGetValue(scope, out var set))
                    {
                        set = new HashSet<string>(StringComparer.Ordinal);
                        foreach (var ident in (scope.ParameterIdentifiers ?? Enumerable.Empty<SimpleIdentifier>()).Concat(scope.scopedIdentifiers))
                        {
                            set.Add(ident.GetName());
                        }

                        this.own.Add(scope, set);
                    }

                    return set;
                }
            }
        }
    }
}
