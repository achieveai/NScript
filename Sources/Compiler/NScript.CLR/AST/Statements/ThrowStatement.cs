namespace NScript.CLR.AST
{
    using NScript.Utils;

    public class ThrowStatement : Statement
    {
        public ThrowStatement(
            ClrContext context,
            Location location,
            Expression expressionOpt) : base(context, location)
        {
            Expression = expressionOpt;
        }

        public Expression Expression { get; }
    }
}
