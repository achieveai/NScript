//-----------------------------------------------------------------------
// <copyright file="BinaryExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for BinaryExpressionConverter
    /// </summary>
    public static class BinaryExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            BinaryExpression expression)
        {
            if (expression.IsAssignmentOperator &&
                BinaryExpressionConverter.IsResolvingToFunctionCall(expression.Left))
            {
                return BinaryExpressionConverter.ConvertFunctionAssignmentExpression(
                    converter,
                    expression);
            }

            return BinaryExpressionConverter.ConvertInternal(
                converter,
                expression.Location,
                expression.Left,
                expression.Operator,
                expression.Right,
                expression.ResultType);
        }

        /// <summary>
        /// Determines whether the specified expressionis resolving to function call.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// <c>true</c> if the specified expression is resolving to function call ; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsResolvingToFunctionCall(
            Expression expression)
        {
            // TODO:
            // Once we add attribute support for Script# with import attribute fix
            // code below.
            if (expression is VariableAddressReference ||
                expression is PropertyReferenceExpression ||
                expression is ArrayElementExpression)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts the function assignment expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Converted expression</returns>
        internal static JST.Expression ConvertFunctionAssignmentExpression(
            IMethodScopeConverter converter,
            BinaryExpression expression)
        {
            List<JST.Expression> arguments = new List<JST.Expression>();

            bool isIntrinsic;

            if (BinaryExpressionConverter.IsIntrinsicExpression(
                converter,
                expression.Left))
            {
                return BinaryExpressionConverter.ConvertInternal(
                    converter,
                    expression.Location,
                    expression.Left,
                    expression.Operator,
                    expression.Right,
                    expression.ResultType);
            }

            BinaryOperator op = expression.Operator;
            switch (expression.Operator)
            {
                case BinaryOperator.Assignment:
                    return BinaryExpressionConverter.GetWriteFunction(
                        converter,
                        expression.Left,
                        ExpressionConverterBase.Convert(converter, expression.Right));

                case BinaryOperator.BitwiseAndAssignment:
                    op = BinaryOperator.BitwiseAnd;
                    break;
                case BinaryOperator.BitwiseOrAssignment:
                    op = BinaryOperator.BitwiseOr;
                    break;
                case BinaryOperator.BitwiseXorAssignment:
                    op = BinaryOperator.BitwiseXor;
                    break;
                case BinaryOperator.DivAssignment:
                    op = BinaryOperator.Div;
                    break;
                case BinaryOperator.LeftShiftAssignment:
                    op = BinaryOperator.LeftShift;
                    break;
                case BinaryOperator.MinusAssignment:
                    op = BinaryOperator.Minus;
                    break;
                case BinaryOperator.ModAssignment:
                    op = BinaryOperator.Mod;
                    break;
                case BinaryOperator.PlusAssignment:
                    op = BinaryOperator.Plus;
                    break;
                case BinaryOperator.RightShiftAssignment:
                    op = BinaryOperator.RightShift;
                    break;
                case BinaryOperator.MulAssignment:
                    op = BinaryOperator.Mul;
                    break;
                case BinaryOperator.UnsignedRightShiftAssignment:
                    op = BinaryOperator.UnsignedRightShift;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return BinaryExpressionConverter.GetWriteFunction(
                converter,
                expression.Left,
                BinaryExpressionConverter.ConvertInternal(
                    converter,
                    expression.Location,
                    expression.Left,
                    op,
                    expression.Right,
                    expression.ResultType));
        }

        /// <summary>
        /// Gets the write function.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>Function for write operation.</returns>
        internal static JST.Expression GetWriteFunction(
            IMethodScopeConverter converter,
            Expression expression,
            JST.Expression value)
        {
            VariableAddressReference refVariable = expression as VariableAddressReference;

            if (refVariable != null)
            {
                return VariableAddressReferenceConverter.Convert(
                    converter,
                    refVariable,
                    false);
            }

            ArrayElementExpression arrayElementExpression = expression as ArrayElementExpression;
            if (arrayElementExpression != null)
            {
                expression = PropertyReferenceConverter.ConvertToPropertyReference(
                    converter.RuntimeManager.Context,
                    arrayElementExpression);
            }

            PropertyReferenceExpression propertyReferenceExpression = expression as PropertyReferenceExpression;
            if (propertyReferenceExpression != null)
            {
                JST.Expression returnValue = PropertyReferenceConverter.Convert(
                    converter,
                    propertyReferenceExpression,
                    value);

                return returnValue;
            }

            throw new NotSupportedException("Only VariableAddressReference and PropertyReferenceExpression is used");
        }

        /// <summary>
        /// Gets an intrinsic expression.
        /// </summary>
        /// <param name="converter">  The converter. </param>
        /// <param name="expression"> The expression. </param>
        /// <returns>
        /// The intrinsic expression.
        /// </returns>
        internal static bool IsIntrinsicExpression(
            IMethodScopeConverter converter,
            Expression expression)
        {
            ArrayElementExpression arrayElementExpression = expression as ArrayElementExpression;
            if (arrayElementExpression != null)
            {
                expression = PropertyReferenceConverter.ConvertToPropertyReference(
                    converter.RuntimeManager.Context,
                    arrayElementExpression);
            }

            PropertyReferenceExpression propertyReferenceExpression = expression as PropertyReferenceExpression;
            if (propertyReferenceExpression != null)
            {
                PropertyDefinition propertyDefinition = propertyReferenceExpression.PropertyReference.Resolve();
                return converter.RuntimeManager.Context.IsIntrinsicProperty(propertyDefinition);
            }

            return false;
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="location">The location.</param>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="op">The op.</param>
        /// <param name="rightExpression">The right expression.</param>
        /// <returns>Converted expression</returns>
        internal static JST.Expression ConvertInternal(
            IMethodScopeConverter converter,
            Location location,
            Expression leftExpression,
            BinaryOperator op,
            Expression rightExpression,
            TypeReference resultType)
        {
            JST.BinaryOperator jstOperator;
            bool isAssignmentOp = false;
            bool isLifted = resultType.IsSameDefinition(converter.ClrKnownReferences.NullableType);

            if (isLifted)
            {
                resultType = ((GenericInstanceType)resultType).GenericArguments[0];
            }

            switch (op)
            {
                case BinaryOperator.Assignment:
                    jstOperator = JST.BinaryOperator.Assignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.BitwiseAnd:
                    jstOperator = JST.BinaryOperator.BitwiseAnd;
                    break;
                case BinaryOperator.BitwiseAndAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.BitwiseAnd
                        : JST.BinaryOperator.BitwiseAndAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.BitwiseOr:
                    jstOperator = JST.BinaryOperator.BitwiseOr;
                    break;
                case BinaryOperator.BitwiseOrAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.BitwiseOr
                        : JST.BinaryOperator.BitwiseOrAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.BitwiseXor:
                    jstOperator = JST.BinaryOperator.BitwiseXor;
                    break;
                case BinaryOperator.BitwiseXorAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.BitwiseXor
                        : JST.BinaryOperator.BitwiseXorAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.Comma:
                    jstOperator = JST.BinaryOperator.Comma;
                    break;
                case BinaryOperator.Div:
                    jstOperator = JST.BinaryOperator.Div;
                    break;
                case BinaryOperator.DivAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.Div
                        : JST.BinaryOperator.DivAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.Equals:
                    jstOperator = JST.BinaryOperator.StrictEquals;
                    break;
                case BinaryOperator.GreaterThan:
                    jstOperator = JST.BinaryOperator.GreaterThan;
                    break;
                case BinaryOperator.GreaterThanOrEqual:
                    jstOperator = JST.BinaryOperator.GreaterThanOrEqual;
                    break;
                case BinaryOperator.IsTypeOf:
                    throw new NotImplementedException();
                case BinaryOperator.LeftShift:
                    jstOperator = JST.BinaryOperator.LeftShift;
                    break;
                case BinaryOperator.LeftShiftAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.LeftShift
                        : JST.BinaryOperator.LeftShiftAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.LessThan:
                    jstOperator = JST.BinaryOperator.LessThan;
                    break;
                case BinaryOperator.LessThanOrEqual:
                    jstOperator = JST.BinaryOperator.LessThanOrEqual;
                    break;
                case BinaryOperator.LogicalAnd:
                    jstOperator = JST.BinaryOperator.LogicalAnd;
                    break;
                case BinaryOperator.LogicalOr:
                    jstOperator = JST.BinaryOperator.LogicalOr;
                    break;
                case BinaryOperator.Minus:
                    jstOperator = JST.BinaryOperator.Minus;
                    break;
                case BinaryOperator.MinusAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.Minus
                        : JST.BinaryOperator.MinusAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.Mod:
                    jstOperator = JST.BinaryOperator.Mod;
                    break;
                case BinaryOperator.ModAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.Mod
                        : JST.BinaryOperator.ModAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.NotEquals:
                    jstOperator = JST.BinaryOperator.StrictNotEquals;
                    break;
                case BinaryOperator.Plus:
                    jstOperator = JST.BinaryOperator.Plus;
                    break;
                case BinaryOperator.PlusAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.Plus
                        : JST.BinaryOperator.PlusAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.RightShift:
                    jstOperator = JST.BinaryOperator.RightShift;
                    break;
                case BinaryOperator.RightShiftAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.RightShift
                        : JST.BinaryOperator.RightShiftAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.Mul:
                    jstOperator = JST.BinaryOperator.Mul;
                    break;
                case BinaryOperator.MulAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.Mul
                        : JST.BinaryOperator.MulAssignment;
                    isAssignmentOp = true;
                    break;
                case BinaryOperator.UnsignedRightShift:
                    jstOperator = JST.BinaryOperator.UnsignedRightShift;
                    break;
                case BinaryOperator.UnsignedRightShiftAssignment:
                    jstOperator = isLifted
                        ? JST.BinaryOperator.UnsignedRightShift
                        : JST.BinaryOperator.UnsignedRightShiftAssignment;
                    isAssignmentOp = true;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            JST.Expression leftJstExpression;
            JST.Expression rightJstExpression;
            JST.Expression leftConditionPartExpression = null;
            JST.Expression rightConditionPartExpression = null;
            JST.Expression leftStoreExpression = null;

            // Lifted binary op becomes
            // left === null || right === null ? null : left [op] right;
            // So let's modify the left and right so that we can change
            // the expression as above.
            if (isLifted && leftExpression is FromNullable)
            {
                leftConditionPartExpression = ExpressionConverterBase.Convert(
                    converter,
                    ((FromNullable)leftExpression).InnerExpression);

                leftJstExpression = leftConditionPartExpression;
                if (BinaryExpressionConverter.MayHaveSideEffects(leftConditionPartExpression))
                {
                    if (isAssignmentOp)
                    {
                        leftStoreExpression = leftJstExpression;
                    }

                    var tmpVar = converter.GetTempVariable();

                    leftConditionPartExpression = new JST.BinaryExpression(
                        leftConditionPartExpression.Location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        new JST.IdentifierExpression(tmpVar),
                        leftConditionPartExpression);

                    leftJstExpression = new JST.IdentifierExpression(tmpVar);
                }
            }
            else
            {
                leftJstExpression = ExpressionConverterBase.Convert(
                    converter,
                    leftExpression);
            }

            if (isLifted && rightExpression is FromNullable)
            {
                rightConditionPartExpression = ExpressionConverterBase.Convert(
                    converter,
                    ((FromNullable)rightExpression).InnerExpression);
                rightJstExpression = rightConditionPartExpression;

                if (BinaryExpressionConverter.MayHaveSideEffects(rightConditionPartExpression))
                {
                    var tmpVar = converter.GetTempVariable();

                    rightConditionPartExpression = new JST.BinaryExpression(
                        rightConditionPartExpression.Location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        new JST.IdentifierExpression(tmpVar),
                        rightConditionPartExpression);

                    rightJstExpression = new JST.IdentifierExpression(tmpVar);
                }
            }
            else
            {
                rightJstExpression = ExpressionConverterBase.Convert(
                    converter,
                    rightExpression);
            }

            JST.Expression rv = null;
            // When ever we divide an int by another int, it becomes double in JS.
            // So we need to convert this double back to int and that is done by BitwiseOr of the
            // result and zero.
            if (BinaryExpressionConverter.IsIntBased(resultType)
                && (jstOperator == JST.BinaryOperator.Div
                    || jstOperator == JST.BinaryOperator.DivAssignment))
            {
                rv = new JST.BinaryExpression(
                    location,
                    converter.Scope,
                    JST.BinaryOperator.BitwiseOr,
                    new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.Div,
                        leftJstExpression,
                        rightJstExpression),
                    new JST.NumberLiteralExpression(converter.Scope, 0));

                if (jstOperator == JST.BinaryOperator.DivAssignment)
                {
                    rv = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        leftJstExpression,
                        rv);
                }
            }
            else if (resultType.IsDelegate()
                && (jstOperator == JST.BinaryOperator.Plus
                    || jstOperator == JST.BinaryOperator.PlusAssignment
                    || jstOperator == JST.BinaryOperator.Minus
                    || jstOperator == JST.BinaryOperator.MinusAssignment))
            {
                rv = new JST.MethodCallExpression(
                    location,
                    converter.Scope,
                    JST.IdentifierExpression.Create(
                    location,
                    converter.Scope,
                    converter.ResolveStaticMember(
                        jstOperator == JST.BinaryOperator.Plus || jstOperator == JST.BinaryOperator.PlusAssignment
                            ? converter.KnownReferences.DelegateCombineMethod
                            : converter.KnownReferences.DelegateRemoveMethod)),
                    leftJstExpression,
                    rightJstExpression);

                if (isAssignmentOp)
                {
                    rv = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        leftJstExpression,
                        rv);
                }
            }
            else
            {
                rv = new JST.BinaryExpression(
                    location,
                    converter.Scope,
                    jstOperator,
                    leftJstExpression,
                    rightJstExpression);
            }

            if (isLifted)
            {
                JST.Expression leftCondition = null,
                    rightCondition = null,
                    condition = null;

                if (leftConditionPartExpression != null)
                {
                    leftCondition = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.StrictEquals,
                        leftConditionPartExpression,
                        new JST.NullLiteralExpression(converter.Scope));
                }

                if (rightConditionPartExpression != null)
                {
                    rightCondition = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.StrictEquals,
                        rightConditionPartExpression,
                        new JST.NullLiteralExpression(converter.Scope));
                }

                if (rightCondition != null && leftCondition != null)
                {
                    condition = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.LogicalOr,
                        leftCondition,
                        rightCondition);
                }
                else if (rightCondition != null)
                {
                    condition = rightCondition;
                }
                else
                {
                    condition = leftCondition;
                }

                if (condition != null)
                {
                    rv = new JST.ConditionalOperatorExpression(
                        location,
                        converter.Scope,
                        condition,
                        new JST.NullLiteralExpression(converter.Scope),
                        rv);
                }
            }

            return rv;
        }

        /// <summary>
        /// Determines whether the specified type reference base is int based.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <returns>
        /// <c>true</c> if the specified type reference base is int based; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIntBased(TypeReference typeReference)
        {
            return typeReference.IsIntegerOrEnum();
        }

        /// <summary>
        /// Determines whether the specified type reference base is float based.
        /// </summary>
        /// <param name="typeReferenceBase">The type reference base.</param>
        /// <returns>
        /// <c>true</c> if the specified type reference base is float based; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsFloatBased(TypeReference typeReferenceBase)
        {
            return typeReferenceBase.IsDouble()
                || typeReferenceBase.FullName == "System.Number";
        }

        /// <summary>
        /// Mays the have side effects.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>True if may have side effects.</returns>
        private static bool MayHaveSideEffects(JST.Expression expression)
        {
            if (expression is JST.IdentifierExpression)
            {
                return false;
            }

            if (expression is JST.IndexExpression)
            {
                JST.IndexExpression indexer = (JST.IndexExpression)expression;
                return BinaryExpressionConverter.MayHaveSideEffects(indexer.LeftExpression);
            }

            return true;
        }
    }
}