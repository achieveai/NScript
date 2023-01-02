namespace NScript.JST.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MoreLinq.Extensions;
    using System.Diagnostics;

    public class InlineableVisitor : IJstVisitor
    {
        Dictionary<IIdentifier, FuncStackCapture> functions = new();
        Stack<FuncStackCapture> identfierScopes = new();

        public Dictionary<IIdentifier, FuncStackCapture> Functions => functions;

        public void VisitFunctionExpression(FunctionExpression funcExpression)
        {
            identfierScopes.Push(new()
            {
                FunctionExpression = funcExpression,
            });

            this.VisitFunctionExpressionExt(funcExpression);

            var leakedScopes = identfierScopes.Pop();

            if (funcExpression.Name != null)
            {
                bool simpleMethodCall = false;
                // If no parameter is used more than once,
                // and the order of use of parameters is exactly as passed in.
                var simpleInlinableMethod = !leakedScopes.ParameterUses.Any(kv => kv.Value > 1)
                    && !leakedScopes.ParameterUsed.Pairwise((l, r) => l < r).Any(v => !v);

                leakedScopes.SimpleInlinableMethod = simpleInlinableMethod;
                leakedScopes.MethodName = funcExpression.Name.GetName();

                if (funcExpression.Statements.Count == 1)
                {
                    simpleMethodCall = ((funcExpression.Statements[0] switch
                    {
                        ScopeBlock scopeBlock => scopeBlock.Statements.Count == 1 ? scopeBlock.Statements[0] : null,
                        Statement stmt => stmt
                    }) switch
                    {
                        ReturnStatement returnStatement => returnStatement.ReturnExpression as MethodCallExpression,
                        ExpressionStatement exprStatement => exprStatement.Expression as MethodCallExpression,
                        _ => null
                    }) switch
                    {
                        MethodCallExpression callExpr => callExpr.MethodExpression is IdentifierExpression identifier
                            && identifier.Identifier is SimpleIdentifier
                            && !callExpr.Arguments.Any(expr => expr is not IdentifierExpression),
                        _ => false,
                    };
                }

                if (simpleMethodCall && simpleInlinableMethod)
                {
                    leakedScopes.SimpleMethodProxy = true;
                }

                functions.Add(
                    funcExpression.Name,
                    leakedScopes);
            }
        }

        public void VisitIdentifierExpression(IdentifierExpression identifierExpression)
        {
            var identifier = identifierExpression.Identifier;
            VisitIdentifier(identifier);
        }

        public void VisitThisExpression(ThisExpression expr)
        {
            identfierScopes.Peek().UsesThis = true;
        }

        private void VisitIdentifier(IIdentifier identifier)
        {
            if (identifier.IsEmpty)
            {
                return;
            }

            switch(identifier)
            {
                case SimpleIdentifier simpleIdent:
                    {
                        var scope = simpleIdent.OwnerScope;
                        var funcScopeInfo = identfierScopes.Peek();
                        var funcScope = funcScopeInfo.FunctionExpression.Scope;

                        if (scope == funcScope
                            && funcScope.ParameterIdentifiers.Contains(simpleIdent))
                        {
                            var parmIdx = funcScope.ParameterIdentifiers.IndexOf(simpleIdent);
                            if (!funcScopeInfo.ParameterUsed.Contains(parmIdx))
                            {
                                funcScopeInfo.ParameterUsed.Add(parmIdx);
                                funcScopeInfo.ParameterUses.Add(parmIdx, 1);
                            }
                            else
                            {
                                funcScopeInfo.ParameterUsed[parmIdx]++;
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
