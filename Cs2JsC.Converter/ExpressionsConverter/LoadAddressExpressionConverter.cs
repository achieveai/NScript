//-----------------------------------------------------------------------
// <copyright file="LoadAddressExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.JST;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for LoadAddressExpressionConverter
    /// </summary>
    public static class LoadAddressExpressionConverter
    {
        public static JST.Expression Convert(
            MethodConverter converter,
            LoadAddressExpression expression)
        {
            if (expression.NestedExpression is VariableReference)
            {
                return LoadAddressExpressionConverter.ConvertVaraiableAddress(
                    converter,
                    (VariableReference)expression.NestedExpression);
            }

            if (expression.NestedExpression is FieldReferenceExpression)
            {
                return LoadAddressExpressionConverter.ConvertObjectFieldAddress(
                    converter,
                    (FieldReferenceExpression)expression.NestedExpression);
            }

            if (expression.NestedExpression is ArrayElementExpression)
            {
                return LoadAddressExpressionConverter.ConvertArrayElementAddress(
                    converter,
                    (ArrayElementExpression)expression.NestedExpression);
            }

            throw new NotSupportedException(
                string.Format("Don't know how to generate address of {0}", expression.NestedExpression));
        }

        /// <summary>
        /// Converts the varaiable address.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Address access expression for variable.</returns>
        public static JST.Expression ConvertVaraiableAddress(
            MethodConverter converter,
            VariableReference variable)
        {
            return LoadAddressExpressionConverter.GetReferenceObject(
                converter,
                converter.Scope,
                variable.Location,
                (scope) => ExpressionConverterBase.Convert(converter, variable));
        }

        /// <summary>
        /// Converts the object field address.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="fieldReference">The field reference.</param>
        /// <returns>Address access expression for given field.</returns>
        public static JST.Expression ConvertObjectFieldAddress(
            MethodConverter converter,
            FieldReferenceExpression fieldReference)
        {
            IdentifierScope objectBuilderScope = new IdentifierScope(converter.Scope, 1);
            JST.FunctionExpression objectBuilderFunction = new FunctionExpression(
                fieldReference.Location,
                converter.Scope,
                objectBuilderScope,
                objectBuilderScope.ParameterIdentifiers,
                null);

            Identifier objectRefIdentifier = objectBuilderScope.ParameterIdentifiers[0];

            objectBuilderFunction.AddStatement(
                new JST.ReturnStatement(
                    objectBuilderFunction.Location,
                    objectBuilderScope,
                    LoadAddressExpressionConverter.GetReferenceObject(
                        converter,
                        objectBuilderScope,
                        objectBuilderFunction.Location,
                        (scope) =>
                        {
                            var fieldRef = fieldReference.FieldReference;
                            return fieldRef.Resolve().IsStatic
                                ? JST.IdentifierExpression.Create(
                                    objectBuilderFunction.Location,
                                    scope,
                                    converter.ResolveStaticMember(fieldRef))
                                : (JST.Expression)new JST.IndexExpression(
                                    objectBuilderFunction.Location,
                                    scope,
                                    new JST.IdentifierExpression(objectRefIdentifier),
                                    new JST.IdentifierExpression(converter.Resolve(fieldReference.FieldReference)));
                        })));

            JST.Expression objectRefExpression;
            if (fieldReference.LeftExpression == null)
            {
                objectRefExpression =
                    JST.IdentifierExpression.Create(
                        fieldReference.Location,
                        converter.Scope,
                        converter.Resolve(fieldReference.FieldReference.DeclaringType));
            }
            else
            {
                objectRefExpression = ExpressionConverterBase.Convert(
                    converter,
                    fieldReference.LeftExpression);
            }

            return new JST.MethodCallExpression(
                objectRefExpression.Location,
                converter.Scope,
                objectBuilderFunction,
                new JST.Expression[] { objectRefExpression });
        }

        /// <summary>
        /// Converts the array element address.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="arrayElementExpression">The array element expression.</param>
        /// <returns>Address access expression for array element.</returns>
        private static JST.Expression ConvertArrayElementAddress(
            MethodConverter converter,
            ArrayElementExpression arrayElementExpression)
        {
            IdentifierScope objectBuilderScope = new IdentifierScope(converter.Scope, 2);
            JST.FunctionExpression objectBuilderFunction = new FunctionExpression(
                arrayElementExpression.Location,
                converter.Scope,
                objectBuilderScope,
                objectBuilderScope.ParameterIdentifiers,
                null);

            Identifier arrayRefIdentifier = objectBuilderScope.ParameterIdentifiers[0];
            Identifier arrayIndexIdentifier = objectBuilderScope.ParameterIdentifiers[1];

            objectBuilderFunction.AddStatement(
                new JST.ReturnStatement(
                    objectBuilderFunction.Location,
                    objectBuilderScope,
                    LoadAddressExpressionConverter.GetReferenceObject(
                        converter,
                        objectBuilderScope,
                        objectBuilderFunction.Location,
                        (scope) => (JST.Expression)new JST.IndexExpression(
                            objectBuilderFunction.Location,
                            scope,
                            new JST.IdentifierExpression(arrayRefIdentifier),
                            new JST.IdentifierExpression(arrayIndexIdentifier),
                            true))));

            JST.Expression arrayRefExpression = ExpressionConverterBase.Convert(
                converter,
                arrayElementExpression.Array);

            JST.Expression arrayIndexExpression = ExpressionConverterBase.Convert(
                converter,
                arrayElementExpression.Index);

            return new JST.MethodCallExpression(
                arrayRefExpression.Location,
                converter.Scope,
                objectBuilderFunction,
                new JST.Expression[] { arrayRefExpression, arrayIndexExpression });
        }

        /// <summary>
        /// Gets the reference object.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="parentScope">The parent scope.</param>
        /// <param name="location">The location.</param>
        /// <param name="expressionCreator">The expression creator.</param>
        /// <returns>
        /// reference object that contains functions for accessing the data.
        /// </returns>
        public static JST.Expression GetReferenceObject(
            MethodConverter converter,
            IdentifierScope parentScope,
            Location location,
            Func<IdentifierScope, JST.Expression> expressionCreator)
        {
            JST.InlineObjectInitializer initializer = new InlineObjectInitializer(
                location,
                parentScope);

            IdentifierScope innerScope =
                new IdentifierScope(parentScope);

            JST.FunctionExpression func = new JST.FunctionExpression(
                initializer.Location,
                parentScope,
                innerScope,
                innerScope.ParameterIdentifiers,
                null);

            func.AddStatement(
                new JST.ReturnStatement(
                    func.Location,
                    innerScope,
                    expressionCreator(innerScope)));

            initializer.AddInitializer(
                converter.RuntimeManager.ReferenceManager.ReaderIdentifier,
                func);

            innerScope = new IdentifierScope(parentScope, 1);
            Identifier setterArgument = innerScope.ParameterIdentifiers[0];

            func = new JST.FunctionExpression(
                initializer.Location,
                parentScope,
                innerScope,
                innerScope.ParameterIdentifiers,
                null);

            func.AddStatement(
                new JST.ReturnStatement(
                    func.Location,
                    innerScope,
                    new JST.BinaryExpression(
                        func.Location,
                        innerScope,
                        JST.BinaryOperator.Assignment,
                        expressionCreator(innerScope),
                        new IdentifierExpression(setterArgument))));

            initializer.AddInitializer(
                converter.RuntimeManager.ReferenceManager.WriterIdentifier,
                func);

            return initializer;
        }
    }
}