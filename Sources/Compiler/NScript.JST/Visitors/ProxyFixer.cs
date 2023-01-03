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
            => identifierExpression.Identifier is SimpleIdentifier identifier
                && _inlinableInfo.TryGetValue(identifier, out var capture)
                && capture.SimpleMethodProxy
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
}
