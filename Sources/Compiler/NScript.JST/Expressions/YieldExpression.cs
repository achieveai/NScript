using NScript.Utils;

namespace NScript.JST
{
    public class YieldExpression : Expression
    {
        public YieldExpression(Location location, IdentifierScope scope, Expression? yieldValueOpt)
            : base(location, scope)
        {
            this.YieldValueOpt = yieldValueOpt;
        }

        public Expression YieldValueOpt { get; private set; }

        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Yield);
            if (YieldValueOpt != null)
            {
                writer.Write(YieldValueOpt);
            }
        }
    }
}
