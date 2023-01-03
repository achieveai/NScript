namespace NScript.JST.Visitors
{
    using System.Collections.Generic;

    public class ProxyFixer : ITransformerVisitor
    {
        private Dictionary<IIdentifier, InlineableVisitor.FuncStackCapture> _inlinableInfo;

        public ProxyFixer(Dictionary<IIdentifier, InlineableVisitor.FuncStackCapture> inlinableInfo)
        {
            _inlinableInfo = inlinableInfo;
        }

        public Expression VisitIdentifierExpression(IdentifierExpression identifierExpression)
        {
            if (identifierExpression.Identifier is SimpleIdentifier identifier
                && _inlinableInfo.TryGetValue(identifier, out var capture)
                )
            {
                return capture.SimpleMethodProxy
                    && IsAccessible((SimpleIdentifier)capture.ProxyMethodIdentifier, identifierExpression.Scope)
                    ? VisitIdentifierExpression(
                        // Note: we have to make sure a proxy method is not pointing to another proxy method.
                        new IdentifierExpression(
                            capture.ProxyMethodIdentifier,
                            identifierExpression.Scope,
                            identifierExpression.Location))
                    : new IdentifierExpression(
                        identifierExpression.Identifier,
                        identifierExpression.Scope,
                        identifierExpression.Location);
            }

            return new IdentifierExpression(
                        identifierExpression.Identifier,
                        identifierExpression.Scope,
                        identifierExpression.Location);
        }

        private static bool IsAccessible(
            SimpleIdentifier proxyIdentifier,
            IdentifierScope currentScope)
        {
            while(currentScope != null) {
                if (proxyIdentifier.OwnerScope == currentScope) {
                    return true;
                }

                currentScope = currentScope.ParentScope;
            }

            return false;
        }
    }
}
