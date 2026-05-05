using Mono.Cecil;
using NScript.Utils;

namespace NScript.CLR.AST
{
    public abstract class Pattern: Node
    {
        protected Pattern(ClrContext ctx, Location location)
            : base(ctx, location)
        {
        }
    }

    public class DeclarationPattern : Pattern
    {
        public DeclarationPattern(
            ClrContext ctx,
            Location location,
            VariableReference? localVariableOpt,
            TypeReference ty,
            Expression? whenExpressionOpt)
            : base(ctx, location)
        {
            VariableOpt = localVariableOpt;
            TypeReference = ty;
            WhenExpressionOpt = whenExpressionOpt;
        }

        public TypeReference TypeReference { get; }

        public VariableReference? VariableOpt { get; }

        public Expression? WhenExpressionOpt { get;  }

    }

    public class ConstantPattern : Pattern
    {
        public ConstantPattern(ClrContext ctx, Location location, Expression constantExpression)
            : base(ctx, location)
        {
            ConstantExpression = constantExpression;
        }

        public Expression ConstantExpression { get;  }
    }

    public class DiscardPattern : Pattern
    {
        public DiscardPattern(ClrContext ctx, Location location)
            : base(ctx, location)
        { }
    }

    /// <summary>
    /// C# 9 relational pattern: <c>x is &lt; 5</c>, <c>&gt;= 0</c>.
    /// </summary>
    public class RelationalPattern : Pattern
    {
        public RelationalPattern(ClrContext ctx, Location location, BinaryOperator op, Expression constantExpression)
            : base(ctx, location)
        {
            Operator = op;
            ConstantExpression = constantExpression;
        }

        public BinaryOperator Operator { get; }

        public Expression ConstantExpression { get; }
    }

    /// <summary>
    /// C# 9 logical pattern combinator: <c>and</c> / <c>or</c>.
    /// </summary>
    public class BinaryPattern : Pattern
    {
        public BinaryPattern(ClrContext ctx, Location location, bool disjunction, Pattern left, Pattern right)
            : base(ctx, location)
        {
            Disjunction = disjunction;
            Left = left;
            Right = right;
        }

        /// <summary>True for <c>or</c>, false for <c>and</c>.</summary>
        public bool Disjunction { get; }

        public Pattern Left { get; }

        public Pattern Right { get; }
    }

    /// <summary>
    /// C# 9 logical pattern combinator: <c>not</c>.
    /// </summary>
    public class NegatedPattern : Pattern
    {
        public NegatedPattern(ClrContext ctx, Location location, Pattern inner)
            : base(ctx, location)
        {
            Inner = inner;
        }

        public Pattern Inner { get; }
    }
}
