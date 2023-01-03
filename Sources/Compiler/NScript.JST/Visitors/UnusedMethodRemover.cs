namespace NScript.JST.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UnusedMethodRemover : ITransformerVisitor
    {
        private bool methodRemoved = false;

        public Statement VisitExpressionStatement(ExpressionStatement statement)
        {
            if (statement.Expression is FunctionExpression function)
            {
                if (function.Name == null || ((SimpleIdentifier)function.Name).UsageCount == 1)
                {
                    methodRemoved = true;
                    return new EmptyStatement(
                        statement.Location,
                        statement.Scope);
                }
            }

            return this.VisitExpressionStatementExt(statement);
        }

        public bool MethodRemove => methodRemoved;

        public void Reset()
        {
            methodRemoved = false;
        }
    }
}
