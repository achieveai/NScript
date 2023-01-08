namespace NScript.JST.Visitors
{
    using MoreLinq;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class IdentifierCounterVisitor : IJstVisitor
    {
        public void VisitIdentifierExpression(IdentifierExpression expression)
        {
            expression.Identifier.AddUsage(expression.Scope);
        }

        public void VisitFunctionExpression(FunctionExpression functionExpression)
        {
            functionExpression.Name?.AddUsage(functionExpression.Scope);
            functionExpression.Parameters.ForEach(parm => parm.AddUsage(functionExpression.InnerScope));

            this.VisitFunctionExpressionExt(functionExpression);
        }
    }
}
