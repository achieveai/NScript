namespace NScript.JST.Visitors
{
    using System.Linq;

    public class MethodNameRemover : ITransformerVisitor
    {
        public Expression VisitFunctionExpression(FunctionExpression functionExpression)
        {
            if (functionExpression.Name != null
                && ((SimpleIdentifier)functionExpression.Name).UsageCount == 1)
            {
                var rv = new FunctionExpression(
                    functionExpression.Location,
                    functionExpression.Scope,
                    functionExpression.InnerScope,
                    functionExpression.Parameters,
                    null,
                    functionExpression.IsAsync,
                    functionExpression.IsGenerator);

                rv.AddStatements(
                    functionExpression.Statements
                        .Select(this.DispatchStatementExt)
                        .ToList());

                return rv;
            }

            return TransformerVisitorExtension.VisitFunctionExpressionExt(this, functionExpression);
        }
    }
}
