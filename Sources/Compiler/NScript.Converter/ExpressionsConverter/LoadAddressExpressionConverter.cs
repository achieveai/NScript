//-----------------------------------------------------------------------
// <copyright file="LoadAddressExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;

    /// <summary>
    /// Definition for LoadAddressExpressionConverter
    /// </summary>
    public static class LoadAddressExpressionConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
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
            IMethodScopeConverter converter,
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
            IMethodScopeConverter converter,
            FieldReferenceExpression fieldReference)
        {
            IdentifierScope objectBuilderScope = new IdentifierScope(converter.Scope, 1);
            FunctionExpression objectBuilderFunction = new FunctionExpression(
                fieldReference.Location,
                converter.Scope,
                objectBuilderScope,
                objectBuilderScope.ParameterIdentifiers,
                null);

            SimpleIdentifier objectRefIdentifier = objectBuilderScope.ParameterIdentifiers[0];

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
                                : (JST.Expression)new IndexExpression(
                                    objectBuilderFunction.Location,
                                    scope,
                                    new IdentifierExpression(objectRefIdentifier, scope),
                                    new IdentifierExpression(converter.Resolve(fieldReference.FieldReference), scope));
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
            IMethodScopeConverter converter,
            ArrayElementExpression arrayElementExpression)
        {
            IdentifierScope objectBuilderScope = new IdentifierScope(converter.Scope, 2);
            FunctionExpression objectBuilderFunction = new FunctionExpression(
                arrayElementExpression.Location,
                converter.Scope,
                objectBuilderScope,
                objectBuilderScope.ParameterIdentifiers,
                null);

            SimpleIdentifier arrayRefIdentifier = objectBuilderScope.ParameterIdentifiers[0];
            SimpleIdentifier arrayIndexIdentifier = objectBuilderScope.ParameterIdentifiers[1];

            objectBuilderFunction.AddStatement(
                new JST.ReturnStatement(
                    objectBuilderFunction.Location,
                    objectBuilderScope,
                    LoadAddressExpressionConverter.GetReferenceObject(
                        converter,
                        objectBuilderScope,
                        objectBuilderFunction.Location,
                        (scope) => (JST.Expression)new IndexExpression(
                            objectBuilderFunction.Location,
                            scope,
                            new IdentifierExpression(arrayRefIdentifier, objectBuilderScope),
                            new IdentifierExpression(arrayIndexIdentifier, objectBuilderScope),
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
            IMethodScopeConverter converter,
            IdentifierScope parentScope,
            Location location,
            Func<IdentifierScope, JST.Expression> expressionCreator)
        {
            var initializer = new InlineObjectInitializer(
                location,
                parentScope);

            IdentifierScope innerScope =
                new IdentifierScope(parentScope);

            var func = new FunctionExpression(
                initializer.Location,
                parentScope,
                innerScope,
                innerScope.ParameterIdentifiers,
                null);

            converter.PushJsScope(innerScope);
            func.AddStatement(
                new JST.ReturnStatement(
                    func.Location,
                    innerScope,
                    expressionCreator(innerScope)));
            converter.PopJsScope();

            initializer.AddInitializer(
                converter.RuntimeManager.ReferenceManager.ReaderIdentifier,
                func);

            innerScope = new IdentifierScope(parentScope, 1);
            SimpleIdentifier setterArgument = innerScope.ParameterIdentifiers[0];

            func = new FunctionExpression(
                initializer.Location,
                parentScope,
                innerScope,
                innerScope.ParameterIdentifiers,
                null);

            converter.PushJsScope(innerScope);
            func.AddStatement(
                new JST.ReturnStatement(
                    func.Location,
                    innerScope,
                    new JST.BinaryExpression(
                        func.Location,
                        innerScope,
                        JST.BinaryOperator.Assignment,
                        expressionCreator(innerScope),
                        new IdentifierExpression(setterArgument, innerScope))));
            converter.PopJsScope();

            initializer.AddInitializer(
                converter.RuntimeManager.ReferenceManager.WriterIdentifier,
                func);

            return initializer;
        }
    }
}