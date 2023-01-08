namespace NScript.JST.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MoreLinq.Extensions;
    using System.Diagnostics;

    public class InlineableVisitor : IJstVisitor
    {
        readonly Stack<FuncStackCapture> _identifierScopes = new();

        public Dictionary<IIdentifier, FuncStackCapture> Functions { get; } = new();

        public void VisitFunctionExpression(FunctionExpression funcExpression)
        {
            _identifierScopes.Push(new()
            {
                FunctionExpression = funcExpression,
            });

            this.VisitFunctionExpressionExt(funcExpression);

            var leakedScopes = _identifierScopes.Pop();

            if (funcExpression.Name != null)
            {
                SimpleIdentifier proxyMethod = null;
                // If no parameter is used more than once,
                // and the order of use of parameters is exactly as passed in.
                var simpleInlinableMethod = !leakedScopes.ParameterUses.Any(kv => kv.Value > 1)
                    && leakedScopes.ParameterUsed.Pairwise((l, r) => l < r).All(v => v);

                leakedScopes.SimpleInlinableMethod = simpleInlinableMethod;
                leakedScopes.MethodName = funcExpression.Name.GetName();

                if (funcExpression.Statements.Count == 1)
                {
                    proxyMethod = funcExpression.Statements[0] switch
                    {
                        ScopeBlock scopeBlock => scopeBlock.Statements.Count == 1 ? scopeBlock.Statements[0] : null,
                        Statement stmt => stmt
                    } switch
                    {
                        ReturnStatement returnStatement => returnStatement.ReturnExpression as MethodCallExpression,
                        ExpressionStatement exprStatement => exprStatement.Expression as MethodCallExpression,
                        _ => null
                    } switch
                    {
                        MethodCallExpression callExpr => callExpr.MethodExpression is IdentifierExpression identifier
                            && identifier.Identifier is SimpleIdentifier simpleIdentifier
                            && callExpr.Arguments // All the parameters should be in same positions (trailing could be missing)
                                .Select((a,i) => a is IdentifierExpression identExpr
                                    && identExpr.Identifier is SimpleIdentifier simpleIdent
                                    && funcExpression.Parameters.IndexOf(simpleIdent) == i)
                                .All(t => t)
                            ? simpleIdentifier : null,
                        _ => null,
                    };
                }

                if (proxyMethod != null && simpleInlinableMethod)
                {
                    leakedScopes.SimpleMethodProxy = true;
                    leakedScopes.ProxyMethodIdentifier = proxyMethod;
                }

                if (!Functions.ContainsKey(funcExpression.Name))
                {
                    Functions.Add(
                        funcExpression.Name,
                        leakedScopes);
                }
            }
        }

        public void VisitIdentifierExpression(IdentifierExpression identifierExpression)
        {
            var identifier = identifierExpression.Identifier;
            VisitIdentifier(identifier);
        }

        public void VisitThisExpression(ThisExpression expr)
        {
            _identifierScopes.Peek().UsesThis = true;
        }

        private void VisitIdentifier(IIdentifier identifier)
        {
            if (_identifierScopes.Count == 0)
            {
                return;
            }

            switch(identifier)
            {
                case SimpleIdentifier simpleIdent:
                    {
                        var scope = simpleIdent.OwnerScope;
                        var funcScopeInfo = _identifierScopes.Peek();
                        var funcScope = funcScopeInfo.FunctionExpression.Scope;

                        if (scope == funcScope
                            && funcScope.ParameterIdentifiers.Contains(simpleIdent))
                        {
                            var parmIdx = funcScope.ParameterIdentifiers.IndexOf(simpleIdent);
                            if (!funcScopeInfo.ParameterUses.ContainsKey(parmIdx))
                            {
                                funcScopeInfo.ParameterUsed.Add(parmIdx);
                                funcScopeInfo.ParameterUses.Add(parmIdx, 1);
                            }
                            else
                            {
                                funcScopeInfo.ParameterUses[parmIdx]++;
                            }
                        }

                        if (scope.IsExecutionScope)
                        {
                            return;
                        }

                        while(funcScope != null)
                        {
                            if (funcScope == scope)
                            {
                                funcScopeInfo.ReferencedScopes.Add(scope);
                                funcScopeInfo.LeakyIdentifiers.Add(simpleIdent);
                                return;
                            }

                            funcScope = funcScope.ParentScope;
                        }
                    }
                    break;
                case CompoundIdentifier compoundIdent:
                    foreach (var ident in compoundIdent.Identifiers)
                    {
                        VisitIdentifier(ident);
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }

        [DebuggerDisplay("{MethodName}: {SimpleMethodProxy} {SimpleInlinableMethod} {UsesThis}")]
        public class FuncStackCapture
        {
            public bool SimpleMethodProxy { get; set; }

            public bool UsesThis { get; set; }

            public bool SimpleInlinableMethod { get; set; }

            public IIdentifier ProxyMethodIdentifier { get; set; }

            public string MethodName { get; set; }

            public FunctionExpression FunctionExpression { get; set; }

            public HashSet<IdentifierScope> ReferencedScopes { get; } = new HashSet<IdentifierScope>();

            public List<int> ParameterUsed { get; } = new List<int>();

            public Dictionary<int, int> ParameterUses { get; } = new Dictionary<int, int>();

            public HashSet<SimpleIdentifier> SimpleIdentifiers { get; } = new HashSet<SimpleIdentifier>();

            public HashSet<SimpleIdentifier> LeakyIdentifiers { get; } = new HashSet<SimpleIdentifier>();
        }
    }
}
