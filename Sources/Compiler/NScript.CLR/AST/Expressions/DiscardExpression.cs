namespace NScript.CLR.AST
{
    using NScript.Utils;

    public class DiscardExpression : Expression
    {
        public DiscardExpression(
            ClrContext clrContext,
            Location location)
            : base(clrContext, location)
        {
        }
    }
}
