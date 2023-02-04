using NScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScript.CLR.AST
{
    public class SwitchExpression : Expression
    {
        public SwitchExpression(ClrContext context, Location location, Expression switchValue, List<CaseLabel> caseLabels, List<Expression> expressions) : base(context, location)
        {
            SwitchValue = switchValue;
            CaseLabels = caseLabels;
            Expressions = expressions;
        }

        public Expression SwitchValue { get; set; }

        public List<CaseLabel> CaseLabels { get; set; }

        public List<Expression> Expressions { get; set; }
    }
}
