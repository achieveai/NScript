using System;
using System.Collections.Generic;
using System.Text;

namespace NScript.JST
{
    class TupleExpression: Expression
    {
        private readonly IList<Expression> arguments;
        private readonly IList<string> parameterNames;

        TupleExpression(NScript.Utils.Location location, IdentifierScope scope,
            IList<string> parameterNames, IList<Expression> arguments)
            : base(location, scope)
        {
            this.parameterNames = parameterNames;
            this.arguments = arguments;
        }

        public override void Write(JSWriter writer)
        {
        }
    }
}
