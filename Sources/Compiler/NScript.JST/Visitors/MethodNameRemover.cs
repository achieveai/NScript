namespace NScript.JST.Visitors
{
    public class MethodNameRemover : ITransformerVisitor
    {
        public Expression VisitFunctionExpression(FunctionExpression functionExpression)
        {
            if (functionExpression.Name != null
                && ((SimpleIdentifier)functionExpression.Name).UsageCount == 1)
            {
                return new FunctionExpression(
                    functionExpression.Location,
                    functionExpression.Scope,
                    functionExpression.InnerScope,
                    functionExpression.Parameters,
                    null);
            }

            return TransformerVisitorExtension.VisitFunctionExpressionExt(this, functionExpression);
        }
    }
}
