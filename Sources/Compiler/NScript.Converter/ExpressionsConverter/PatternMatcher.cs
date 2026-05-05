using NScript.CLR;
using NScript.CLR.AST;
using NScript.Converter.TypeSystemConverter;
using System;

namespace NScript.Converter.ExpressionsConverter
{
    /// <summary>
    /// Lowers C# patterns to JavaScript boolean expressions over a scrutinee.
    ///
    /// Centralizes the pattern → JS conversion so that both <see cref="IsPatternConverter"/>
    /// and <see cref="SwitchExpressionConverter"/> share the same lowering rules.
    ///
    /// Today supports: ConstantPattern, DeclarationPattern, DiscardPattern,
    /// RelationalPattern (C# 9), BinaryPattern (and/or, C# 9), NegatedPattern (not, C# 9).
    /// Recursive, list, and slice patterns are not yet supported and surface a clear
    /// <see cref="NotImplementedException"/> with a docs link.
    /// </summary>
    internal static class PatternMatcher
    {
        /// <summary>
        /// Build a JS boolean expression that evaluates whether <paramref name="scrutinee"/>
        /// satisfies <paramref name="pattern"/>. The resulting expression may have side
        /// effects (assignments to declaration-pattern capture variables).
        /// </summary>
        public static JST.Expression LowerToCondition(
            IMethodScopeConverter converter,
            Pattern pattern,
            JST.Expression scrutinee,
            Mono.Cecil.TypeReference scrutineeStaticType = null)
        {
            switch (pattern)
            {
                case ConstantPattern constantPattern:
                    return new JST.BinaryExpression(
                        scrutinee.Location,
                        converter.Scope,
                        JST.BinaryOperator.StrictEquals,
                        scrutinee,
                        ExpressionConverterBase.Convert(converter, constantPattern.ConstantExpression));

                case DiscardPattern _:
                    return new JST.BooleanLiteralExpression(converter.Scope, true);

                case RelationalPattern relPattern:
                    return new JST.BinaryExpression(
                        relPattern.Location,
                        converter.Scope,
                        ToJsOperator(relPattern.Operator),
                        scrutinee,
                        ExpressionConverterBase.Convert(converter, relPattern.ConstantExpression));

                case NegatedPattern negPattern:
                    return new JST.UnaryExpression(
                        negPattern.Location,
                        converter.Scope,
                        JST.UnaryOperator.LogicalNot,
                        LowerToCondition(converter, negPattern.Inner, scrutinee, scrutineeStaticType));

                case BinaryPattern binPattern:
                    return new JST.BinaryExpression(
                        binPattern.Location,
                        converter.Scope,
                        binPattern.Disjunction ? JST.BinaryOperator.LogicalOr : JST.BinaryOperator.LogicalAnd,
                        LowerToCondition(converter, binPattern.Left, scrutinee, scrutineeStaticType),
                        LowerToCondition(converter, binPattern.Right, scrutinee, scrutineeStaticType));

                case DeclarationPattern declarationPattern:
                    return LowerDeclarationPattern(converter, declarationPattern, scrutinee, scrutineeStaticType);

                default:
                    throw new NotImplementedException(
                        $"Pattern shape '{pattern.GetType().Name}' is not yet supported. "
                        + "See docs/language/csharp9-13-status.md for the supported subset.");
            }
        }

        private static JST.Expression LowerDeclarationPattern(
            IMethodScopeConverter converter,
            DeclarationPattern declarationPattern,
            JST.Expression scrutinee,
            Mono.Cecil.TypeReference scrutineeStaticType)
        {
            // (x is Type2 y) → ((y = Type.AsType(Type2, x)) != null)
            JST.Expression variableAccess = declarationPattern.VariableOpt != null
                ? ExpressionConverterBase.Convert(converter, declarationPattern.VariableOpt)
                : null;

            if (declarationPattern.VariableOpt != null
                && scrutineeStaticType != null
                && declarationPattern.VariableOpt.ResultType.IsSame(scrutineeStaticType))
            {
                // Same static type: just bind and return true.
                return new JST.BinaryExpression(
                    null,
                    converter.Scope,
                    JST.BinaryOperator.LogicalOr,
                    new JST.BinaryExpression(
                        null,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        variableAccess,
                        scrutinee),
                    new JST.BooleanLiteralExpression(converter.Scope, true));
            }

            var ty = declarationPattern.TypeReference;
            var methodReference = converter.KnownReferences.AsTypeMethod;
            var typeRefExpr = JST.IdentifierExpression.Create(null, converter.Scope, converter.Resolve(ty));
            var asTypeCall = MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(typeRefExpr, methodReference, false),
                new JST.Expression[] { scrutinee },
                converter,
                converter.RuntimeManager);

            JST.Expression rhs = variableAccess != null
                ? new JST.BinaryExpression(
                    declarationPattern.Location,
                    converter.Scope,
                    JST.BinaryOperator.Assignment,
                    variableAccess,
                    asTypeCall)
                : asTypeCall;

            return new JST.BinaryExpression(
                declarationPattern.Location,
                converter.Scope,
                JST.BinaryOperator.NotEquals,
                rhs,
                new JST.NullLiteralExpression(converter.Scope));
        }

        private static JST.BinaryOperator ToJsOperator(CLR.AST.BinaryOperator op)
            => op switch
            {
                CLR.AST.BinaryOperator.LessThan => JST.BinaryOperator.LessThan,
                CLR.AST.BinaryOperator.LessThanOrEqual => JST.BinaryOperator.LessThanOrEqual,
                CLR.AST.BinaryOperator.GreaterThan => JST.BinaryOperator.GreaterThan,
                CLR.AST.BinaryOperator.GreaterThanOrEqual => JST.BinaryOperator.GreaterThanOrEqual,
                CLR.AST.BinaryOperator.Equals => JST.BinaryOperator.StrictEquals,
                CLR.AST.BinaryOperator.NotEquals => JST.BinaryOperator.StrictNotEquals,
                _ => throw new NotImplementedException(
                    $"Relational pattern operator {op} is not supported.")
            };
    }
}
