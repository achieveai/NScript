namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class IdentifierScope
    {
        public class IdentifierMinifiedNamer
        {
            private readonly bool _releaseNaming;
            private readonly HashSet<string> _usedNames = new HashSet<string>();
            private readonly SortedSet<NameNode> _names = new SortedSet<NameNode>();
            private readonly Dictionary<SimpleIdentifier, NameNode> _identifierToNode = new Dictionary<SimpleIdentifier, NameNode>();

            private IdentifierMinifiedNamer(
                IdentifierScope rootScope,
                bool releaseNaming)
            {
                _releaseNaming = releaseNaming;
                if (rootScope.IsExecutionScope)
                {
                    ProcessExecutionScope(rootScope);
                }
                else
                {
                    ProcessTypeScope(rootScope);
                }

                AssignNamesToNodes();
            }

            public static void MinifyNames(
                IdentifierScope rootScope,
                bool releaseNaming = false)
            {
                _ = new IdentifierMinifiedNamer(
                    rootScope,
                    releaseNaming);
            }

            public string GetName(SimpleIdentifier ident)
            {
                if (ident.ShouldEnforceSuggestion)
                {
                    return ident.SuggestedName;
                }

                return _releaseNaming
                    ? _identifierToNode[ident].Name
                    : ident.SuggestedName + "_" + _identifierToNode[ident].Name;
            }

            private void ProcessExecutionScope(IdentifierScope scope)
            {
                HashSet<SimpleIdentifier> conflictingNames = new HashSet<SimpleIdentifier>();
                scope.assignedNames = this.GetName;
                scope.UsedIdentifiers
                    .Where(ident => ident.OwnerScope != scope)
                    .ToHashSet();

                ProcessScope(scope, conflictingNames);

                foreach (var child in scope.ChildScopes)
                {
                    ProcessExecutionScope(child);
                }
            }

            private void ProcessTypeScope(IdentifierScope scope)
            {
                HashSet<SimpleIdentifier> conflictingNames = new HashSet<SimpleIdentifier>();
                scope.assignedNames = this.GetName;

                var parentScope = scope.ParentScope;
                while(parentScope != null)
                {
                    foreach (var ident in parentScope.scopedIdentifiers)
                    {
                        conflictingNames.Add(ident);
                    }

                    parentScope = parentScope.parentScope;
                }

                ProcessScope(scope, conflictingNames);

                foreach (var child in scope.ChildScopes)
                {
                    ProcessTypeScope(child);
                }
            }

            private void ProcessScope(
                IdentifierScope scope,
                HashSet<SimpleIdentifier> conflictingNames)
            {
                var localIdentToNameCount = scope
                    .scopedIdentifiers
                    .Where(ident =>
                    {
                        if (ident.ShouldEnforceSuggestion)
                        {
                            _usedNames.Add(ident.SuggestedName);
                            return false;
                        }

                        return true;
                    })
                    .Count(ident => !ident.ShouldEnforceSuggestion);

                var parameterIdentToNameCount = scope.ParameterIdentifiers?.Count ?? 0;

                foreach (var pair in _names
                    .Reverse()
                    .Where(nn => !conflictingNames.Any(ident => nn.HashIdentifier(ident)))
                    .Take(localIdentToNameCount + parameterIdentToNameCount)
                    .Merge(
                        scope
                            .scopedIdentifiers
                            .Where(ident => !ident.ShouldEnforceSuggestion)
                            .Concat(scope.ParameterIdentifiers ?? Enumerable.Empty<SimpleIdentifier>())
                            .OrderByDescending(ident => ident.UsageCount))
                    .ToArray())
                {
                    if (pair.Item1 != null)
                    {
                        pair.Item1.Add(pair.Item2);
                        _identifierToNode.Add(pair.Item2, pair.Item1);
                        _names.Remove(pair.Item1);
                        _names.Add(pair.Item1);
                    }
                    else
                    {
                        var nn = new NameNode(pair.Item2);
                        _identifierToNode.Add(pair.Item2, nn);
                        _names.Add(nn);
                    }
                }
            }

            private void AssignNamesToNodes()
            {
                int nameId = 0;
                var countToNameId = new CountToNamer(_usedNames);
                foreach (var nn in _names.Reverse())
                {
                    nn.AssignNameId(
                        nameId,
                        countToNameId.IntIdToName(nameId++));
                }
            }

            private class NameNode : IComparable<NameNode>
            {
                private int _useCount;
                private int _nameId = -1;
                private string _name;
                private HashSet<SimpleIdentifier> _identifiers = new HashSet<SimpleIdentifier>();

                public NameNode(SimpleIdentifier ident)
                {
                    _identifiers.Add(ident);
                    _useCount += ident.UsageCount;
                }

                public int UseCount => _useCount;

                public string Name => _name;

                public IEnumerable<SimpleIdentifier> Identifiers => _identifiers;

                public void Add(SimpleIdentifier identifier)
                {
                    CheckCanAdd(identifier);
                    _identifiers.Add(identifier);
                    _useCount += identifier.UsageCount;
                }

                public void AssignNameId(
                    int nameId,
                    string name)
                {
                    _nameId = nameId;
                    _name = name;
                }

                public bool HashIdentifier(SimpleIdentifier identifier)
                    => _identifiers.Contains(identifier);

                public int CompareTo(NameNode other)
                {
                    if (other == null)
                    {
                        return 1;
                    }

                    if (ReferenceEquals(this, other))
                    {
                        return 0;
                    }

                    if (_useCount != other._useCount)
                    {
                        return _useCount.CompareTo(other._useCount);
                    }

                    if (_identifiers.Count != other._identifiers.Count)
                    {
                        return _identifiers.Count.CompareTo(other._identifiers.Count);
                    }

                    return GetHashCode().CompareTo(other.GetHashCode());
                }

                [System.Diagnostics.Conditional("Debug")]
                private void CheckCanAdd(SimpleIdentifier identifier)
                {
                    if (identifier == null)
                    {
                        throw new ArgumentNullException(nameof(identifier));
                    }

                    if (_identifiers.Any(ident => identifier.OwnerScope == ident.OwnerScope))
                    {
                        throw new InvalidOperationException("Can't have same name for multiple identifier in same scope");
                    }
                }
            }
        }
    }
}
