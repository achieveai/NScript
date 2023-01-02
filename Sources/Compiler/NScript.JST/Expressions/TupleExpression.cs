namespace NScript.JST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class TupleExpression: Expression
    {
        private readonly IList<Expression> arguments;

        private readonly IList<string> parameterNames;

        public TupleExpression(
            NScript.Utils.Location location,
            IdentifierScope scope,
            IList<string> parameterNames,
            IList<Expression> arguments)
            : base(location, scope)
        {
            this.parameterNames = parameterNames;
            this.arguments = arguments;
            Arguments = new ReadOnlyCollection<Expression>(arguments);
            ParameterNames = new ReadOnlyCollection<string>(parameterNames);
        }

        public ReadOnlyCollection<Expression> Arguments { get; }

        public ReadOnlyCollection<string> ParameterNames { get; }

        public override void Write(JSWriter writer)
        {
        }
    }
}
